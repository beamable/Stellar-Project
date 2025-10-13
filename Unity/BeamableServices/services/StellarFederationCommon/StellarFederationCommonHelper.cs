using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StellarFederationCommon
{
    /// <summary>
    ///  StellarFederationCommonHelper
    /// </summary>
    public static class StellarFederationCommonHelper
    {
        /// <summary>
        /// Get federation types
        /// </summary>
        /// <returns></returns>
        public static HashSet<Type> GetFederationTypes()
        {
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.Namespace == "StellarFederationCommon.FederationContent")
                .ToHashSet();
        }
    }
}