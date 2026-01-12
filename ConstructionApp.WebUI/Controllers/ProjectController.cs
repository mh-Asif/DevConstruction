using Construction.Infrastructure.Helper;
using Construction.Infrastructure.KeyValues;
using Construction.Infrastructure.Models;
using ConstructionApp.WebUI.Helper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Reflection;
using System.Security.Cryptography;

namespace ConstructionApp.WebUI.Controllers
{
    public class ProjectController(ILogger<ProjectController> logger, IMemoryCache memoryCache) : Controller
    {
        private readonly IMemoryCache _memoryCache =memoryCache;
        private readonly ILogger<ProjectController> _logger = logger;
        public async Task<IActionResult> ProjectOverview()
        {
            ProjectsDTO objOutPut = new ProjectsDTO();
           

            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (strMenuSession == null)
                return RedirectToAction("Index", "Home");

            AccessMasterDTO? empSession = (AccessMasterDTO?)(JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession));

            objOutPut.AccessList = empSession.AccessList;

            int? unitId = HttpContext.Session.GetInt32("UnitId");
            int isAdmin = HttpContext.Session.GetInt32("IsAdmin") ?? 0;
            string projectDashboardKey = string.Empty, taskDashboardKey = string.Empty;
            objOutPut.UserType = HttpContext.Session.GetInt32("UserType");

            var userIdVal = HttpContext.Session.GetInt32("UserId");
            string dashboardKey = userIdVal != null ? $"ProjectDashboard_{userIdVal}" : "ProjectDashboard_Anonymous";
            if (_memoryCache.Get(dashboardKey) == null)
            {
                objOutPut.ProjectDashboardList = (userIdVal != null) ? (await GetProjectDashboard(userIdVal) ?? new List<ProjectDashboardDTO>()) : new List<ProjectDashboardDTO>();
                _memoryCache.Set(dashboardKey, objOutPut.ProjectDashboardList);
            }
            else
            {
                objOutPut.ProjectDashboardList = _memoryCache.Get<List<ProjectDashboardDTO>>(dashboardKey);
            }

            //int? userId = HttpContext.Session.GetInt32("UserId");
            
            if (isAdmin == 1)
              taskDashboardKey = userIdVal != null && unitId != null ? $"TskDashboard_{unitId}" : "TskDashboard_Global";
            else
             taskDashboardKey = userIdVal != null && unitId != null ? $"TskDashboard_{unitId}_{userIdVal}" : "TskDashboard_Global";

            if (_memoryCache.TryGetValue(taskDashboardKey, out var cachedTaskDashboardObj) && cachedTaskDashboardObj is List<TasksDashboardDTO> cachedTaskDashboard && cachedTaskDashboard != null)
            {
                if (objOutPut.ProjectSummeryList == null)
                    objOutPut.ProjectSummeryList = new List<ProjectSummeryDTO>();

                objOutPut.ProjectSummeryList = cachedTaskDashboard
                    .Select(task => new ProjectSummeryDTO
                    {
                          InProgressTask = task.InProgressTask,
                        CompletedTask = task.CompletedTask,
                        OverdueTask = task.OverdueTask,
                        TotalTask = (int)task.TotalTask,
                        OpenTask=task.OpenTask,
                         ApprovedTask= task.ApprovedTask

                    })
                    .ToList();
            }
            //objOutPut.ProjectSummeryList

            return View(objOutPut);
        }
        [HttpGet]
        [Route("Project/ProjectOverview/{pId}")]
        public async Task<IActionResult> ProjectOverview(int pId)
        {
            ProjectsDTO objOutPut = new ProjectsDTO();
            var userIdVal = HttpContext.Session.GetInt32("UserId");
            int? userId = HttpContext.Session.GetInt32("UserId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            if (pId == 0)
            {
                if(_memoryCache.TryGetValue($"ProjectIdPreview_{userIdVal}", out int cachedProjectId))
                {
                    pId = cachedProjectId;
                }
                else
                {
                    var ePID = HttpContext.Session.GetString("ePID");
                    if (string.IsNullOrEmpty(ePID))
                    {
                                           

                        // ProjectDashboardList caching (per user)
                        string dashboardKey = userId != null ? $"ProjectDashboard_{userId}" : "ProjectDashboard_Anonymous";
                        var cachedDashboard = _memoryCache.Get<List<ProjectDashboardDTO>>(dashboardKey);
                        if (cachedDashboard == null)
                        {
                            if (userId != null)
                            {
                                var dashboard = await GetProjectDashboard(userId);
                                objOutPut.ProjectDashboardList = dashboard ?? new List<ProjectDashboardDTO>();
                                _memoryCache.Set(dashboardKey, objOutPut.ProjectDashboardList);
                            }
                            else
                            {
                                objOutPut.ProjectDashboardList = new List<ProjectDashboardDTO>();
                            }
                        }
                        else
                        {
                            objOutPut.ProjectDashboardList = cachedDashboard;
                        }
                        return View(objOutPut);
                       // return RedirectToAction("Index", "Home");
                    }

                    objOutPut.EncProjectId = ePID;
                    pId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ePID));
                    _memoryCache.Set($"ProjectIdPreview_{userIdVal}", pId);
                }
              
            }
            else
            {
                _memoryCache.Set($"ProjectIdPreview_{userIdVal}", pId);
            }

           
            CommentsDashboardDTO inputs = new CommentsDashboardDTO();
            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (strMenuSession == null)
                return RedirectToAction("Index", "Home");

