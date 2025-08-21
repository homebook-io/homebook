namespace HomeBook.Backend.EnvironmentHandler;

public static class PathHandler
{
    // the base-paht is the parent path inside the docker container which should be mounted to the host system
    private const string BASE_PATH = "/var/lib/homebook";

    public static readonly string ConfigurationPath = Path.Combine(BASE_PATH, "config");
    public static readonly string CacheDirectory = Path.Combine(BASE_PATH, "cache");
    public static readonly string LogDirectory = Path.Combine(BASE_PATH, "logs");
    public static readonly string DataDirectory = Path.Combine(BASE_PATH, "data");
    public static readonly string TempDirectory = Path.Combine(BASE_PATH, "temp");
}
