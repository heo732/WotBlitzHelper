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
    var allTanks = requester.GetAllTanks().ToList();
    var userTanksIds = requester.GetTanksIdFromUserStats("t3mp0").ToList();
    var exceptionalAvailableTankIds = new[] { 21793 }; // Sheridan Rocket.
    var unexistingTanks = allTanks.ExceptBy(userTanksIds, t => t.Id).ExceptBy(exceptionalAvailableTankIds, t => t.Id).ToList();
    var unavailableInGame = userTanksIds.Except(allTanks.Select(t => t.Id)).ToList();

    Console.WriteLine($"Number of all available tanks in game: {allTanks.Count()} (=)");
    Console.WriteLine();
    Console.WriteLine($"Number of unexisting tanks for user: {unexistingTanks.Count()} (+)");
    Console.WriteLine($"Number of tanks in user's stats: {userTanksIds.Count()} (+)");
    Console.WriteLine($"Number of exceptional tanks available to user: {exceptionalAvailableTankIds.Count()} (+)");
    Console.WriteLine($"Number of tanks from user stats that are no more available in game: {unavailableInGame.Count()} (-)");
    Console.WriteLine();
    Console.WriteLine($"{unexistingTanks.Count()} + {userTanksIds.Count()} + {exceptionalAvailableTankIds.Count()} - {unavailableInGame.Count()} = {unexistingTanks.Count() + userTanksIds.Count() + exceptionalAvailableTankIds.Count() - unavailableInGame.Count()}  <=>  {allTanks.Count()}");
    Console.WriteLine();
    Console.WriteLine($"Unexisting tanks for user ({unexistingTanks.Count()}):");
    
    foreach (var tank in unexistingTanks.OrderBy(t => t.IsPremium).ThenBy(t => t.Level).ThenBy(t => t.Name))
    {
        Console.WriteLine($"({tank.Level}) {tank.Name} [{tank.Id}]");
    }
}

var appId = GetAppId();
var requester = new Requester(appId, Region.Ru);

ShowUnexistingTanks(requester);

Console.Read();
