public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifyHttp.Enable();
        VerifierSettings
            .ScrubLinesContaining("traceparent");
    }
}