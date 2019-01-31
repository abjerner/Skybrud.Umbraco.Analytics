using System.Collections.Generic;
using Newtonsoft.Json;

namespace Skybrud.Umbraco.Analytics.Models.ChartJs {

    public class ChartJsDataSet {

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
        public List<string> Data { get; set; }

        public ChartJsDataSet() {
            Data = new List<string>();
        }

        public ChartJsDataSet(string label, string color) {
            Label = label;
            FillColor = StrokeColor = PointColor = PointHighlightStroke = color;
            PointStrokeColor = PointHighlightFill = "#fff";
            Data = new List<string>();
        }

    }

}