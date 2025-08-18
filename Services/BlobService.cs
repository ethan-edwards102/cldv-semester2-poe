using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace ABCRetails.Services;

public class BlobService
{
    private readonly BlobContainerClient _containerClient;

    public BlobService(IConfiguration configuration)
    {
        var connectionString = configuration["ConnectionStrings:StorageAccount"];
        var containerName = "abc-retails";
        
        _containerClient = new BlobContainerClient(connectionString, containerName);
        _containerClient.CreateIfNotExists(PublicAccessType.Blob);
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        var blobClient = _containerClient.GetBlobClient(file.FileName);

        using (var stream = file.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, true);
        }
        
        return blobClient.Uri.ToString();
    }

    public async Task DeleteFileAsync(string url)
    {
        var blobName = Path.GetFileName(new Uri(url).LocalPath);
        
        var blobClient = _containerClient.GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync();
    }
}