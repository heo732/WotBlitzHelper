using Engine.Models;
using Newtonsoft.Json.Linq;

namespace Engine;

public class Requester
{
    private string AppId { get; }
    private Region Region { get; }
    private List<Tank> Tanks { get; } = new();
    private HttpClient HttpClient { get; } = new();

    public Requester(string appId, Region region)
    {
        AppId = appId;
        Region = region;
    }

    #region Public methods

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

    public Tank GetTankById(int tankId)
    {
        if (!Tanks.Any())
            LoadTanks();

        return Tanks.FirstOrDefault(t => t.Id == tankId);
    }

    public Tank GetTankByName(string tankName)
    {
        if (!Tanks.Any())
            LoadTanks();

        return Tanks.FirstOrDefault(t => t.Name.Contains(tankName));
    }

    public IEnumerable<Tank> GetAllTanks()
    {
        if (!Tanks.Any())
            LoadTanks();

        return Tanks;
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
                    TankName = GetTankById(tankId)?.Name,
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

    /// <summary>
    /// TODO: Improve this method.
    /// </summary>
    public bool IsTankExistsOnAccount(string nickname, string tankName)
    {
        var accId = GetAccountIdByNickname(nickname);
        var tank = GetTankByName(tankName);
        var content = new RequestBuilder(AppId, "tanks/stats", Region, new() { ["account_id"] = accId.ToString(), ["tank_id"] = tank.Id.ToString() }).Build(HttpClient).Run();
        var data = JObject.Parse(content);
        if (data.Value<string>("status") == "ok")
        {
            foreach (var item in data["data"].AsJEnumerable().Values())
            {
                return item.HasValues;
            }
        }

        return false;
    }

    public IEnumerable<int> GetTanksIdFromUserStats(string nickname)
    {
        string content = new RequestBuilder(AppId, "tanks/stats", Region, new() { ["account_id"] = GetAccountIdByNickname(nickname).ToString() })
            .Build(HttpClient).Run();

        var data = JObject.Parse(content);
        if (data.Value<string>("status") == "ok")
        {
            foreach (var item in data["data"].AsJEnumerable().Values().Values())
            {
                yield return item.Value<int>("tank_id");
            }
        }
    }

    #endregion

    #region Private methods

    private void LoadTanks()
    {
        string content = new RequestBuilder(AppId, "encyclopedia/vehicles", Region)
            .Build(HttpClient).Run();

        var data = JObject.Parse(content);
        if (data.Value<string>("status") == "ok")
        {
            foreach (var item in data["data"].AsJEnumerable().Values())
            {
                Tanks.Add(new()
                {
                    Id = item.Value<int>("tank_id"),
                    Name = item.Value<string>("name"),
                    IsPremium = item.Value<bool>("is_premium"),
                    Level = item.Value<int>("tier")
                });
            }
        }
    }

    #endregion
}
