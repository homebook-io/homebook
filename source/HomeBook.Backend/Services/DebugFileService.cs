using HomeBook.Backend.Abstractions;

namespace HomeBook.Backend.Services;

public class DebugFileService : IFileSystemService
{
    public bool FileExists(string path) => throw new NotImplementedException();

    public Task<string> FileReadAllTextAsync(string path, CancellationToken cancellationToken) => throw new NotImplementedException();

    public Task FileWriteAllTextAsync(string path, string content, CancellationToken cancellationToken) => throw new NotImplementedException();
}
