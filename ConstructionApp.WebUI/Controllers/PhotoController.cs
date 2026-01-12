using Construction.Infrastructure.Helper;
using Construction.Infrastructure.KeyValues;
using Construction.Infrastructure.Models;
using ConstructionApp.WebUI.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Rotativa.AspNetCore;
using System.Security.Cryptography;
using static NuGet.Packaging.PackagingConstants;

namespace ConstructionApp.WebUI.Controllers
{
    public class PhotoController : Controller
    {
        private readonly ILogger<PhotoController> _logger;
        private IWebHostEnvironment _environment;
        private readonly IMemoryCache _memoryCache;
        public PhotoController(ILogger<PhotoController> logger, IWebHostEnvironment environment, IMemoryCache memoryCache)
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
                    ProjectLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProjectDashboardDTO>>(data) ?? new List<ProjectDashboardDTO>();

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
        [Route("Photo/ProjectCategoryDirectory/{epId}")]
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
            objOutPut.UserType = HttpContext.Session.GetInt32("UserType");
            objOutPut.EncProjectId = HttpContext.Session.GetString("ePID");
            var userId = HttpContext.Session.GetInt32("UserId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            var projectCacheKey = $"Projects_{unitId}_{pId}";
            if (_memoryCache.TryGetValue(projectCacheKey, out var cachedProjectListObj) && cachedProjectListObj is List<ProjectDashboardDTO> cachedProjectList && cachedProjectList != null)
            {
                objOutPut.ProjectDashboardList = cachedProjectList;
            }
            else
            {
                objOutPut.ProjectDashboardList = await GetProjectDashboard(userId) ?? new List<ProjectDashboardDTO>();
                _memoryCache.Set(projectCacheKey, objOutPut.ProjectDashboardList);
            }
            objOutPut.ProjectName = objOutPut.ProjectDashboardList?.Where(p => p.ProjectId == pId).Select(m => m.ProjectName).FirstOrDefault() ?? string.Empty;
            //var photoCategoryCacheKey = $"PhotoCategory_{pId}";
            //if (_memoryCache.TryGetValue(photoCategoryCacheKey, out var cachedCategoryListObj) && cachedCategoryListObj is List<PhotoCategoryDTO> cachedCategoryList && cachedCategoryList != null)
            //{
            //    objOutPut.PhotoCategoryList = cachedCategoryList;
            //}
            //else
            //{
            objOutPut.PhotoCategoryList = await GetCategoryList() ?? new List<PhotoCategoryDTO>();
            foreach (var item in objOutPut.PhotoCategoryList)
            {
                item.EncCategoryId = CommonHelper.EncryptURLHTML(item.Id.ToString());
            }
            //  _memoryCache.Set(photoCategoryCacheKey, objOutPut.PhotoCategoryList, TimeSpan.FromMinutes(30));
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
            string? ePID = HttpContext.Session.GetString("ePID");
            if (!string.IsNullOrEmpty(ePID))
            {
                pId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ePID));
            }
            //int pId = Convert.ToInt32(CommonHelper.DecryptURLHTML(epId));

            HttpContext.Session.SetInt32("PID", pId);
            // HttpContext.Session.SetString("ePID", epId);
            //string? strMenuSession = HttpContext.Session.GetString("Menu");
            //if (strMenuSession == null)
            //    return RedirectToAction("Index", "Home");

            //AccessMasterDTO? empSession = (AccessMasterDTO?)(JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession));
            //objOutPut.AccessList = empSession.AccessList.ToList();
            objOutPut.UserType = HttpContext.Session.GetInt32("UserType");
            objOutPut.EncProjectId = HttpContext.Session.GetString("ePID");
            if (_memoryCache.TryGetValue($"Projects_{pId}", out List<ProjectDashboardDTO> cachedProjectList))
            {
                objOutPut.ProjectDashboardList = cachedProjectList;
            }
            else
            {
                objOutPut.ProjectDashboardList = await GetProjectDashboard(HttpContext.Session.GetInt32("UserId"));
                _memoryCache.Set($"Projects_{pId}", objOutPut.ProjectDashboardList);
            }
            // var projectList = _memoryCache.Get("Projects") as List<ProjectDashboardDTO>;
            objOutPut.ProjectName = objOutPut.ProjectDashboardList.Where(p => p.ProjectId == HttpContext.Session.GetInt32("PID")).Select(m => m.ProjectName).FirstOrDefault();


            //if (_memoryCache.TryGetValue("PhotoCategory", out List<PhotoCategoryDTO> cachedCategortyList))
            //{
            //    objOutPut.PhotoCategoryList = cachedCategortyList;
            //}
            //else
            //{
            objOutPut.PhotoCategoryList = await GetCategoryList();
            foreach (var item in objOutPut.PhotoCategoryList)
            {

                item.EncCategoryId = CommonHelper.EncryptURLHTML(item.Id.ToString());

            }

            //_memoryCache.Set("PhotoCategory", objOutPut.PhotoCategoryList, TimeSpan.FromMinutes(30));
            //  }

            //objOutPut.PhotoCategoryList = await GetCategoryList();
            //foreach (var item in objOutPut.PhotoCategoryList)
            //{

            //    item.EncCategoryId = CommonHelper.EncryptURLHTML(item.Id.ToString());

