using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


//*************************************************************************************
//Note: This webjob is schedule on every 5 minuts and used for send expired notification on schedule time if user want to get expired notification.
//*************************************************************************************

namespace Hollyhood.WebJobPushBroadcast
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage

        public static SqlCommand statusCommand;
        public static SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SQConnectionString"].ConnectionString);

        static void Main()
        {
            var config = new JobHostConfiguration();
            //Console.WriteLine("Step 1");
            if (config.IsDevelopment)
            {
                //Console.WriteLine("Step 2");
                config.UseDevelopmentSettings();
                //Console.WriteLine("Step 3");
            }

            //Console.WriteLine("Step 4");
            //SendPushNotification().GetAwaiter().GetResult();
            BroadcastMessage(); 
            //Console.WriteLine("Step 5");
        }

        //#region Send notification reminder on expire item
        ///// <summary>
        ///// This method is used to sending push notification on Hollyhood activities.
        ///// </summary>
        //public static async Task SendPushNotification()
        //{
        //    SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SQConnectionString"].ConnectionString);
        //    SqlCommand command;
            
        //    conn.Open();
        //    try
        //    {
        //        SqlDataAdapter da = new SqlDataAdapter();
        //        DataTable dt = new DataTable();
        //        try
        //        {
        //            command = new SqlCommand("GetNewPushNotifications", conn);
        //            command.CommandType = CommandType.StoredProcedure;
        //            da.SelectCommand = command;
        //            da.Fill(dt);
        //            if (dt.Rows.Count > 0)
        //            {
        //                for (int i = 0; i < dt.Rows.Count; i++)
        //                {
        //                    // Sending push notification
        //                    HubNotification hubNotification = new HubNotification
        //                    {
        //                        NotificationId = Convert.ToInt64(dt.Rows[i]["NotificationId"]),
        //                        FromUserId = Convert.ToInt64(dt.Rows[i]["FromUserId"]),
        //                        ToUserId = Convert.ToInt64(dt.Rows[i]["ToUserId"]),
        //                        ActivityId = Convert.ToInt64(dt.Rows[i]["ActivityId"]),
        //                        NotificationType = Convert.ToInt32(dt.Rows[i]["NotificationType"]),
        //                        FromUniqueScreenName = dt.Rows[i]["FromUniqueScreenName"].ToString(),
        //                        ToUniqueScreenName = dt.Rows[i]["ToUniqueScreenName"].ToString(),
        //                        DisplayMessage = dt.Rows[i]["DisplayMessage"].ToString(),
        //                        DeviceId = dt.Rows[i]["DeviceId"].ToString(), // This is ToUser's Device Id
        //                        DeviceType = dt.Rows[i]["DeviceType"].ToString(), // This is ToUser's Device Type
        //                        FromDeviceId = dt.Rows[i]["FromDeviceId"].ToString(),
        //                        FromDeviceType = dt.Rows[i]["FromDeviceType"].ToString(),
        //                        CreatedDate = Convert.ToDateTime(dt.Rows[i]["CreatedDate"].ToString()),
        //                        WalletTransactionId = dt.Rows[i]["WalletTransactionId"] != DBNull.Value? Convert.ToInt64(dt.Rows[i]["WalletTransactionId"]): 1,
        //                        FromBadgeCount = Convert.ToInt32(dt.Rows[i]["FromBadgeCount"]),
        //                        ToBadgeCount = Convert.ToInt32(dt.Rows[i]["ToBadgeCount"])
        //                    };

        //                    await SendPushNotificationToDevice(hubNotification);
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
                      
        //        }
        //        finally
        //        {
        //            if (conn.State == ConnectionState.Open)
        //                conn.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
                
        //    }
        //}
        //#endregion

        //#region Sending Push Notification
        ///// <summary>
        ///// This method is created to send push notification to apple and android.
        ///// </summary>
        ///// <param name="hubNotification"></param>
        ///// <returns></returns>
        //public async static Task SendPushNotificationToDevice(HubNotification hubNotification)
        //{
        //    if (hubNotification != null)
        //    {
        //        string convertedString = string.Empty;
        //        string jsonPayLoad = string.Empty;
        //        try
        //        {
        //            NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString(ConfigurationManager.AppSettings["HubConnectionString"], ConfigurationManager.AppSettings["HubName"], true);
        //            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        //            if (!string.IsNullOrEmpty(hubNotification.DeviceId) && !string.IsNullOrEmpty(hubNotification.DeviceType))
        //            {
        //                //check if already registered deivce 
        //                var test = hub.DeleteRegistrationAsync(hubNotification.DeviceId.ToString());
        //                var test1 = hub.DeleteRegistrationAsync(hubNotification.ToUserId.ToString());
        //                var test2 = hub.DeleteRegistrationsByChannelAsync(hubNotification.FromUserId.ToString());
        //                var test3 = hub.DeleteRegistrationsByChannelAsync(hubNotification.ToUserId.ToString());
                        
        //                //set message
        //                hubNotification.DisplayMessage = string.Format(hubNotification.DisplayMessage);
        //                if (!string.IsNullOrEmpty(hubNotification.DisplayMessage))
        //                {
        //                    NotificationOutcome outcome;
        //                    if (hubNotification.DeviceType.ToLower().Contains("android"))
        //                    {
        //                        var registeredNewDevice = hub.CreateGcmNativeRegistrationAsync(hubNotification.DeviceId.Replace("-", ""), new[] { hubNotification.ToUserId.ToString() }).Result;

        //                        jsonPayLoad = "{ \"data\": " +
        //                        "{ \"NotificationId\": \"" + hubNotification.NotificationId.ToString() + "\"," +
        //                        "\"FromUserId\": \"" + hubNotification.FromUserId.ToString() + "\"," +
        //                        "\"ToUserId\": \"" + hubNotification.ToUserId.ToString()+ "\"," +
        //                        "\"ActivityId\": \"" + hubNotification.ActivityId.ToString() + "\"," +
        //                        "\"NotificationType\": \"" + hubNotification.NotificationType.ToString() + "\"," +
        //                        "\"FromUniqueScreenName\": \"" + hubNotification.FromUniqueScreenName + "\"," +
        //                        "\"ToUniqueScreenName\": \"" + hubNotification.ToUniqueScreenName + "\"," +
        //                        "\"DisplayMessage\": \"" + hubNotification.DisplayMessage + "\"," +
        //                        "\"DeviceId\": \"" + hubNotification.DeviceId + "\"," +
        //                        "\"DeviceType\": \"" + hubNotification.DeviceType + "\"," +
        //                        "\"CreatedDate\": \"" + hubNotification.CreatedDate.ToString() + "\"," +
        //                        //"\"badge\": \"" + hubNotification.badge + "\"," +
        //                        //"\"inProgress\": \"" + hubNotification.InProgress + "\"," +                                
        //                        //"\"headercolor\": \"" + hubNotification.ListHeaderColor + "\"," +
        //                        //"\"sound\":\"accomplished.caf\"," +
        //                        //"\"userProfileId\":\"" + hubNotification.ToUserProfileId.ToString().Trim() + "\"," +
        //                        //"\"type\":\"sendfriend\"}}";
        //                        "\"type\":\"HHPushNotification\"}}";

        //                        outcome = hub.SendGcmNativeNotificationAsync(jsonPayLoad, hubNotification.ToUserId.ToString()).Result;
        //                    }
        //                    else // When iPhone
        //                    {
        //                        HHJSonPayload hhJSonPayload = new HHJSonPayload();
        //                        hhJSonPayload.aps = new APS();
        //                        hhJSonPayload.payLoadDetail = new JSonPayloadDetail();
        //                        hhJSonPayload.aps.alert = hubNotification.DisplayMessage;
        //                        hhJSonPayload.aps.badge = await GetBadgeCount(hubNotification);
        //                        hhJSonPayload.aps.sound = "accomplished.caf";
        //                        hhJSonPayload.payLoadDetail.ActivityId = hubNotification.ActivityId;
        //                        hhJSonPayload.payLoadDetail.NotificationType = hubNotification.NotificationType;
        //                        hhJSonPayload.payLoadDetail.ToUserId = hubNotification.ToUserId;
        //                        hhJSonPayload.payLoadDetail.FromUserId = hubNotification.FromUserId;
        //                        hhJSonPayload.payLoadDetail.LoggedInUserId = hubNotification.LoggedInUserId;
        //                        hhJSonPayload.payLoadDetail.WalletTransactionId = hubNotification.WalletTransactionId;

        //                        convertedString = JsonConvert.SerializeObject(hhJSonPayload);

        //                        switch (hubNotification.NotificationType)
        //                        {
        //                            case 2:
        //                            case 3:
        //                            case 4:
        //                            case 5:
        //                            case 10:
        //                            case 11:
        //                            case 12:
        //                            case 13:
        //                            case 16:
        //                                // All cases which are Like, Comment, Share, Send Friend Request, Follow, Un-Follow, Mentioned, LIVE, Hangout
        //                                var registationDescriptions = hub.GetRegistrationsByTagAsync(hubNotification.ToUserId.ToString(), 100).Result;
        //                                var registeredNewDevice = hub.CreateAppleNativeRegistrationAsync(hubNotification.DeviceId.Replace("-", ""), new[] { hubNotification.ToUserId.ToString() }).Result;
        //                                outcome = hub.SendAppleNativeNotificationAsync(convertedString, hubNotification.ToUserId.ToString()).Result;
        //                                break;
        //                            case 6:
        //                            case 7:
        //                                // Only cases which are Friend request accepted, rejected
        //                                var registationDescriptions1 = hub.GetRegistrationsByTagAsync(hubNotification.FromUserId.ToString(), 100).Result;
        //                                var registeredNewDevice1 = hub.CreateAppleNativeRegistrationAsync(hubNotification.FromDeviceId.Replace("-", ""), new[] { hubNotification.FromUserId.ToString() }).Result;
        //                                outcome = hub.SendAppleNativeNotificationAsync(convertedString, hubNotification.FromUserId.ToString()).Result;
        //                                break;
        //                            default:
        //                                var registationDescriptions2 = hub.GetRegistrationsByTagAsync(hubNotification.ToUserId.ToString(), 100).Result;
        //                                var registeredNewDevice2 = hub.CreateAppleNativeRegistrationAsync(hubNotification.DeviceId.Replace("-", ""), new[] { hubNotification.ToUserId.ToString() }).Result;
        //                                outcome = hub.SendAppleNativeNotificationAsync(convertedString, hubNotification.ToUserId.ToString()).Result;
        //                                break;
        //                        }
        //                    }

        //                    if (connection.State == ConnectionState.Closed)
        //                        connection.Open();
        //                    statusCommand = new SqlCommand("LogWebJobStatus", connection);
        //                    statusCommand.CommandType = CommandType.StoredProcedure;

        //                    statusCommand.Parameters.AddWithValue("@WebJobStatus", outcome.State + " | jsonPayLoad = " + convertedString);

        //                    switch (hubNotification.NotificationType)
        //                    {
        //                        case 2:
        //                        case 3:
        //                        case 4:
        //                        case 5:
        //                        case 10:
        //                        case 11:
        //                        case 12:
        //                        case 13:
        //                        case 16:
        //                            // All cases which are Like, Comment, Share, Send Friend Request, Follow, Un-Follow, Mentioned, LIVE, Hangout
        //                            statusCommand.Parameters.AddWithValue("@ForUserId", hubNotification.ToUserId);
        //                            break;
        //                        case 6:
        //                        case 7:
        //                            // Only cases which are Friend request accepted, rejected
        //                            statusCommand.Parameters.AddWithValue("@ForUserId", hubNotification.FromUserId);
        //                            break;
        //                        default:
        //                            statusCommand.Parameters.AddWithValue("@ForUserId", hubNotification.ToUserId);
        //                            break;
        //                    }
                            
        //                    statusCommand.ExecuteNonQuery();
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            if (connection.State == ConnectionState.Closed)
        //                connection.Open();

        //            statusCommand = new SqlCommand("LogWebJobStatus", connection);
        //            statusCommand.CommandType = CommandType.StoredProcedure;

        //            statusCommand.Parameters.AddWithValue("@WebJobStatus", "Catch: " + convertedString + " | " + $"{ex.Message}\n{ex.StackTrace}");
        //            statusCommand.Parameters.AddWithValue("@ForUserId", hubNotification.ToUserId);
        //            statusCommand.ExecuteNonQuery();
        //            Console.WriteLine(ex.Message);
        //        }
        //        finally
        //        {
        //            if (connection.State == ConnectionState.Open)
        //                connection.Close();
        //        }
        //    }
        //}

        //private async static Task<int> GetBadgeCount(HubNotification hubNotification)
        //{
        //    int badgeCount = 1;
        //    switch (hubNotification.NotificationType)
        //    {
        //        case 2:
        //        case 3:
        //        case 4:
        //        case 5:
        //        case 10:
        //        case 11:
        //        case 12:
        //        case 13:
        //        case 16:
        //            // All cases which are Like, Comment, Share, Send Friend Request, Follow, Un-Follow, Mentioned, LIVE, Hangout
        //            badgeCount = hubNotification.ToBadgeCount;
        //            break;
        //        case 6:
        //        case 7:
        //            // Only cases which are Friend request accepted, rejected
        //            badgeCount = hubNotification.FromBadgeCount;
        //            break;
        //        default:
        //            badgeCount = hubNotification.ToBadgeCount;
        //            break;
        //    }
        //    return badgeCount;
        //}
        //#endregion

        #region -- Broadcast Message --
        public async static Task BroadcastMessage()
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SQConnectionString"].ConnectionString);
            SqlCommand command;

            conn.Open();
            try
            {
                SqlDataAdapter da = new SqlDataAdapter();
                DataTable dt = new DataTable();
                try
                {
                    command = new SqlCommand("GetBroadcastPushNotification", conn);
                    command.CommandType = CommandType.StoredProcedure;
                    da.SelectCommand = command;
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            // Sending push notification
                            BroadcastNotification broadcastNotification = new BroadcastNotification
                            {
                                BroadcastId = Convert.ToInt64(dt.Rows[i]["BroadcastId"]),
                                FromUserId = Convert.ToInt64(dt.Rows[i]["FromUserId"]),
                                ToUserId = Convert.ToInt64(dt.Rows[i]["ToUserId"]),
                                Message = dt.Rows[i]["Message"].ToString(),
                                DeviceId = dt.Rows[i]["DeviceId"].ToString(), // This is ToUser's Device Id
                                DeviceType = dt.Rows[i]["DeviceType"].ToString(), // This is ToUser's Device Type
                                CreatedDate = Convert.ToDateTime(dt.Rows[i]["CreatedDate"].ToString()),
                                ToBadgeCount = Convert.ToInt32(dt.Rows[i]["ToBadgeCount"]),
                                PushSentOn = Convert.ToDateTime(dt.Rows[i]["PushSentOn"].ToString()),
                                FromUniqueScreenName = dt.Rows[i]["FromUniqueScreenName"].ToString(),
                                FromUserProfileImageUrl = dt.Rows[i]["FromUserProfileImageUrl"].ToString(),
                                NotificationType = Convert.ToInt32(dt.Rows[i]["NotificationType"]),
                                Title = dt.Rows[i]["Title"].ToString()
                            };

                            await SendBroadcastToDevice(broadcastNotification);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    statusCommand = new SqlCommand("LogWebJobStatus", connection);
                    statusCommand.CommandType = CommandType.StoredProcedure;

                    statusCommand.Parameters.AddWithValue("@WebJobStatus", "Catch: EXCEPTION OCCURED When Calling SP \n"  + $"{ex.Message}\n{ex.StackTrace}");
                    statusCommand.Parameters.AddWithValue("@ForUserId", 0);
                    statusCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                statusCommand = new SqlCommand("LogWebJobStatus", connection);
                statusCommand.CommandType = CommandType.StoredProcedure;

                statusCommand.Parameters.AddWithValue("@WebJobStatus", "Catch: EXCEPTION OCCURED When opening a database connection \n" + $"{ex.Message}\n{ex.StackTrace}");
                statusCommand.Parameters.AddWithValue("@ForUserId", 0);
                statusCommand.ExecuteNonQuery();
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        public async static Task SendBroadcastToDevice(BroadcastNotification broadcastNotification)
        {
            if (broadcastNotification != null)
            {
                string convertedString = string.Empty;
                string jsonPayLoad = string.Empty;
                try
                {
                    NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString(ConfigurationManager.AppSettings["HubConnectionString"], ConfigurationManager.AppSettings["HubName"], true);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    if (!string.IsNullOrEmpty(broadcastNotification.DeviceId) && !string.IsNullOrEmpty(broadcastNotification.DeviceType))
                    {
                        //check if already registered deivce 
                        var test = hub.DeleteRegistrationAsync(broadcastNotification.DeviceId.ToString());
                        var test1 = hub.DeleteRegistrationAsync(broadcastNotification.ToUserId.ToString());
                        var test2 = hub.DeleteRegistrationsByChannelAsync(broadcastNotification.FromUserId.ToString());
                        var test3 = hub.DeleteRegistrationsByChannelAsync(broadcastNotification.ToUserId.ToString());

                        //set message
                        broadcastNotification.Message = string.Format(broadcastNotification.Message);
                        if (!string.IsNullOrEmpty(broadcastNotification.Message))
                        {
                            NotificationOutcome outcome;
                            if (broadcastNotification.DeviceType.ToLower().Contains("android"))
                            {
                                var registeredNewDevice = hub.CreateGcmNativeRegistrationAsync(broadcastNotification.DeviceId.Replace("-", ""), new[] { broadcastNotification.ToUserId.ToString() }).Result;

                                jsonPayLoad = "{ \"data\": " +
                                "{ \"NotificationId\": \"" + broadcastNotification.BroadcastId.ToString() + "\"," +
                                "\"FromUserId\": \"" + broadcastNotification.FromUserId.ToString() + "\"," +
                                "\"ToUserId\": \"" + broadcastNotification.ToUserId.ToString() + "\"," +
                                "\"DisplayMessage\": \"" + broadcastNotification.Message + "\"," +
                                "\"DeviceId\": \"" + broadcastNotification.DeviceId + "\"," +
                                "\"DeviceType\": \"" + broadcastNotification.DeviceType + "\"," +
                                "\"CreatedDate\": \"" + broadcastNotification.CreatedDate.ToString() + "\"," +
                                //"\"badge\": \"" + hubNotification.badge + "\"," +
                                //"\"inProgress\": \"" + hubNotification.InProgress + "\"," +                                
                                //"\"headercolor\": \"" + hubNotification.ListHeaderColor + "\"," +
                                //"\"sound\":\"accomplished.caf\"," +
                                //"\"userProfileId\":\"" + hubNotification.ToUserProfileId.ToString().Trim() + "\"," +
                                //"\"type\":\"sendfriend\"}}";
                                "\"type\":\"HHPushNotification\"}}";

                                //var registationDescriptions = hub.GetRegistrationsByTagAsync(broadcastNotification.ToUserId.ToString(), 100).Result;
                                //var registeredNewDevice = hub.CreateAppleNativeRegistrationAsync(broadcastNotification.DeviceId.Replace("-", ""), new[] { broadcastNotification.ToUserId.ToString() }).Result;
                                outcome = hub.SendGcmNativeNotificationAsync(jsonPayLoad, broadcastNotification.ToUserId.ToString()).Result;
                            }
                            else // When iPhone
                            {
                                HHBroadcastJSonPayload hhJSonPayload = new HHBroadcastJSonPayload();
                                hhJSonPayload.aps = new APSBroadcast();
                                hhJSonPayload.payLoadDetail = new JSonPayloadDetail();
                                
                                hhJSonPayload.aps.alert = new Alert();
                                hhJSonPayload.aps.alert.title = broadcastNotification.Title;
                                hhJSonPayload.aps.alert.body = broadcastNotification.Message;
                                
                                hhJSonPayload.aps.badge = broadcastNotification.ToBadgeCount;
                                hhJSonPayload.aps.sound = "Broadcast.caf";
                                
                                hhJSonPayload.payLoadDetail.ToUserId = broadcastNotification.ToUserId;
                                hhJSonPayload.payLoadDetail.FromUserId = broadcastNotification.FromUserId;
                                hhJSonPayload.payLoadDetail.FromUniqueScreenName = broadcastNotification.FromUniqueScreenName;
                                hhJSonPayload.payLoadDetail.FromUserProfileImageUrl = broadcastNotification.FromUserProfileImageUrl;
                                hhJSonPayload.payLoadDetail.BroadcastId = broadcastNotification.BroadcastId;
                                hhJSonPayload.payLoadDetail.NotificationType = broadcastNotification.NotificationType;

                                convertedString = JsonConvert.SerializeObject(hhJSonPayload);

                                // All cases which are Like, Comment, Share, Send Friend Request, Follow, Un-Follow, Mentioned, LIVE, Hangout
                                var registationDescriptions = hub.GetRegistrationsByTagAsync(broadcastNotification.ToUserId.ToString(), 100).Result;
                                var registeredNewDevice = hub.CreateAppleNativeRegistrationAsync(broadcastNotification.DeviceId.Replace("-", ""), new[] { broadcastNotification.ToUserId.ToString() }).Result;
                                outcome = hub.SendAppleNativeNotificationAsync(convertedString, broadcastNotification.ToUserId.ToString()).Result;
                            }

                            if (connection.State == ConnectionState.Closed)
                                connection.Open();
                            statusCommand = new SqlCommand("LogWebJobStatus", connection);
                            statusCommand.CommandType = CommandType.StoredProcedure;

                            statusCommand.Parameters.AddWithValue("@WebJobStatus", outcome.State + " | jsonPayLoad = " + convertedString);
                            statusCommand.Parameters.AddWithValue("@ForUserId", broadcastNotification.ToUserId);

                            statusCommand.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    statusCommand = new SqlCommand("LogWebJobStatus", connection);
                    statusCommand.CommandType = CommandType.StoredProcedure;

                    statusCommand.Parameters.AddWithValue("@WebJobStatus", "Catch: " + convertedString + " | " + $"{ex.Message}\n{ex.StackTrace}");
                    statusCommand.Parameters.AddWithValue("@ForUserId", broadcastNotification.ToUserId);
                    statusCommand.ExecuteNonQuery();
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
        }
        #endregion -- Admin Message --
    }
}
