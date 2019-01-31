using System.Collections.Generic;
using Newtonsoft.Json;

namespace Skybrud.Umbraco.Analytics.Models.Data.History {

    public class AnalyticsHistoryDataSet {

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("fillColor")]
        public string FillColor { get; set; }

        [JsonProperty("strokeColor")]
        public string StrokeColor { get; set; }

        [JsonProperty("pointColor")]
        public string PointColor { get; set; }

        [JsonProperty("pointStrokeColor")]
        public string PointStrokeColor { get; set; }

        [JsonProperty("pointHighlightFill")]
        public string PointHighlightFill { get; set; }

        [JsonProperty("pointHighlightStroke")]
        public string PointHighlightStroke { get; set; }

        [JsonProperty("data")]
        public List<AnalyticsHistoryDataItem> Data { get; set; }

        public AnalyticsHistoryDataSet() {
            Data = new List<AnalyticsHistoryDataItem>();
        }

        public AnalyticsHistoryDataSet(string label, string color) {
            Label = label;
            FillColor = StrokeColor = PointColor = PointHighlightStroke = color;
            PointStrokeColor = PointHighlightFill = "#fff";
            Data = new List<AnalyticsHistoryDataItem>();
        }

    }

}