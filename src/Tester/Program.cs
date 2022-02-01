using Engine;
using System.Reflection;

string GetAppId()
{
    using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Tester.AppId.txt");
    using var reader = new StreamReader(stream);
    return reader.ReadToEnd();
}

var appId = GetAppId();
var requester = new Requester(appId, Region.Ru);
Console.WriteLine(requester.GetTankIdByName("E 100"));
