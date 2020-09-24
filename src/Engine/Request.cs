using System.IO;
using System.Net;

namespace Engine
{
    public class Request
    {
        public string Uri { get; private set; }

        public Request(string uri)
        {
            Uri = uri ?? "";
        }

        public string GetResponseContent()
        {
            var request = WebRequest.Create(Uri);
            request.Method = "GET";
            using (WebResponse response = request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}