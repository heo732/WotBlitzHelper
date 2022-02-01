using Engine;
using System.Reflection;

static string GetAppId()
{
    using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Tester.AppId.txt");
    using var reader = new StreamReader(stream);
    return reader.ReadToEnd();
}

static void ShowUnexistingTanks(Requester requester)
{
    var allTanks = requester.GetAllTanks();
    var userTanksIds = requester.GetTanksIdFromUserStats("t3mp0");
    var unexistingTanks = allTanks.ExceptBy(userTanksIds, t => t.Id);

    Console.WriteLine($"Number of all available tanks in game: {allTanks.Count()}");
    Console.WriteLine($"Number of unexisting tanks for user: {unexistingTanks.Count()}");
    Console.WriteLine();
    Console.WriteLine("Unexisting tanks for user:");
    
    foreach (var tank in unexistingTanks)
    {
        Console.WriteLine($"({tank.Level}) {tank.Name} [{tank.Id}]");
    }
}

var appId = GetAppId();
var requester = new Requester(appId, Region.Ru);

ShowUnexistingTanks(requester);

Console.Read();
