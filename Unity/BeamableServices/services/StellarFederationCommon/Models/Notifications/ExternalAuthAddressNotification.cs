namespace SuiFederationCommon.Models.Notifications
{
    /// <summary>
    /// ExternalAuthNotification
    /// </summary>
    public class ExternalAuthAddressNotification : IPlayerNotification
    {
        /// <summary>
        /// Context
        /// </summary>
        public string Context { get; } = PlayerNotificationContext.ExternalAuthAddress;
        /// <summary>
        /// Value of the transaction.
        /// </summary>
        public string Value { get; set; }
    }
}