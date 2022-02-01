namespace Engine;

internal class RequestBuilder
{
    private string AppId { get; }
    private string RequestSpecialPart { get; }
    private string BaseUri => $"https://api.wotblitz.{Region}/wotb/{RequestSpecialPart}/?";
    private Region Region { get; }
    private Dictionary<string, string> Properties { get; }

    public RequestBuilder(string appId, string requestSpecificPart, Region region, Dictionary<string, string> properties = default)
    {
        AppId = appId; 
        RequestSpecialPart = requestSpecificPart;
        Region = region;
        Properties = new(properties ?? new());
    }

    public Request Build(HttpClient httpClient)
    {
        var allProperties = new Dictionary<string, string>(Properties);
        allProperties.Add("application_id", AppId);
        return new Request(BaseUri + string.Join("&", allProperties.Select(p => $"{p.Key}={p.Value}")), httpClient);
    }
}
