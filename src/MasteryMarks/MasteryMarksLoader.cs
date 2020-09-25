using Engine;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace MasteryMarks
{
    public class MasteryMarksLoader : Requester
    {
        public string Nickname { get; set; }

        public List<TankMasteryMarks> GetMasteryMarks()
        {
            var list = new List<TankMasteryMarks>();

            int accountId = GetAccountIdByNickname(Nickname);

            string contentAchievements = new RequestBuilder(AppId, "tanks/achievements")
            {
                Region = Region,
                Properties = new Dictionary<string, string>
                {
                    ["account_id"] = accountId.ToString()
                }
            }.Create().GetResponseContent();

            string contentStats = new RequestBuilder(AppId, "tanks/stats")
            {
                Region = Region,
                Properties = new Dictionary<string, string>
                {
                    ["account_id"] = accountId.ToString()
                }
            }.Create().GetResponseContent();

            var dataAchievements = JObject.Parse(contentAchievements);
            var dataStats = JObject.Parse(contentStats);


            if (dataAchievements.Value<string>("status") == "ok")
            {
                foreach (var item in dataAchievements["data"].AsJEnumerable().Values().Values())
                {
                    int tankId = item.Value<int>("tank_id");
                    JToken achievements = item["achievements"];
                    list.Add(new TankMasteryMarks
                    {
                        TankId = tankId,
                        TankName = GetTankNameById(tankId),
                        NumberOfBattles = -1,
                        NumberOfMasteryMarks = achievements.Value<int>("markOfMastery"),
                        NumberOfMastery1Marks = achievements.Value<int>("markOfMasteryI"),
                        NumberOfMastery2Marks = achievements.Value<int>("markOfMasteryII"),
                        NumberOfMastery3Marks = achievements.Value<int>("markOfMasteryIII")
                    });
                }
            }

            if (dataStats.Value<string>("status") == "ok")
            {
                foreach (var item in dataStats["data"].AsJEnumerable().Values().Values())
                {
                    int tankId = item.Value<int>("tank_id");
                    list.First(l => l.TankId == tankId).NumberOfBattles = item["all"].Value<int>("battles");
                }
            }

            return list;
        }
    }
}