using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Engine
{
    public class Requester
    {
        public string AppId { get; set; }

        public Region Region { get; set; } = Region.Ru;

        private string BaseUri => $"https://api.wotblitz.{Region}/wotb/account/list/?";

        public int GetAccountIdByNickname(string nickname)
        {
            string content = new RequestBuilder(AppId)
            {
                Region = Region,
                Properties = new Dictionary<string, string>
                {
                    ["search"] = nickname
                }
            }.Create().GetResponseContent();

            var data = JObject.Parse(content);
            if (data.ContainsKey("data"))
            {
                JToken dataValue = data.GetValue("data");
                return dataValue.AsJEnumerable().FirstOrDefault(p => p.Value<string>("nickname") == nickname)?.Value<int>("account_id") ?? -1;
            }

            return -1;
        }
    }
}