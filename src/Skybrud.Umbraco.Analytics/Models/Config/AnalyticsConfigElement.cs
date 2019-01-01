using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;

namespace Skybrud.Umbraco.Analytics.Models.Config {

    public class AnalyticsConfigElement {

        #region Properties

        [JsonProperty("clients")]
        public List<AnalyticsConfigClient> Clients { get; set; }

        #endregion

        #region COnstructors

        public AnalyticsConfigElement() {
            Clients = new List<AnalyticsConfigClient>();
        }
        
        public AnalyticsConfigElement(JObject obj) {
            Clients = obj.GetArrayItems("clients", AnalyticsConfigClient.Parse).ToList();
        }

        #endregion

        #region Static methods

        public static AnalyticsConfigElement Parse(JObject obj) {
            return obj == null ? null : new AnalyticsConfigElement(obj);
        }

        #endregion

    }

}