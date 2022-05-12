namespace AssemblyToReference;

public class SentryHttpClient :
    HttpClient
{
    public SentryHttpClient() :
        base (new SentryHttpClientHandler())
    {
    }
    public SentryHttpClient(HttpMessageHandler handler) :
        base (new SentryHttpClientHandler(handler))
    {
    }
}