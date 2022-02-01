namespace Engine;

internal class Request
{
    private string Uri { get; }
    private HttpClient HttpClient { get; }

    public Request(string uri, HttpClient httpClient)
    {
        Uri = uri;
        HttpClient = httpClient;
    }

    public string Run()
    {
        return HttpClient.GetStringAsync(Uri).GetAwaiter().GetResult();
    }
}
