using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hollyhood.WebJobPushBroadcast
{
    public class HubNotification
    {
        public long NotificationId { get; set; }
        public long FromUserId { get; set; }
        public long ToUserId { get; set; }
        public long ActivityId { get; set; }
        public int NotificationType { get; set; }
        public string FromUniqueScreenName { get; set; }
        public string ToUniqueScreenName { get; set; }
        public string DisplayMessage { get; set; }
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public string FromDeviceId { get; set; }
        public string FromDeviceType { get; set; }
        public DateTime CreatedDate { get; set; }
        public long LoggedInUserId { get; set; }
        public long? WalletTransactionId { get; set; }
        public int FromBadgeCount { get; set; }
        public int ToBadgeCount { get; set; }
    }

    public class HHJSonPayload
    {
        public APS aps { get; set; }
        public JSonPayloadDetail payLoadDetail { get; set; }
    }
    public class HHBroadcastJSonPayload
    {
        public APSBroadcast aps { get; set; }
        public JSonPayloadDetail payLoadDetail { get; set; }
    }
    public class APS
    {
        public string alert { get; set; }
        public string sound { get; set; }
        public int badge { get; set; }
    }

    public class APSBroadcast
    {
        public Alert alert { get; set; }
        public string sound { get; set; }
        public int badge { get; set; }
    }
    public class Alert
    {
        public string title { get; set; }
        public string body { get; set; }
    }

    public class JSonPayloadDetail
    {
        public int NotificationType { get; set; }
        public long ActivityId { get; set; }
        public long BroadcastId { get; set; }
        public long FromUserId { get; set; }
        public long ToUserId { get; set; }
        public long LoggedInUserId { get; set; }
        public long? WalletTransactionId { get; set; }
        public string FromUniqueScreenName { get; set; }
        public string FromUserProfileImageUrl { get; set; }
    }
    public class BroadcastNotification
    {
        public long BroadcastId { get; set; }
        public int NotificationType { get; set; }
        public long FromUserId { get; set; }
        public string FromUniqueScreenName { get; set; }
        public string FromUserProfileImageUrl { get; set; }
        public long ToUserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public int ToBadgeCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime PushSentOn { get; set; }
    }
}
