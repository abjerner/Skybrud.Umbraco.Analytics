using System.Net;
using Newtonsoft.Json;

namespace Skybrud.Umbraco.Analytics.Models.Json {

    /// <summary>
    /// Class representing the a JSON response.
    /// </summary>
    public class JsonMetaResponse {

        #region Properties

        /// <summary>
        /// Gets or sets the meta data for the response.
        /// </summary>
        [JsonProperty(PropertyName = "meta")]
        public JsonMetaData Meta { get; set; }

        /// <summary>
        /// Gets or sets the data object.
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public object Data { get; set; }

        #endregion

        #region Constructors

        public JsonMetaResponse() {
            Meta = new JsonMetaData();
        }

        #endregion

        #region Member methods
        
        /// <summary>
        /// Creates a new success response object with a 200 status message.
        /// </summary>
        /// <param name="data">The data object.</param>
        public static JsonMetaResponse GetSuccess(object data) {
            return GetSuccess(data, HttpStatusCode.OK);
        }

        /// <summary>
        /// Creates a new success response with the specified <code>code</code>.
        /// </summary>
        /// <param name="data">The data object.</param>
        /// <param name="code">The status code of the response.</param>
        /// <returns></returns>
        public static JsonMetaResponse GetSuccess(object data, HttpStatusCode code) {
            return new JsonMetaResponse {
                Meta = { Code = code },
                Data = data
            };
        }

        /// <summary>
        /// Creates a new error response with the specified error message.
        /// </summary>
        /// <param name="error">The error message of the response.</param>
        public static JsonMetaResponse GetError(string error) {
            return GetError(HttpStatusCode.InternalServerError, error);
        }

        /// <summary>
        /// Creates a new error response with the specified status code and error message.
        /// </summary>
        /// <param name="code">The status code.</param>
        /// <param name="error">The error message of the response.</param>
        public static JsonMetaResponse GetError(HttpStatusCode code, string error) {
            return GetError(code, error, null);
        }

        /// <summary>
        /// Creates a new error response with the specified status code and error message.
        /// </summary>
        /// <param name="code">The status code.</param>
        /// <param name="error">The error message of the response.</param>
        /// <param name="data">The data object.</param>
        public static JsonMetaResponse GetError(HttpStatusCode code, string error, object data) {
            return new JsonMetaResponse {
                Meta = { Code = code, Error = error },
                Data = data
            };
        }

        #endregion

    }

}