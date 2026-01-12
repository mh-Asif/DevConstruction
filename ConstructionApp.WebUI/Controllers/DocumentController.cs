using Construction.Infrastructure.Helper;
using Construction.Infrastructure.KeyValues;
using Construction.Infrastructure.Models;
//using ConstructionApp.Core.Entities;
using ConstructionApp.WebUI.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Security.Cryptography;
using static NuGet.Packaging.PackagingConstants;

namespace ConstructionApp.WebUI.Controllers
{
    public class DocumentController : Controller
    {
        private readonly ILogger<DocumentController> _logger;
        private IWebHostEnvironment _environment;
        private readonly IMemoryCache _memoryCache;
        public DocumentController(ILogger<DocumentController> logger, IWebHostEnvironment environment, IMemoryCache memoryCache)
        {
            _logger = logger;
            _environment = environment;
            _memoryCache = memoryCache;
        }

        public async Task<IActionResult> ProjectDirectory()
        {
            var objOutPut = new ProjectsDTO();
            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (string.IsNullOrEmpty(strMenuSession))
                return RedirectToAction("Index", "Home");

            var empSession = JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession);
            objOutPut.AccessList = empSession?.AccessList?.Where(p => p.ModuleCode?.Trim() == "P").ToList() ?? new List<AccessMasterDTO>();
            objOutPut.UserType = HttpContext.Session.GetInt32("UserType");
            var userId = HttpContext.Session.GetInt32("UserId");
               var unitId = HttpContext.Session.GetInt32("UnitId");
            var cacheKey = $"Projects_{unitId}";
            if (!_memoryCache.TryGetValue(cacheKey, out List<ProjectDashboardDTO>? cachedProjectList) || cachedProjectList == null)
            {
                cachedProjectList = await GetProjectDashboard(userId) ?? new List<ProjectDashboardDTO>();
                _memoryCache.Set(cacheKey, cachedProjectList);
            }
            objOutPut.ProjectDashboardList = cachedProjectList;
            return View(objOutPut);
        }

