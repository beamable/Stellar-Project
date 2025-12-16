using System;

namespace StellarFederationCommon.Models.Response
{
    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class AccountResponse
    {
        /// <summary>
        /// Wallet address
        /// </summary>
        public string wallet;
        /// <summary>
        /// Is Wallet created on-chain
        /// </summary>
        public bool created;
    }
}