using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Engine
{
    public class Requester
    {
        public string AppId { get; set; } = File.ReadAllText(@"..\..\..\..\appId.txt");

        public Region Region { get; set; } = Region.Ru;

        private Dictionary<int, string> TankIdToName { get; set; } = new Dictionary<int, string>();

        public int GetAccountIdByNickname(string nickname)
        {
            string content = new RequestBuilder(AppId, "account/list")
            {
                Region = Region,
                Properties = new Dictionary<string, string>
                {
                    ["search"] = nickname
                }
            }.Create().GetResponseContent();

            var data = JObject.Parse(content);
            if (data.Value<string>("status") == "ok")
            {
                return data.GetValue("data").AsJEnumerable()
                    .FirstOrDefault(p => p.Value<string>("nickname") == nickname)
                    ?.Value<int>("account_id") ?? -1;
            }

            return -1;
        }

        public string GetTankNameById(int tankId)
        {
            if (TankIdToName.Count == 0)
            {
                LoadTanks();
            }
            if (TankIdToName.ContainsKey(tankId))
            {
                return TankIdToName[tankId];
            }
            return "";
        }

        public int GetTankIdByName(string tankName)
        {
            if (TankIdToName.Count == 0)
            {
                LoadTanks();
            }
            if (TankIdToName.ContainsValue(tankName))
            {
                return TankIdToName.First(i => i.Value == tankName).Key;
            }
            return -1;
        }

        protected void LoadTanks()
        {
            string content = new RequestBuilder(AppId, "encyclopedia/vehicles")
            {
                Region = Region
            }.Create().GetResponseContent();

            var data = JObject.Parse(content);
            if (data.Value<string>("status") == "ok")
            {
                foreach (var item in data["data"].AsJEnumerable().Values())
                {
                    TankIdToName.Add(item.Value<int>("tank_id"), item.Value<string>("name"));
                }
            }
        }
    }
}