        public async Task<List<ProjectDashboardDTO>> GetProjectDashboard(int? unitId)
        {
            var projectList = new List<ProjectDashboardDTO>();
            using var client = new HttpClient();
            client.BaseAddress = new Uri(EnvironmentUrl.apiProjectUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            var response = await client.GetAsync($"GetProjectDashboard?unitId={unitId}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                projectList = JsonConvert.DeserializeObject<List<ProjectDashboardDTO>>(data) ?? new List<ProjectDashboardDTO>();
                foreach (var item in projectList)
                    item.EncProjectId = CommonHelper.EncryptURLHTML(item.ProjectId.ToString());
            }
            return projectList ?? new List<ProjectDashboardDTO>();
        }

        [HttpGet]
        [Route("Document/ProjectCategoryDirectory/{epId}")]
        public async Task<IActionResult> ProjectCategoryDirectory(string epId)
        {
            ProjectsDTO objOutPut = new ProjectsDTO();

            int pId = Convert.ToInt32(CommonHelper.DecryptURLHTML(epId));
            objOutPut.ProjectId = pId;
            objOutPut.EncProjectId = epId;
            HttpContext.Session.SetInt32("PID", pId);
            HttpContext.Session.SetString("ePID", epId);
            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (strMenuSession == null)
                return RedirectToAction("Index", "Home");

            AccessMasterDTO? empSession = (AccessMasterDTO?)(JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession));
            objOutPut.AccessList = empSession?.AccessList?.ToList() ?? new List<AccessMasterDTO>();
            objOutPut.UserType = HttpContext.Session.GetInt32("UserType");
            var userId = HttpContext.Session.GetInt32("UserId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            var projectCacheKey = $"Projects_{unitId}_{pId}";
            if (!_memoryCache.TryGetValue(projectCacheKey, out List<ProjectDashboardDTO>? cachedProjectList) || cachedProjectList == null)
            {
                cachedProjectList = await GetProjectDashboard(userId) ?? new List<ProjectDashboardDTO>();
                _memoryCache.Set(projectCacheKey, cachedProjectList);
            }
            objOutPut.ProjectDashboardList = cachedProjectList;
            objOutPut.ProjectName = (objOutPut.ProjectDashboardList ?? new List<ProjectDashboardDTO>())
                .Where(p => p.ProjectId == (HttpContext.Session.GetInt32("PID") ?? 0))
                .Select(m => m.ProjectName)
                .FirstOrDefault() ?? string.Empty;
            //var docCategoryCacheKey = $"DocumentCategory_{pId}";
            //if (_memoryCache.TryGetValue(docCategoryCacheKey, out var cachedCategoryListObj) && cachedCategoryListObj is List<DocumentCategoryDTO> cachedCategoryList && cachedCategoryList != null)
            //{
            //    objOutPut.DocumentCategoryList = cachedCategoryList;
            //}
            //else
            //{
                objOutPut.DocumentCategoryList = await GetCategoryList() ?? new List<DocumentCategoryDTO>();
                foreach (var item in objOutPut.DocumentCategoryList)
                {
                    item.EncCategoryId = CommonHelper.EncryptURLHTML(item.Id.ToString());
                }
             //   _memoryCache.Set(docCategoryCacheKey, objOutPut.DocumentCategoryList, TimeSpan.FromMinutes(30));
          //  }
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
                if(cachedDashboard[0].TotalP<=0)
                    return View(objOutPut);
            }

            int pId = 0;
            var ePID = HttpContext.Session.GetString("ePID");
            if (!string.IsNullOrEmpty(ePID))
            {
                pId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ePID));
            }
           // int? unitId = HttpContext.Session.GetInt32("UnitId");
            HttpContext.Session.SetInt32("PID", pId);
            objOutPut.ProjectId = pId;
            // HttpContext.Session.SetString("ePID", epId);
           
           // var userId = HttpContext.Session.GetInt32("UserId");
            var projectCacheKey = $"Projects_{unitId}_{pId}";
            if (!_memoryCache.TryGetValue(projectCacheKey, out List<ProjectDashboardDTO>? cachedProjectList) || cachedProjectList == null)
            {
                cachedProjectList = await GetProjectDashboard(userId) ?? new List<ProjectDashboardDTO>();
                _memoryCache.Set(projectCacheKey, cachedProjectList);
            }
            objOutPut.ProjectDashboardList = cachedProjectList;
            objOutPut.ProjectName = (objOutPut.ProjectDashboardList ?? new List<ProjectDashboardDTO>())
                .Where(p => p.ProjectId == (HttpContext.Session.GetInt32("PID") ?? 0))
                .Select(m => m.ProjectName)
                .FirstOrDefault() ?? string.Empty;
            // Use project-specific category cache key
            //var categoryCacheKey = $"DocumentCategory_{pId}";
            //if (_memoryCache.TryGetValue(categoryCacheKey, out var cachedCategoryListObj) && cachedCategoryListObj is List<DocumentCategoryDTO> cachedCategoryList && cachedCategoryList != null)
            //{
            //    objOutPut.DocumentCategoryList = cachedCategoryList;
            //}
            //else
            //{
                objOutPut.DocumentCategoryList = await GetCategoryList() ?? new List<DocumentCategoryDTO>();
                foreach (var item in objOutPut.DocumentCategoryList)
                {
                    item.EncCategoryId = CommonHelper.EncryptURLHTML(item.Id.ToString());
                }
               // _memoryCache.Set(categoryCacheKey, objOutPut.DocumentCategoryList, TimeSpan.FromMinutes(30));
           // }
            return View(objOutPut);
        }
        public async Task<List<DocumentCategoryDTO>> GetCategoryList()
        {
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            List<DocumentCategoryDTO> PriorityLst = new List<DocumentCategoryDTO>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("GetDocumentCategory");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    PriorityLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DocumentCategoryDTO>>(data) ?? new List<DocumentCategoryDTO>();
                }
            }
            return PriorityLst ?? new List<DocumentCategoryDTO>();
        }

        //[HttpGet]
        //public async Task<List<DocumentCategoryDTO>> GetCategoryList(int? id)
        //{
        //    int? unitId = HttpContext.Session.GetInt32("UnitId");
        //    List<DocumentCategoryDTO> PriorityLst = new List<DocumentCategoryDTO>();
        //    using (HttpClient client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        //        HttpResponseMessage response = await client.GetAsync("GetDocumentCategory?id="+ id);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            var data = await response.Content.ReadAsStringAsync();
        //            PriorityLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DocumentCategoryDTO>>(data) ?? new List<DocumentCategoryDTO>();
        //        }
        //    }
        //    return PriorityLst ?? new List<DocumentCategoryDTO>();
        //}

        [HttpGet]
        [Route("Document/ProjectDocuments/{eCategoryId}")]
        public async Task<IActionResult> ProjectDocuments(string eCategoryId)
        {
            ProjectsDTO objOutPut = new ProjectsDTO();
            int CategoryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eCategoryId));
           
            int? pId = HttpContext.Session.GetInt32("PID");
            HttpContext.Session.SetString("eCID", eCategoryId);
            HttpContext.Session.SetInt32("CID", CategoryId);
            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (strMenuSession == null)
                return RedirectToAction("Index", "Home");
            AccessMasterDTO? empSession = JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession);
            var userId = HttpContext.Session.GetInt32("UserId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            var projectCacheKey = $"Projects_{unitId}_{pId}";
            if (!_memoryCache.TryGetValue(projectCacheKey, out List<ProjectDashboardDTO>? cachedProjectList) || cachedProjectList == null)
            {
                cachedProjectList = await GetProjectDashboard(userId) ?? new List<ProjectDashboardDTO>();
                _memoryCache.Set(projectCacheKey, cachedProjectList);
            }
            objOutPut.ProjectDashboardList = cachedProjectList;
            objOutPut.ProjectName = objOutPut.ProjectDashboardList?.Where(p => p.ProjectId == pId).Select(m => m.ProjectName).FirstOrDefault() ?? string.Empty;
            objOutPut.EncProjectId = HttpContext.Session.GetString("ePID");
            objOutPut.AccessList = empSession?.AccessList?.ToList() ?? new List<AccessMasterDTO>();
            objOutPut.UserType = HttpContext.Session.GetInt32("UserType");
            var docCategoryCacheKey = $"DocumentCategory_{pId}";
            if (_memoryCache.TryGetValue(docCategoryCacheKey, out var cachedCategoryListObj) && cachedCategoryListObj is List<DocumentCategoryDTO> cachedCategoryList && cachedCategoryList != null)
            {
                objOutPut.DocumentCategoryList = cachedCategoryList;
            }
            else  
            {
                objOutPut.DocumentCategoryList = await GetCategoryList() ?? new List<DocumentCategoryDTO>();
                _memoryCache.Set(docCategoryCacheKey, objOutPut.DocumentCategoryList);
            }
            objOutPut.CategoryName = objOutPut.DocumentCategoryList?.Where(p => p.Id == CategoryId).Select(m => m.Category).FirstOrDefault() ?? string.Empty;
            var inviteDetailsCacheKey = $"InviteDetails_{pId}_{CategoryId}";
            if (_memoryCache.TryGetValue(inviteDetailsCacheKey, out var cachedProjectFolderListObj) && cachedProjectFolderListObj is List<ProjectFolderDTO> cachedProjectFolderList && cachedProjectFolderList != null)
            {
                objOutPut.ProjectFolderList = cachedProjectFolderList;
            }
            else
            {
                objOutPut.ProjectFolderList = await GetProjectFolder(pId, CategoryId) ?? new List<ProjectFolderDTO>();
                _memoryCache.Set(inviteDetailsCacheKey, objOutPut.ProjectFolderList);
            }
            var fileDetailsCacheKey = $"FileDetails_{pId}_{CategoryId}";
            if (_memoryCache.TryGetValue(fileDetailsCacheKey, out var cachedProjectFileListObj) && cachedProjectFileListObj is List<ProjectFolderDTO> cachedProjectFileList && cachedProjectFileList != null)
            {
                objOutPut.ProjectFileList = cachedProjectFileList;
            }
            else
            {
                objOutPut.ProjectFileList = await GetDocumentFiles(pId, CategoryId) ?? new List<ProjectFolderDTO>();
                _memoryCache.Set(fileDetailsCacheKey, objOutPut.ProjectFileList);
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

            //var roleTask = _memoryCache.Get(roleKey) as List<ProjectRoleSummeryDTO> ?? null;
            //var roleFetch = roleTask != null ? Task.FromResult(roleTask) : GetProjectRoleSummery(unitId, pId) ?? Task.FromResult(new List<ProjectRoleSummeryDTO>());

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
            objOutPut.ProjectId = pId ?? 0;
            objOutPut.EncProjectId = CommonHelper.EncryptURLHTML(objOutPut.ProjectId.ToString());
            return View(objOutPut);
        }

        public async Task<IActionResult> ProjectDocumentsById(int CategoryId)
        {
            ProjectsDTO objOutPut = new ProjectsDTO();
           string eCategoryId = CommonHelper.EncryptURLHTML(Convert.ToString(CategoryId));
            int? pId = HttpContext.Session.GetInt32("PID");
            HttpContext.Session.SetString("eCID", eCategoryId);
            HttpContext.Session.SetInt32("CID", CategoryId);
            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (strMenuSession == null)
                return RedirectToAction("Index", "Home");
            AccessMasterDTO? empSession = JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession);
            var userId = HttpContext.Session.GetInt32("UserId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            var projectCacheKey = $"Projects_{unitId}_{pId}";
            if (!_memoryCache.TryGetValue(projectCacheKey, out List<ProjectDashboardDTO>? cachedProjectList) || cachedProjectList == null)
            {
                cachedProjectList = await GetProjectDashboard(userId) ?? new List<ProjectDashboardDTO>();
                _memoryCache.Set(projectCacheKey, cachedProjectList);
            }
            objOutPut.ProjectDashboardList = cachedProjectList;
            objOutPut.ProjectName = objOutPut.ProjectDashboardList?.Where(p => p.ProjectId == pId).Select(m => m.ProjectName).FirstOrDefault() ?? string.Empty;
            objOutPut.EncProjectId = HttpContext.Session.GetString("ePID");
            objOutPut.AccessList = empSession?.AccessList?.ToList() ?? new List<AccessMasterDTO>();
            objOutPut.UserType = HttpContext.Session.GetInt32("UserType");
            var docCategoryCacheKey = $"DocumentCategory_{pId}";
            if (_memoryCache.TryGetValue(docCategoryCacheKey, out var cachedCategoryListObj) && cachedCategoryListObj is List<DocumentCategoryDTO> cachedCategoryList && cachedCategoryList != null)
            {
                objOutPut.DocumentCategoryList = cachedCategoryList;
            }
            else
            {
                objOutPut.DocumentCategoryList = await GetCategoryList() ?? new List<DocumentCategoryDTO>();
                _memoryCache.Set(docCategoryCacheKey, objOutPut.DocumentCategoryList);
            }
            objOutPut.CategoryName = objOutPut.DocumentCategoryList?.Where(p => p.Id == CategoryId).Select(m => m.Category).FirstOrDefault() ?? string.Empty;
            var inviteDetailsCacheKey = $"InviteDetails_{pId}_{CategoryId}";
            if (_memoryCache.TryGetValue(inviteDetailsCacheKey, out var cachedProjectFolderListObj) && cachedProjectFolderListObj is List<ProjectFolderDTO> cachedProjectFolderList && cachedProjectFolderList != null)
            {
                objOutPut.ProjectFolderList = cachedProjectFolderList;
            }
            else
            {
                objOutPut.ProjectFolderList = await GetProjectFolder(pId, CategoryId) ?? new List<ProjectFolderDTO>();
                _memoryCache.Set(inviteDetailsCacheKey, objOutPut.ProjectFolderList);
            }
            var fileDetailsCacheKey = $"FileDetails_{pId}_{CategoryId}";
            if (_memoryCache.TryGetValue(fileDetailsCacheKey, out var cachedProjectFileListObj) && cachedProjectFileListObj is List<ProjectFolderDTO> cachedProjectFileList && cachedProjectFileList != null)
            {
                objOutPut.ProjectFileList = cachedProjectFileList;
            }
            else
            {
                objOutPut.ProjectFileList = await GetDocumentFiles(pId, CategoryId) ?? new List<ProjectFolderDTO>();
                _memoryCache.Set(fileDetailsCacheKey, objOutPut.ProjectFileList);
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

            //var roleTask = _memoryCache.Get(roleKey) as List<ProjectRoleSummeryDTO> ?? null;
            //var roleFetch = roleTask != null ? Task.FromResult(roleTask) : GetProjectRoleSummery(unitId, pId) ?? Task.FromResult(new List<ProjectRoleSummeryDTO>());

            if (_memoryCache.Get("UserList") == null)
            {
                objOutPut.UserList = await GetUsers(1);
                _memoryCache.Set("UserList", objOutPut.UserList);
            }
            else
            {
                objOutPut.UserList = _memoryCache.Get<List<UserKeyValues>>("UserList");
            }
            objOutPut.CategoryId = CategoryId;
            return View("ProjectDocuments",objOutPut);
        }
        public async Task<IActionResult> ProjectDocuments()
        {
            int CategoryId = 0;
            ProjectsDTO objOutPut = new ProjectsDTO();
            var eCID = HttpContext.Session.GetString("eCID");
            if (!string.IsNullOrEmpty(eCID))
            {
                CategoryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eCID));
            }

            // HttpContext.Session.SetString("eCID", eCategoryId);
            HttpContext.Session.SetInt32("CID", CategoryId);
            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (strMenuSession == null)
                return RedirectToAction("Index", "Home");



            AccessMasterDTO? empSession = (AccessMasterDTO?)(JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession));
            var userId = HttpContext.Session.GetInt32("UserId");
            var pId = HttpContext.Session.GetInt32("PID");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            var projectCacheKey = $"Projects_{unitId}_{pId}";
            if (!_memoryCache.TryGetValue(projectCacheKey, out List<ProjectDashboardDTO>? cachedProjectList) || cachedProjectList == null)
            {
                cachedProjectList = await GetProjectDashboard(userId) ?? new List<ProjectDashboardDTO>();
                _memoryCache.Set(projectCacheKey, cachedProjectList);
            }
            objOutPut.ProjectDashboardList = cachedProjectList;
            objOutPut.ProjectName = objOutPut.ProjectDashboardList?.Where(p => p.ProjectId == pId).Select(m => m.ProjectName).FirstOrDefault() ?? string.Empty;
            objOutPut.EncProjectId = HttpContext.Session.GetString("ePID");
            objOutPut.AccessList = empSession?.AccessList?.ToList() ?? new List<AccessMasterDTO>();
            objOutPut.UserType = HttpContext.Session.GetInt32("UserType");
            // Use project+category-specific cache key for DocumentCategory
            var categoryCacheKey = $"DocumentCategory_{pId}";
            if (_memoryCache.TryGetValue(categoryCacheKey, out var cachedCategoryListObj) && cachedCategoryListObj is List<DocumentCategoryDTO> cachedCategoryList && cachedCategoryList != null)
            {
                objOutPut.DocumentCategoryList = cachedCategoryList;
            }
            else
            {
                objOutPut.DocumentCategoryList = await GetCategoryList() ?? new List<DocumentCategoryDTO>();
                foreach (var item in objOutPut.DocumentCategoryList)
                {
                    item.EncCategoryId = CommonHelper.EncryptURLHTML(item.Id.ToString());
                }
                _memoryCache.Set(categoryCacheKey, objOutPut.DocumentCategoryList, TimeSpan.FromMinutes(30));
            }
            objOutPut.CategoryName = objOutPut.DocumentCategoryList?.Where(p => p.Id == CategoryId).Select(m => m.Category).FirstOrDefault() ?? string.Empty;
            // Use project+category-specific cache keys for folders/files
            var inviteDetailsCacheKey = $"InviteDetails_{pId}_{CategoryId}";
            if (_memoryCache.TryGetValue(inviteDetailsCacheKey, out var cachedProjectFolderListObj) && cachedProjectFolderListObj is List<ProjectFolderDTO> cachedProjectFolderList && cachedProjectFolderList != null)
            {
                objOutPut.ProjectFolderList = cachedProjectFolderList;
            }
            else
            {
                objOutPut.ProjectFolderList = await GetProjectFolder(pId, CategoryId);
                _memoryCache.Set(inviteDetailsCacheKey, objOutPut.ProjectFolderList);
            }
            var fileDetailsCacheKey = $"FileDetails_{pId}_{CategoryId}";
            if (_memoryCache.TryGetValue(fileDetailsCacheKey, out var cachedProjectFileListObj) && cachedProjectFileListObj is List<ProjectFolderDTO> cachedProjectFileList && cachedProjectFileList != null)
            {
                objOutPut.ProjectFileList = cachedProjectFileList;
            }
            else
            {
                objOutPut.ProjectFileList = await GetDocumentFiles(pId, CategoryId);
                _memoryCache.Set(fileDetailsCacheKey, objOutPut.ProjectFileList);
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

            //objOutPut.DocumentCategoryList = await GetCategoryList();
            //objOutPut.UserList = await GetUsers();
            //_memoryCache.Set("Category", objOutPut.DocumentCategoryList);
            //objOutPut.ProjectFolderList = await GetProjectFolder(HttpContext.Session.GetInt32("PID"), CategoryId);
            //objOutPut.ProjectFileList = await GetDocumentFiles(HttpContext.Session.GetInt32("PID"), CategoryId);
            //_memoryCache.Set("InviteDetails", objOutPut.ProjectFolderList);
            objOutPut.CategoryId = CategoryId;
            objOutPut.ProjectId = pId ?? 0;
            return View(objOutPut);
        }
        public async Task<OutPutResponse> CreateDocumentFolder(string folderName, int cId)
        {
            OutPutResponse objRespnose = new OutPutResponse();
            ProjectFolderDTO inputs = new ProjectFolderDTO();
            int? pId = HttpContext.Session.GetInt32("PID");
            // int? cId = HttpContext.Session.GetInt32("CID");
            inputs.ProjectId = pId;
            inputs.CategoryId = cId;
            inputs.IsFolder = true;
            inputs.UserId = HttpContext.Session.GetInt32("UserId");
            inputs.FolderName = folderName;
            string path = "", nPath = "", response = "0";
            try
            {
                //string path = Server.MapPath("~/YourFolder/" + folderName);
                string folderpath = Path.Combine(this._environment.WebRootPath, "Documents");
                path = Path.Combine(folderpath ?? string.Empty, Convert.ToString(pId) ?? string.Empty, Convert.ToString(cId) ?? string.Empty);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);

                    nPath = Path.Combine(path, folderName);
                    if (!Directory.Exists(nPath))
                    {
                        Directory.CreateDirectory(nPath);
                    }
                }
                else
                {
                    nPath = Path.Combine(path, folderName);
                    if (!Directory.Exists(nPath))
                    {
                        Directory.CreateDirectory(nPath);
                    }
                }
                objRespnose = await SaveProjectFolder(inputs);

                // RedirectToAction("ProjectDocuments", "Document", new { eCategoryId = HttpContext.Session.GetString("eCID") });
            }
            catch (Exception)
            {
                response = "0";
            }
            //  RedirectToAction("ProjectDocuments", "Document", new { eCategoryId = HttpContext.Session.GetString("eCID") });
            return objRespnose;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFiles([FromForm] string cId, [FromForm] string fileName)
        {
            await Task.CompletedTask;
            // var files = Request.Form.Files;
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(209715200)]
        public async Task<IActionResult> CreateDocumentFiles([FromForm] string cId, [FromForm] string fileName)
        {
            ProjectsDTO objOutPut = new ProjectsDTO();
            var files = Request.Form.Files;
            ProjectFolderDTO inputs = new ProjectFolderDTO();
            int? pId = HttpContext.Session.GetInt32("PID");
            // int? cId = HttpContext.Session.GetInt32("CID");
            inputs.ProjectId = pId;
            inputs.CategoryId = Convert.ToInt32(cId);
            inputs.IsFolder = false;
            inputs.UserId = HttpContext.Session.GetInt32("UserId");
            //  inputs.FolderName = folderName;
            string path = "", nPath = "", response = "0", fName = "";
            try
            {
                //string path = Server.MapPath("~/YourFolder/" + folderName);
                string folderpath = Path.Combine(this._environment.WebRootPath, "Documents");
                path = Path.Combine(folderpath ?? string.Empty, Convert.ToString(pId) ?? string.Empty, cId);


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
                        fName = fName.Replace("'", "").Replace("\"", "_");
                        inputs.FilePath = fName;
                        inputs.FolderName = fileName;
                        using (FileStream stream = new FileStream(Path.Combine(nPath, fName), FileMode.Create))
                        {
                            postedFile.CopyTo(stream);

                        }

                        await SaveProjectFile(inputs);

                    }

                }
                //else
                //{
                //    inputs.FilePath = "";
                //    inputs.FolderName = "";
                //}


                //string? strMenuSession = HttpContext.Session.GetString("Menu");
                //AccessMasterDTO? empSession = (AccessMasterDTO?)(JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession));
                //var projectList = _memoryCache.Get("Projects") as List<ProjectDashboardDTO>;
                //objOutPut.ProjectName = projectList.Where(p => p.ProjectId == HttpContext.Session.GetInt32("PID")).Select(m => m.ProjectName).FirstOrDefault();

                //objOutPut.EncProjectId = HttpContext.Session.GetString("ePID");
                //objOutPut.AccessList = empSession.AccessList.ToList();
                //objOutPut.UserType = HttpContext.Session.GetInt32("UserType");
                //objOutPut.DocumentCategoryList = await GetCategoryList();
                //_memoryCache.Set("Category", objOutPut.DocumentCategoryList);
                //objOutPut.ProjectFolderList = await GetProjectFolder(HttpContext.Session.GetInt32("PID"), HttpContext.Session.GetInt32("CID"));
                //objOutPut.ProjectFileList = await GetDocumentFiles(HttpContext.Session.GetInt32("PID"), HttpContext.Session.GetInt32("CID"));
                ////  GetProjectFolder
                //objOutPut.CategoryId = HttpContext.Session.GetInt32("CID");

                // RedirectToAction("ProjectDocuments", "Document", new { eCategoryId = HttpContext.Session.GetString("eCID") });
            }
            catch (Exception)
            {
                //  return Ok(new { count = files.Count });
            }
            // RedirectToAction("ProjectDocuments","Document", new { eCategoryId = HttpContext.Session.GetString("eCID") });
            //  return View("ProjectDocuments", objOutPut);
            return Ok(new { count = files.Count });
        }


        public class FileDeleteModel
        {
            public string FileName { get; set; }
        }

        public async Task<OutPutResponse>? SaveProjectFolder(ProjectFolderDTO inputs)
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

                    HttpResponseMessage response = await client.PostAsJsonAsync("SaveProjectFile", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        // Remove the correct composite cache key for folder list
                        string inviteDetailsCacheKey = $"InviteDetails_{inputs.ProjectId}_{inputs.CategoryId}";
                        _memoryCache.Remove(inviteDetailsCacheKey);
                        var data = await response.Content.ReadAsStringAsync();
                        res = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data) ?? new OutPutResponse();
                    }
                }
            }
            catch (SystemException ex)
            {
                res.DisplayMessage = "Project failed!" + ex.Message;
            }
            return res;
        }


        public async Task<List<ProjectFolderDTO>> GetProjectFolder(int? pId, int? cId)
        {
            // DataTable dt = new DataTable();
            //  int? unitId = HttpContext.Session.GetInt32("UnitId");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            int? UType = HttpContext.Session.GetInt32("UserType");
            List<ProjectFolderDTO> PriorityLst = new List<ProjectFolderDTO>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiProjectFileUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetProjectCategoryFolders?pId=" + pId + "&cId=" + cId + "&uId=" + UserId + "&uType=" + UType);
                if (response.IsSuccessStatusCode)
                {
                    //string inviteDetailsCacheKey = $"InviteDetails_{pId}_{cId}";
                    //_memoryCache.Remove(inviteDetailsCacheKey);
                    // _memoryCache.Remove("InviteDetails");
                    var data = await response.Content.ReadAsStringAsync();
                    PriorityLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProjectFolderDTO>>(data) ?? new List<ProjectFolderDTO>();

                    foreach (var item in PriorityLst)
                    {
                        if(item.UserId== UserId)
                        {
                            item.UserId = 0;
                        }
                        item.EnFolderId = CommonHelper.EncryptURLHTML(item.Id.ToString());
                       
                    }
                }


            }
            return PriorityLst;
        }


        public async Task<OutPutResponse>? SaveProjectFile(ProjectFolderDTO inputs)
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

                    HttpResponseMessage response = await client.PostAsJsonAsync("SaveDocumentFile", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var pId = HttpContext.Session.GetInt32("PID");
                        var CategoryId = inputs.CategoryId;

                        // Remove the correct composite cache key for file details
                        string fileDetailsCacheKey = $"FileDetails_{pId}_{CategoryId}";
                        _memoryCache.Remove(fileDetailsCacheKey);
                        var data = await response.Content.ReadAsStringAsync();
                        res = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data) ?? new OutPutResponse();

                    }

                }


            }
            catch (SystemException ex)
            {
                res.DisplayMessage = "Project failed!" + ex.Message;
            }
            return res;
        }


        public async Task<List<ProjectFolderDTO>> GetDocumentFiles(int? pId, int? cId)
        {
            // DataTable dt = new DataTable();
            //  int? unitId = HttpContext.Session.GetInt32("UnitId");
           int? UserId = HttpContext.Session.GetInt32("UserId");
            int? UType = HttpContext.Session.GetInt32("UserType");
            List<ProjectFolderDTO> PriorityLst = new List<ProjectFolderDTO>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiProjectFileUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetProjectCategoryFiles?pId=" + pId + "&cId=" + cId + "&uType=" + UType);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    PriorityLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProjectFolderDTO>>(data) ?? new List<ProjectFolderDTO>();

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
        [Route("Document/FolderFiles/{eFolderId}")]
        public async Task<IActionResult> FolderFiles(string? eFolderId)
        {
            ProjectsDTO objOutPut = new ProjectsDTO();
            
            int FolderId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eFolderId));
           
            int? pId = HttpContext.Session.GetInt32("PID");
            int? cId = HttpContext.Session.GetInt32("CID");
            HttpContext.Session.SetInt32("FID", FolderId);
            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (strMenuSession == null)
                return RedirectToAction("Index", "Home");

            AccessMasterDTO? empSession = (AccessMasterDTO?)(JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession));

            objOutPut.EncProjectId = CommonHelper.EncryptURLHTML(pId.ToString());
            objOutPut.AccessList = empSession.AccessList.ToList();
            objOutPut.UserType = HttpContext.Session.GetInt32("UserType");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            if (_memoryCache.Get("UserList") == null)
            {
                objOutPut.UserList = await GetUsers(1);
            }
            else
            {
                objOutPut.UserList = _memoryCache.Get<List<UserKeyValues>>("UserList");
            }
            var userId = HttpContext.Session.GetInt32("UserId");
            var projectCacheKey = $"Projects_{unitId}_{pId}";
            if (!_memoryCache.TryGetValue(projectCacheKey, out List<ProjectDashboardDTO>? cachedProjectList) || cachedProjectList == null)
            {
                cachedProjectList = await GetProjectDashboard(userId) ?? new List<ProjectDashboardDTO>();
                _memoryCache.Set(projectCacheKey, cachedProjectList);
            }
            objOutPut.ProjectDashboardList = cachedProjectList;
            objOutPut.ProjectName = objOutPut.ProjectDashboardList?.Where(p => p.ProjectId == pId).Select(m => m.ProjectName).FirstOrDefault() ?? string.Empty;
            var inviteDetailsCacheKey = $"InviteDetails_{pId}_{cId}";
            if (_memoryCache.TryGetValue(inviteDetailsCacheKey, out var cachedProjectFolderListObj) && cachedProjectFolderListObj is List<ProjectFolderDTO> cachedProjectFolderList && cachedProjectFolderList != null)
            {
                objOutPut.ProjectFolderList = cachedProjectFolderList;
            }
            else
            {
                objOutPut.ProjectFolderList = await GetProjectFolder(pId, cId) ?? new List<ProjectFolderDTO>();
                _memoryCache.Set(inviteDetailsCacheKey, objOutPut.ProjectFolderList);
            }
            var folderFileDetailsCacheKey = $"FolderFileDetails_{pId}_{cId}_{FolderId}";
            if (_memoryCache.TryGetValue(folderFileDetailsCacheKey, out List<ProjectFolderFilesDTO>? cachedFolderFileList) && cachedFolderFileList != null)
            {
                objOutPut.FolderFileList = cachedFolderFileList;
            }
            else
            {
                objOutPut.FolderFileList = await GetFolderFiles(pId, cId, FolderId) ?? new List<ProjectFolderFilesDTO>();
                _memoryCache.Set(folderFileDetailsCacheKey, objOutPut.FolderFileList);
            }
            objOutPut.FileDetails = objOutPut.ProjectFolderList?.Where(p => p.Id == FolderId).Select(m => m.FolderName).FirstOrDefault() ?? string.Empty;
            var docCategoryCacheKey = $"DocumentCategory_{pId}";
            if (_memoryCache.TryGetValue(docCategoryCacheKey, out var cachedCategoryListObj) && cachedCategoryListObj is List<DocumentCategoryDTO> cachedCategoryList && cachedCategoryList != null)
            {
                objOutPut.DocumentCategoryList = cachedCategoryList;
            }
            else
            {
                objOutPut.DocumentCategoryList = await GetCategoryList() ?? new List<DocumentCategoryDTO>();
                _memoryCache.Set(docCategoryCacheKey, objOutPut.DocumentCategoryList);
            }
            objOutPut.CategoryName = objOutPut.DocumentCategoryList?.Where(p => p.Id == cId).Select(m => m.Category).FirstOrDefault() ?? string.Empty;

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
            objOutPut.ProjectId = pId ?? 0;
            return View(objOutPut);
        }


        public async Task<IActionResult> FolderFilesById(int FolderId)
        {
            ProjectsDTO objOutPut = new ProjectsDTO();

            string eFolderId = CommonHelper.EncryptURLHTML(Convert.ToString(FolderId));
            int? pId = HttpContext.Session.GetInt32("PID");
            int? cId = HttpContext.Session.GetInt32("CID");
            HttpContext.Session.SetInt32("FID", FolderId);
            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (strMenuSession == null)
                return RedirectToAction("Index", "Home");

            AccessMasterDTO? empSession = (AccessMasterDTO?)(JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession));

            objOutPut.AccessList = empSession.AccessList.ToList();
            objOutPut.UserType = HttpContext.Session.GetInt32("UserType");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            if (_memoryCache.Get("UserList") == null)
            {
                objOutPut.UserList = await GetUsers(1);
            }
            else
            {
                objOutPut.UserList = _memoryCache.Get<List<UserKeyValues>>("UserList");
            }
            var userId = HttpContext.Session.GetInt32("UserId");
            var projectCacheKey = $"Projects_{unitId}_{pId}";
            if (!_memoryCache.TryGetValue(projectCacheKey, out List<ProjectDashboardDTO>? cachedProjectList) || cachedProjectList == null)
            {
                cachedProjectList = await GetProjectDashboard(userId) ?? new List<ProjectDashboardDTO>();
                _memoryCache.Set(projectCacheKey, cachedProjectList);
            }
            objOutPut.ProjectDashboardList = cachedProjectList;
            objOutPut.ProjectName = objOutPut.ProjectDashboardList?.Where(p => p.ProjectId == pId).Select(m => m.ProjectName).FirstOrDefault() ?? string.Empty;
            var inviteDetailsCacheKey = $"InviteDetails_{pId}_{cId}";
            if (_memoryCache.TryGetValue(inviteDetailsCacheKey, out var cachedProjectFolderListObj) && cachedProjectFolderListObj is List<ProjectFolderDTO> cachedProjectFolderList && cachedProjectFolderList != null)
            {
                objOutPut.ProjectFolderList = cachedProjectFolderList;
            }
            else
            {
                objOutPut.ProjectFolderList = await GetProjectFolder(pId, cId) ?? new List<ProjectFolderDTO>();
                _memoryCache.Set(inviteDetailsCacheKey, objOutPut.ProjectFolderList);
            }
            var folderFileDetailsCacheKey = $"FolderFileDetails_{pId}_{cId}_{FolderId}";
            if (_memoryCache.TryGetValue(folderFileDetailsCacheKey, out List<ProjectFolderFilesDTO>? cachedFolderFileList) && cachedFolderFileList != null)
            {
                objOutPut.FolderFileList = cachedFolderFileList;
            }
            else
            {
                objOutPut.FolderFileList = await GetFolderFiles(pId, cId, FolderId) ?? new List<ProjectFolderFilesDTO>();
                _memoryCache.Set(folderFileDetailsCacheKey, objOutPut.FolderFileList);
            }
            objOutPut.FileDetails = objOutPut.ProjectFolderList?.Where(p => p.Id == FolderId).Select(m => m.FolderName).FirstOrDefault() ?? string.Empty;
            var docCategoryCacheKey = $"DocumentCategory_{pId}";
            if (_memoryCache.TryGetValue(docCategoryCacheKey, out var cachedCategoryListObj) && cachedCategoryListObj is List<DocumentCategoryDTO> cachedCategoryList && cachedCategoryList != null)
            {
                objOutPut.DocumentCategoryList = cachedCategoryList;
            }
            else
            {
                objOutPut.DocumentCategoryList = await GetCategoryList() ?? new List<DocumentCategoryDTO>();
                _memoryCache.Set(docCategoryCacheKey, objOutPut.DocumentCategoryList);
            }
            objOutPut.CategoryName = objOutPut.DocumentCategoryList?.Where(p => p.Id == cId).Select(m => m.Category).FirstOrDefault() ?? string.Empty;

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
        [RequestSizeLimit(209715200)] // 200 MB limit
        public async Task<IActionResult> CreateFolderFile([FromForm] string Fid, [FromForm] string fileName)
        {

            // int cId = 0; string fileName = "";
            var files = Request.Form.Files;
            ProjectFolderFilesDTO inputs = new ProjectFolderFilesDTO();
            int? pId = HttpContext.Session.GetInt32("PID");
            int? cId = HttpContext.Session.GetInt32("CID");
            inputs.ProjectId = pId;
            inputs.CategoryId = cId;
            inputs.FolderId = Convert.ToInt32(Fid);
            inputs.UserId = HttpContext.Session.GetInt32("UserId");
            //  inputs.FolderName = folderName;
            string path = "", nPath = "", response = "0", fName = "";
            try
            {
                //string path = Server.MapPath("~/YourFolder/" + folderName);
                string folderpath = Path.Combine(this._environment.WebRootPath, "Documents");
                path = Path.Combine(folderpath ?? string.Empty, Convert.ToString(pId) ?? string.Empty, Convert.ToString(cId) ?? string.Empty, Convert.ToString(Fid));


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

                        await SaveFolderFile(inputs);
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

        public async Task<OutPutResponse>? SaveFolderFile(ProjectFolderFilesDTO inputs)
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

                    HttpResponseMessage response = await client.PostAsJsonAsync("SaveFolderFile", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var folderFileDetailsCacheKey = $"FolderFileDetails_{inputs.ProjectId}_{inputs.CategoryId}_{inputs.FolderId}";
                        _memoryCache.Remove(folderFileDetailsCacheKey);
                        var data = await response.Content.ReadAsStringAsync();
                        res = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data) ?? new OutPutResponse();

                    }

                }


            }
            catch (SystemException ex)
            {
                res.DisplayMessage = "Project failed!" + ex.Message;
            }
            return res;
        }

        public async Task<List<ProjectFolderFilesDTO>> GetFolderFiles(int? pId, int? cId, int? fId)
        {
            // DataTable dt = new DataTable();
            int? UType = HttpContext.Session.GetInt32("UserType");
            List<ProjectFolderFilesDTO> PriorityLst = new List<ProjectFolderFilesDTO>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiProjectFileUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetProjectFolderFiles?pId=" + pId + "&cId=" + cId + "&fId=" + fId);
                if (response.IsSuccessStatusCode)
                {
                    string fileDetailsCacheKey = $"FileDetails_{pId}_{cId}";
                    _memoryCache.Remove(fileDetailsCacheKey);

                    var data = await response.Content.ReadAsStringAsync();
                    PriorityLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProjectFolderFilesDTO>>(data) ?? new List<ProjectFolderFilesDTO>();

                    foreach (var item in PriorityLst)
                    {
                        item.UserType = UType;
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

                HttpResponseMessage response = await client.GetAsync("DeleteFolderFile?pId=" + FileId);
                if (response.IsSuccessStatusCode)
                {
                    var folderFileDetailsCacheKey = $"FolderFileDetails_{pId}_{cID}_{fId}";
                    //string fileDetailsCacheKey = $"FileDetails_{pId}_{cID}";

                    _memoryCache.Remove(folderFileDetailsCacheKey);
                  //  _memoryCache.Remove(fileDetailsCacheKey);

                    // _memoryCache.Remove("FolderFileDetails");
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

                HttpResponseMessage response = await client.GetAsync("DeleteFolder?pId=" + FileId);
                if (response.IsSuccessStatusCode)
                {
                    string inviteDetailsCacheKey = $"InviteDetails_{pId}_{cID}";
                    string fileDetailsCacheKey = $"FileDetails_{pId}_{cID}";

                    _memoryCache.Remove(inviteDetailsCacheKey);
                    _memoryCache.Remove(fileDetailsCacheKey);

                    // _memoryCache.Remove("InviteDetails");
                    var data = await response.Content.ReadAsStringAsync();
                    res = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data) ?? new OutPutResponse();

                }
            }

            return res;
        }


        public IActionResult Download(string fileName)
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

                HttpResponseMessage response = await client.GetAsync("GetUserList?unitId=" + unitId + "&userType="+ userType);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    PriorityLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserKeyValues>>(data);

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
        public async Task<OutPutResponse>? InviteSent(string? accessId, int? AccessType, int Id, string? OptMessage,string? UserType)
        {
            OutPutResponse res = new OutPutResponse();
            ProjectFolderDTO inputs = new ProjectFolderDTO();
            int? pId = HttpContext.Session.GetInt32("PID");
            int? cID = HttpContext.Session.GetInt32("CID");

            inputs.IsActive = true;
            inputs.CreationDate = DateTime.Now;
            inputs.UserId = HttpContext.Session.GetInt32("UserId");
           
            inputs.AccessType = AccessType;
            inputs.OptMessage = OptMessage;
            inputs.Id = Id;
            if(UserType=="C")
            {
                inputs.ClientIds = accessId;
                inputs.AccessIds =null;
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

                    HttpResponseMessage response = await client.PostAsJsonAsync("EditDocumentFolder", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        string inviteDetailsCacheKey = $"InviteDetails_{pId}_{cID}";
                        _memoryCache.Remove(inviteDetailsCacheKey);
                        var data = await response.Content.ReadAsStringAsync();

                        res = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data) ?? new OutPutResponse();


                    }

                }


            }
            catch (SystemException ex)
            {
                res.DisplayMessage = "Project failed!" + ex.Message;
            }
            return res;
        }

        [HttpGet]
        public async Task<OutPutResponse>? LinkShareForDownload(string? accessId, int? AccessType, int Id, string? OptMessage, string? emailIds)
        {
            OutPutResponse res = new OutPutResponse();
            ProjectFolderDTO inputs = new ProjectFolderDTO();
            inputs.IsActive = true;
            inputs.CreationDate = DateTime.Now;
            inputs.AccessIds = accessId;
            inputs.AccessType = AccessType;
            inputs.OptMessage = OptMessage;
            inputs.UserId = HttpContext.Session.GetInt32("UserId");
            inputs.EmailIds = emailIds;           
            inputs.Id = Id;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiProjectFileUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("EditDocumentFolder", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        int? pId = HttpContext.Session.GetInt32("PID");
                        int? cID = HttpContext.Session.GetInt32("CID");
                        string inviteDetailsCacheKey = $"InviteDetails_{pId}_{cID}";
                        string fileDetailsCacheKey = $"FileDetails_{pId}_{cID}";
                        _memoryCache.Remove(inviteDetailsCacheKey);
                        _memoryCache.Remove(fileDetailsCacheKey);

                        var data = await response.Content.ReadAsStringAsync();
                        res = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data) ?? new OutPutResponse();


                    }

                }


            }
            catch (SystemException ex)
            {
                res.DisplayMessage = "Project failed!" + ex.Message;
            }
            return res;
        }

        [HttpGet]
        public async Task<OutPutResponse>? DocFileShareForDownload(string? accessId, int? AccessType, int Id, string? OptMessage, string? emailIds)
        {
            OutPutResponse res = new OutPutResponse();
            ProjectFolderFilesDTO inputs = new ProjectFolderFilesDTO();
            inputs.IsActive = true;
            inputs.CreationDate = DateTime.Now;
            inputs.AccessIds = accessId;
            inputs.AccessType = AccessType;
            inputs.OptMessage = OptMessage;
            inputs.UserId = HttpContext.Session.GetInt32("UserId");
          
            inputs.Id = Id;
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
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiProjectFileUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("EditDocumentFile", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var folderFileDetailsCacheKey = $"FolderFileDetails_{pId}_{cID}_{fId}";
                        _memoryCache.Remove(folderFileDetailsCacheKey);
                        var data = await response.Content.ReadAsStringAsync();

                        res = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data) ?? new OutPutResponse();


                    }

                }


            }
            catch (SystemException ex)
            {
                res.DisplayMessage = "Project failed!" + ex.Message;
            }
            return res;
        }


        [HttpGet]
        public async Task<ProjectFolderDTO>? GetInviteUsers(int Id)
        {
            ProjectFolderDTO outPut = new ProjectFolderDTO();

            int? pId = HttpContext.Session.GetInt32("PID");
            int? cID = HttpContext.Session.GetInt32("CID");

            string inviteDetailsCacheKey = $"InviteDetails_{pId}_{cID}";

            var FolderList = _memoryCache.Get(inviteDetailsCacheKey) as List<ProjectFolderDTO>;
            outPut = FolderList.Where(x => x.Id == Id).SingleOrDefault();
            return outPut;
        }

        public async Task<ProjectFolderDTO>? GetInviteFileUsers(int Id)
        {
            ProjectFolderDTO outPut = new ProjectFolderDTO();
            int? pId = HttpContext.Session.GetInt32("PID");
            int? cID = HttpContext.Session.GetInt32("CID");

            string fileDetailsCacheKey = $"FileDetails_{pId}_{cID}";
            var FolderList = _memoryCache.Get(fileDetailsCacheKey) as List<ProjectFolderDTO>;
            outPut = FolderList.Where(x => x.Id == Id).SingleOrDefault();
            return outPut;
        }

        public async Task<ProjectFolderFilesDTO>? GetFileUsers(int Id)
        {
            ProjectFolderFilesDTO outPut = new ProjectFolderFilesDTO();
            int? pId = HttpContext.Session.GetInt32("PID");
            int? cID = HttpContext.Session.GetInt32("CID");
            int? fId = HttpContext.Session.GetInt32("FID");

            var folderFileDetailsCacheKey = $"FolderFileDetails_{pId}_{cID}_{fId}";
            var FolderList = _memoryCache.Get(folderFileDetailsCacheKey) as List<ProjectFolderFilesDTO>;
            outPut = FolderList.Where(x => x.Id == Id).SingleOrDefault();
            return outPut;
        }

        [HttpGet]
        public async Task<OutPutResponse>? EditFileName(int Id, string? Name)
        {
            OutPutResponse res = new OutPutResponse();
            ProjectFolderDTO inputs = new ProjectFolderDTO();
            inputs.IsActive = true;
            inputs.CreationDate = DateTime.Now;
            inputs.UserId = HttpContext.Session.GetInt32("UserId");
            inputs.FolderName =Name;            
            inputs.Id = Id;

            int? pId = HttpContext.Session.GetInt32("PID");
            int? cID = HttpContext.Session.GetInt32("CID");

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiProjectFileUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("EditFileName", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var inviteDetailsCacheKey = $"InviteDetails_{pId}_{cID}";
                        //string fileDetailsCacheKey = $"FileDetails_{pId}_{cID}";
                        _memoryCache.Remove(inviteDetailsCacheKey);

                        var data = await response.Content.ReadAsStringAsync();

                        res = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data) ?? new OutPutResponse();


                    }

                }


            }
            catch (SystemException ex)
            {
                res.DisplayMessage = "Project failed!" + ex.Message;
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
