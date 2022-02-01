using Engine;
using System.Reflection;

static string GetAppId()
{
    using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Tester.AppId.txt");
    using var reader = new StreamReader(stream);
    return reader.ReadToEnd();
}

var appId = GetAppId();
var requester = new Requester(appId, Region.Ru);
Console.WriteLine(requester.IsTankExistsOnAccount("t3mp0", "E 100"));
