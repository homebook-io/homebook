namespace HomeBook.Backend.Abstractions;

public interface IApplicationPathProvider
{
    string ConfigurationPath { get; }
    string RuntimeConfigurationFilePath { get; }
    string CacheDirectory { get; }
    string LogDirectory { get; }
    string DataDirectory { get; }
    string TempDirectory { get; }
}
