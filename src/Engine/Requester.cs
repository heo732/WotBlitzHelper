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

    public async Task<int> GetAccountIdByNicknameAsync(string nickname)
    {
        string content = await new RequestBuilder(AppId, "account/list", Region, new() { ["search"] = nickname })
            .Build(HttpClient).RunAsync().ConfigureAwait(false);

        var data = JObject.Parse(content);
        if (data.Value<string>("status") == "ok")
        {
            return data.GetValue("data").AsJEnumerable()
                .FirstOrDefault(p => p.Value<string>("nickname") == nickname)
                ?.Value<int>("account_id") ?? -1;
        }

        return -1;
    }

    public async Task<Tank> GetTankByIdAsync(int tankId)
    {
        if (!Tanks.Any())
            await LoadTanksAsync().ConfigureAwait(false);

        return Tanks.FirstOrDefault(t => t.Id == tankId);
    }

    public async Task<Tank> GetTankByNameAsync(string tankName)
    {
        if (!Tanks.Any())
            await LoadTanksAsync().ConfigureAwait(false);

        return Tanks.FirstOrDefault(t => t.Name.Contains(tankName));
    }

    public async Task<IEnumerable<Tank>> GetAllTanksAsync()
    {
        if (!Tanks.Any())
            await LoadTanksAsync().ConfigureAwait(false);

        return Tanks;
    }

    public async Task<List<TankMasteryMarks>> GetMasteryMarksAsync(string nickname)
    {
        var list = new List<TankMasteryMarks>();

        int accountId = await GetAccountIdByNicknameAsync(nickname).ConfigureAwait(false);

        string contentAchievements = await new RequestBuilder(AppId, "tanks/achievements", Region, new() { ["account_id"] = accountId.ToString() })
            .Build(HttpClient).RunAsync().ConfigureAwait(false);

        string contentStats = await new RequestBuilder(AppId, "tanks/stats", Region, new() { ["account_id"] = accountId.ToString() })
            .Build(HttpClient).RunAsync().ConfigureAwait(false);

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
                    TankName = (await GetTankByIdAsync(tankId).ConfigureAwait(false))?.Name,
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
    public async Task<bool> IsTankExistsOnAccount(string nickname, string tankName)
    {
        var accId = await GetAccountIdByNicknameAsync(nickname).ConfigureAwait(false);
        var tank = await GetTankByNameAsync(tankName).ConfigureAwait(false);
        var content = await new RequestBuilder(AppId, "tanks/stats", Region, new() { ["account_id"] = accId.ToString(), ["tank_id"] = tank.Id.ToString() }).Build(HttpClient).RunAsync().ConfigureAwait(false);
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

    public async Task<IEnumerable<int>> GetTanksIdFromUserStatsAsync(string nickname)
    {
        string content = await new RequestBuilder(AppId, "tanks/stats", Region, new() { ["account_id"] = (await GetAccountIdByNicknameAsync(nickname).ConfigureAwait(false)).ToString() })
            .Build(HttpClient).RunAsync().ConfigureAwait(false);

        var result = new List<int>();
        var data = JObject.Parse(content);
        if (data.Value<string>("status") == "ok")
        {
            foreach (var item in data["data"].AsJEnumerable().Values().Values())
            {
                result.Add(item.Value<int>("tank_id"));
            }
        }

        return result;
    }

    #endregion

    #region Private methods

    private async Task LoadTanksAsync()
    {
        string content = await new RequestBuilder(AppId, "encyclopedia/vehicles", Region)
            .Build(HttpClient).RunAsync().ConfigureAwait(false);

        lock (Tanks)
        {
            if (Tanks.Any())
                return;

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
    }

    #endregion
}
