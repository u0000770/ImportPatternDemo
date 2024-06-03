using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using PatternDemo.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using StrategyRepository;
using GenRepository;
using Repository;
using Domain;

namespace PatternDemo.Controllers
{
    public class HomeController : Controller
    {
       // private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IRepository<Item> _repository;

        public HomeController(IWebHostEnvironment hostingEnvironment, IRepository<Item> repository )
        {
           // _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public string GetDataFilePath()
        {
            string contentRootPath = _hostingEnvironment.ContentRootPath;
            string dataFilePath = Path.Combine(contentRootPath, "AppData");
            return dataFilePath;
        }

        [HttpPost("FileUpload")]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> FileUpload(List<Microsoft.AspNetCore.Http.IFormFile> files)
        {
            try
            {
                // Handle file upload logic here
                long size = files.Sum(f => f.Length);
                var filePaths = new List<string>();
                var originalFileNames = new List<string>(); // List to store original file names
                var extensions = new List<string>(); // List to store file extensions

                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
    
                        var originalFileName = formFile.FileName;
                        var extension = Path.GetExtension(originalFileName);
                        extensions.Add(extension);
                        var evnPath = GetDataFilePath();
                        var relativePath = Path.Combine(evnPath, Guid.NewGuid().ToString() + extension);
                        filePaths.Add(relativePath);
                        using (var stream = new FileStream(relativePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                    }
                }
                // process uploaded files
                ProcessAndSaveData(filePaths);

                return RedirectToAction("index", "Items");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return View();
        }

        // The Client Code that uses the strategy and injects the repository
        public void ProcessAndSaveData(List<string> filePaths)
        {
			ImporterStrategyContext importerStrategy = new ImporterStrategyContext();
            foreach (var file in filePaths)
            {
                var ext = Path.GetExtension(file);
                if (ext == ".xlsx" || ext == ".xls")
                {
                    importerStrategy.SetImportStrategy(new ExcelImportStrategy());
                    bool excelImportResult = importerStrategy.ImportData(file, _repository);
                }
                if (ext == "csv" )
                {
					importerStrategy.SetImportStrategy(new CSVImportStrategy());
                }
                if (ext == "json")
                {
					importerStrategy.SetImportStrategy(new JSONImportStrategy());
                }
            }

        }
    }


}

