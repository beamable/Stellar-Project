using System.Collections.Generic;

namespace StellarFederationCommon.FederationContent
{
    /// <summary>
    /// Request to update a CropItem NFT
    /// </summary>
    [System.Serializable]
    public class CropUpdateRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public string ContentId;
        /// <summary>
        /// 
        /// </summary>
        public long InstanceId;
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> Properties = new Dictionary<string, string>();
    }
}