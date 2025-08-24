using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;

namespace ABCRetails.Services;

public class FileService
{
    private readonly ShareClient _fs;

    public FileService(IConfiguration configuration)
    {
        var connectionString = configuration["ConnectionStrings:StorageAccount"];
        
        _fs = new ShareClient(connectionString, "abc-retails");
        _fs.CreateIfNotExists();
    }

    public async Task UploadFileAsync(string directory, IFormFile file)
    {
        var directoryClient = _fs.GetDirectoryClient(directory);
        await directoryClient.CreateIfNotExistsAsync();
        
        var shareFile = directoryClient.GetFileClient(file.FileName);
        
        // Upload file via stream
        await using Stream stream = file.OpenReadStream();
        await shareFile.CreateAsync(stream.Length);
        await shareFile.UploadRangeAsync
        (
            new HttpRange(0, stream.Length),
            stream
        );
    }

    public async Task<ShareFileDownloadInfo> DownloadFileAsync(string directory, string file)
    {
        var shareDirectory = _fs.GetDirectoryClient(directory);
        var shareFile = shareDirectory.GetFileClient(file);
        
        ShareFileDownloadInfo download = await shareFile.DownloadAsync();
        
        return download;
    }

    public async Task DeleteFileAsync(string directory, string file)
    {
        var shareDirectory = _fs.GetDirectoryClient(directory);
        var shareFile = shareDirectory.GetFileClient(file);
        
        await shareFile.DeleteAsync();
    }
    
    public async Task<List<string>> ListFiles(string directory)
    {
        var shareDirectory = _fs.GetDirectoryClient(directory);
        await shareDirectory.CreateIfNotExistsAsync();

        var items = shareDirectory.GetFilesAndDirectories();
        var names = new List<string>();

        foreach (var item in items)
        {
            names.Add(item.Name);
        }
        
        return names;
    }
}