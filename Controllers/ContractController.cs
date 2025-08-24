using ABCRetails.Models;
using ABCRetails.Services;
using Azure.Storage.Files.Shares.Models;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetails.Controllers;

public class ContractController : Controller
{
    private readonly FileService _fs;

    public ContractController(FileService fs)
    {
        _fs = fs; 
    }
    
    // GET: Contracts
    public async Task<IActionResult> Index()
    {
        var contracts = new List<Contract>();
        var filenames = await _fs.ListFiles("contracts");

        foreach (var filename in filenames)
        {
            contracts.Add
            (
                new Contract {Name = filename}
            );
        }
        
        return View(contracts);
    }
    
    // POST: Contracts/Upload
    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file.Length == 0)
        {
            return BadRequest();
        }
        
        await _fs.UploadFileAsync("contracts", file);
        
        return RedirectToAction("Index");
    }
    
    // POST: Contracts/Delete
    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        await _fs.DeleteFileAsync("contracts", id);
        
        return RedirectToAction("Index");
    }
    
    // GET: Contracts/Download
    public async Task<IActionResult> Download(string id)
    {
        ShareFileDownloadInfo download = await _fs.DownloadFileAsync("contracts", id);

        return File(download.Content, "application/octet-stream", id);
    }
}