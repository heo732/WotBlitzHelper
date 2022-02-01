using Engine;

var appId = File.ReadAllText(@"..\..\..\..\appId.txt");
var requester = new Requester(appId, Region.Ru);
Console.WriteLine(requester.GetTankIdByName("E 100"));
