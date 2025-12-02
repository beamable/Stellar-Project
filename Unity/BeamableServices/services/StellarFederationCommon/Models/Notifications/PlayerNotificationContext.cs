namespace SuiFederationCommon.Models.Notifications
{
    /// <summary>
    /// Represents the context for player notification subscriptions.
    /// </summary>
    public class PlayerNotificationContext
    {
        /// <summary>
        /// Context for external authentication address notifications.
        /// </summary>
        public const string ExternalAuthAddress = "external-auth-address";
        /// <summary>
        /// Context for external authentication signature notifications.
        /// </summary>
        public const string ExternalAuthSignature = "external-auth-signature";
        /// <summary>
        /// Context for custodial account created notifications.
        /// </summary>
        public const string CustodialAccountCreated = "custodial-account-created";
    }
}