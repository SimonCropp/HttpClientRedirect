namespace AssemblyToReference;

public class SentryHttpClientHandler :
    DelegatingHandler
{
    public static int SendWasCalled { get; set; }

    public SentryHttpClientHandler() :
        base(new HttpClientHandler())
    {
    }

    public SentryHttpClientHandler(HttpMessageHandler handler) :
        base(handler)
    {
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellation)
    {
        SendWasCalled ++;
        return base.SendAsync(request, cancellation);
    }
}