using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;

namespace Skybrud.Umbraco.Analytics.Models.Selection {

    public class AnalyticsAccount {

        #region Properties

        /// <summary>
        /// Gets the ID of the Analytics account.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the name of the Analytics account.
        /// </summary>
        public string Name { get; }

        #endregion

        #region Constructors

        internal AnalyticsAccount(JObject obj) {
            Id = obj.GetString("id");
            Name = obj.GetString("name");
        }

        #endregion

        #region Static methods

        public static AnalyticsAccount Parse(JObject obj) {
            return obj == null ? null : new AnalyticsAccount(obj);
        }

        #endregion

    }

}