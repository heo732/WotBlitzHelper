using Engine.Models;
using Newtonsoft.Json.Linq;

namespace Engine;

public class Requester
{
    private string AppId { get; }
    private Region Region { get; }
    private Dictionary<int, string> TankIdToName { get; } = new();
    private HttpClient HttpClient { get; } = new();

    public Requester(string appId, Region region)
    {
        AppId = appId;
        Region = region;
    }

    public int GetAccountIdByNickname(string nickname)
    {
        string content = new RequestBuilder(AppId, "account/list", Region, new() { ["search"] = nickname })
            .Build(HttpClient).Run();

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
            LoadTanks();

        if (TankIdToName.ContainsValue(tankName))
            return TankIdToName.First(i => i.Value == tankName).Key;

        return -1;
    }

    public List<TankMasteryMarks> GetMasteryMarks(string nickname)
    {
        var list = new List<TankMasteryMarks>();

        int accountId = GetAccountIdByNickname(nickname);

        string contentAchievements = new RequestBuilder(AppId, "tanks/achievements", Region, new() { ["account_id"] = accountId.ToString() })
            .Build(HttpClient).Run();

        string contentStats = new RequestBuilder(AppId, "tanks/stats", Region, new() { ["account_id"] = accountId.ToString() })
            .Build(HttpClient).Run();

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
                list.First(l => l.TankId == tankId).NumberOfBattles = item["all"]?.Value<int>("battles") ?? -1;
            }
        }

        return list;
    }

    private void LoadTanks()
    {
        string content = new RequestBuilder(AppId, "encyclopedia/vehicles", Region)
            .Build(HttpClient).Run();

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
