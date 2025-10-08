using System;

namespace StellarFederationCommon.Models.Response
{
    /// <summary>
    /// MS ConfigurationResponse
    /// </summary>
    [Serializable]
    public class ConfigurationResponse
    {
        /// <summary>
        /// Current Stellar Network
        /// </summary>
        public string network;
        /// <summary>
        /// Wallet Connect Bridge URL
        /// </summary>
        public string walletConnectBridgeUrl;
    }
}