            AccessMasterDTO? empSession = JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession);
            objOutPut.AccessList = empSession?.AccessList ?? new List<AccessMasterDTO>();
            objOutPut.UserType = HttpContext.Session.GetInt32("UserType");

            if (pId > 0)
            {
                objOutPut.ProjectId = pId;
                objOutPut.EncProjectId = CommonHelper.EncryptURLHTML(pId.ToString());
                //int? userId = HttpContext.Session.GetInt32("UserId");
                //int? unitId = HttpContext.Session.GetInt32("UnitId");

                // ProjectDashboardList caching (per user)
                string dashboardKey = userId != null ? $"ProjectDashboard_{userId}" : "ProjectDashboard_Anonymous";
                var cachedDashboard = _memoryCache.Get<List<ProjectDashboardDTO>>(dashboardKey);
                if (cachedDashboard == null)
                {
                    if (userId != null)
                    {
                        var dashboard = await GetProjectDashboard(userId);
                        objOutPut.ProjectDashboardList = dashboard ?? new List<ProjectDashboardDTO>();
                        _memoryCache.Set(dashboardKey, objOutPut.ProjectDashboardList);
                    }
                    else
                    {
                        objOutPut.ProjectDashboardList = new List<ProjectDashboardDTO>();
                    }
                }
                else
                {
                    objOutPut.ProjectDashboardList = cachedDashboard;
                }

                // ProjectSummeryList caching (per user, per project)
                string projectSummeryKey = userId != null ? $"ProjectSummery_{pId}" : $"ProjectSummery_Anonymous_{pId}";
                var cachedSummery = _memoryCache.Get<List<ProjectSummeryDTO>>(projectSummeryKey);
                if (cachedSummery == null)
                {
                    if (userId != null)
                    {
                        var summery = await GetProjectSummery(unitId, pId);
                        objOutPut.ProjectSummeryList = summery ?? new List<ProjectSummeryDTO>();
                        _memoryCache.Set(projectSummeryKey, objOutPut.ProjectSummeryList);
                    }
                    else
                    {
                        objOutPut.ProjectSummeryList = new List<ProjectSummeryDTO>();
                    }
                }
                else
                {
                    objOutPut.ProjectSummeryList = cachedSummery;
                }

                // ProjectRoleSummeryList caching (per user, per project, per unit)
                string projectRoleKey = (unitId != null && userId != null) ? $"ProjectRole_{pId}" : $"ProjectRole_Anonymous_{pId}";
                var cachedRole = _memoryCache.Get<List<ProjectRoleSummeryDTO>>(projectRoleKey);
                if (cachedRole == null)
                {
                    if (unitId.HasValue)
                    {
                        var roleSummery = await GetProjectRoleSummery(unitId.Value, pId) ?? new List<ProjectRoleSummeryDTO>();
                        objOutPut.ProjectRoleSummeryList = roleSummery;
                        _memoryCache.Set(projectRoleKey, objOutPut.ProjectRoleSummeryList);
                    }
                    else
                    {
                        objOutPut.ProjectRoleSummeryList = new List<ProjectRoleSummeryDTO>();
                    }
                }
                else
                {
                    objOutPut.ProjectRoleSummeryList = cachedRole;
                }

                // Comments caching (per user, per project)
                string commentsKey = userId != null ? $"Comments_{pId}" : $"Comments_Anonymous_{pId}";
                var cachedComments = _memoryCache.Get<List<CommentsDashboardDTO>>(commentsKey);
                inputs.IsComment = false;
                inputs.ProjectId = pId;
                inputs.SubTaskId = 0;
                inputs.TaskId = 0;
                if (cachedComments != null)
                {
                    objOutPut.ActivityList = cachedComments;
                }
                else
                {
                    var comments = await GetComments(inputs);
                    objOutPut.ActivityList = comments ?? new List<CommentsDashboardDTO>();
                    _memoryCache.Set(commentsKey, objOutPut.ActivityList, TimeSpan.FromMinutes(30));
                }
            }
            return View(objOutPut);
        }

        public async Task<IActionResult> ProjectOverviewWithFilter(int pId)
        {
            ProjectsDTO objOutPut = new ProjectsDTO();
            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (strMenuSession == null)
                return RedirectToAction("Index", "Home");

            CommentsDashboardDTO inputs = new CommentsDashboardDTO();
            AccessMasterDTO? empSession = JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession);
            objOutPut.ProjectId = pId;
            int? userId = HttpContext.Session.GetInt32("UserId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            // ProjectDashboardList (per user)
            string dashboardKey = userId != null ? $"ProjectDashboard_{userId}" : "ProjectDashboard_Anonymous";
            var cachedDashboard = _memoryCache.Get<List<ProjectDashboardDTO>>(dashboardKey);
            if (cachedDashboard == null)
            {
                if (userId.HasValue)
                {
                    var dashboard = await GetProjectDashboard(userId.Value) ?? new List<ProjectDashboardDTO>();
                    objOutPut.ProjectDashboardList = dashboard;
                    _memoryCache.Set(dashboardKey, objOutPut.ProjectDashboardList);
                }
                else
                {
                    objOutPut.ProjectDashboardList = new List<ProjectDashboardDTO>();
                }
            }
            else
            {
                objOutPut.ProjectDashboardList = cachedDashboard;
            }
            // ProjectSummeryList (per user, per project)
            string projectSummeryKey = userId != null ? $"ProjectSummery_{pId}" : $"ProjectSummery_Anonymous_{pId}";
            var cachedSummery = _memoryCache.Get<List<ProjectSummeryDTO>>(projectSummeryKey);
            if (cachedSummery == null)
            {
                if (userId.HasValue)
                {
                    var summery = await GetProjectSummery(unitId, pId) ?? new List<ProjectSummeryDTO>();
                    objOutPut.ProjectSummeryList = summery;
                    _memoryCache.Set(projectSummeryKey, objOutPut.ProjectSummeryList);
                }
                else
                {
                    objOutPut.ProjectSummeryList = new List<ProjectSummeryDTO>();
                }
            }
            else
            {
                objOutPut.ProjectSummeryList = cachedSummery;
            }
            // ProjectRoleSummeryList (per user, per project, per unit)
            string projectRoleKey = (unitId != null && userId != null) ? $"ProjectRole_{pId}" : $"ProjectRole_Anonymous_{pId}";
            var cachedRole = _memoryCache.Get<List<ProjectRoleSummeryDTO>>(projectRoleKey);
            if (cachedRole == null)
            {
                if (unitId.HasValue)
                {
                    var roleSummery = await GetProjectRoleSummery(unitId.Value, pId) ?? new List<ProjectRoleSummeryDTO>();
                    objOutPut.ProjectRoleSummeryList = roleSummery;
                    _memoryCache.Set(projectRoleKey, objOutPut.ProjectRoleSummeryList);
                }
                else
                {
                    objOutPut.ProjectRoleSummeryList = new List<ProjectRoleSummeryDTO>();
                }
            }
            else
            {
                objOutPut.ProjectRoleSummeryList = cachedRole;
            }
            objOutPut.AccessList = empSession?.AccessList ?? new List<AccessMasterDTO>();
            objOutPut.UserType = HttpContext.Session.GetInt32("UserType");

            inputs.IsComment = false;
            inputs.ProjectId = pId;
            inputs.SubTaskId = 0;
            inputs.TaskId = 0;
            // Comments caching (per user, per project)
            string commentsKey = userId != null ? $"Comments_{pId}" : $"Comments_Anonymous_{pId}";
            var cachedComments = _memoryCache.Get<List<CommentsDashboardDTO>>(commentsKey);
            inputs.IsComment = false;
            inputs.ProjectId = pId;
            inputs.SubTaskId = 0;
            inputs.TaskId = 0;
            if (cachedComments != null)
            {
                objOutPut.ActivityList = cachedComments;
            }
            else
            {
                var comments = await GetComments(inputs) ?? new List<CommentsDashboardDTO>();
                objOutPut.ActivityList = comments;
                _memoryCache.Set(commentsKey, objOutPut.ActivityList, TimeSpan.FromMinutes(30));
            }

            return View("ProjectOverview", objOutPut);
        }
        public async Task<IActionResult> ProjectManagement()
        {
            ProjectsDTO objOutPut = new ProjectsDTO();
            objOutPut.UserType = HttpContext.Session.GetInt32("UserType");
            objOutPut.UserList = await GetUsers();
            objOutPut.ClientList = await GetClientList(2);
            objOutPut.CategoryList = await GetUnitCategoryList();
            objOutPut.PriorityList = await GetUnitPriorityList();
            objOutPut.StatusList = await GetUnitStatusList();
            objOutPut.ProjectDashboardList = await GetProjectDashboard(HttpContext.Session.GetInt32("UserId"));
            return View(objOutPut);
        }

        //[ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> ProjectDetails()
        {
            ProjectsDTO objOutPut = new ProjectsDTO();

         
            string? strMenuSession = HttpContext.Session.GetString("Menu");
            var uId = HttpContext.Session.GetString("UserId");
            if (strMenuSession == null || uId == null)
                return RedirectToAction("Index", "Home");

            AccessMasterDTO? empSession = (AccessMasterDTO?)(JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession));

            objOutPut.AccessList = empSession?.AccessList?.Where(p => p.ModuleCode.Trim() == "P").ToList() ?? new List<AccessMasterDTO>();
            objOutPut.UserType = HttpContext.Session.GetInt32("UserType");

            //objOutPut.UserList = await GetUsers();
            //objOutPut.ClientList = await GetClientList(2);
            //objOutPut.CategoryList = await GetUnitCategoryList();
            //objOutPut.PriorityList = await GetUnitPriorityList();
            //objOutPut.StatusList = await GetUnitStatusList();

            if (_memoryCache.Get("UserList") == null)
            {
                objOutPut.UserList = await GetUsers();
                _memoryCache.Set("UserList", objOutPut.UserList);
            }
            else
            {
                objOutPut.UserList = _memoryCache.Get<List<UserKeyValues>>("UserList");
            }
            if (_memoryCache.Get("ClientList") == null)
            {
                objOutPut.ClientList = await GetClientList(2);
                _memoryCache.Set("ClientList", objOutPut.ClientList);
            }
            else
            {
                objOutPut.ClientList = _memoryCache.Get<List<UserKeyValues>>("ClientList");
            }
            if (_memoryCache.Get("CategoryList") == null)
            {
                objOutPut.CategoryList = await GetUnitCategoryList();
                _memoryCache.Set("CategoryList", objOutPut.CategoryList);
            }
            else
            {
                objOutPut.CategoryList = _memoryCache.Get<List<CategoryKeyValues>>("CategoryList");
            }

            if (_memoryCache.Get("PriorityList") == null)
            {
                objOutPut.PriorityList = await GetUnitPriorityList();
                _memoryCache.Set("PriorityList", objOutPut.PriorityList);
            }
            else
            {
                objOutPut.PriorityList = _memoryCache.Get<List<UnitPriorityKeyValues>>("PriorityList");
            }
            if (_memoryCache.Get("StatusList") == null)
            {
                objOutPut.StatusList = await GetUnitStatusList();
                if(objOutPut.AccessList.Where(x => x.ModuleCode.Trim() == "P" && x.IsApproval == false).Count() > 0)
                {
                    var itemToRemove = objOutPut.StatusList.SingleOrDefault(x => x.Status == "Approved");
                    if (itemToRemove != null)
                    {
                        objOutPut.StatusList.Remove(itemToRemove);
                    }                    

                }
                _memoryCache.Set("StatusList", objOutPut.StatusList);
            }
            else
            {
                objOutPut.StatusList = _memoryCache.Get<List<UnitStatusKeyValues>>("StatusList");
            }


           var userId = HttpContext.Session.GetInt32("UserId");
            string dashboardKey = userId != null ? $"ProjectDashboard_{userId}" : "ProjectDashboard_Anonymous";
            if (_memoryCache.Get(dashboardKey) == null)
            {
                if (userId.HasValue)
                {
                    objOutPut.ProjectDashboardList = await GetProjectDashboard(userId.Value) ?? new List<ProjectDashboardDTO>();
                }
                else
                {
                    objOutPut.ProjectDashboardList = new List<ProjectDashboardDTO>();
                }
                _memoryCache.Set(dashboardKey, objOutPut.ProjectDashboardList);
            }
            else
            {
                objOutPut.ProjectDashboardList = _memoryCache.Get<List<ProjectDashboardDTO>>(dashboardKey);
            }
         //   objOutPut.ProjectDashboardList = await GetProjectDashboard(HttpContext.Session.GetInt32("UserId"));
            return View(objOutPut);
        }
        public async Task<List<UnitPriorityKeyValues>> GetUnitPriorityList()
        {
            // DataTable dt = new DataTable();
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            List<UnitPriorityKeyValues> PriorityLst = new List<UnitPriorityKeyValues>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetUnitPriority?unitId=" + unitId);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    PriorityLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UnitPriorityKeyValues>>(data) ?? new List<UnitPriorityKeyValues>();

                }


            }
            return PriorityLst;
        }

        public async Task<List<CategoryKeyValues>> GetUnitCategoryList()
        {
            // DataTable dt = new DataTable();
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            List<CategoryKeyValues> PriorityLst = new List<CategoryKeyValues>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetUnitCategory?unitId=" + unitId);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    PriorityLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CategoryKeyValues>>(data) ?? new List<CategoryKeyValues>();

                    //if (PriorityLst.Count > 0)
                    //{
                    //    foreach (var item in PriorityLst)
                    //    {
                    //        item.EncCompanyPriorityId = CommonHelper.EncryptURLHTML(item.CompanyPriorityId.ToString());
                    //    }

                    //}
                }


            }
            return PriorityLst;
        }

        public async Task<List<UnitStatusKeyValues>> GetUnitStatusList()
        {
            // DataTable dt = new DataTable();
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            List<UnitStatusKeyValues> PriorityLst = new List<UnitStatusKeyValues>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetUnitStatus?unitId=" + unitId + "&statusType=P");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    PriorityLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UnitStatusKeyValues>>(data) ?? new List<UnitStatusKeyValues>();

                }


            }
            return PriorityLst;
        }

        public async Task<List<UserKeyValues>> GetUsers()
        {
            // DataTable dt = new DataTable();
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            List<UserKeyValues> PriorityLst = new List<UserKeyValues>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiUserUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetUnitUsers?unitId=" + unitId);
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

        public async Task<List<UserKeyValues>> GetClientList(int? userType)
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

        [HttpPost]
        public async Task<IActionResult>? SaveProject(ProjectsDTO inputs)
        {
           
            ProjectsDTO OutPut = new ProjectsDTO();
            ClientMail obj = new ClientMail();
            inputs.IsActive = true;
            inputs.CreatedBy = HttpContext.Session.GetInt32("UserId");
            // inputs.IsAdmin = false;
            inputs.CreatedOn = DateTime.Now;
            inputs.UnitId = HttpContext.Session.GetInt32("UnitId");
            // inputs.LoginPassword = CommonHelper.Encrypt(CommonHelper.RandomString());
            inputs.FileDetails = "";
            inputs.EncryptedId = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiProjectUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("SaveProject", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var userId = HttpContext.Session.GetInt32("UserId");
                        var unitId = HttpContext.Session.GetInt32("UnitId");
                        UserCommentsDTO commentsInputs = new UserCommentsDTO();
                        var data = await response.Content.ReadAsStringAsync();

                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);
                        var projectId = inputs.ProjectId == 0 && result != null ? result.RespId : inputs.ProjectId;
                        // Remove all dynamic cache keys for this user/project/unit
                        if (userId != null)
                        {
                            if (_memoryCache is MemoryCache memCache)
                            {
                                memCache.Clear();
                            }
                            //_memoryCache.Remove($"ProjectDashboard_{userId}");
                            //if (projectId != null)
                            //{
                            //    _memoryCache.Remove($"ProjectSummery_{projectId}");
                            //    _memoryCache.Remove($"Comments_{projectId}");
                            //    if (unitId != null)
                            //        _memoryCache.Remove($"ProjectRole_{projectId}");
                            //}
                        }
                        // Remove fallback/legacy keys as well
                       // _memoryCache.Remove("ProjectId");
                        //_memoryCache.Remove("Notifications");

                        OutPut.HttpStatusCode = result.HttpStatusCode;
                        OutPut.DisplayMessage = result.DisplayMessage;
                        if (inputs.ProjectId == 0)
                        {
                            commentsInputs.TaskId = 0;
                            commentsInputs.ProjectId = result.RespId;
                            commentsInputs.SubTaskId = 0;
                            commentsInputs.Comments = "Created a project: " + inputs.ProjectName;
                            commentsInputs.Summary = "Project Management";
                            AddUserComments(commentsInputs);

                            OutPut.IsCreated = true;
                            OutPut.ProjectDashboardList = await GetProjectForTemplate(result.RespId);
                            OutPut.ProjectRoleSummeryList = await GetProjectRoleSummery(HttpContext.Session.GetInt32("UnitId"), result.RespId);                           
                         await obj.SendProjectMail(OutPut);
                        }
                        else
                        {
                            commentsInputs.TaskId = 0;
                            commentsInputs.ProjectId = inputs.ProjectId;
                            commentsInputs.SubTaskId = 0;
                            commentsInputs.Comments = "Updated a project: " + inputs.ProjectName;
                            commentsInputs.Summary = "Project Management";
                            AddUserComments(commentsInputs);

                            OutPut.IsCreated = false;
                            OutPut.ProjectDashboardList = await GetProjectForTemplate(inputs.ProjectId);
                            OutPut.ProjectRoleSummeryList = await GetProjectRoleSummery(HttpContext.Session.GetInt32("UnitId"), inputs.ProjectId);
                           await obj.SendProjectMail(OutPut);

                        }

                        return RedirectToAction("ProjectDetails", "Project");

                    }

                }


            }
            catch (SystemException ex)
            {
                OutPut.DisplayMessage = "Project failed!" + ex.Message;
            }
            return RedirectToAction("ProjectDetails", "Project");
        }


        public async Task<int> AddUserComments(UserCommentsDTO inputs)
        {
            try
            {
                inputs.IsActive = true;
                inputs.UserId = HttpContext.Session.GetInt32("UserId");

                // inputs.Comments = comments;
                // inputs.IsAdmin = false;
                inputs.CreationDate = DateTime.Now;

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiCommentsUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("UsersComments", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();

                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                    }

                }


            }
            catch (SystemException ex)
            {

            }

            return 1;
        }
        public async Task<List<ProjectsDTO>>? GetProject()
        {
            // DataTable dt = new DataTable();
            List<ProjectsDTO> UserLst = new List<ProjectsDTO>();
            int? UnitId = HttpContext.Session.GetInt32("UnitId");
            // int countryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ecountryId));
            //  inputs.CountryId = countryId;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiProjectUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                //HttpResponseMessage response = await client.GetAsync("GetUserListing?userType=" + userType + "&logInUser="+ logInUser);
                HttpResponseMessage response = await client.GetAsync("GetProjects?unitId=" + UnitId);
                //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    UserLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProjectsDTO>>(data);

                    foreach (var item in UserLst)
                    {

                        item.EncryptedId = CommonHelper.EncryptURLHTML(item.ProjectId.ToString());
                        // if (item.ProfileImage != null)
                        //    item.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);

                    }
                }

                return UserLst;
            }
        }

        [HttpGet]
        [Route("Project/GetProjectId/{epId}")]
        public async Task<IActionResult>? GetProjectId(string epId)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            ProjectsDTO objOutPut = new ProjectsDTO();

            int pId = Convert.ToInt32(CommonHelper.DecryptURLHTML(epId));
            if (pId > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string? strMenuSession = HttpContext.Session.GetString("Menu");
                        if (strMenuSession == null)
                            return RedirectToAction("Index", "Home");

                        AccessMasterDTO? empSession = (AccessMasterDTO?)(JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession));

                        

                        client.BaseAddress = new Uri(EnvironmentUrl.apiProjectUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("GetProjectById?pId=" + pId);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            objOutPut = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjectsDTO>(data);

                            //objOutPut.UserList = await GetUsers();
                            //objOutPut.ClientList = await GetClientList(2);
                            //objOutPut.CategoryList = await GetUnitCategoryList();
                            //objOutPut.PriorityList = await GetUnitPriorityList();
                            //objOutPut.StatusList = await GetUnitStatusList();

                            if (_memoryCache.Get("UserList") == null)
                            {
                                objOutPut.UserList = await GetUsers();
                                _memoryCache.Set("UserList", objOutPut.UserList);
                            }
                            else
                            {
                                objOutPut.UserList = _memoryCache.Get<List<UserKeyValues>>("UserList");
                            }
                            if (_memoryCache.Get("ClientList") == null)
                            {
                                objOutPut.ClientList = await GetClientList(2);
                                _memoryCache.Set("ClientList", objOutPut.ClientList);
                            }
                            else
                            {
                                objOutPut.ClientList = _memoryCache.Get<List<UserKeyValues>>("ClientList");
                            }
                            if (_memoryCache.Get("CategoryList") == null)
                            {
                                objOutPut.CategoryList = await GetUnitCategoryList();
                                _memoryCache.Set("CategoryList", objOutPut.CategoryList);
                            }
                            else
                            {
                                objOutPut.CategoryList = _memoryCache.Get<List<CategoryKeyValues>>("CategoryList");
                            }

                            if (_memoryCache.Get("PriorityList") == null)
                            {
                                objOutPut.PriorityList = await GetUnitPriorityList();
                                _memoryCache.Set("PriorityList", objOutPut.PriorityList);
                            }
                            else
                            {
                                objOutPut.PriorityList = _memoryCache.Get<List<UnitPriorityKeyValues>>("PriorityList");
                            }
                            if (_memoryCache.Get("StatusList") == null)
                            {
                                objOutPut.StatusList = await GetUnitStatusList();
                                _memoryCache.Set("StatusList", objOutPut.StatusList);
                            }
                            else
                            {
                                objOutPut.StatusList = _memoryCache.Get<List<UnitStatusKeyValues>>("StatusList");
                            }

                            var userId = HttpContext.Session.GetInt32("UserId");
                            string dashboardKey = userId != null ? $"ProjectDashboard_{userId}" : "ProjectDashboard_Anonymous";
                            if (_memoryCache.Get(dashboardKey) == null)
                            {
                                if (userId.HasValue)
                                {
                                    objOutPut.ProjectDashboardList = await GetProjectDashboard(userId.Value) ?? new List<ProjectDashboardDTO>();
                                }
                                else
                                {
                                    objOutPut.ProjectDashboardList = new List<ProjectDashboardDTO>();
                                }
                                _memoryCache.Set(dashboardKey, objOutPut.ProjectDashboardList);
                            }
                            else
                            {
                                objOutPut.ProjectDashboardList = _memoryCache.Get<List<ProjectDashboardDTO>>(dashboardKey);
                            }

                            // objOutPut.ProjectDashboardList = await GetProjectDashboard(HttpContext.Session.GetInt32("UserId"));
                        }
                        objOutPut.AccessList = empSession.AccessList.Where(p => p.ModuleCode.Trim() == "P").ToList();
                        objOutPut.UserType = HttpContext.Session.GetInt32("UserType");

                    }

                }
                catch (SystemException ex)
                {
                    objOutPut.DisplayMessage = "Clinic failed!" + ex.Message;
                }

            }
            return View("ProjectDetails", objOutPut);
        }

        [HttpGet]
        //[Route("Project/DeleteProject/{epId}")]
        public async Task<ProjectsDTO>? DeleteProject(string epId)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            ProjectsDTO OutPut = new ProjectsDTO();
            int? userId = HttpContext.Session.GetInt32("UserId");
            var unitId = HttpContext.Session.GetInt32("UnitId");
            int pId = Convert.ToInt32(CommonHelper.DecryptURLHTML(epId));
            string dashboardKey = userId != null ? $"ProjectDashboard_{pId}" : "ProjectDashboard";

            if (pId > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiProjectUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("DeleteProject?pId=" + pId);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            if (_memoryCache is MemoryCache memCache)
                            {
                                memCache.Clear();
                            }
                            //_memoryCache.Remove($"ProjectDashboard_{userId}");
                            //if (pId != null)
                            //{
                            //    _memoryCache.Remove($"ProjectSummery_{pId}");
                            //    _memoryCache.Remove($"Comments_{pId}");
                            //    if (unitId != null)
                            //        _memoryCache.Remove($"ProjectRole_{pId}");
                            //}
                        }
                        var data = await response.Content.ReadAsStringAsync();
                            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                            OutPut.HttpStatusCode = result.HttpStatusCode;
                            OutPut.DisplayMessage = result.DisplayMessage;

                        //var projectListTask = GetOrSetCache(dashboardKey, async () =>
                        //{
                        //    var dashboardList = await GetProjectDashboard(userId);
                        //    var result = dashboardList?.Select(x => new ProjectKeyValues { ProjectId = x.ProjectId, ProjectName = x.ProjectName }).ToList();
                        //    return result ?? new List<ProjectKeyValues>();
                        //});

                       // OutPut.ProjectList = await projectListTask;
                       //  OutPut.ProjectList = await GetProject();
                    }

                    }

                
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "Clinic failed!" + ex.Message;
                }

            }
            return  OutPut;
        }
        private Task<T> GetOrSetCache<T>(string key, Func<Task<T>> factory, int minutes = 30) where T : class
        {
            if (_memoryCache.TryGetValue(key, out T? value) && value != null)
                return Task.FromResult(value);
            return GetOrSetCacheInternal(key, factory, minutes);
        }
        private async Task<T> GetOrSetCacheInternal<T>(string key, Func<Task<T>> factory, int minutes) where T : class
        {
            var value = await factory();
            if (value != null)
                _memoryCache.Set(key, value, TimeSpan.FromMinutes(minutes));
            return value ?? Activator.CreateInstance<T>();
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

        public async Task<List<ProjectSummeryDTO>>? GetProjectSummery(int? unitId, int? projectId)
        {
            // DataTable dt = new DataTable();
            List<ProjectSummeryDTO> ProjectLst = new List<ProjectSummeryDTO>();

            // int countryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ecountryId));
            //  inputs.CountryId = countryId;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiProjectUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                //HttpResponseMessage response = await client.GetAsync("GetUserListing?userType=" + userType + "&logInUser="+ logInUser);
                HttpResponseMessage response = await client.GetAsync("GetProjectSummery?unitId=" + unitId + "&projectId=" + projectId);
                //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    ProjectLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProjectSummeryDTO>>(data);

                }

                return ProjectLst;
            }

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
                //HttpResponseMessage response = await client.GetAsync("GetUserListing?userType=" + userType + "&logInUser="+ logInUser);
                HttpResponseMessage response = await client.GetAsync("GetProjectRoleSummery?unitId=" + unitId + "&projectId=" + projectId);
                //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    ProjectLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProjectRoleSummeryDTO>>(data);

                }
                foreach (var item in ProjectLst)
                {

                    // item.EnycUserId = CommonHelper.EncryptURLHTML(item.UserId.ToString());
                    if (item.ProfilePic != null)
                        item.Base64ProfileImage = item.ProfilePic;

                }

                return ProjectLst;
            }

        }

        public async Task<IActionResult> ProjectTest()
        {
            return View();
        }

        [HttpPost]
        public async Task<List<CommentsDashboardDTO>>? GetComments(CommentsDashboardDTO inputs)
        {
            // UserCommentsDTO inputs = new UserCommentsDTO();
            List<CommentsDashboardDTO> CommentsLst = new List<CommentsDashboardDTO>();
            inputs.IsActive = true;
            inputs.UserId = HttpContext.Session.GetInt32("UserId");

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiCommentsUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("GetCommentsDashboard", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();

                        CommentsLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CommentsDashboardDTO>>(data);

                        //foreach (var item in CommentsLst)
                        //{
                        //    if (item.ProfileImage != null)
                        //        item.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);

                        //}
                        return CommentsLst;

                    }

                }


            }
            catch (SystemException ex)
            {

            }
            return null;

        }

        public async Task<List<ProjectDashboardDTO>>? GetProjectForTemplate(int? pId)
        {
            // DataTable dt = new DataTable();
            List<ProjectDashboardDTO> ProjectLst = new List<ProjectDashboardDTO>();
           

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiProjectUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                 HttpResponseMessage response = await client.GetAsync("GetProjectDetailsForTemplate?Pid=" + pId);              
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    ProjectLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProjectDashboardDTO>>(data);

                    //foreach (var item in ProjectLst)
                    //{

                    //    item.EncProjectId = CommonHelper.EncryptURLHTML(item.ProjectId.ToString());
                    //    //if (item.ProfileImage != null)
                    //    //        item.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);

                    //}



                }

                return ProjectLst;
            }






        }

        public async Task<IActionResult> ProjectDetailsWithFilters(int? priorityId, int? statusId)
        {
            ProjectsDTO objOutPut = new ProjectsDTO();


            string? strMenuSession = HttpContext.Session.GetString("Menu");
            var uId = HttpContext.Session.GetString("UserId");
            if (strMenuSession == null || uId == null)
                return RedirectToAction("Index", "Home");

            AccessMasterDTO? empSession = (AccessMasterDTO?)(JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession));

            objOutPut.AccessList = empSession?.AccessList?.Where(p => p.ModuleCode.Trim() == "P").ToList() ?? new List<AccessMasterDTO>();
            objOutPut.UserType = HttpContext.Session.GetInt32("UserType");

            if (_memoryCache.Get("UserList") == null)
            {
                objOutPut.UserList = await GetUsers();
                _memoryCache.Set("UserList", objOutPut.UserList);
            }
            else
            {
                objOutPut.UserList = _memoryCache.Get<List<UserKeyValues>>("UserList");
            }
            if (_memoryCache.Get("ClientList") == null)
            {
                objOutPut.ClientList = await GetClientList(2);
                _memoryCache.Set("ClientList", objOutPut.ClientList);
            }
            else
            {
                objOutPut.ClientList = _memoryCache.Get<List<UserKeyValues>>("ClientList");
            }
            if (_memoryCache.Get("CategoryList") == null)
            {
                objOutPut.CategoryList = await GetUnitCategoryList();
                _memoryCache.Set("CategoryList", objOutPut.CategoryList);
            }
            else
            {
                objOutPut.CategoryList = _memoryCache.Get<List<CategoryKeyValues>>("CategoryList");
            }

            if (_memoryCache.Get("PriorityList") == null)
            {
                objOutPut.PriorityList = await GetUnitPriorityList();
                _memoryCache.Set("PriorityList", objOutPut.PriorityList);
            }
            else
            {
                objOutPut.PriorityList = _memoryCache.Get<List<UnitPriorityKeyValues>>("PriorityList");
            }
            if (_memoryCache.Get("StatusList") == null)
            {
                objOutPut.StatusList = await GetUnitStatusList();
                _memoryCache.Set("StatusList", objOutPut.StatusList);
            }
            else
            {
                objOutPut.StatusList = _memoryCache.Get<List<UnitStatusKeyValues>>("StatusList");
            }


            var userId = HttpContext.Session.GetInt32("UserId");
            string dashboardKey = userId != null ? $"ProjectDashboard_{userId}" : "ProjectDashboard_Anonymous";
            if (_memoryCache.Get(dashboardKey) == null)
            {
                if (userId.HasValue)
                {
                    objOutPut.ProjectDashboardList = await GetProjectDashboard(userId.Value) ?? new List<ProjectDashboardDTO>();
                }
                else
                {
                    objOutPut.ProjectDashboardList = new List<ProjectDashboardDTO>();
                }
                _memoryCache.Set(dashboardKey, objOutPut.ProjectDashboardList);
            }
            else
            {
                objOutPut.ProjectDashboardList = _memoryCache.Get<List<ProjectDashboardDTO>>(dashboardKey);
            }

            var list = objOutPut.ProjectDashboardList ?? new List<ProjectDashboardDTO>();

            if (priorityId > 0 && statusId > 0)
                list = list.Where(p => p.PriorityId == priorityId && p.StatusId == statusId).ToList();
            else if (priorityId > 0)
                list = list.Where(p => p.PriorityId == priorityId).ToList();
            else if (statusId > 0)
                list = list.Where(p => p.StatusId == statusId).ToList();

            objOutPut.ProjectDashboardList = list;

            objOutPut.PriorityId = priorityId;
            objOutPut.StatusId = statusId;

            return View("ProjectDetails", objOutPut);
        }

    }
}