            //}
            objOutPut.ProjectId = pId;
            return View(objOutPut);
        }

        public async Task<List<PhotoCategoryDTO>> GetCategoryList()
        {
            // DataTable dt = new DataTable();
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            List<PhotoCategoryDTO> PriorityLst = new List<PhotoCategoryDTO>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetPhotoCategory");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    PriorityLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PhotoCategoryDTO>>(data) ?? new List<PhotoCategoryDTO>();

                }


            }
            return PriorityLst;
        }

        [HttpGet]
        [Route("Photo/ProjectPhoto/{eCategoryId}")]
        public async Task<IActionResult> ProjectPhoto(string eCategoryId)
        {
            ProjectsDTO objOutPut = new ProjectsDTO();
            int CategoryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eCategoryId));
            HttpContext.Session.SetString("eCID", eCategoryId);
            HttpContext.Session.SetInt32("CID", CategoryId);
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (strMenuSession == null)
                return RedirectToAction("Index", "Home");
            AccessMasterDTO? empSession = JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession);
            var userId = HttpContext.Session.GetInt32("UserId");
            var pId = HttpContext.Session.GetInt32("PID");
            //var unitId = HttpContext.Session.GetInt32("UnitId");
            var projectCacheKey = $"Projects_{unitId}_{pId}";
            if (_memoryCache.TryGetValue(projectCacheKey, out var cachedProjectListObj) && cachedProjectListObj is List<ProjectDashboardDTO> cachedProjectList && cachedProjectList != null)
            {
                objOutPut.ProjectDashboardList = cachedProjectList;
            }
            else
            {
                objOutPut.ProjectDashboardList = await GetProjectDashboard(userId) ?? new List<ProjectDashboardDTO>();
                _memoryCache.Set(projectCacheKey, objOutPut.ProjectDashboardList);
            }
            objOutPut.ProjectName = objOutPut.ProjectDashboardList?.Where(p => p.ProjectId == pId).Select(m => m.ProjectName).FirstOrDefault() ?? string.Empty;
            objOutPut.EncProjectId = HttpContext.Session.GetString("ePID");
            objOutPut.AccessList = empSession?.AccessList?.ToList() ?? new List<AccessMasterDTO>();
            objOutPut.UserType = HttpContext.Session.GetInt32("UserType");
            var photoCategoryCacheKey = $"PhotoCategory_{pId}";
            if (_memoryCache.TryGetValue(photoCategoryCacheKey, out var cachedCategoryListObj) && cachedCategoryListObj is List<PhotoCategoryDTO> cachedCategoryList && cachedCategoryList != null)
            {
                objOutPut.PhotoCategoryList = cachedCategoryList;
            }
            else
            {
                objOutPut.PhotoCategoryList = await GetCategoryList() ?? new List<PhotoCategoryDTO>();
                foreach (var item in objOutPut.PhotoCategoryList)
                {
                    item.EncCategoryId = CommonHelper.EncryptURLHTML(item.Id.ToString());
                }
                _memoryCache.Set(photoCategoryCacheKey, objOutPut.PhotoCategoryList, TimeSpan.FromMinutes(30));
            }
            objOutPut.CategoryName = objOutPut.PhotoCategoryList?.Where(p => p.Id == CategoryId).Select(m => m.Category).FirstOrDefault() ?? string.Empty;
            //if (_memoryCache.Get("UserList") == null)
            //{
            //    objOutPut.UserList = await GetUsers() ?? new List<UserKeyValues>();
            //    _memoryCache.Set("UserList", objOutPut.UserList, TimeSpan.FromMinutes(30));
            //}
            //else
            //{
            //    objOutPut.UserList = _memoryCache.Get<List<UserKeyValues>>("UserList") ?? new List<UserKeyValues>();
            //}
            var photoFolderDetailsCacheKey = $"PhotoFolderDetails_{pId}_{CategoryId}";
            if (_memoryCache.TryGetValue(photoFolderDetailsCacheKey, out var cachedFolderListObj) && cachedFolderListObj is List<ProjectPhotosDTO> cachedFolderList && cachedFolderList != null)
            {
                objOutPut.PhotoFolderList = cachedFolderList;
            }
            else
            {
                objOutPut.PhotoFolderList = await GetPhotoFolder(pId, CategoryId) ?? new List<ProjectPhotosDTO>();
                _memoryCache.Set(photoFolderDetailsCacheKey, objOutPut.PhotoFolderList, TimeSpan.FromMinutes(30));
            }

            string roleKey = $"ProjectRole_{unitId}";

            if (_memoryCache.TryGetValue(roleKey, out var cachedProjectRoleObj) && cachedProjectRoleObj is List<ProjectRoleSummeryDTO> cachedProjectRoleList && cachedProjectRoleList != null)
            {
                objOutPut.ProjectRoleSummeryList = cachedProjectRoleList;
            }
            else
            {
                objOutPut.ProjectRoleSummeryList = await GetProjectRoleSummery(unitId, pId);
                _memoryCache.Set(roleKey, objOutPut.ProjectRoleSummeryList);
            }
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
            objOutPut.CategoryId = CategoryId;
            objOutPut.ProjectId = (int)pId;
            return View(objOutPut);
        }

        public async Task<IActionResult> ProjectPhotoById(int CategoryId)
        {
            ProjectsDTO objOutPut = new ProjectsDTO();
            string eCategoryId = CommonHelper.EncryptURLHTML(Convert.ToString(CategoryId));
            HttpContext.Session.SetString("eCID", eCategoryId);
            HttpContext.Session.SetInt32("CID", CategoryId);
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (strMenuSession == null)
                return RedirectToAction("Index", "Home");
            AccessMasterDTO? empSession = JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession);
            var userId = HttpContext.Session.GetInt32("UserId");
            var pId = HttpContext.Session.GetInt32("PID");
            var projectCacheKey = $"Projects_{unitId}_{pId}";
            if (_memoryCache.TryGetValue(projectCacheKey, out var cachedProjectListObj) && cachedProjectListObj is List<ProjectDashboardDTO> cachedProjectList && cachedProjectList != null)
            {
                objOutPut.ProjectDashboardList = cachedProjectList;
            }
            else
            {
                objOutPut.ProjectDashboardList = await GetProjectDashboard(userId) ?? new List<ProjectDashboardDTO>();
                _memoryCache.Set(projectCacheKey, objOutPut.ProjectDashboardList);
            }
            objOutPut.ProjectName = objOutPut.ProjectDashboardList?.Where(p => p.ProjectId == pId).Select(m => m.ProjectName).FirstOrDefault() ?? string.Empty;
            objOutPut.EncProjectId = HttpContext.Session.GetString("ePID");
            objOutPut.AccessList = empSession?.AccessList?.ToList() ?? new List<AccessMasterDTO>();
            objOutPut.UserType = HttpContext.Session.GetInt32("UserType");
            var photoCategoryCacheKey = $"PhotoCategory_{pId}";
            if (_memoryCache.TryGetValue(photoCategoryCacheKey, out var cachedCategoryListObj) && cachedCategoryListObj is List<PhotoCategoryDTO> cachedCategoryList && cachedCategoryList != null)
            {
                objOutPut.PhotoCategoryList = cachedCategoryList;
            }
            else
            {
                objOutPut.PhotoCategoryList = await GetCategoryList() ?? new List<PhotoCategoryDTO>();
                foreach (var item in objOutPut.PhotoCategoryList)
                {
                    item.EncCategoryId = CommonHelper.EncryptURLHTML(item.Id.ToString());
                }
                _memoryCache.Set(photoCategoryCacheKey, objOutPut.PhotoCategoryList, TimeSpan.FromMinutes(30));
            }
            objOutPut.CategoryName = objOutPut.PhotoCategoryList?.Where(p => p.Id == CategoryId).Select(m => m.Category).FirstOrDefault() ?? string.Empty;
            if (_memoryCache.Get("UserList") == null)
            {
                objOutPut.UserList = await GetUsers(1) ?? new List<UserKeyValues>();
                _memoryCache.Set("UserList", objOutPut.UserList, TimeSpan.FromMinutes(30));
            }
            else
            {
                objOutPut.UserList = _memoryCache.Get<List<UserKeyValues>>("UserList") ?? new List<UserKeyValues>();
            }
            var photoFolderDetailsCacheKey = $"PhotoFolderDetails_{pId}_{CategoryId}";
            if (_memoryCache.TryGetValue(photoFolderDetailsCacheKey, out var cachedFolderListObj) && cachedFolderListObj is List<ProjectPhotosDTO> cachedFolderList && cachedFolderList != null)
            {
                objOutPut.PhotoFolderList = cachedFolderList;
            }
            else
            {
                objOutPut.PhotoFolderList = await GetPhotoFolder(pId, CategoryId) ?? new List<ProjectPhotosDTO>();
                _memoryCache.Set(photoFolderDetailsCacheKey, objOutPut.PhotoFolderList, TimeSpan.FromMinutes(30));
            }

            string roleKey = $"ProjectRole_{unitId}";

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
            return View("ProjectPhoto",objOutPut);
        }
        public async Task<IActionResult> ProjectPhoto()
        {
            int CategoryId = 0;
            ProjectsDTO objOutPut = new ProjectsDTO();
            string? eCID = HttpContext.Session.GetString("eCID");
            if (!string.IsNullOrEmpty(eCID))
            {
                CategoryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(HttpContext.Session.GetString("eCID")));
            }
            //  HttpContext.Session.SetString("eCID", eCategoryId);
            HttpContext.Session.SetInt32("CID", CategoryId);
            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (strMenuSession == null)
                return RedirectToAction("Index", "Home");


            AccessMasterDTO? empSession = (AccessMasterDTO?)(JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession));
            //  var projectList = _memoryCache.Get("Projects") as List<ProjectDashboardDTO>;
            //  objOutPut.ProjectName = projectList.Where(p => p.ProjectId == HttpContext.Session.GetInt32("PID")).Select(m => m.ProjectName).FirstOrDefault();

            objOutPut.EncProjectId = HttpContext.Session.GetString("ePID");
            objOutPut.AccessList = empSession.AccessList.ToList();
            objOutPut.UserType = HttpContext.Session.GetInt32("UserType");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            var pId = HttpContext.Session.GetInt32("PID");
            //objOutPut.UserList = await GetUsers();
            //objOutPut.PhotoCategoryList = await GetCategoryList();
            //_memoryCache.Set("Category", objOutPut.PhotoCategoryList);
            //objOutPut.PhotoFolderList = await GetPhotoFolder(HttpContext.Session.GetInt32("PID"), CategoryId);
            //_memoryCache.Set("PhotoFolderDetails", objOutPut.PhotoFolderList);
            var projectCacheKey = $"Projects_{unitId}_{pId}";
            if (_memoryCache.TryGetValue(projectCacheKey, out var cachedProjectListObj) && cachedProjectListObj is List<ProjectDashboardDTO> cachedProjectList && cachedProjectList != null)
            {
                objOutPut.ProjectDashboardList = cachedProjectList;
            }
            else
            {
                objOutPut.ProjectDashboardList = await GetProjectDashboard(HttpContext.Session.GetInt32("UserId"));
                _memoryCache.Set(projectCacheKey, objOutPut.ProjectDashboardList);
            }
            // var projectList = _memoryCache.Get("Projects") as List<ProjectDashboardDTO>;
            objOutPut.ProjectName = objOutPut.ProjectDashboardList.Where(p => p.ProjectId == HttpContext.Session.GetInt32("PID")).Select(m => m.ProjectName).FirstOrDefault() ?? string.Empty;
            var photoCategoryCacheKey = $"PhotoCategory_{HttpContext.Session.GetInt32("PID")}";
            if (_memoryCache.TryGetValue(photoCategoryCacheKey, out var cachedCategoryListObj) && cachedCategoryListObj is List<PhotoCategoryDTO> cachedCategoryList && cachedCategoryList != null)
            {
                objOutPut.PhotoCategoryList = cachedCategoryList;
            }
            else
            {
                objOutPut.PhotoCategoryList = await GetCategoryList() ?? new List<PhotoCategoryDTO>();
                foreach (var item in objOutPut.PhotoCategoryList)
                {

                    item.EncCategoryId = CommonHelper.EncryptURLHTML(item.Id.ToString());

                }

                _memoryCache.Set(photoCategoryCacheKey, objOutPut.PhotoCategoryList, TimeSpan.FromMinutes(30));
            }
            objOutPut.CategoryName = objOutPut.PhotoCategoryList?.Where(p => p.Id == CategoryId).Select(m => m.Category).FirstOrDefault() ?? string.Empty;
            //if (_memoryCache.Get("UserList") == null)
            //{
            //    objOutPut.UserList = await GetUsers() ?? new List<UserKeyValues>();
            //    _memoryCache.Set("UserList", objOutPut.UserList, TimeSpan.FromMinutes(30));
            //}
            //else
            //{
            //    objOutPut.UserList = _memoryCache.Get<List<UserKeyValues>>("UserList") ?? new List<UserKeyValues>();
            //}
            var photoFolderDetailsCacheKey = $"PhotoFolderDetails_{HttpContext.Session.GetInt32("PID")}_{CategoryId}";
            if (_memoryCache.TryGetValue(photoFolderDetailsCacheKey, out var cachedFolderListObj) && cachedFolderListObj is List<ProjectPhotosDTO> cachedFolderList && cachedFolderList != null)
            {
                objOutPut.PhotoFolderList = cachedFolderList;
            }
            else
            {
                objOutPut.PhotoFolderList = await GetPhotoFolder(HttpContext.Session.GetInt32("PID"), CategoryId) ?? new List<ProjectPhotosDTO>();
                _memoryCache.Set(photoFolderDetailsCacheKey, objOutPut.PhotoFolderList, TimeSpan.FromMinutes(30));
            }
            string roleKey = $"ProjectRole_{unitId}";

            if (_memoryCache.TryGetValue(roleKey, out var cachedProjectRoleObj) && cachedProjectRoleObj is List<ProjectRoleSummeryDTO> cachedProjectRoleList && cachedProjectRoleList != null)
            {
                objOutPut.ProjectRoleSummeryList = cachedProjectRoleList;
            }
            else
            {
                objOutPut.ProjectRoleSummeryList = await GetProjectRoleSummery(unitId, pId);
                _memoryCache.Set(roleKey, objOutPut.ProjectRoleSummeryList);
            }

            if (_memoryCache.Get("VendorList") == null)
            {
                objOutPut.UserList = await GetUsers(3);
                _memoryCache.Set("VendorList", objOutPut.UserList);
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

            objOutPut.CategoryId = CategoryId;
            objOutPut.ProjectId = (int)pId;
            return View(objOutPut);
        }

        public async Task<OutPutResponse> CreatePhotoAlbum(string folderName, int cId)
        {
            OutPutResponse objRespnose = new OutPutResponse();
            ProjectPhotosDTO inputs = new ProjectPhotosDTO();
            int? pId = HttpContext.Session.GetInt32("PID");
            // int? cId = HttpContext.Session.GetInt32("CID");
            inputs.ProjectId = pId;
            inputs.CategoryId = cId;
            inputs.IsFolder = true;
            inputs.UserId = HttpContext.Session.GetInt32("UserId");
            inputs.FolderName = folderName;
            string path = "", response = "0";
            try
            {
                ////string path = Server.MapPath("~/YourFolder/" + folderName);
                //string folderpath = Path.Combine(this._environment.WebRootPath, "Photos");
                //path = Path.Combine(folderpath, Convert.ToString(pId), Convert.ToString(cId));
                //if (!Directory.Exists(path))
                //{
                //    Directory.CreateDirectory(path);

                //    nPath = Path.Combine(path, folderName);
                //    if (!Directory.Exists(nPath))
                //    {
                //        Directory.CreateDirectory(nPath);
                //    }
                //}
                //else
                //{
                //    nPath = Path.Combine(path, folderName);
                //    if (!Directory.Exists(nPath))
                //    {
                //        Directory.CreateDirectory(nPath);
                //    }
                //}
                objRespnose = await SavePhotoFolder(inputs);

                // RedirectToAction("ProjectDocuments", "Document", new { eCategoryId = HttpContext.Session.GetString("eCID") });
            }
            catch (Exception)
            {
                response = "0";
            }
            //  RedirectToAction("ProjectDocuments", "Document", new { eCategoryId = HttpContext.Session.GetString("eCID") });
            return objRespnose;
        }

        public async Task<OutPutResponse>? SavePhotoFolder(ProjectPhotosDTO inputs)
        {
            OutPutResponse res = new OutPutResponse();
            inputs.IsActive = true;
            inputs.CreationDate = DateTime.Now;
            inputs.UserId = HttpContext.Session.GetInt32("UserId");

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiPhotoFileUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("SaveProjectPhotoFile", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var photoFolderDetailsCacheKey = $"PhotoFolderDetails_{inputs.ProjectId}_{inputs.CategoryId}";
                        _memoryCache.Remove(photoFolderDetailsCacheKey);

                        var data = await response.Content.ReadAsStringAsync();

                        res = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data) ?? new OutPutResponse();


                    }

                }


            }
            catch (SystemException)
            {
                //if (res != null) res.DisplayMessage = "Project failed!" + ex.Message;
            }
            return res;
        }

        public async Task<List<ProjectPhotosDTO>> GetPhotoFolder(int? pId, int? cId)
        {
            // DataTable dt = new DataTable();
            //  int? unitId = HttpContext.Session.GetInt32("UnitId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            int? UType = HttpContext.Session.GetInt32("UserType");
            List<ProjectPhotosDTO> PriorityLst = new List<ProjectPhotosDTO>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiPhotoFileUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetProjectPhotoFolders?pId=" + pId + "&cId=" + cId + "&uId=" + UserId + "&uType=" + UType);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    PriorityLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProjectPhotosDTO>>(data) ?? new List<ProjectPhotosDTO>();

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

        [HttpGet]
        [Route("Photo/Albums/{eFolderId}")]
        public async Task<IActionResult> Albums(string? eFolderId)
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
            HttpContext.Session.SetInt32("FID", FolderId);
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            var projectCacheKey = $"Projects_{unitId}_{pId}";
            if (_memoryCache.TryGetValue(projectCacheKey, out var cachedProjectListObj) && cachedProjectListObj is List<ProjectDashboardDTO> cachedProjectList && cachedProjectList != null)
            {
                objOutPut.ProjectDashboardList = cachedProjectList;
            }
            else
            {
                objOutPut.ProjectDashboardList = await GetProjectDashboard(userId) ?? new List<ProjectDashboardDTO>();
                _memoryCache.Set(projectCacheKey, objOutPut.ProjectDashboardList);
            }
            objOutPut.ProjectName = objOutPut.ProjectDashboardList?.Where(p => p.ProjectId == pId).Select(m => m.ProjectName).FirstOrDefault() ?? string.Empty;
            objOutPut.EncProjectId = HttpContext.Session.GetString("ePID");
            if (_memoryCache.Get("UserList") == null)
            {
                objOutPut.UserList = await GetUsers(1) ?? new List<UserKeyValues>();
                _memoryCache.Set("UserList", objOutPut.UserList);
            }
            else
            {
                objOutPut.UserList = _memoryCache.Get<List<UserKeyValues>>("UserList") ?? new List<UserKeyValues>();
            }
            var photoCategoryCacheKey = $"PhotoCategory_{pId}";
            if (_memoryCache.TryGetValue(photoCategoryCacheKey, out var cachedCategoryListObj) && cachedCategoryListObj is List<PhotoCategoryDTO> cachedCategoryList && cachedCategoryList != null)
            {
                objOutPut.PhotoCategoryList = cachedCategoryList;
            }
            else
            {
                objOutPut.PhotoCategoryList = await GetCategoryList() ?? new List<PhotoCategoryDTO>();
                foreach (var item in objOutPut.PhotoCategoryList)
                {
                    item.EncCategoryId = CommonHelper.EncryptURLHTML(item.Id.ToString());
                }
                _memoryCache.Set(photoCategoryCacheKey, objOutPut.PhotoCategoryList, TimeSpan.FromMinutes(30));
            }
            objOutPut.CategoryName = objOutPut.PhotoCategoryList?.Where(p => p.Id == cId).Select(m => m.Category).FirstOrDefault() ?? string.Empty;
            var photoFolderDetailsCacheKey = $"PhotoFolderDetails_{pId}_{cId}";
            if (_memoryCache.TryGetValue(photoFolderDetailsCacheKey, out var cachedFolderListObj) && cachedFolderListObj is List<ProjectPhotosDTO> cachedFolderList && cachedFolderList != null)
            {
                objOutPut.PhotoFolderList = cachedFolderList;
            }
            else
            {
                objOutPut.PhotoFolderList = await GetPhotoFolder(pId, cId) ?? new List<ProjectPhotosDTO>();
                _memoryCache.Set(photoFolderDetailsCacheKey, objOutPut.PhotoFolderList, TimeSpan.FromMinutes(30));
            }
            objOutPut.FileDetails = objOutPut.PhotoFolderList?.Where(p => p.Id == FolderId).Select(m => m.FolderName).FirstOrDefault() ?? string.Empty;
            var photoFileListCacheKey = $"PhotoFileList_{pId}_{cId}_{FolderId}";
            if (_memoryCache.TryGetValue(photoFileListCacheKey, out var cachedFileListObj) && cachedFileListObj is List<ProjectFilePhotosDTO> cachedFileList && cachedFileList != null)
            {
                objOutPut.PhotoFolderFileList = cachedFileList;
            }
            else
            {
                objOutPut.PhotoFolderFileList = await GetFolderFiles(pId, cId, FolderId) ?? new List<ProjectFilePhotosDTO>();
                _memoryCache.Set(photoFileListCacheKey, objOutPut.PhotoFolderFileList, TimeSpan.FromMinutes(30));
            }
            string roleKey = $"ProjectRole_{unitId}";

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

        public async Task<IActionResult> AlbumsById(int FolderId)
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
                objOutPut.ProjectDashboardList = await GetProjectDashboard(userId) ?? new List<ProjectDashboardDTO>();
                _memoryCache.Set(projectCacheKey, objOutPut.ProjectDashboardList);
            }
            objOutPut.ProjectName = objOutPut.ProjectDashboardList?.Where(p => p.ProjectId == pId).Select(m => m.ProjectName).FirstOrDefault() ?? string.Empty;
            objOutPut.EncProjectId = HttpContext.Session.GetString("ePID");
            if (_memoryCache.Get("UserList") == null)
            {
                objOutPut.UserList = await GetUsers(1) ?? new List<UserKeyValues>();
                _memoryCache.Set("UserList", objOutPut.UserList);
            }
            else
            {
                objOutPut.UserList = _memoryCache.Get<List<UserKeyValues>>("UserList") ?? new List<UserKeyValues>();
            }
            var photoCategoryCacheKey = $"PhotoCategory_{pId}";
            if (_memoryCache.TryGetValue(photoCategoryCacheKey, out var cachedCategoryListObj) && cachedCategoryListObj is List<PhotoCategoryDTO> cachedCategoryList && cachedCategoryList != null)
            {
                objOutPut.PhotoCategoryList = cachedCategoryList;
            }
            else
            {
                objOutPut.PhotoCategoryList = await GetCategoryList() ?? new List<PhotoCategoryDTO>();
                foreach (var item in objOutPut.PhotoCategoryList)
                {
                    item.EncCategoryId = CommonHelper.EncryptURLHTML(item.Id.ToString());
                }
                _memoryCache.Set(photoCategoryCacheKey, objOutPut.PhotoCategoryList, TimeSpan.FromMinutes(30));
            }
            objOutPut.CategoryName = objOutPut.PhotoCategoryList?.Where(p => p.Id == cId).Select(m => m.Category).FirstOrDefault() ?? string.Empty;
            var photoFolderDetailsCacheKey = $"PhotoFolderDetails_{pId}_{cId}";
            if (_memoryCache.TryGetValue(photoFolderDetailsCacheKey, out var cachedFolderListObj) && cachedFolderListObj is List<ProjectPhotosDTO> cachedFolderList && cachedFolderList != null)
            {
                objOutPut.PhotoFolderList = cachedFolderList;
            }
            else
            {
                objOutPut.PhotoFolderList = await GetPhotoFolder(pId, cId) ?? new List<ProjectPhotosDTO>();
                _memoryCache.Set(photoFolderDetailsCacheKey, objOutPut.PhotoFolderList, TimeSpan.FromMinutes(30));
            }
            objOutPut.FileDetails = objOutPut.PhotoFolderList?.Where(p => p.Id == FolderId).Select(m => m.FolderName).FirstOrDefault() ?? string.Empty;
            var photoFileListCacheKey = $"PhotoFileList_{pId}_{cId}_{FolderId}";
            if (_memoryCache.TryGetValue(photoFileListCacheKey, out var cachedFileListObj) && cachedFileListObj is List<ProjectFilePhotosDTO> cachedFileList && cachedFileList != null)
            {
                objOutPut.PhotoFolderFileList = cachedFileList;
            }
            else
            {
                objOutPut.PhotoFolderFileList = await GetFolderFiles(pId, cId, FolderId) ?? new List<ProjectFilePhotosDTO>();
                _memoryCache.Set(photoFileListCacheKey, objOutPut.PhotoFolderFileList, TimeSpan.FromMinutes(30));
            }
            string roleKey = $"ProjectRole_{unitId}";

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
            return View("Albums",objOutPut);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(209715200)]
        public async Task<IActionResult> CreateFolderFile([FromForm] string Fid, [FromForm] string fileName)
        {

            // int cId = 0; string fileName = "";
            var files = Request.Form.Files;
            ProjectFilePhotosDTO inputs = new ProjectFilePhotosDTO();
            int? pId = HttpContext.Session.GetInt32("PID");
            int? cId = HttpContext.Session.GetInt32("CID");
            inputs.ProjectId = pId;
            inputs.CategoryId = cId;
            inputs.FolderId = Convert.ToInt32(Fid);
            inputs.UserId = HttpContext.Session.GetInt32("UserId");
            //  inputs.FolderName = folderName;
            string path = "", response = "0", fName = "";
            try
            {
                //string path = Server.MapPath("~/YourFolder/" + folderName);
                string folderpath = Path.Combine(this._environment.WebRootPath, "Photos");
                path = Path.Combine(folderpath ?? string.Empty, Convert.ToString(pId) ?? string.Empty, Convert.ToString(cId) ?? string.Empty, Convert.ToString(Fid) ?? string.Empty);


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
                        inputs.FilePath = fName.Replace("'", "").Replace("\"", "_");
                        inputs.FileName = fileName;

                        using (FileStream stream = new FileStream(Path.Combine(path, fName), FileMode.Create))
                        {
                            postedFile.CopyTo(stream);

                        }

                        await SavePhotoFolderFile(inputs);
                    }

                }
                else
                {
                    inputs.FilePath = "";
                    inputs.FileName = "";
                }


                response = "1";
            }
            catch (Exception)
            {
                response = "0";
            }
            // return Ok();
            return Ok(new { count = files.Count });

        }


        public async Task<OutPutResponse>? SavePhotoFolderFile(ProjectFilePhotosDTO inputs)
        {
            OutPutResponse res = new OutPutResponse();
            inputs.IsActive = true;
            inputs.CreationDate = DateTime.Now;
            //  inputs.UserId = HttpContext.Session.GetInt32("UserId");

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiPhotoFileUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("SavePhotoFolderFile", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var photoFileListCacheKey = $"PhotoFileList_{inputs.ProjectId}_{inputs.CategoryId}_{inputs.FolderId}";
                        _memoryCache.Remove(photoFileListCacheKey);
                        var data = await response.Content.ReadAsStringAsync();
                        res = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data) ?? new OutPutResponse();

                    }

                }


            }
            catch (SystemException)
            {
                //if (res != null) res.DisplayMessage = "Project failed!" + ex.Message;
            }
            return res;
        }

        public async Task<List<ProjectFilePhotosDTO>> GetFolderFiles(int? pId, int? cId, int? fId)
        {
            // DataTable dt = new DataTable();
            //  int? unitId = HttpContext.Session.GetInt32("UnitId");
            List<ProjectFilePhotosDTO> PriorityLst = new List<ProjectFilePhotosDTO>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiPhotoFileUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetProjectPhotoFolderFiles?pId=" + pId + "&cId=" + cId + "&fId=" + fId);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    PriorityLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProjectFilePhotosDTO>>(data) ?? new List<ProjectFilePhotosDTO>();

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
                client.BaseAddress = new Uri(EnvironmentUrl.apiPhotoFileUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("DeletePhotoAlbumFile?pId=" + FileId);
                if (response.IsSuccessStatusCode)
                {
                    var photoFileListCacheKey = $"PhotoFileList_{pId}_{cID}_{fId}";
                    _memoryCache.Remove(photoFileListCacheKey);
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

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiPhotoFileUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("DeletePhotoFolder?pId=" + FileId);
                if (response.IsSuccessStatusCode)
                {
                    int? pId = HttpContext.Session.GetInt32("PID");
                    int? cID = HttpContext.Session.GetInt32("CID");
                    var photoFolderDetailsCacheKey = $"PhotoFolderDetails_{pId}_{cID}";
                    _memoryCache.Remove(photoFolderDetailsCacheKey);
                    var data = await response.Content.ReadAsStringAsync();
                    res = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data) ?? new OutPutResponse();

                }
            }

            return res;
        }


        public IActionResult Download(string fileName)
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
                //HttpResponseMessage response = await client.GetAsync("GetUnitUsers?unitId=" + unitId);
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
        public async Task<ProjectPhotosDTO> GetInviteUsers(int Id)
        {
            await Task.CompletedTask;
            int? pId = HttpContext.Session.GetInt32("PID");
            int? cID = HttpContext.Session.GetInt32("CID");

            var photoFolderDetailsCacheKey = $"PhotoFolderDetails_{pId}_{cID}";
            var FolderList = _memoryCache.Get(photoFolderDetailsCacheKey) as List<ProjectPhotosDTO> ?? new List<ProjectPhotosDTO>();
            return FolderList.Where(x => x.Id == Id).SingleOrDefault() ?? new ProjectPhotosDTO();
        }

        public async Task<ProjectFilePhotosDTO> GetFileUsers(int Id)
        {
            int? pId = HttpContext.Session.GetInt32("PID");
            int? cID = HttpContext.Session.GetInt32("CID");
            int? fId = HttpContext.Session.GetInt32("FID");

            await Task.CompletedTask;
            var photoFileListCacheKey = $"PhotoFileList_{pId}_{cID}_{fId}";
            var FolderList = _memoryCache.Get(photoFileListCacheKey) as List<ProjectFilePhotosDTO> ?? new List<ProjectFilePhotosDTO>();
            return FolderList.Where(x => x.Id == Id).SingleOrDefault() ?? new ProjectFilePhotosDTO();
        }
        [HttpGet]
        public async Task<List<ProjectFilePhotosDTO>> GetAllPhotoFile()
        {
            int? pId = HttpContext.Session.GetInt32("PID");
            int? cID = HttpContext.Session.GetInt32("CID");
            int? fId = HttpContext.Session.GetInt32("FID");
            await Task.CompletedTask;
            var photoFileListCacheKey = $"PhotoFileList_{pId}_{cID}_{fId}";
            var FolderList = _memoryCache.Get(photoFileListCacheKey) as List<ProjectFilePhotosDTO> ?? new List<ProjectFilePhotosDTO>();
            return FolderList;
        }

        [HttpGet]
        public async Task<OutPutResponse>? InviteSent(string? accessId, int? AccessType, int Id, string? OptMessage, string? UserType)
        {
            OutPutResponse res = new OutPutResponse();
            ProjectPhotosDTO inputs = new ProjectPhotosDTO();
            inputs.IsActive = true;
            inputs.CreationDate = DateTime.Now;
            inputs.UserId = HttpContext.Session.GetInt32("UserId");
            int? pId = HttpContext.Session.GetInt32("PID");
            int? cID = HttpContext.Session.GetInt32("CID");

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
                    client.BaseAddress = new Uri(EnvironmentUrl.apiPhotoFileUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("EditPhotoFolder", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var photoFolderDetailsCacheKey = $"PhotoFolderDetails_{pId}_{cID}";
                        _memoryCache.Remove(photoFolderDetailsCacheKey);

                        var data = await response.Content.ReadAsStringAsync();

                        res = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data) ?? new OutPutResponse();


                    }

                }


            }
            catch (SystemException)
            {
                //if (res != null) res.DisplayMessage = "Project failed!" + ex.Message;
            }
            return res;
        }

        [HttpGet]
        public async Task<OutPutResponse>? PhotoFileShareForDownload(string? accessId, int? AccessType, int Id, string? OptMessage, string? emailIds)
        {
            OutPutResponse res = new OutPutResponse();
            ProjectFilePhotosDTO inputs = new ProjectFilePhotosDTO();
            inputs.IsActive = true;
            inputs.CreationDate = DateTime.Now;
            inputs.AccessIds = accessId;
            inputs.AccessType = AccessType;
            inputs.OptMessage = OptMessage;
            inputs.UserId = HttpContext.Session.GetInt32("UserId");
            int? pId = HttpContext.Session.GetInt32("PID");
            int? cID = HttpContext.Session.GetInt32("CID");
            int? fId = HttpContext.Session.GetInt32("FID");

            inputs.EmailIds = emailIds;
            inputs.Id = Id;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiPhotoFileUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("EditPhotoFile", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var photoFileListCacheKey = $"PhotoFileList_{pId}_{cID}_{fId}";
                        _memoryCache.Remove(photoFileListCacheKey);

                        var data = await response.Content.ReadAsStringAsync();
                        res = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data) ?? new OutPutResponse();

                    }
                }
            }
            catch (SystemException)
            {
                //if (res != null) res.DisplayMessage = "Project failed!" + ex.Message;
            }
            return res;
        }

        [HttpGet]
        public async Task<OutPutResponse>? PhotoFilesShareForDownload(string? accessId, int? AccessType, string Ids, string? OptMessage, string? emailIds)
        {
            OutPutResponse res = new OutPutResponse();
            ProjectFilePhotosDTO inputs = new ProjectFilePhotosDTO();
            PublicUsersDTO outPuts = new PublicUsersDTO();
            inputs.IsActive = true;
            inputs.CreationDate = DateTime.Now;
            inputs.AccessIds = accessId;
            inputs.AccessType = AccessType;
            inputs.OptMessage = OptMessage;
            inputs.UserId = HttpContext.Session.GetInt32("UserId");
            int? pId = HttpContext.Session.GetInt32("PID");
            int? cID = HttpContext.Session.GetInt32("CID");
            int? fId = HttpContext.Session.GetInt32("FID");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            // Get the email IDs of the users who have access to the files
           // List<string> emailIdsList = new List<string>();
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
                        // emailIdsList.Add(user.Email);
                        // should be comma separated
                        emailIds = string.IsNullOrEmpty(emailIds) ? user.Email : emailIds + "," + user.Email;
                        //if (!string.IsNullOrEmpty(emailIds))
                        //{
                        //    emailIdsList.Add(user.Email);
                        //}
                        //else
                        //{
                        //    emailIdsList.Add(user.Email);
                        //}
                    }
                }


            }

            // End

            inputs.EmailIds = emailIds;
            inputs.Ids = Ids;
            inputs.emailPath = Path.Combine(_environment.WebRootPath, "GeneratedPdfs", Convert.ToString(inputs.UserId));
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiPhotoFileUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(Ids))    
                    {
                        // pdf generation
                   var resp= await GeneratePdfFromIds(Ids,Convert.ToString(inputs.UserId));                       
                        // end generation

                        HttpResponseMessage response = await client.PostAsJsonAsync("EditPhotoFile", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                           
                            var photoFileListCacheKey = $"PhotoFileList_{pId}_{cID}_{fId}";
                            _memoryCache.Remove(photoFileListCacheKey);
                            var data = await response.Content.ReadAsStringAsync();
                            res = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data) ?? new OutPutResponse();

                        }
                        //    }
                        //}

                    }
                }
            }
            catch (SystemException)
            {
                //if (res != null) res.DisplayMessage = "Project failed!" + ex.Message;
            }
            return res;
        }

        [HttpGet]
        public async Task<List<PublicUsersDTO>>? GetRecordsForPdfFile(string? ids)
        {
            try
            {
                List<PublicUsersDTO>? res = new List<PublicUsersDTO>();
                //ProjectFilePhotosDTO inputs = new ProjectFilePhotosDTO();
                //inputs.IsActive = true;
                //inputs.CreationDate = DateTime.Now;
                //inputs.UserId = HttpContext.Session.GetInt32("UserId");
                //inputs.Ids = ids;

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiPhotoFileUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync("PdfFile?ids=" + ids + "");
                  //  HttpResponseMessage response = await client.GetAsync("PdfFile");

                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        res = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PublicUsersDTO>>(data) ?? new List<PublicUsersDTO>();
                        foreach (var item in res ?? new List<PublicUsersDTO>())
                        {
                            if (item.FilePath != null)
                                item.FilePath = EnvironmentUrl.webApp+ "/Photos/" + item.PId + "/" + item.CId + "/" + item.FolderId + "/" + item.FilePath;
                        }
                        return res;
                    }
                }
            }
            catch (SystemException)
            {
                //if (res != null) res.DisplayMessage = "Project failed!" + ex.Message;
            }

            return null;
        }

        [HttpGet]
        public async Task<OutPutResponse>? EditFileName(int Id, string? Name)
        {
            OutPutResponse res = new OutPutResponse();
            ProjectPhotosDTO inputs = new ProjectPhotosDTO();
            inputs.IsActive = true;
            inputs.CreationDate = DateTime.Now;
            inputs.UserId = HttpContext.Session.GetInt32("UserId");
            inputs.FolderName = Name;
            inputs.Id = Id;
            int? pId = HttpContext.Session.GetInt32("PID");
            int? cID = HttpContext.Session.GetInt32("CID");
            int? fId = HttpContext.Session.GetInt32("FID");
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiPhotoFileUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("EditPhotoFileName", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var photoFolderDetailsCacheKey = $"PhotoFolderDetails_{HttpContext.Session.GetInt32("PID")}_{cID}";
                        //var photoFileListCacheKey = $"PhotoFileList_{pId}_{cID}_{fId}";
                        _memoryCache.Remove(photoFolderDetailsCacheKey);

                        var data = await response.Content.ReadAsStringAsync();

                        res = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data) ?? new OutPutResponse();


                    }

                }


            }
            catch (SystemException)
            {
                //if (res != null) res.DisplayMessage = "Project failed!" + ex.Message;
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


        //public async Task<IActionResult> MyPhotoPdf()
        //{
        //    PublicUsersDTO objOutPut = new PublicUsersDTO();
        //    return View(objOutPut);
        //}
        [HttpGet]
        public async Task<bool> GeneratePdfFromIds(string ids, string userId)
        {
            // Get the records for the PDF
            var imagesList = await GetRecordsForPdfFile(ids) ?? new List<PublicUsersDTO>();
            var model = new PublicUsersDTO { PdfImagesList = imagesList };

            // Generate the PDF
            var pdfResult = new ViewAsPdf("MyPhotoPdf", model)
            {
                FileName = "Photo-files.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                CustomSwitches = "--disable-smart-shrinking --no-stop-slow-scripts --debug-javascript"
            };

            // Ensure the directory exists
            var pdfFolder = Path.Combine(_environment.WebRootPath, "GeneratedPdfs", userId);
            if (!Directory.Exists(pdfFolder))
            {
                Directory.CreateDirectory(pdfFolder);
            }
            var pdfPath = Path.Combine(pdfFolder, "Photo-files.pdf");

            // Remove the existing file if it exists
            if (System.IO.File.Exists(pdfPath))
            {
                System.IO.File.Delete(pdfPath);
            }

            // Build the PDF file and save it
            var pdfBytes = await pdfResult.BuildFile(ControllerContext);
            await System.IO.File.WriteAllBytesAsync(pdfPath, pdfBytes);

            // Return a download link
           // var downloadUrl = Url.Content("~/GeneratedPdfs/Photo-files.pdf");
            return true;
        }

    }

}
