using Newtonsoft.Json;

namespace SuiFederationCommon.Models.Notifications
{
    /// <summary>
    ///  Interface for player notifications.
    /// </summary>
    public interface IPlayerNotification
    {
        /// <summary>
        /// Context of the notification, used to determine the type of notification.
        /// </summary>
        [JsonIgnore]
        string Context { get; }
        /// <summary>
        /// Value of the notification, typically an identifier or relevant data.
        /// </summary>
        string Value { get; }
    }
}