using System.Net;

namespace Engine;

public class Request
{
    public string Uri { get; private set; }

    public Request(string uri)
    {
        Uri = uri ?? string.Empty;
    }

    public string GetResponseContent()
    {
        var request = WebRequest.Create(Uri);
        request.Method = "GET";
        using var response = request.GetResponse();
        using var stream = response.GetResponseStream();
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
