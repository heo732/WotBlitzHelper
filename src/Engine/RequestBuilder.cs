namespace Engine;

public class RequestBuilder
{
    public string AppId { get; private set; }
    public string RequestSpecialPart { get; private set; }
    private string BaseUri => $"https://api.wotblitz.{Region.Value}/wotb/{RequestSpecialPart}/?";
    public Region Region { get; private set; }
    public Dictionary<string, string> Properties { get; private set; }

    public RequestBuilder(string appId, string requestSpecificPart, Region region, Dictionary<string, string>? properties = default)
    {
        AppId = appId; 
        RequestSpecialPart = requestSpecificPart;
        Region = region;
        Properties = new(properties ?? new());
    }

    public Request Build()
    {
        var allProperties = new Dictionary<string, string>(Properties);
        allProperties.Add("application_id", AppId);
        return new Request(BaseUri + string.Join("&", allProperties.Select(p => $"{p.Key}={p.Value}")));
    }
}
