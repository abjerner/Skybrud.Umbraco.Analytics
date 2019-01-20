using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;

namespace Skybrud.Umbraco.Analytics.Models.Selection {

    /// <summary>
    /// Class describing the Google user of an Analytics site selection.
    /// </summary>
    public class AnalyticsUser {

        #region Properties

        /// <summary>
        /// Gets the ID of the Google user.
        /// </summary>
        public string Id { get; }

        #endregion

        #region Constructors

        internal AnalyticsUser(JObject obj) {
            Id = obj.GetString("id");
        }

        #endregion

        #region Static methods

        public static AnalyticsUser Parse(JObject obj) {
            return obj == null ? null : new AnalyticsUser(obj);
        }

        #endregion

    }

}