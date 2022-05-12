using AssemblyToReference;

public static class ClassToTest
{
    public static Task<string> MethodWithHttpClient()
    {
        var client = new HttpClient();
        return client.GetStringAsync("https://httpbin.org/status/200");
    }
    public static Task<string> MethodWithHttpClientAndHandler()
    {
        var client = new HttpClient(new HttpClientHandler());
        return client.GetStringAsync("https://httpbin.org/status/200");
    }
}