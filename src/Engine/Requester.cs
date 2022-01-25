using Engine.Models;
using Newtonsoft.Json.Linq;

namespace Engine;

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
        }.Build().GetResponseContent();

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

    public List<TankMasteryMarks> GetMasteryMarks(string nickname)
    {
        var list = new List<TankMasteryMarks>();

        int accountId = GetAccountIdByNickname(nickname);

        string contentAchievements = new RequestBuilder(AppId, "tanks/achievements")
        {
            Region = Region,
            Properties = new Dictionary<string, string>
            {
                ["account_id"] = accountId.ToString()
            }
        }.Build().GetResponseContent();

        string contentStats = new RequestBuilder(AppId, "tanks/stats")
        {
            Region = Region,
            Properties = new Dictionary<string, string>
            {
                ["account_id"] = accountId.ToString()
            }
        }.Build().GetResponseContent();

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

    protected void LoadTanks()
    {
        string content = new RequestBuilder(AppId, "encyclopedia/vehicles")
        {
            Region = Region
        }.Build().GetResponseContent();

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
