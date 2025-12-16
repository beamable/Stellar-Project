using System;

namespace StellarFederationCommon.Models.Response
{
    /// <summary>
    /// SchedulerJobResponse
    /// </summary>
    [Serializable]
    public class SchedulerJobResponse
    {
        /// <summary>
        /// Jos names
        /// </summary>
        public string[] names;

        /// <summary>
        /// Empty response
        /// </summary>
        /// <returns></returns>
        public static SchedulerJobResponse Empty()
        {
            return new SchedulerJobResponse();
        }
    }
}