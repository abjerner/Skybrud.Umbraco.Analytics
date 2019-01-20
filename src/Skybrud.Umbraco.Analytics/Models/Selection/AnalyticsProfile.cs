using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;

namespace Skybrud.Umbraco.Analytics.Models.Selection {

    public class AnalyticsProfile {

        #region Properties

        public string Id { get; }

        public string Name { get; }

        public string Currency { get; }

        public string Timezone { get; }

        public string WebsiteUrl { get; }

        public string Type { get; }

        #endregion

        #region Constructors

        internal AnalyticsProfile(JObject obj) {
            Id = obj.GetString("id");
            Name = obj.GetString("name");
            Currency = obj.GetString("currency");
            Timezone = obj.GetString("timezone");
            WebsiteUrl = obj.GetString("websiteUrl");
            Type = obj.GetString("type");
        }

        #endregion

        #region Static methods

        public static AnalyticsProfile Parse(JObject obj) {
            return obj == null ? null : new AnalyticsProfile(obj);
        }

        #endregion

    }

}