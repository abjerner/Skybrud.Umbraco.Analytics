using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;

namespace Skybrud.Umbraco.Analytics.Models.Selection {

    public class AnalyticsProfileSelection {

        #region Properties

        /// <summary>
        /// Gets a reference to the Google user of the selection.
        /// </summary>
        public AnalyticsUser User { get; }

        /// <summary>
        /// Gets a reference to the Analytics account of the selection.
        /// </summary>
        public AnalyticsAccount Account { get; }

        /// <summary>
        /// Gets a reference to the web property of the selection.
        /// </summary>
        public AnalyticsWebProperty WebProperty { get; }

        /// <summary>
        /// Gets a reference to the Analytics profile of the selection.
        /// </summary>
        public AnalyticsProfile Profile { get; }

        /// <summary>
        /// Gets whether the selection has a value.
        /// </summary>
        public bool IsValid => Profile != null;

        #endregion

        #region Constructors

        private AnalyticsProfileSelection(JObject obj) {
            User = obj.GetObject("user", AnalyticsUser.Parse);
            Account = obj.GetObject("account", AnalyticsAccount.Parse);
            WebProperty = obj.GetObject("webProperty", AnalyticsWebProperty.Parse);
            Profile = obj.GetObject("profile", AnalyticsProfile.Parse);
        }

        #endregion

        #region Static methods

        public static AnalyticsProfileSelection Deserialize(string str) {
            if (str == null || !str.StartsWith("{") || !str.EndsWith("}")) return new AnalyticsProfileSelection(null);
            return Parse(JsonConvert.DeserializeObject<JObject>(str));
        }

        public static AnalyticsProfileSelection Parse(JObject obj) {
            return obj == null ? null : new AnalyticsProfileSelection(obj);
        }

        #endregion

    }

}