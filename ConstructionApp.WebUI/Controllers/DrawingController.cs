using Construction.Infrastructure.Helper;
using Construction.Infrastructure.KeyValues;
using Construction.Infrastructure.Models;
using ConstructionApp.WebUI.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Security.Cryptography;
using static NuGet.Packaging.PackagingConstants;

namespace ConstructionApp.WebUI.Controllers
{


    public class DrawingController : Controller
    {

        private readonly ILogger<DrawingController> _logger;
        private IWebHostEnvironment _environment;
        private readonly IMemoryCache _memoryCache;
        public DrawingController(ILogger<DrawingController> logger, IWebHostEnvironment environment, IMemoryCache memoryCache)
        {
            _logger = logger;
            _environment = environment;
            _memoryCache = memoryCache;
        }
        public async Task<IActionResult> ProjectDirectory()
        {
            ProjectsDTO objOutPut = new ProjectsDTO();


            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (strMenuSession == null)
                return RedirectToAction("Index", "Home");

            AccessMasterDTO? empSession = (AccessMasterDTO?)(JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession));

            objOutPut.AccessList = empSession.AccessList.ToList();
            objOutPut.UserType = HttpContext.Session.GetInt32("UserType");

          objOutPut.ProjectDashboardList = await GetProjectDashboard(HttpContext.Session.GetInt32("UserId"));
            _memoryCache.Set("Projects", objOutPut.ProjectDashboardList);
            return View(objOutPut);
        }

        public async Task<List<ProjectDashboardDTO>>? GetProjectDashboard(int? unitId)
        {
            // DataTable dt = new DataTable();
            List<ProjectDashboardDTO> ProjectLst = new List<ProjectDashboardDTO>();

            // int countryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ecountryId));
            //  inputs.CountryId = countryId;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiProjectUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                //HttpResponseMessage response = await client.GetAsync("GetUserListing?userType=" + userType + "&logInUser="+ logInUser);
                HttpResponseMessage response = await client.GetAsync("GetProjectDashboard?unitId=" + unitId);
                //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    ProjectLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProjectDashboardDTO>>(data);

                    foreach (var item in ProjectLst)
                    {

                        item.EncProjectId = CommonHelper.EncryptURLHTML(item.ProjectId.ToString());
                        //if (item.ProfileImage != null)
                        //        item.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);

                    }



                }

