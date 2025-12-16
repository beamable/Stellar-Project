namespace SuiFederationCommon.Models.Notifications
{
    /// <summary>
    /// CustodialAccountCreatedNotification
    /// </summary>
    public class CustodialAccountCreatedNotification : IPlayerNotification
    {
        /// <summary>
        /// Context
        /// </summary>
        public string Context { get; } = PlayerNotificationContext.CustodialAccountCreated;

        /// <summary>
        /// Value of the transaction.
        /// </summary>
        public string Value { get; set; } = "";
    }
}