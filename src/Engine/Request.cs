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

    public async Task<string> RunAsync()
    {
        return await HttpClient.GetStringAsync(Uri).ConfigureAwait(false);
    }
}
