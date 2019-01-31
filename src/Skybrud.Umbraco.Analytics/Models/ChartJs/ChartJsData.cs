using System.Collections.Generic;
using Newtonsoft.Json;

namespace Skybrud.Umbraco.Analytics.Models.ChartJs {

    public class ChartJsData {

        [JsonProperty("labels")]
        public List<string> Labels { get; set; }

        [JsonProperty("rows")]
        public List<object> Rows { get; set; }

        [JsonProperty("datasets")]
        public List<ChartJsDataSet> DataSets { get; set; }

        public ChartJsData() {
            Labels = new List<string>();
            DataSets = new List<ChartJsDataSet>();
            Rows = new List<object>();
        }

    }

}