                return ProjectLst;
            }






        }


        [HttpGet]
        [Route("Drawing/ProjectCategoryDirectory/{epId}")]
        public async Task<IActionResult> ProjectCategoryDirectory(string epId)
        {
            ProjectsDTO objOutPut = new ProjectsDTO();
            int pId = Convert.ToInt32(CommonHelper.DecryptURLHTML(epId));
            HttpContext.Session.SetInt32("PID", pId);
            HttpContext.Session.SetString("ePID", epId);
            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (strMenuSession == null)
                return RedirectToAction("Index", "Home");
            AccessMasterDTO? empSession = JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession);
            objOutPut.AccessList = empSession?.AccessList?.ToList() ?? new List<AccessMasterDTO>();
            var userId = HttpContext.Session.GetInt32("UserId");
            var unitId = HttpContext.Session.GetInt32("UnitId");
            var projectCacheKey = $"Projects_{unitId}_{pId}";
            if (_memoryCache.TryGetValue(projectCacheKey, out var cachedProjectListObj) && cachedProjectListObj is List<ProjectDashboardDTO> cachedProjectList && cachedProjectList != null)
            {
                objOutPut.ProjectDashboardList = cachedProjectList;
            }
            else
            {
                var dashboard = await GetProjectDashboard(userId);
                objOutPut.ProjectDashboardList = dashboard ?? new List<ProjectDashboardDTO>();
                _memoryCache.Set(projectCacheKey, objOutPut.ProjectDashboardList);
            }
            objOutPut.ProjectName = objOutPut.ProjectDashboardList?.Where(p => p.ProjectId == pId).Select(m => m.ProjectName).FirstOrDefault() ?? string.Empty;
            objOutPut.UserType = HttpContext.Session.GetInt32("UserType");
            //var drawingCategoryCacheKey = $"DrawingCategory_{pId}";
            //if (_memoryCache.TryGetValue(drawingCategoryCacheKey, out var cachedCategoryListObj) && cachedCategoryListObj is List<DrawingCategoryDTO> cachedCategoryList && cachedCategoryList != null)
            //{
            //    objOutPut.DrawingCategoryList = cachedCategoryList;
            //}
            //else
            //{
                objOutPut.DrawingCategoryList = await GetCategoryList() ?? new List<DrawingCategoryDTO>();
                foreach (var item in objOutPut.DrawingCategoryList)
                {
                    item.EncCategoryId = CommonHelper.EncryptURLHTML(item.Id.ToString());
                }
            //  _memoryCache.Set(drawingCategoryCacheKey, objOutPut.DrawingCategoryList, TimeSpan.FromMinutes(30));
            // }
            objOutPut.ProjectId = pId;
            return View(objOutPut);
        }

        public async Task<IActionResult> ProjectCategoryDirectory()
        {
            ProjectsDTO objOutPut = new ProjectsDTO();

            int? userId = HttpContext.Session.GetInt32("UserId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            string projectDashboardKey = userId != null && unitId != null ? $"Dashboard_{unitId}_{userId}" : "Dashboard_Global";
            string taskDashboardKey = userId != null && unitId != null ? $"TskDashboard_{unitId}_{userId}" : "TskDashboard_Global";

            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (strMenuSession == null)
                return RedirectToAction("Index", "Home");

            var empSession = JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession);
            objOutPut.AccessList = empSession?.AccessList?.ToList() ?? new List<AccessMasterDTO>();
            objOutPut.UserType = HttpContext.Session.GetInt32("UserType");
            if (_memoryCache.TryGetValue(projectDashboardKey, out var cachedDashboardObj) && cachedDashboardObj is List<ProjectsDashboardDTO> cachedDashboard && cachedDashboard != null)
            {
                if (cachedDashboard[0].TotalP <= 0)
                    return View(objOutPut);
            }

            int pId = 0;
            var ePID = HttpContext.Session.GetString("ePID");
            if (!string.IsNullOrEmpty(ePID))
            {
                pId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ePID));
            }
           // int pId = Convert.ToInt32(CommonHelper.DecryptURLHTML(epId));

            HttpContext.Session.SetInt32("PID", pId);
           // HttpContext.Session.SetString("ePID", epId);
            //string? strMenuSession = HttpContext.Session.GetString("Menu");
            //if (strMenuSession == null)
            //    return RedirectToAction("Index", "Home");

            //AccessMasterDTO? empSession = (AccessMasterDTO?)(JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession));
            //objOutPut.AccessList = empSession.AccessList.ToList();


            //var userId = HttpContext.Session.GetInt32("UserId");
            //int? unitId = HttpContext.Session.GetInt32("UnitId");
            var projectCacheKey = $"Projects_{unitId}_{pId}";
            if (_memoryCache.TryGetValue(projectCacheKey, out var cachedProjectListObj) && cachedProjectListObj is List<ProjectDashboardDTO> cachedProjectList && cachedProjectList != null)
            {
                objOutPut.ProjectDashboardList = cachedProjectList;
            }
            else
            {
                var dashboard = await GetProjectDashboard(userId);
                objOutPut.ProjectDashboardList = dashboard ?? new List<ProjectDashboardDTO>();
                _memoryCache.Set(projectCacheKey, objOutPut.ProjectDashboardList);
            }
            objOutPut.ProjectName = objOutPut.ProjectDashboardList?.Where(p => p.ProjectId == pId).Select(m => m.ProjectName).FirstOrDefault() ?? string.Empty;
            objOutPut.UserType = HttpContext.Session.GetInt32("UserType");
            var drawingCategoryCacheKey = $"DrawingCategory_{pId}";
            if (_memoryCache.TryGetValue(drawingCategoryCacheKey, out var cachedCategoryListObj) && cachedCategoryListObj is List<DrawingCategoryDTO> cachedCategoryList && cachedCategoryList != null)
            {
                objOutPut.DrawingCategoryList = cachedCategoryList;
            }
            else
            {
                objOutPut.DrawingCategoryList = await GetCategoryList() ?? new List<DrawingCategoryDTO>();
                foreach (var item in objOutPut.DrawingCategoryList)
                {
                    item.EncCategoryId = CommonHelper.EncryptURLHTML(item.Id.ToString());
                    
                }
                _memoryCache.Set(drawingCategoryCacheKey, objOutPut.DrawingCategoryList, TimeSpan.FromMinutes(30));
            }
            objOutPut.ProjectId = pId;


            return View(objOutPut);
        }

        public async Task<List<DrawingCategoryDTO>> GetCategoryList()
        {
            // DataTable dt = new DataTable();
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            List<DrawingCategoryDTO> PriorityLst = new List<DrawingCategoryDTO>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetDrawingCategory");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    PriorityLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DrawingCategoryDTO>>(data) ?? new List<DrawingCategoryDTO>();

                }


            }
            return PriorityLst;
        }

        [HttpGet]
        [Route("Drawing/ProjectDrawing/{eCategoryId}")]
        public async Task<IActionResult> ProjectDrawing(string eCategoryId)
        {
            ProjectsDTO objOutPut = new ProjectsDTO();
            int CategoryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eCategoryId));
            HttpContext.Session.SetString("eCID", eCategoryId);
            HttpContext.Session.SetInt32("CID", CategoryId);
            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (strMenuSession == null)
                return RedirectToAction("Index", "Home");
            AccessMasterDTO? empSession = JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession);
            var userId = HttpContext.Session.GetInt32("UserId");
            var pId = HttpContext.Session.GetInt32("PID");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            var projectCacheKey = $"Projects_{unitId}_{pId}";
            if (_memoryCache.TryGetValue(projectCacheKey, out var cachedProjectListObj) && cachedProjectListObj is List<ProjectDashboardDTO> cachedProjectList && cachedProjectList != null)
            {
                objOutPut.ProjectDashboardList = cachedProjectList;
            }
            else
            {
                var dashboard = await GetProjectDashboard(userId);
                objOutPut.ProjectDashboardList = dashboard ?? new List<ProjectDashboardDTO>();
                _memoryCache.Set(projectCacheKey, objOutPut.ProjectDashboardList);
            }
            objOutPut.ProjectName = objOutPut.ProjectDashboardList?.Where(p => p.ProjectId == pId).Select(m => m.ProjectName).FirstOrDefault() ?? string.Empty;
            objOutPut.EncProjectId = HttpContext.Session.GetString("ePID");
            objOutPut.AccessList = empSession?.AccessList?.ToList() ?? new List<AccessMasterDTO>();
            objOutPut.UserType = HttpContext.Session.GetInt32("UserType");
            var drawingCategoryCacheKey = $"DrawingCategory_{pId}";
            if (_memoryCache.TryGetValue(drawingCategoryCacheKey, out var cachedCategoryListObj) && cachedCategoryListObj is List<DrawingCategoryDTO> cachedCategoryList && cachedCategoryList != null)
            {
                objOutPut.DrawingCategoryList = cachedCategoryList;
            }
            else
            {
                objOutPut.DrawingCategoryList = await GetCategoryList() ?? new List<DrawingCategoryDTO>();
                _memoryCache.Set(drawingCategoryCacheKey, objOutPut.DrawingCategoryList, TimeSpan.FromMinutes(30));
            }
            objOutPut.CategoryName = objOutPut.DrawingCategoryList?.Where(p => p.Id == CategoryId).Select(m => m.Category).FirstOrDefault() ?? string.Empty;

            if (_memoryCache.Get("UserList") == null)
            {
                objOutPut.UserList = await GetUsers(1);
                _memoryCache.Set("UserList", objOutPut.UserList);
            }
            else
            {
                objOutPut.UserList = _memoryCache.Get<List<UserKeyValues>>("UserList");
            }

            if (_memoryCache.Get("VendorList") == null)
            {
                objOutPut.VendorList = await GetUsers(3);
                _memoryCache.Set("VendorList", objOutPut.VendorList);
            }
            else
            {
                objOutPut.VendorList = _memoryCache.Get<List<UserKeyValues>>("VendorList");
            }

            if (_memoryCache.Get("ClientList") == null)
            {
                objOutPut.ClientList = await GetUsers(2);
                _memoryCache.Set("ClientList", objOutPut.ClientList);
            }
            else
            {
                objOutPut.ClientList = _memoryCache.Get<List<UserKeyValues>>("ClientList");
            }

            var drFolderDetailsCacheKey = $"DrFolderDetails_{pId}_{CategoryId}";
            if (_memoryCache.TryGetValue(drFolderDetailsCacheKey, out var cachedFolderListObj) && cachedFolderListObj is List<ProjectDrawingsDTO> cachedFolderList && cachedFolderList != null)
            {
                objOutPut.ProjectDrawingList = cachedFolderList;
            }
            else
            {
                objOutPut.ProjectDrawingList = await GetProjectFolder(pId, CategoryId) ?? new List<ProjectDrawingsDTO>();
                _memoryCache.Set(drFolderDetailsCacheKey, objOutPut.ProjectDrawingList);
            }
            var drFileDetailsCacheKey = $"DrFileDetails_{pId}_{CategoryId}";
            if (_memoryCache.TryGetValue(drFileDetailsCacheKey, out var cachedFileListObj) && cachedFileListObj is List<ProjectDrawingsDTO> cachedFileList && cachedFileList != null)
            {
                objOutPut.ProjectDrawingFileList = cachedFileList;
            }
            else
            {
                objOutPut.ProjectDrawingFileList = await GetDrawingFiles(pId, CategoryId) ?? new List<ProjectDrawingsDTO>();
                _memoryCache.Set(drFileDetailsCacheKey, objOutPut.ProjectDrawingFileList);
            }

            string roleKey = $"ProjectRole_{unitId}_{pId}";

            if (_memoryCache.TryGetValue(roleKey, out var cachedProjectRoleObj) && cachedProjectRoleObj is List<ProjectRoleSummeryDTO> cachedProjectRoleList && cachedProjectRoleList != null)
            {
                objOutPut.ProjectRoleSummeryList = cachedProjectRoleList;
            }
            else
            {
                objOutPut.ProjectRoleSummeryList = await GetProjectRoleSummery(unitId, pId);
                _memoryCache.Set(roleKey, objOutPut.ProjectRoleSummeryList);
            }
            objOutPut.CategoryId = CategoryId;
            objOutPut.ProjectId = (int)pId;
            return View(objOutPut);
        }

        public async Task<IActionResult> ProjectDrawingById(int CategoryId)
        {
            ProjectsDTO objOutPut = new ProjectsDTO();
            string eCategoryId = CommonHelper.EncryptURLHTML(Convert.ToString(CategoryId));
            HttpContext.Session.SetString("eCID", eCategoryId);
            HttpContext.Session.SetInt32("CID", CategoryId);
            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (strMenuSession == null)
                return RedirectToAction("Index", "Home");
            AccessMasterDTO? empSession = JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession);
            var userId = HttpContext.Session.GetInt32("UserId");
            var pId = HttpContext.Session.GetInt32("PID");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            var projectCacheKey = $"Projects_{unitId}_{pId}";
            if (_memoryCache.TryGetValue(projectCacheKey, out var cachedProjectListObj) && cachedProjectListObj is List<ProjectDashboardDTO> cachedProjectList && cachedProjectList != null)
            {
                objOutPut.ProjectDashboardList = cachedProjectList;
            }
            else
            {
                var dashboard = await GetProjectDashboard(userId);
                objOutPut.ProjectDashboardList = dashboard ?? new List<ProjectDashboardDTO>();
                _memoryCache.Set(projectCacheKey, objOutPut.ProjectDashboardList);
            }
            objOutPut.ProjectName = objOutPut.ProjectDashboardList?.Where(p => p.ProjectId == pId).Select(m => m.ProjectName).FirstOrDefault() ?? string.Empty;
            objOutPut.EncProjectId = HttpContext.Session.GetString("ePID");
            objOutPut.AccessList = empSession?.AccessList?.ToList() ?? new List<AccessMasterDTO>();
            objOutPut.UserType = HttpContext.Session.GetInt32("UserType");
            var drawingCategoryCacheKey = $"DrawingCategory_{pId}";
            if (_memoryCache.TryGetValue(drawingCategoryCacheKey, out var cachedCategoryListObj) && cachedCategoryListObj is List<DrawingCategoryDTO> cachedCategoryList && cachedCategoryList != null)
            {
                objOutPut.DrawingCategoryList = cachedCategoryList;
            }
            else
            {
                objOutPut.DrawingCategoryList = await GetCategoryList() ?? new List<DrawingCategoryDTO>();
                _memoryCache.Set(drawingCategoryCacheKey, objOutPut.DrawingCategoryList, TimeSpan.FromMinutes(30));
            }
            objOutPut.CategoryName = objOutPut.DrawingCategoryList?.Where(p => p.Id == CategoryId).Select(m => m.Category).FirstOrDefault() ?? string.Empty;
            if (_memoryCache.Get("UserList") == null)
            {
                objOutPut.UserList = await GetUsers(1) ?? new List<UserKeyValues>();
                _memoryCache.Set("UserList", objOutPut.UserList);
            }
            else
            {
                objOutPut.UserList = _memoryCache.Get<List<UserKeyValues>>("UserList") ?? new List<UserKeyValues>();
            }
            var drFolderDetailsCacheKey = $"DrFolderDetails_{pId}_{CategoryId}";
            if (_memoryCache.TryGetValue(drFolderDetailsCacheKey, out var cachedFolderListObj) && cachedFolderListObj is List<ProjectDrawingsDTO> cachedFolderList && cachedFolderList != null)
            {
                objOutPut.ProjectDrawingList = cachedFolderList;
            }
            else
            {
                objOutPut.ProjectDrawingList = await GetProjectFolder(pId, CategoryId) ?? new List<ProjectDrawingsDTO>();
                _memoryCache.Set(drFolderDetailsCacheKey, objOutPut.ProjectDrawingList);
            }
            var drFileDetailsCacheKey = $"DrFileDetails_{pId}_{CategoryId}";
            if (_memoryCache.TryGetValue(drFileDetailsCacheKey, out var cachedFileListObj) && cachedFileListObj is List<ProjectDrawingsDTO> cachedFileList && cachedFileList != null)
            {
                objOutPut.ProjectDrawingFileList = cachedFileList;
            }
            else
            {
                objOutPut.ProjectDrawingFileList = await GetDrawingFiles(pId, CategoryId) ?? new List<ProjectDrawingsDTO>();
                _memoryCache.Set(drFileDetailsCacheKey, objOutPut.ProjectDrawingFileList);
            }

            string roleKey = $"ProjectRole_{unitId}_{pId}";

            if (_memoryCache.TryGetValue(roleKey, out var cachedProjectRoleObj) && cachedProjectRoleObj is List<ProjectRoleSummeryDTO> cachedProjectRoleList && cachedProjectRoleList != null)
            {
                objOutPut.ProjectRoleSummeryList = cachedProjectRoleList;
            }
            else
            {
                objOutPut.ProjectRoleSummeryList = await GetProjectRoleSummery(unitId, pId);
                _memoryCache.Set(roleKey, objOutPut.ProjectRoleSummeryList);
            }
            objOutPut.CategoryId = CategoryId;
            return View("ProjectDrawing", objOutPut);
        }

        public async Task<IActionResult> ProjectDrawing()
        {
            int CategoryId = 0;
            ProjectsDTO objOutPut = new ProjectsDTO();
            var eCID = HttpContext.Session.GetString("eCID");
            if (!string.IsNullOrEmpty(eCID))
            {
                CategoryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(HttpContext.Session.GetString("eCID")));
            }
           
           // HttpContext.Session.SetString("eCID", eCategoryId);
            HttpContext.Session.SetInt32("CID", CategoryId);
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (strMenuSession == null)
                return RedirectToAction("Index", "Home");


            AccessMasterDTO? empSession = (AccessMasterDTO?)(JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession));

            objOutPut.AccessList = empSession.AccessList.ToList();

            objOutPut.EncProjectId = HttpContext.Session.GetString("ePID");

            objOutPut.UserType = HttpContext.Session.GetInt32("UserType");

            var projectList = _memoryCache.Get("Projects") as List<ProjectDashboardDTO>;
            objOutPut.ProjectName = projectList.Where(p => p.ProjectId == HttpContext.Session.GetInt32("PID")).Select(m => m.ProjectName).FirstOrDefault() ?? string.Empty;
            var pId = HttpContext.Session.GetInt32("PID");
            var drawingCategoryCacheKey = $"DrawingCategory_{pId}";

            if (_memoryCache.TryGetValue(drawingCategoryCacheKey, out var cachedCategoryListObj) && cachedCategoryListObj is List<DrawingCategoryDTO> cachedCategoryList && cachedCategoryList != null)
            {
                objOutPut.DrawingCategoryList = cachedCategoryList;
            }
            else
            {
                objOutPut.DrawingCategoryList = await GetCategoryList() ?? new List<DrawingCategoryDTO>();
                _memoryCache.Set(drawingCategoryCacheKey, objOutPut.DrawingCategoryList, TimeSpan.FromMinutes(30));
            }
            objOutPut.CategoryName = objOutPut.DrawingCategoryList?.Where(p => p.Id == CategoryId).Select(m => m.Category).FirstOrDefault() ?? string.Empty;
            //if (_memoryCache.Get("UserList") == null)
            //{
            //    objOutPut.UserList = await GetUsers() ?? new List<UserKeyValues>();
            //    _memoryCache.Set("UserList", objOutPut.UserList);
            //}
            //else
            //{
            //    objOutPut.UserList = _memoryCache.Get<List<UserKeyValues>>("UserList") ?? new List<UserKeyValues>();
            //}


            //  objOutPut.DrawingCategoryList = await GetCategoryList() ?? new List<DrawingCategoryDTO>();
            //  objOutPut.UserList = await GetUsers() ?? new List<UserKeyValues>();
            //// _memoryCache.Set("Category", objOutPut.DrawingCategoryList);
            //objOutPut.ProjectDrawingList = await GetProjectFolder(HttpContext.Session.GetInt32("PID"), CategoryId) ?? new List<ProjectDrawingsDTO>();
            //objOutPut.ProjectDrawingFileList = await GetDrawingFiles(HttpContext.Session.GetInt32("PID"), CategoryId) ?? new List<ProjectDrawingsDTO>();
            //var drFolderDetailsCacheKey = $"DrFolderDetails_{HttpContext.Session.GetInt32("PID")}_{CategoryId}";
            //_memoryCache.Set(drFolderDetailsCacheKey, objOutPut.ProjectDrawingList);

            if (_memoryCache.Get("UserList") == null)
            {
                objOutPut.UserList = await GetUsers(1);
                _memoryCache.Set("UserList", objOutPut.UserList);
            }
            else
            {
                objOutPut.UserList = _memoryCache.Get<List<UserKeyValues>>("UserList");
            }

            if (_memoryCache.Get("VendorList") == null)
            {
                objOutPut.VendorList = await GetUsers(3);
                _memoryCache.Set("VendorList", objOutPut.VendorList);
            }
            else
            {
                objOutPut.UserList = _memoryCache.Get<List<UserKeyValues>>("VendorList");
            }

            if (_memoryCache.Get("ClientList") == null)
            {
                objOutPut.ClientList = await GetUsers(2);
                _memoryCache.Set("ClientList", objOutPut.ClientList);
            }
            else
            {
                objOutPut.ClientList = _memoryCache.Get<List<UserKeyValues>>("ClientList");
            }

            var drFolderDetailsCacheKey = $"DrFolderDetails_{pId}_{CategoryId}";
            if (_memoryCache.TryGetValue(drFolderDetailsCacheKey, out var cachedFolderListObj) && cachedFolderListObj is List<ProjectDrawingsDTO> cachedFolderList && cachedFolderList != null)
            {
                objOutPut.ProjectDrawingList = cachedFolderList;
            }
            else
            {
                objOutPut.ProjectDrawingList = await GetProjectFolder(pId, CategoryId) ?? new List<ProjectDrawingsDTO>();
                _memoryCache.Set(drFolderDetailsCacheKey, objOutPut.ProjectDrawingList);
            }
            var drFileDetailsCacheKey = $"DrFileDetails_{pId}_{CategoryId}";
            if (_memoryCache.TryGetValue(drFileDetailsCacheKey, out var cachedFileListObj) && cachedFileListObj is List<ProjectDrawingsDTO> cachedFileList && cachedFileList != null)
            {
                objOutPut.ProjectDrawingFileList = cachedFileList;
            }
            else
            {
                objOutPut.ProjectDrawingFileList = await GetDrawingFiles(pId, CategoryId) ?? new List<ProjectDrawingsDTO>();
                _memoryCache.Set(drFileDetailsCacheKey, objOutPut.ProjectDrawingFileList);
            }

            string roleKey = $"ProjectRole_{unitId}_{pId}";

            if (_memoryCache.TryGetValue(roleKey, out var cachedProjectRoleObj) && cachedProjectRoleObj is List<ProjectRoleSummeryDTO> cachedProjectRoleList && cachedProjectRoleList != null)
            {
                objOutPut.ProjectRoleSummeryList = cachedProjectRoleList;
            }
            else
            {
                objOutPut.ProjectRoleSummeryList = await GetProjectRoleSummery(unitId, pId);
                _memoryCache.Set(roleKey, objOutPut.ProjectRoleSummeryList);
            }

            objOutPut.CategoryId = CategoryId;
            objOutPut.ProjectId = (int)pId;
            return View(objOutPut);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(209715200)]
        public async Task<IActionResult> CreateDrawingFiles([FromForm] string cId, [FromForm] string fileName)
        {
            ProjectsDTO objOutPut = new ProjectsDTO();
            var files = Request.Form.Files;
            ProjectDrawingsDTO inputs = new ProjectDrawingsDTO();
            int? pId = HttpContext.Session.GetInt32("PID");
            // int? cId = HttpContext.Session.GetInt32("CID");
            inputs.ProjectId = pId;
            inputs.CategoryId = Convert.ToInt32(cId);
            inputs.IsFolder = false;
            inputs.UserId = HttpContext.Session.GetInt32("UserId");
            //  inputs.FolderName = folderName;
            string path = "", fName = "", nPath="";
            try
            {
                //string path = Server.MapPath("~/YourFolder/" + folderName);
                string folderpath = Path.Combine(this._environment.WebRootPath, "Drawings");
                path = Path.Combine(folderpath ?? string.Empty, Convert.ToString(pId) ?? string.Empty, Convert.ToString(cId) ?? string.Empty);


                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);

                     nPath = Path.Combine(path, "Files");
                    if (!Directory.Exists(nPath))
                    {
                        Directory.CreateDirectory(nPath);
                    }
                }
                else
                {
                     nPath = Path.Combine(path, "Files");
                    if (!Directory.Exists(nPath))
                    {
                        Directory.CreateDirectory(nPath);
                    }

                }

                if (files.Count > 0)
                {
                    List<string> uploadedFiles = new List<string>();
                    foreach (IFormFile postedFile in files)
                    {
                        fName = Path.GetFileName(postedFile.FileName);
                        inputs.FilePath = fName = fName.Replace("'", "").Replace("\"", "_");
                        inputs.FolderName = fileName;
                        using (FileStream stream = new FileStream(Path.Combine(nPath, fName), FileMode.Create))
                        {
                            postedFile.CopyTo(stream);

                        }

                       await SaveDrawingFile(inputs);

                    }

                }
               
            }
            catch (Exception)
            {
                //  return Ok(new { count = files.Count });
            }
           
            return Ok(new { count = files.Count });
        }

        public async Task<OutPutResponse>? SaveDrawingFile(ProjectDrawingsDTO inputs)
        {
            OutPutResponse res = new OutPutResponse();
            inputs.IsActive = true;
            inputs.CreationDate = DateTime.Now;
            inputs.UserId = HttpContext.Session.GetInt32("UserId");

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiProjectFileUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("SaveDrawingFile", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var drFileDetailsCacheKey = $"DrFileDetails_{inputs.ProjectId}_{inputs.CategoryId}";
                        _memoryCache.Remove(drFileDetailsCacheKey);
                        var data = await response.Content.ReadAsStringAsync();
                        res = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data) ?? new OutPutResponse();

                    }

                }


            }
            catch (SystemException ex)
            {
                if (res != null) res.DisplayMessage = "Project failed!" + ex.Message;
            }
            return res;
        }

        public async Task<List<ProjectDrawingsDTO>> GetProjectFolder(int? pId, int? cId)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            int? UType = HttpContext.Session.GetInt32("UserType");
            List<ProjectDrawingsDTO> PriorityLst = new List<ProjectDrawingsDTO>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiProjectFileUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetProjectDrawingFolders?pId=" + pId + "&cId=" + cId + "&uId=" + UserId + "&uType=" + UType);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    PriorityLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProjectDrawingsDTO>>(data) ?? new List<ProjectDrawingsDTO>();

                    foreach (var item in PriorityLst)
                    {
                        if (item.UserId == UserId)
                        {
                            item.UserId = 0;
                        }
                        item.EnFolderId = CommonHelper.EncryptURLHTML(item.Id.ToString());
                        //if (item.ProfileImage != null)
                        //        item.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);

                    }
                }


            }
            return PriorityLst;
        }

        public async Task<List<ProjectDrawingsDTO>> GetDrawingFiles(int? pId, int? cId)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            int? UType = HttpContext.Session.GetInt32("UserType");
            List<ProjectDrawingsDTO> PriorityLst = new List<ProjectDrawingsDTO>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiProjectFileUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetProjectDrawingFiles?pId=" + pId + "&cId=" + cId + "&uType=" + UType);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    PriorityLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProjectDrawingsDTO>>(data) ?? new List<ProjectDrawingsDTO>();

                    foreach (var item in PriorityLst)
                    {
                        if (item.UserId == UserId)
                        {
                            item.UserId = 0;
                        }
                        item.EnFolderId = CommonHelper.EncryptURLHTML(item.Id.ToString());
                        //if (item.ProfileImage != null)
                        //        item.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);

                    }

                }


            }
            return PriorityLst;
        }

        public async Task<OutPutResponse> CreateDrawingFolder(string folderName, int cId)
        {
            OutPutResponse objRespnose = new OutPutResponse();
            ProjectDrawingsDTO inputs = new ProjectDrawingsDTO();
            int? pId = HttpContext.Session.GetInt32("PID");
            // int? cId = HttpContext.Session.GetInt32("CID");
            inputs.ProjectId = pId;
            inputs.CategoryId = cId;
            inputs.IsFolder = true;
            inputs.UserId = HttpContext.Session.GetInt32("UserId");
            inputs.FolderName = folderName;
            string path = "";
            try
            {
                //string path = Server.MapPath("~/YourFolder/" + folderName);
                string folderpath = Path.Combine(this._environment.WebRootPath, "Drawings");
                path = Path.Combine(folderpath, Convert.ToString(pId), Convert.ToString(cId));
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);

                    string nPath = Path.Combine(path, folderName);
                    if (!Directory.Exists(nPath))
                    {
                        Directory.CreateDirectory(nPath);
                    }
                }
                else
                {
                    string nPath = Path.Combine(path, folderName);
                    if (!Directory.Exists(nPath))
                    {
                        Directory.CreateDirectory(nPath);
                    }
                }
               objRespnose = await SaveDrawingFolder(inputs);

                // RedirectToAction("ProjectDocuments", "Document", new { eCategoryId = HttpContext.Session.GetString("eCID") });
            }
            catch (Exception)
            {
                // response = "0";
            }
            //  RedirectToAction("ProjectDocuments", "Document", new { eCategoryId = HttpContext.Session.GetString("eCID") });
            return objRespnose;
        }

        public async Task<OutPutResponse>? SaveDrawingFolder(ProjectDrawingsDTO inputs)
        {
            OutPutResponse res = new OutPutResponse();
            inputs.IsActive = true;
            inputs.CreationDate = DateTime.Now;
            inputs.UserId = HttpContext.Session.GetInt32("UserId");

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiProjectFileUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("SaveDrawingFile", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var drFolderDetailsCacheKey = $"DrFolderDetails_{inputs.ProjectId}_{inputs.CategoryId}";
                        _memoryCache.Remove(drFolderDetailsCacheKey);
                        var data = await response.Content.ReadAsStringAsync();

                        res = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data) ?? new OutPutResponse();


                    }

                }


            }
            catch (SystemException ex)
            {
                if (res != null) res.DisplayMessage = "Project failed!" + ex.Message;
            }
            return res;
        }


        [HttpGet]
        [Route("Drawing/FolderFiles/{eFolderId}")]
        public async Task<IActionResult> FolderFiles(string? eFolderId)
        {
            ProjectsDTO objOutPut = new ProjectsDTO();
            int FolderId = 0;
            if (!string.IsNullOrEmpty(eFolderId))
            {
                FolderId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eFolderId));
            }
            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (strMenuSession == null)
                return RedirectToAction("Index", "Home");
            AccessMasterDTO? empSession = JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession);
            objOutPut.AccessList = empSession?.AccessList?.ToList() ?? new List<AccessMasterDTO>();
            objOutPut.UserType = HttpContext.Session.GetInt32("UserType");
            var userId = HttpContext.Session.GetInt32("UserId");
            var pId = HttpContext.Session.GetInt32("PID");
            var cId = HttpContext.Session.GetInt32("CID");
            HttpContext.Session.SetInt32("FID",FolderId);
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            var projectCacheKey = $"Projects_{unitId}_{pId}";
            if (_memoryCache.TryGetValue(projectCacheKey, out var cachedProjectListObj) && cachedProjectListObj is List<ProjectDashboardDTO> cachedProjectList && cachedProjectList != null)
            {
                objOutPut.ProjectDashboardList = cachedProjectList;
            }
            else
            {
                var dashboard = await GetProjectDashboard(userId);
                objOutPut.ProjectDashboardList = dashboard ?? new List<ProjectDashboardDTO>();
                _memoryCache.Set(projectCacheKey, objOutPut.ProjectDashboardList);
            }
            objOutPut.ProjectName = objOutPut.ProjectDashboardList?.Where(p => p.ProjectId == pId).Select(m => m.ProjectName).FirstOrDefault() ?? string.Empty;
            objOutPut.EncProjectId = HttpContext.Session.GetString("ePID");
            var drawingCategoryCacheKey = $"DrawingCategory_{pId}";
            if (_memoryCache.TryGetValue(drawingCategoryCacheKey, out var cachedCategoryListObj) && cachedCategoryListObj is List<DrawingCategoryDTO> cachedCategoryList && cachedCategoryList != null)
            {
                objOutPut.DrawingCategoryList = cachedCategoryList;
            }
            else
            {
                objOutPut.DrawingCategoryList = await GetCategoryList() ?? new List<DrawingCategoryDTO>();
                _memoryCache.Set(drawingCategoryCacheKey, objOutPut.DrawingCategoryList, TimeSpan.FromMinutes(30));
            }
            objOutPut.CategoryName = objOutPut.DrawingCategoryList?.Where(p => p.Id == cId).Select(m => m.Category).FirstOrDefault() ?? string.Empty;
            if (_memoryCache.Get("UserList") == null)
            {
                objOutPut.UserList = await GetUsers(1) ?? new List<UserKeyValues>();
                _memoryCache.Set("UserList", objOutPut.UserList);
            }
            else
            {
                objOutPut.UserList = _memoryCache.Get<List<UserKeyValues>>("UserList") ?? new List<UserKeyValues>();
            }
            var drFolderDetailsCacheKey = $"DrFolderDetails_{pId}_{cId}";
            if (_memoryCache.TryGetValue(drFolderDetailsCacheKey, out var cachedFolderListObj) && cachedFolderListObj is List<ProjectDrawingsDTO> cachedFolderList && cachedFolderList != null)
            {
                objOutPut.ProjectDrawingList = cachedFolderList;
            }
            else
            {
                objOutPut.ProjectDrawingList = await GetProjectFolder(pId, cId) ?? new List<ProjectDrawingsDTO>();
                _memoryCache.Set(drFolderDetailsCacheKey, objOutPut.ProjectDrawingList);
            }
            objOutPut.FileDetails = objOutPut.ProjectDrawingList?.Where(p => p.Id == FolderId).Select(m => m.FolderName).FirstOrDefault() ?? string.Empty;
            var drawingFileDetailsCacheKey = $"DrawingFileDetails_{pId}_{cId}_{FolderId}";
            if (_memoryCache.TryGetValue(drawingFileDetailsCacheKey, out var cachedFileListObj) && cachedFileListObj is List<DrawingFolderFilesDTO> cachedFileList && cachedFileList != null)
            {
                objOutPut.DrawingFileList = cachedFileList;
            }
            else
            {
                objOutPut.DrawingFileList = await GetFolderFiles(pId, cId, FolderId) ?? new List<DrawingFolderFilesDTO>();
                _memoryCache.Set(drawingFileDetailsCacheKey, objOutPut.DrawingFileList);
            }

            string roleKey = $"ProjectRole_{unitId}_{pId}";

            if (_memoryCache.TryGetValue(roleKey, out var cachedProjectRoleObj) && cachedProjectRoleObj is List<ProjectRoleSummeryDTO> cachedProjectRoleList && cachedProjectRoleList != null)
            {
                objOutPut.ProjectRoleSummeryList = cachedProjectRoleList;
            }
            else
            {
                objOutPut.ProjectRoleSummeryList = await GetProjectRoleSummery(unitId, pId);
                _memoryCache.Set(roleKey, objOutPut.ProjectRoleSummeryList);
            }

            objOutPut.FolderId = FolderId;
            objOutPut.ProjectId = (int)pId;
            return View(objOutPut);
        }

        public async Task<IActionResult> FolderFilesById(int FolderId)
        {
            ProjectsDTO objOutPut = new ProjectsDTO();
            string eFolderId = "";
            if (FolderId>0)
            {
                eFolderId = CommonHelper.EncryptURLHTML(Convert.ToString(FolderId));
            }
            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (strMenuSession == null)
                return RedirectToAction("Index", "Home");
            AccessMasterDTO? empSession = JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession);
            objOutPut.AccessList = empSession?.AccessList?.ToList() ?? new List<AccessMasterDTO>();
            objOutPut.UserType = HttpContext.Session.GetInt32("UserType");
            var userId = HttpContext.Session.GetInt32("UserId");
            var pId = HttpContext.Session.GetInt32("PID");
            var cId = HttpContext.Session.GetInt32("CID");
            HttpContext.Session.SetInt32("FID", FolderId);
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            var projectCacheKey = $"Projects_{unitId}_{pId}";
            if (_memoryCache.TryGetValue(projectCacheKey, out var cachedProjectListObj) && cachedProjectListObj is List<ProjectDashboardDTO> cachedProjectList && cachedProjectList != null)
            {
                objOutPut.ProjectDashboardList = cachedProjectList;
            }
            else
            {
                var dashboard = await GetProjectDashboard(userId);
                objOutPut.ProjectDashboardList = dashboard ?? new List<ProjectDashboardDTO>();
                _memoryCache.Set(projectCacheKey, objOutPut.ProjectDashboardList);
            }
            objOutPut.ProjectName = objOutPut.ProjectDashboardList?.Where(p => p.ProjectId == pId).Select(m => m.ProjectName).FirstOrDefault() ?? string.Empty;
            objOutPut.EncProjectId = HttpContext.Session.GetString("ePID");
            var drawingCategoryCacheKey = $"DrawingCategory_{pId}";
            if (_memoryCache.TryGetValue(drawingCategoryCacheKey, out var cachedCategoryListObj) && cachedCategoryListObj is List<DrawingCategoryDTO> cachedCategoryList && cachedCategoryList != null)
            {
                objOutPut.DrawingCategoryList = cachedCategoryList;
            }
            else
            {
                objOutPut.DrawingCategoryList = await GetCategoryList() ?? new List<DrawingCategoryDTO>();
                _memoryCache.Set(drawingCategoryCacheKey, objOutPut.DrawingCategoryList, TimeSpan.FromMinutes(30));
            }
            objOutPut.CategoryName = objOutPut.DrawingCategoryList?.Where(p => p.Id == cId).Select(m => m.Category).FirstOrDefault() ?? string.Empty;
            if (_memoryCache.Get("UserList") == null)
            {
                objOutPut.UserList = await GetUsers(1) ?? new List<UserKeyValues>();
                _memoryCache.Set("UserList", objOutPut.UserList);
            }
            else
            {
                objOutPut.UserList = _memoryCache.Get<List<UserKeyValues>>("UserList") ?? new List<UserKeyValues>();
            }
            var drFolderDetailsCacheKey = $"DrFolderDetails_{pId}_{cId}";
            if (_memoryCache.TryGetValue(drFolderDetailsCacheKey, out var cachedFolderListObj) && cachedFolderListObj is List<ProjectDrawingsDTO> cachedFolderList && cachedFolderList != null)
            {
                objOutPut.ProjectDrawingList = cachedFolderList;
            }
            else
            {
                objOutPut.ProjectDrawingList = await GetProjectFolder(pId, cId) ?? new List<ProjectDrawingsDTO>();
                _memoryCache.Set(drFolderDetailsCacheKey, objOutPut.ProjectDrawingList);
            }
            objOutPut.FileDetails = objOutPut.ProjectDrawingList?.Where(p => p.Id == FolderId).Select(m => m.FolderName).FirstOrDefault() ?? string.Empty;
            var drawingFileDetailsCacheKey = $"DrawingFileDetails_{pId}_{cId}_{FolderId}";
            if (_memoryCache.TryGetValue(drawingFileDetailsCacheKey, out var cachedFileListObj) && cachedFileListObj is List<DrawingFolderFilesDTO> cachedFileList && cachedFileList != null)
            {
                objOutPut.DrawingFileList = cachedFileList;
            }
            else
            {
                objOutPut.DrawingFileList = await GetFolderFiles(pId, cId, FolderId) ?? new List<DrawingFolderFilesDTO>();
                _memoryCache.Set(drawingFileDetailsCacheKey, objOutPut.DrawingFileList);
            }

            string roleKey = $"ProjectRole_{unitId}_{pId}";

            if (_memoryCache.TryGetValue(roleKey, out var cachedProjectRoleObj) && cachedProjectRoleObj is List<ProjectRoleSummeryDTO> cachedProjectRoleList && cachedProjectRoleList != null)
            {
                objOutPut.ProjectRoleSummeryList = cachedProjectRoleList;
            }
            else
            {
                objOutPut.ProjectRoleSummeryList = await GetProjectRoleSummery(unitId, pId);
                _memoryCache.Set(roleKey, objOutPut.ProjectRoleSummeryList);
            }

            objOutPut.FolderId = FolderId;
            return View("FolderFiles",objOutPut);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(209715200)]
        public async Task<IActionResult> CreateFolderFile([FromForm] string Fid, [FromForm] string fileName)
        {

            // int cId = 0; string fileName = "";
            var files = Request.Form.Files;
            DrawingFolderFilesDTO inputs = new DrawingFolderFilesDTO();
            int? pId = HttpContext.Session.GetInt32("PID");
            int? cId = HttpContext.Session.GetInt32("CID");
            inputs.ProjectId = pId;
            inputs.CategoryId = cId;
            inputs.FolderId = Convert.ToInt32(Fid);
            inputs.UserId = HttpContext.Session.GetInt32("UserId");
            //  inputs.FolderName = folderName;
            string path = "", fName = "";
            try
            {
                //string path = Server.MapPath("~/YourFolder/" + folderName);
                string folderpath = Path.Combine(this._environment.WebRootPath, "Drawings");
                path = Path.Combine(folderpath, Convert.ToString(pId), Convert.ToString(cId), Convert.ToString(Fid));


                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }


                if (files.Count > 0)
                {
                    List<string> uploadedFiles = new List<string>();
                    foreach (IFormFile postedFile in files)
                    {
                        fName = Path.GetFileName(postedFile.FileName);
                        inputs.FilePath = fName = fName.Replace("'", "").Replace("\"", "_");
                        inputs.FileName = fileName;

                        using (FileStream stream = new FileStream(Path.Combine(path, fName), FileMode.Create))
                        {
                            postedFile.CopyTo(stream);

                        }

                       await SaveDrawingFolderFile(inputs);
                    }

                }
                else
                {
                    inputs.FilePath = "";
                    inputs.FileName = "";
                }


            }
            catch (Exception)
            {
                // response = "0";
            }
            // return Ok();
            return Ok(new { count = files.Count });

        }


        public async Task<OutPutResponse>? SaveDrawingFolderFile(DrawingFolderFilesDTO inputs)
        {
            OutPutResponse res = new OutPutResponse();
            inputs.IsActive = true;
            inputs.CreationDate = DateTime.Now;
            //  inputs.UserId = HttpContext.Session.GetInt32("UserId");

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiProjectFileUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("SaveDrawingFolderFile", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var drawingFileDetailsCacheKey = $"DrawingFileDetails_{inputs.ProjectId}_{inputs.CategoryId}_{inputs.FolderId}";
                        _memoryCache.Remove(drawingFileDetailsCacheKey);
                        var data = await response.Content.ReadAsStringAsync();
                        res = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data) ?? new OutPutResponse();

                    }

                }


            }
            catch (SystemException ex)
            {
                if (res != null) res.DisplayMessage = "Project failed!" + ex.Message;
            }
            return res;
        }

        public async Task<List<DrawingFolderFilesDTO>> GetFolderFiles(int? pId, int? cId, int? fId)
        {
            // DataTable dt = new DataTable();
            //  int? unitId = HttpContext.Session.GetInt32("UnitId");
            List<DrawingFolderFilesDTO> PriorityLst = new List<DrawingFolderFilesDTO>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiProjectFileUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetProjectDrawingFolderFiles?pId=" + pId + "&cId=" + cId + "&fId=" + fId);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    PriorityLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DrawingFolderFilesDTO>>(data) ?? new List<DrawingFolderFilesDTO>();

                    foreach (var item in PriorityLst)
                    {

                        item.EnFileId = CommonHelper.EncryptURLHTML(item.Id.ToString());

                    }

                }


            }
            return PriorityLst;
        }


        [HttpGet]
        // [Route("Document/FolderFiles/{eFolderId}")]
        public async Task<OutPutResponse> DeleteFolderFiles(int? FileId)
        {
            OutPutResponse res = new OutPutResponse();
            int? pId = HttpContext.Session.GetInt32("PID");
            int? cID = HttpContext.Session.GetInt32("CID");
            int? fId = HttpContext.Session.GetInt32("FID");

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiProjectFileUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("DeleteDrawingFolderFile?pId=" + FileId);
                if (response.IsSuccessStatusCode)
                {
                    var drawingFileDetailsCacheKey = $"DrawingFileDetails_{pId}_{cID}_{fId}";
                    _memoryCache.Remove(drawingFileDetailsCacheKey);  
                    var data = await response.Content.ReadAsStringAsync();
                    res = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data) ?? new OutPutResponse();

                }
            }

            return res;
        }

        [HttpGet]
        // [Route("Document/FolderFiles/{eFolderId}")]
        public async Task<OutPutResponse> DeleteFolder(int? FileId)
        {
            OutPutResponse res = new OutPutResponse();
            int? pId = HttpContext.Session.GetInt32("PID");
            int? cID = HttpContext.Session.GetInt32("CID");

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiProjectFileUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("DeleteDrawingFolder?pId=" + FileId);
                if (response.IsSuccessStatusCode)
                {
                    var drFolderDetailsCacheKey = $"DrFolderDetails_{pId}_{cID}";
                    var drFileDetailsCacheKey = $"DrFileDetails_{pId}_{cID}";

                    _memoryCache.Remove(drFolderDetailsCacheKey);
                    _memoryCache.Remove(drFileDetailsCacheKey);

                    var data = await response.Content.ReadAsStringAsync();
                    res = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data) ?? new OutPutResponse();

                }
            }

            return res;
        }


        public IActionResult Download(string fileName)
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

        public async Task<List<UserKeyValues>> GetUsers(int userType)
        {
            // DataTable dt = new DataTable();
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            List<UserKeyValues> PriorityLst = new List<UserKeyValues>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiUserUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetUserList?unitId=" + unitId + "&userType=" + userType);
                // HttpResponseMessage response = await client.GetAsync("GetUnitUsers?unitId=" + unitId);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    PriorityLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserKeyValues>>(data) ?? new List<UserKeyValues>();

                    //if (PriorityLst.Count > 0)
                    //{
                    //    foreach (var item in PriorityLst)
                    //    {
                    //        item.FirstName = (item.FirstName != null ? item.FirstName + " " + item.LastName : item.BusinessOwner);
                    //    }

                    //}
                }


            }
            return PriorityLst;
        }

        [HttpGet]
        public async Task<ProjectDrawingsDTO> GetInviteUsers(int Id)
        {
            await Task.CompletedTask;
            ProjectDrawingsDTO outPut = new ProjectDrawingsDTO();

            int? pId = HttpContext.Session.GetInt32("PID");
            int? cID = HttpContext.Session.GetInt32("CID");

            var drFolderDetailsCacheKey = $"DrFolderDetails_{pId}_{cID}";
            var FolderList = _memoryCache.Get(drFolderDetailsCacheKey) as List<ProjectDrawingsDTO> ?? new List<ProjectDrawingsDTO>();
            outPut = FolderList.Where(x => x.Id == Id).SingleOrDefault() ?? new ProjectDrawingsDTO();
            return outPut;
        }

        public async Task<ProjectDrawingsDTO> GetInviteFileUsers(int Id)
        {
            await Task.CompletedTask;
            int? pId = HttpContext.Session.GetInt32("PID");
            int? cID = HttpContext.Session.GetInt32("CID");          
            var drFileDetailsCacheKey = $"DrFileDetails_{pId}_{cID}";
            var FolderList = _memoryCache.Get(drFileDetailsCacheKey) as List<ProjectDrawingsDTO> ?? new List<ProjectDrawingsDTO>();
            return FolderList.Where(x => x.Id == Id).SingleOrDefault() ?? new ProjectDrawingsDTO();
        }

        [HttpGet]
        public async Task<OutPutResponse>? InviteSent(string? accessId, int? AccessType, int Id, string? OptMessage, string? UserType)
        {
            OutPutResponse res = new OutPutResponse();
            ProjectDrawingsDTO inputs = new ProjectDrawingsDTO();
            inputs.IsActive = true;
            inputs.CreationDate = DateTime.Now;
            inputs.UserId = HttpContext.Session.GetInt32("UserId");
            inputs.AccessIds = accessId;
            inputs.AccessType = AccessType;
            inputs.OptMessage = OptMessage;
            inputs.Id = Id;

            if (UserType == "C")
            {
                inputs.ClientIds = accessId;
                inputs.AccessIds = null;
            }
            else
            {
                inputs.ClientIds = null;
                inputs.AccessIds = accessId;
            }
           

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiProjectFileUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("EditDrawingFolder", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var drFolderDetailsCacheKey = $"DrFolderDetails_{inputs.ProjectId}_{inputs.CategoryId}";
                        _memoryCache.Remove(drFolderDetailsCacheKey);
                      //  _memoryCache.Remove("DrFolderDetails");
                        var data = await response.Content.ReadAsStringAsync();

                        res = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data) ?? new OutPutResponse();


                    }

                }


            }
            catch (SystemException)
            {
                res.DisplayMessage = "Project failed!";
            }
            return res;
        }


        [HttpGet]
        public async Task<OutPutResponse>? LinkShareForDownload(string? accessId, int? AccessType, int Id, string? OptMessage, string? emailIds)
        {
            OutPutResponse res = new OutPutResponse();
            ProjectDrawingsDTO inputs = new ProjectDrawingsDTO();
            inputs.IsActive = true;
            inputs.CreationDate = DateTime.Now;
            inputs.AccessIds = accessId;
            inputs.AccessType = AccessType;
            inputs.OptMessage = OptMessage;
            inputs.UserId = HttpContext.Session.GetInt32("UserId");
            int? pId = HttpContext.Session.GetInt32("PID");
            int? cID = HttpContext.Session.GetInt32("CID");

            inputs.EmailIds = emailIds;
            inputs.Id = Id;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiProjectFileUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("EditDrawingFolder", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                       
                        var drFileDetailsCacheKey = $"DrFileDetails_{pId}_{cID}";
                        _memoryCache.Remove(drFileDetailsCacheKey);
                        var data = await response.Content.ReadAsStringAsync();

                        res = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data) ?? new OutPutResponse();


                    }

                }


            }
            catch (SystemException)
            {
                res.DisplayMessage = "Project failed!";
            }
            return res;
        }

        public async Task<DrawingFolderFilesDTO> GetFileUsers(int Id)
        {
            int? pId = HttpContext.Session.GetInt32("PID");
            int? cID = HttpContext.Session.GetInt32("CID");
            int? fId = HttpContext.Session.GetInt32("FID");

            await Task.CompletedTask;

            var drawingFileDetailsCacheKey = $"DrawingFileDetails_{pId}_{cID}_{fId}";
            var FolderList = _memoryCache.Get(drawingFileDetailsCacheKey) as List<DrawingFolderFilesDTO> ?? new List<DrawingFolderFilesDTO>();
            return FolderList.Where(x => x.Id == Id).SingleOrDefault() ?? new DrawingFolderFilesDTO();
        }

        [HttpGet]
        public async Task<OutPutResponse>? DocFileShareForDownload(string? accessId, int? AccessType, int Id, string? OptMessage, string? emailIds)
        {
            OutPutResponse res = new OutPutResponse();
            DrawingFolderFilesDTO inputs = new DrawingFolderFilesDTO();
            inputs.IsActive = true;
            inputs.CreationDate = DateTime.Now;
            inputs.AccessIds = accessId;
            inputs.AccessType = AccessType;
            inputs.OptMessage = OptMessage;
            inputs.UserId = HttpContext.Session.GetInt32("UserId");
            int? pId = HttpContext.Session.GetInt32("PID");
            int? cID = HttpContext.Session.GetInt32("CID");
            int? fId = HttpContext.Session.GetInt32("FID");

            if (!string.IsNullOrEmpty(accessId))
            {
                List<UserKeyValues> UserList = new List<UserKeyValues>();
                // Get the list of users from the cache or fetch it if not available
                if (_memoryCache.TryGetValue("UserList", out var cachedUserListObj) && cachedUserListObj is List<UserKeyValues> cachedUserList && cachedUserList != null)
                {
                    UserList = cachedUserList;
                }
                else
                {
                    UserList = await GetUsers(1) ?? new List<UserKeyValues>();
                    _memoryCache.Set("UserList", UserList, TimeSpan.FromMinutes(30));
                }
                // Split the accessId string into individual IDs and find the corresponding user emails
                var accessIds = accessId.Split(',').Select(a => a.Trim()).ToList();
                foreach (var uId in accessIds)
                {
                    var user = UserList.FirstOrDefault(u => u.UserId == Convert.ToInt32(uId));
                    if (user != null && !string.IsNullOrEmpty(user.Email))
                    {
                        emailIds = string.IsNullOrEmpty(emailIds) ? user.Email : emailIds + "," + user.Email;

                    }
                }


            }

            inputs.EmailIds = emailIds;
            inputs.Id = Id;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiProjectFileUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("EditDrawingFile", inputs);
                    if (response.IsSuccessStatusCode)
                    {

                        var drawingFileDetailsCacheKey = $"DrawingFileDetails_{pId}_{cID}_{fId}";
                        _memoryCache.Remove(drawingFileDetailsCacheKey);
                        var data = await response.Content.ReadAsStringAsync();
                        res = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data) ?? new OutPutResponse();

                    }

                }


            }
            catch (SystemException)
            {
                res.DisplayMessage = "Project failed!";
            }
            return res;
        }


        [HttpGet]
        public async Task<OutPutResponse>? EditFileName(int Id, string? Name)
        {
            OutPutResponse res = new OutPutResponse();
            ProjectDrawingsDTO inputs = new ProjectDrawingsDTO();
            inputs.IsActive = true;
            inputs.CreationDate = DateTime.Now;
            inputs.UserId = HttpContext.Session.GetInt32("UserId");
            int? pId = HttpContext.Session.GetInt32("PID");
            int? cID = HttpContext.Session.GetInt32("CID");

            inputs.FolderName = Name;
            inputs.Id = Id;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiProjectFileUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("EditDrawingFileName", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var drFileDetailsCacheKey = $"DrFolderDetails_{inputs.ProjectId}_{inputs.CategoryId}";
                        _memoryCache.Remove(drFileDetailsCacheKey);
                        var data = await response.Content.ReadAsStringAsync();

                        res = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data) ?? new OutPutResponse();


                    }

                }


            }
            catch (SystemException)
            {
                res.DisplayMessage = "Project failed!";
            }
            return res;
        }

        public async Task<List<ProjectRoleSummeryDTO>>? GetProjectRoleSummery(int? unitId, int? projectId)
        {
            // DataTable dt = new DataTable();
            List<ProjectRoleSummeryDTO> ProjectLst = new List<ProjectRoleSummeryDTO>();

            // int countryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ecountryId));
            //  inputs.CountryId = countryId;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiProjectUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync($"GetProjectRoleSummery?unitId={unitId}&projectId={projectId}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    ProjectLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProjectRoleSummeryDTO>>(data) ?? new List<ProjectRoleSummeryDTO>();
                    foreach (var item in ProjectLst ?? new List<ProjectRoleSummeryDTO>())
                    {
                        if (item.ProfilePic != null)
                            item.Base64ProfileImage = item.ProfilePic;  //"data:image/png;base64," + Convert.ToBase64String(item.ProfilePic, 0, item.ProfilePic.Length);
                    }
                }
                return ProjectLst;
            }

        }
    }
}
