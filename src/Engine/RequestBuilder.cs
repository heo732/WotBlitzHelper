namespace Engine;

public class RequestBuilder
{
    private string AppId { get; set; }
    private string RequestSpecificPart { get; set; }
    private string BaseUri => $"https://api.wotblitz.{Region.Value}/wotb/{RequestSpecificPart}/?";
    public Region Region { get; set; } = Region.Ru;
    public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

    public RequestBuilder(string appId, string requestSpecificPart)
    {
        AppId = appId ?? string.Empty; 
        RequestSpecificPart = requestSpecificPart;
    }

    public Request Build()
    {
        var allProperties = new Dictionary<string, string>(Properties)
        {
            { "application_id", AppId }
        };
        return new Request(BaseUri + string.Join("&", allProperties.Select(p => $"{p.Key}={p.Value}")));
    }
}
