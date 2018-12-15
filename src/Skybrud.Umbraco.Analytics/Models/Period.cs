using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Skybrud.Umbraco.Analytics.Models {

    public class Period {
        
        [JsonProperty("alias")]
        public string Alias { get; }

        [JsonProperty("startDate")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime StartDate { get; }

        [JsonProperty("endDate")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime EndDate { get; }

        [JsonIgnore]
        public int Days => (int) Math.Ceiling(EndDate.Subtract(StartDate).TotalDays);

        public Period(string alias, string startDate) {
            Alias = alias;
            StartDate = DateTime.Parse(startDate);
            EndDate = StartDate;
        }

        public Period(string alias, string startDate, string endDate) {
            Alias = alias;
            StartDate = DateTime.Parse(startDate);
            EndDate = DateTime.Parse(endDate);
        }

        public Period(string alias, DateTime startDate, DateTime endDate) {
            Alias = alias;
            StartDate = startDate;
            EndDate = endDate;
        }

        public static Period Parse(string period) {

            Match match1 = Regex.Match(period ?? "", "^([0-9]{8})$");
            Match match2 = Regex.Match(period ?? "", "^([0-9]{8})-([0-9]{8})$");

            if (match1.Success) return new Period("custom", match1.Groups[1].Value);
            if (match2.Success) return new Period("custom", match2.Groups[2].Value, match2.Groups[1].Value);

            DateTime yesterday = DateTime.Today.AddDays(-1);

            switch (period) {

                case "lastweek":
                    return new Period(period, yesterday.AddDays(-7), yesterday);

                case "lastmonth":
                    return new Period(period, yesterday.AddDays(-31), yesterday);

                case "lastyear":
                    return new Period(period, yesterday.AddDays(-365), yesterday);

                default:
                    return new Period("yesterday", yesterday, yesterday);

            }

        }

    }

}