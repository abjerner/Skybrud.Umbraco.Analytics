using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Skybrud.Umbraco.Analytics.Models.ChartJs;

namespace Skybrud.Umbraco.Analytics.Models.Data.History {

    public class AnalyticsHistory {

        [JsonProperty("chart")]
        public ChartJsData Chart { get; set; }

    }

}