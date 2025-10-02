namespace SuiFederationCommon.Models.Notifications
{
    /// <summary>
    /// ExternalAuthNotification
    /// </summary>
    public class ExternalAuthNotification : IPlayerNotification
    {
        /// <summary>
        /// Context
        /// </summary>
        public string Context { get; } = PlayerNotificationContext.ExternalAuth;
        /// <summary>
        /// Value of the transaction.
        /// </summary>
        public string Value { get; set; }
    }
}