using Construction.Infrastructure.Helper;
using Construction.Infrastructure.Models;

using ConstructionApp.WebUI.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Rotativa.AspNetCore;
using System.Security.Cryptography;
using Syncfusion.Pdf;
using Syncfusion.HtmlConverter;
using System.IO;


namespace ConstructionApp.WebUI.Controllers
{
    public class PublicController : Controller
    {
        private IWebHostEnvironment _environment;
        private readonly ILogger<DrawingController> _logger;
        public PublicController(IWebHostEnvironment environment, ILogger<DrawingController> logger)
        {
            _environment = environment;
            _logger = logger;
        }
        [HttpGet]
        [Route("Public/MyFiles/{uId}")]
        public async Task<IActionResult> MyFiles(string? uId)
        {
            PublicUsersDTO objOutPut = new PublicUsersDTO();
            
            objOutPut.PublicFileList = await GetPublicFiles(uId);

            return View(objOutPut);
        }
        [Route("Public/MyFiles")]
        public async Task<IActionResult> MyFiles()
        {
            PublicUsersDTO objOutPut = new PublicUsersDTO();

          // objOutPut.PublicFileList = await GetPublicFiles(uId);

            return View(objOutPut);
        }
        public async Task<IActionResult> MyPhotoPdf()
        {
            PublicUsersDTO objOutPut = new PublicUsersDTO();
            return View(objOutPut);
        }
        public async Task<IActionResult> MyPdfDownload()
        {
            string uId = "3DFF5834-9209-4319-B762-5E2676AC245A";
            // Get the public files for the user
            PublicUsersDTO objOutPut = new PublicUsersDTO();
            objOutPut.PublicFileList = await GetPublicFiles(uId);

            // Generate the PDF as a byte array
            var pdfResult = new ViewAsPdf("MyPhotoPdf", objOutPut)
            {
                FileName = "Photo-files.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                CustomSwitches = "--disable-smart-shrinking --no-stop-slow-scripts --debug-javascript"
            };

            // Ensure the directory exists
            var pdfFolder = Path.Combine(_environment.WebRootPath, "GeneratedPdfs");
            if (!Directory.Exists(pdfFolder))
            {
                Directory.CreateDirectory(pdfFolder);
            }
            var pdfPath = Path.Combine(pdfFolder, "Photo-files.pdf");

            // Build the PDF file and save it
            var pdfBytes = await pdfResult.BuildFile(ControllerContext);
            await System.IO.File.WriteAllBytesAsync(pdfPath, pdfBytes);

            // Return a download link or success message
            var downloadUrl = Url.Content("~/GeneratedPdfs/Photo-files.pdf");
            ViewBag.PdfDownloadUrl = downloadUrl;
            return View(); // Create a simple view to show the download link
        }

        public async Task<List<ProjectFolderDTO>> GetDocumentFiles(int? pId, int? cId)
        {
            // DataTable dt = new DataTable();
            //  int? unitId = HttpContext.Session.GetInt32("UnitId");
            List<ProjectFolderDTO> PriorityLst = new List<ProjectFolderDTO>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiProjectFileUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetProjectCategoryFiles?pId=" + pId + "&cId=" + cId);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    PriorityLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProjectFolderDTO>>(data);

                    foreach (var item in PriorityLst)
                    {

                        item.EnFolderId = CommonHelper.EncryptURLHTML(item.Id.ToString());
                        //if (item.ProfileImage != null)
                        //        item.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);

                    }

                }


            }
            return PriorityLst;
        }


        public async Task<List<PublicUsersDTO>> GetPublicFiles(string? uId)
        {
            // DataTable dt = new DataTable();
            //  int? unitId = HttpContext.Session.GetInt32("UnitId");
            List<PublicUsersDTO> PriorityLst = new List<PublicUsersDTO>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiPhotoFileUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetPublicFilesAccess?uniqueId=" + uId);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    PriorityLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PublicUsersDTO>>(data);

                    foreach (var item in PriorityLst)
                    {
                        if (item.DocType == "D")
                        {
                            if (item.FolderId> 0)
                                item.FilePath = "/Documents/" + item.PId + "/" + item.CId + "/" + item.FolderId + "/" + item.FilePath;
                            else
                                item.FilePath = "/Documents/" + item.PId + "/" + item.CId + "/Files/" + item.FilePath;
                        }
                        if (item.DocType == "Dr")
                        {
                            if (item.FolderId > 0)
                                item.FilePath = "/Drawings/" + item.PId + "/" + item.CId + "/" + item.FolderId + "/" + item.FilePath;
                            else
                                item.FilePath = "/Drawings/" + item.PId + "/" + item.CId + "/Files/" + item.FilePath;
                        }
                        if (item.DocType == "PH")
                        {
                            if (item.FolderId > 0)
                                item.FilePath = "/Photos/" + item.PId + "/" + item.CId + "/" + item.FolderId + "/" + item.FilePath;
                            else
                                item.FilePath = "/Photos/" + item.PId + "/" + item.CId + "/Files/" + item.FilePath;
                        }


                        item.FileExt = Path.GetExtension(item.FilePath);


                    }

                }


            }
            return PriorityLst;
        }


        public IActionResult DownloadDoc(string fileName)
        {

            string folderpath = Path.Combine(this._environment.WebRootPath, "Documents");

            string[] paths = fileName.Split('/').Select(a => a.Trim()).ToArray();
            var filePath = Path.Combine(folderpath, paths[2], paths[3], paths[4], paths[5]);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            var contentType = GetContentType(filePath);
            var fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, contentType, paths[5]);
        }

        public IActionResult DownloadDrawing(string fileName)
        {

            string folderpath = Path.Combine(this._environment.WebRootPath, "Drawings");

            string[] paths = fileName.Split('/').Select(a => a.Trim()).ToArray();
            var filePath = Path.Combine(folderpath, paths[2], paths[3], paths[4], paths[5]);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            var contentType = GetContentType(filePath);
            var fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, contentType, paths[5]);
        }

        public IActionResult DownloadPhoto(string fileName)
        {

            string folderpath = Path.Combine(this._environment.WebRootPath, "Photos");

            string[] paths = fileName.Split('/').Select(a => a.Trim()).ToArray();
            var filePath = Path.Combine(folderpath, paths[2], paths[3], paths[4], paths[5]);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            var contentType = GetContentType(filePath);
            var fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, contentType, paths[5]);
        }
        private string GetContentType(string path)
        {
            var types = new Dictionary<string, string>
        {
            { ".doc", "application/msword" },
            { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            { ".pdf", "application/pdf" },
            { ".jpg", "image/jpeg" },
            { ".png", "image/png" }
        };

            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types.GetValueOrDefault(ext, "application/octet-stream");
        }
    }
}
