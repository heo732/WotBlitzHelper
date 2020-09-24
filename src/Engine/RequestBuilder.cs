using System.Collections.Generic;
using System.Linq;

namespace Engine
{
    public class RequestBuilder
    {
        private string BaseUri => $"https://api.wotblitz.{Region.Value}/wotb/account/list/?";

        private string AppId { get; set; }

        public Region Region { get; set; } = Region.Ru;

        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

        public RequestBuilder(string appId)
        {
            AppId = appId ?? "";
        }

        public Request Create()
        {
            var allProperties = new Dictionary<string, string>(Properties)
            {
                { "application_id", AppId }
            };
            return new Request(BaseUri + string.Join("&", allProperties.Select(p => $"{p.Key}={p.Value}")));
        }
    }
}