using System.Net.Http;
using Skybrud.Umbraco.Analytics.Models.Json;

namespace Skybrud.Umbraco.Analytics.Extensions {

    public static class AnalyticsExtensions {

        /// <summary>
        /// Creates a new <see cref="HttpResponseMessage"/> based on the specified <see cref="JsonMetaResponse"/>. The
        /// status code of the server response will automatically be derived from the <see cref="JsonMetaResponse"/>.
        /// </summary>
        /// <param name="request">The current request.</param>
        /// <param name="response">The meta response object to be returned.</param>
        public static HttpResponseMessage CreateResponse(this HttpRequestMessage request, JsonMetaResponse response) {
            return request.CreateResponse(response.Meta.Code, response);
        }

    }

}