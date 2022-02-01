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
    var totalNumber = allTanks.Count();
    var totalCounter = 0;
    var unexistingCounter = 0;
    
    foreach (var tank in allTanks)
    {
        totalCounter++;
        if (!requester.IsTankExistsOnAccount("t3mp0", tank.Name))
        {
            unexistingCounter++;
            Console.WriteLine($"Tank not exists on account: {tank.Name}   ({tank.Level})");
            Console.WriteLine($"({totalCounter} of {totalNumber})");
        }
    }
    
    Console.WriteLine($"Number of unexisting tanks: {unexistingCounter}");
}

var appId = GetAppId();
var requester = new Requester(appId, Region.Ru);

ShowUnexistingTanks(requester);

Console.Read();
