namespace SuiFederationCommon.Models.Notifications
{
    /// <summary>
    /// ExternalAuthNotification
    /// </summary>
    public class ExternalAuthSignatureNotification : IPlayerNotification
    {
        /// <summary>
        /// Context
        /// </summary>
        public string Context { get; } = PlayerNotificationContext.ExternalAuthSignature;
        /// <summary>
        /// Value of the transaction.
        /// </summary>
        public string Value { get; set; }
    }
}