using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;

namespace Skybrud.Umbraco.Analytics.Models.Selection {

    public class AnalyticsWebProperty {

        #region Properties

        /// <summary>
        /// Gets the ID of the web property.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the name of the web property.
        /// </summary>
        public string Name { get; }

        #endregion

        #region Constructors

        internal AnalyticsWebProperty(JObject obj) {
            Id = obj.GetString("id");
            Name = obj.GetString("name");
        }

        #endregion

        #region Static methods

        public static AnalyticsWebProperty Parse(JObject obj) {
            return obj == null ? null : new AnalyticsWebProperty(obj);
        }

        #endregion

    }

}