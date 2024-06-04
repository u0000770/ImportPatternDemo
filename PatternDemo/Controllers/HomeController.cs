using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using PatternDemo.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using StrategyRepository;
using GenRepository;
using Repository;
using Domain;
using StrategyRepo;

namespace PatternDemo.Controllers
{
    public class HomeController : Controller
    {

        #region construction
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IRepository<Item> _repository;

        public HomeController(IWebHostEnvironment hostingEnvironment, IRepository<Item> repository )
        {
            _hostingEnvironment = hostingEnvironment;
            _repository = repository;
        }
        #endregion

        #region Actions
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("FileUpload")]
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
                        // managing file location and name/type
                        var originalFileName = formFile.FileName;
                        var extension = Path.GetExtension(originalFileName);
                        extensions.Add(extension);
                        var evnPath = GetDataFilePath();
                        var relativePath = Path.Combine(evnPath, Guid.NewGuid().ToString() + extension);
                        filePaths.Add(relativePath);
                        // copy file to local file store
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
        #endregion

        #region FileManagement

        /// <summary>
        /// uses host environment to get the path to our local data store
        /// </summary>
        /// <returns></returns>
        private string GetDataFilePath()
        {
            string contentRootPath = _hostingEnvironment.ContentRootPath;
            string dataFilePath = Path.Combine(contentRootPath, "AppData");
            return dataFilePath;
        }


        /// <summary>
        /// The Client Code that selects the appropriate strategy and injects the repository
        /// </summary>
        /// <param name="filePaths"></param>
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
                if (ext == ".csv" )
                {
					importerStrategy.SetImportStrategy(new CSVImportStrategy());
					bool csvImportResult = importerStrategy.ImportData(file, _repository);
				}
                if (ext == ".json")
                {
					importerStrategy.SetImportStrategy(new JSONImportStrategy());
					bool jsonImportResult = importerStrategy.ImportData(file, _repository);
				}
            }

        }

        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }




}

