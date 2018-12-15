using System;
using System.Linq;
using System.Web.Mvc;
using Skybrud.Social.Google.Analytics.Endpoints;
using Skybrud.Social.Google.Analytics.Models.Common;
using Skybrud.Social.Google.Analytics.Models.Data;
using Skybrud.Social.Google.Analytics.Models.Dimensions;
using Skybrud.Social.Google.Analytics.Models.Metrics;
using Skybrud.Social.Google.Analytics.Options.Data;
using Skybrud.Social.Google.Analytics.Responses.Data;
using Skybrud.Social.Google.Common;
using Skybrud.Umbraco.Analytics.Extensions;
using Skybrud.Umbraco.Analytics.Models;
using Skybrud.WebApi.Json;
using Umbraco.Core.Configuration;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Skybrud.Umbraco.Analytics.Controllers.Api {

    [PluginController("Skybrud")]
    [JsonOnlyConfiguration]
    public class AnalyticsController : UmbracoAuthorizedApiController {

        [HttpGet]
        public object GetBlocks(int pageId) {

            return new {
                blocks = new [] {
                    new {
                        title = "History",
                        view = "/App_Plugins/Skybrud.Analytics/Views/Blocks/History.html",
                        periods = new [] {
                            new { name = "Yesterday", alias = "yesterday" },
                            new { name = "Last week", alias = "lastweek" },
                            new { name = "Last month", alias = "lastmonth" },
                            new { name = "Last year", alias = "lastyear" }
                        }
                    }
                }
            };

        }

        
        [HttpGet]
        public object GetHistory(int pageId, string period = "yesterday") {
            return GetData(pageId, period);
        }

        [HttpGet]
        public object GetData(int pageId, string period = "yesterday") {

            IPublishedContent content = Umbraco.Content(pageId);

            if (content == null) throw new Exception("Page not found or not published.");

            var config = UmbracoConfig.For.SkybrudAnalytics();

            GoogleService google = GoogleService.CreateFromRefreshToken(
                config.GoogleClientId,
                config.GoogleClientSecret,
                config.GoogleRefreshToken
            );

            Period p = Period.Parse(period);

            return new {
                period = p,
                page = new {
                    id = content.Id,
                    name = content.Name,
                    url = content.Url
                },
                history = GetHistory(google.Analytics, content, p)
            };

        }

        private object GetHistory(AnalyticsEndpoint analytics, IPublishedContent content, Period period) {

            // Declare the options for the request
            AnalyticsGetDataOptions options = new AnalyticsGetDataOptions {
                ProfileId = UmbracoConfig.For.SkybrudAnalytics().AnalyticsProfileId,
                StartDate = period.StartDate,
                EndDate = period.EndDate,
                Metrics = AnalyticsMetrics.Sessions + AnalyticsMetrics.Pageviews,
                Dimensions = AnalyticsDimensions.Hour
            };

            if (period.Days <= 1) {
                options.Dimensions = AnalyticsDimensions.Hour;
            } else if (period.Days <= 31) {
                options.Dimensions = AnalyticsDimensions.Date;
            } else {
                options.Dimensions = AnalyticsDimensions.YearWeek;
            }

            // Get the data from the Analytics API
            AnalyticsGetDataResponse response = analytics.Data.GetData(options);

            if (response.Body.Rows.Length == 0) {
                return new {
                    columns = new object[0],
                    rows = new object[0]
                };
            }

            object ddata;

            object[] columns = new object[response.Body.ColumnHeaders.Length];
            object[] rows = new object[response.Body.Rows.Length];

            for (int i = 0; i < response.Body.ColumnHeaders.Length; i++) {
                var column = response.Body.ColumnHeaders[i];
                columns[i] = new {
                    alias = column.Name.Substring(3),
                    label = column.Name
                };
            }

            for (int i = 0; i < response.Body.Rows.Length; i++) {

                AnalyticsDataRow row = response.Body.Rows[i];

                object[] rowdata = new object[row.Cells.Length];

                for (int j = 0; j < row.Cells.Length; j++) {
                    rowdata[j] = GetCellData(row.Cells[j]);
                }

                rows[i] = rowdata;

            }

            ddata = new { columns, rows };

            var datasets = new object[] {
                new {
                    label = "Pageviews",
                    fillColor = "#35353d",
                    strokeColor = "#35353d"
                },
                new  {
                    label = "Sessions",
                    fillColor = "red",//"rgba(141, 146, 157, 1)",
                    strokeColor = "red"//"rgba(141, 146, 157, 1)"
                }
            };

            object[] items = (
                from row in response.Body.Rows
                let first = row.Cells[0]
                select new {
                    label = FormatCell(first),
                    visits = FormatInt32(AnalyticsMetrics.Sessions, row),
                    pageviews = FormatInt32(AnalyticsMetrics.Pageviews, row)
                }
            ).ToArray();

            return new {
                hasData = response.Body.Rows.Any(x => x.GetInt32(AnalyticsMetrics.Sessions) > 0),
                data = ddata,
                datasets = datasets,
                items = items.ToArray()
            };

        }
        
        private static object GetCellData(AnalyticsDataCell cell) {

            object raw;
            object value;

            switch (cell.Column.DataType) {

                case AnalyticsDataType.Integer:
                    raw = cell.GetInt32();
                    value = cell.GetInt32().ToString("N0");
                    break;

                case AnalyticsDataType.Float:
                    raw = cell.GetDouble();
                    value = cell.GetDouble().ToString("N2");
                    break;

                default:
                    raw = cell.GetString();
                    value = cell.GetString() + " (" + cell.Column.ColumnType + ")";
                    break;

            }

            return new {
                raw, value
            };

        }

        internal object FormatInt32(IAnalyticsField field, AnalyticsDataRow row) {

            string key = field.Name.Substring(3);

            int value = (row == null ? 0 : row.GetInt32(field));

            return new {
                alias = key,
                label = field.ToString(),
                value = new { raw = value, text = value.ToString() }
            };

        }

        internal object FormatCell(AnalyticsDataCell cell) {

            string key = cell.Column.Name.Substring(3);

            string text = cell.Value;

            //switch (cell.Column.Name) {

            //    case "ga:date":
            //        {
            //            DateTime date = DateTime.ParseExact(text, "yyyyMMdd", null);
            //            if (Context.Culture.TwoLetterISOLanguageName == "en")
            //            {
            //                text = date.Day + GetDaySuffix(date.Day) + " of " + date.ToString("MMM");
            //            }
            //            else
            //            {
            //                text = DateTime.ParseExact(text, "yyyyMMdd", null).ToString("d. MMM", Context.Culture);
            //            }
            //            break;
            //        }

            //    case "ga:yearWeek":
            //        text = Context.Translate("analytics_week_x", text.Substring(4));
            //        break;

            //}

            return new {
                alias = key,
                label = cell.Column.Name,
                value = new { raw = cell.Value, text }
            };

        }

}

}