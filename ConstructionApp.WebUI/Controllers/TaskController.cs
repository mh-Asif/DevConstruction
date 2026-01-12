using Construction.Infrastructure.Helper;
using Construction.Infrastructure.KeyValues;
using Construction.Infrastructure.Models;
using ConstructionApp.WebUI.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ConstructionApp.WebUI.Controllers
{
    public class TaskController(ILogger<TaskController> logger, IMemoryCache memoryCache, IWebHostEnvironment environment) : Controller
    {

        private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly ILogger<TaskController> _logger = logger;
        private IWebHostEnvironment _environment = environment;

        //[ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> TaskManagement()
        {
            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (string.IsNullOrEmpty(strMenuSession))
                return RedirectToAction("Index", "Home");

            AccessMasterDTO? empSession = JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession);
            int? userType = HttpContext.Session.GetInt32("UserType");
            int? userId = HttpContext.Session.GetInt32("UserId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            int? projectId = 0;

            HttpContext.Session.SetInt32("PID", (int)projectId);
            if (!userType.HasValue || !userId.HasValue || !unitId.HasValue || empSession == null)
                return RedirectToAction("Index", "Home");

            // Composite cache keys for user/session safety
            string cachePrefix = $"TaskMgmt_{unitId}_{projectId}";

            var userListTask = GetOrSetCache($"{cachePrefix}_UserList", async () => (await GetUsers()) ?? new List<UserKeyValues>());
            var projectListTask = GetOrSetCache($"{cachePrefix}_ProjectList", async () =>
            {
                var result = await GetProject();
                return result ?? new List<ProjectKeyValues>();
            });
            var vendorListTask = GetOrSetCache($"{cachePrefix}_VendorList", async () => (await GetUsersList(3)) ?? new List<UserKeyValues>());
            var phaseListTask = GetOrSetCache($"{cachePrefix}_PhaseList", async () => (await GetUnitPhaseList()) ?? new List<UnitPhaseKeyValues>());
            var priorityListTask = GetOrSetCache($"{cachePrefix}_PriorityList", async () => (await GetUnitPriorityList()) ?? new List<UnitPriorityKeyValues>());
            var statusListTask = GetOrSetCache($"{cachePrefix}_StatusList", async () => (await GetUnitStatusList()) ?? new List<UnitStatusKeyValues>());

           

            var dashboardTask = GetOrSetCache($"{cachePrefix}_TaskDashboard", async () =>
            {
                if (userId == null)
                    return new List<TaskDashboardDTO>();
                var result = await GetTaskDashboard(userId, projectId);
                return result ?? new List<TaskDashboardDTO>();
            });

            await Task.WhenAll(userListTask, projectListTask, vendorListTask, phaseListTask, priorityListTask, statusListTask, dashboardTask);

            var objOutPut = new ProjectTasksDTO
            {
                AccessList = empSession.AccessList ?? new List<AccessMasterDTO>(),
                UserType = userType,
                UserList = userListTask.Result ?? new List<UserKeyValues>(),
                ProjectList = projectListTask.Result ?? new List<ProjectKeyValues>(),
                VendorList = vendorListTask.Result ?? new List<UserKeyValues>(),
                PhaseList = phaseListTask.Result ?? new List<UnitPhaseKeyValues>(),
                PriorityList = priorityListTask.Result ?? new List<UnitPriorityKeyValues>(),
                StatusList = statusListTask.Result ?? new List<UnitStatusKeyValues>(),
                ProjectTaskList = dashboardTask.Result ?? new List<TaskDashboardDTO>()
            };


            if (objOutPut.AccessList.Where(x => x.ModuleCode.Trim() == "T" && x.IsApproval == false).Count() > 0)
            {
                var itemToRemove = objOutPut.StatusList.SingleOrDefault(x => x.Status == "Approved");
                if (itemToRemove != null)
                {
                    objOutPut.StatusList.Remove(itemToRemove);
                }

            }

            return View(objOutPut);
        }


        public async Task<IActionResult> TaskDetails()
        {
            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (string.IsNullOrEmpty(strMenuSession))
                return RedirectToAction("Index", "Home");

          

            AccessMasterDTO? empSession = JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession);
            int? userType = HttpContext.Session.GetInt32("UserType");
            int? userId = HttpContext.Session.GetInt32("UserId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            int? projectId = 0;
            HttpContext.Session.SetInt32("PID", (int)projectId);

            string dashboardKey = userId != null ? $"ProjectDashboard_{projectId}" : "ProjectDashboard";

            if (!userType.HasValue || !userId.HasValue || !unitId.HasValue || empSession == null)
                return RedirectToAction("Index", "Home");


           
            //if (_memoryCache.Get(dashboardKey) == null)
            //{
            //    var projectListTask = await GetProjectDashboard(uid)!;
            //    objOutPut.ProjectDashboardList = dashboardList ?? new List<ProjectDashboardDTO>();
            //    _memoryCache.Set(dashboardKey, objOutPut.ProjectDashboardList);
            //}
            //else
            //{
            //    objOutPut.ProjectDashboardList = _memoryCache.Get<List<ProjectDashboardDTO>>(dashboardKey) ?? new List<ProjectDashboardDTO>();
            //}
            // Composite cache keys for user/session safety
            string cachePrefix = $"TaskMgmt_{unitId}_{projectId}";

            var userListTask = GetOrSetCache($"{cachePrefix}_UserList", async () => (await GetUsers()) ?? new List<UserKeyValues>());

            var projectListTask = GetOrSetCache(dashboardKey, async () =>
            {
                var dashboardList = await GetProjectDashboard(userId);
                var result = dashboardList?.Select(x => new ProjectKeyValues { ProjectId = x.ProjectId, ProjectName = x.ProjectName }).ToList();
                return result ?? new List<ProjectKeyValues>();
            });

            //var projectListTask = GetOrSetCache($"{cachePrefix}_ProjectList", async () =>
            //{
            //    var result = await GetProject();
            //    return result ?? new List<ProjectKeyValues>();
            //});
            var vendorListTask = GetOrSetCache($"{cachePrefix}_VendorList", async () => (await GetUsersList(3)) ?? new List<UserKeyValues>());
            var phaseListTask = GetOrSetCache($"{cachePrefix}_PhaseList", async () => (await GetUnitPhaseList()) ?? new List<UnitPhaseKeyValues>());
            var priorityListTask = GetOrSetCache($"{cachePrefix}_PriorityList", async () => (await GetUnitPriorityList()) ?? new List<UnitPriorityKeyValues>());
            var statusListTask = GetOrSetCache($"{cachePrefix}_StatusList", async () => (await GetUnitStatusList()) ?? new List<UnitStatusKeyValues>());

            var dashboardTask = GetOrSetCache($"{cachePrefix}_TaskDashboard", async () =>
            {
                if (userId == null)
                    return new List<TaskDashboardDTO>();
                var result = await GetTaskDashboard(userId, projectId);
                return result ?? new List<TaskDashboardDTO>();
            });

            await Task.WhenAll(userListTask, projectListTask, vendorListTask, phaseListTask, priorityListTask, statusListTask, dashboardTask);

            var objOutPut = new ProjectTasksDTO
            {
                AccessList = empSession.AccessList ?? new List<AccessMasterDTO>(),
                UserType = userType,
                UserList = userListTask.Result ?? new List<UserKeyValues>(),
                ProjectList = projectListTask.Result ?? new List<ProjectKeyValues>(),
                VendorList = vendorListTask.Result ?? new List<UserKeyValues>(),
                PhaseList = phaseListTask.Result ?? new List<UnitPhaseKeyValues>(),
                PriorityList = priorityListTask.Result ?? new List<UnitPriorityKeyValues>(),
                StatusList = statusListTask.Result ?? new List<UnitStatusKeyValues>(),
                ProjectTaskList = dashboardTask.Result ?? new List<TaskDashboardDTO>()
            };

            if (objOutPut.AccessList.Where(x => x.ModuleCode.Trim() == "T" && x.IsApproval == false).Count() > 0)
            {
                var itemToRemove = objOutPut.StatusList.SingleOrDefault(x => x.Status == "Approved");
                if (itemToRemove != null)
                {
                    objOutPut.StatusList.Remove(itemToRemove);
                }

            }

            return View(objOutPut);
        }
        // Helper for composite cache with async factory
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

        [HttpGet]
        [Route("Task/TaskView/{etId}")]
        public async Task<IActionResult> TaskView(string? etId)
        {
            var tplTask = new Tuple<ProjectTasksDTO, ProjectSubTasksDTO>(new ProjectTasksDTO(), new ProjectSubTasksDTO());
            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (string.IsNullOrEmpty(strMenuSession))
                return RedirectToAction("Index", "Home");

            AccessMasterDTO? empSession = JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession);
            int? userId = HttpContext.Session.GetInt32("UserId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            if (empSession == null || !userId.HasValue || !unitId.HasValue)
                return RedirectToAction("Index", "Home");
            int safeUserId = userId.Value;
            int safeUnitId = unitId.Value;
            string cachePrefix = $"TaskMgmt_{safeUnitId}_{tplTask.Item1.ProjectId}";

            int tId = 0;
            if (!string.IsNullOrEmpty(etId))
            {
                var decrypted = CommonHelper.DecryptURLHTML(etId);
                if (!string.IsNullOrEmpty(decrypted) && int.TryParse(decrypted, out int parsedId))
                    tId = parsedId;
            }
            if (tId <= 0)
                return RedirectToAction("Index", "Home");

            try
            {
                // Use composite cache keys for all lists
                tplTask.Item1.UserList = _memoryCache.Get<List<UserKeyValues>>($"{cachePrefix}_UserList") ?? await GetOrSetCache($"{cachePrefix}_UserList", async () => (await GetUsers()) ?? new List<UserKeyValues>());
                tplTask.Item1.ProjectList = _memoryCache.Get<List<ProjectKeyValues>>($"{cachePrefix}_ProjectList") ?? await GetOrSetCache($"{cachePrefix}_ProjectList", async () => (await GetProject()) ?? new List<ProjectKeyValues>());
                tplTask.Item1.VendorList = _memoryCache.Get<List<UserKeyValues>>($"{cachePrefix}_VendorList") ?? await GetOrSetCache($"{cachePrefix}_VendorList", async () => (await GetUsersList(3)) ?? new List<UserKeyValues>());
                tplTask.Item1.PhaseList = _memoryCache.Get<List<UnitPhaseKeyValues>>($"{cachePrefix}_PhaseList") ?? await GetOrSetCache($"{cachePrefix}_PhaseList", async () => (await GetUnitPhaseList()) ?? new List<UnitPhaseKeyValues>());
                tplTask.Item1.PriorityList = _memoryCache.Get<List<UnitPriorityKeyValues>>($"{cachePrefix}_PriorityList") ?? await GetOrSetCache($"{cachePrefix}_PriorityList", async () => (await GetUnitPriorityList()) ?? new List<UnitPriorityKeyValues>());
                tplTask.Item1.StatusList = _memoryCache.Get<List<UnitStatusKeyValues>>($"{cachePrefix}_StatusList") ?? await GetOrSetCache($"{cachePrefix}_StatusList", async () => (await GetUnitStatusList()) ?? new List<UnitStatusKeyValues>());

                // Cache task details with composite key
                string taskDetailsKey = $"{cachePrefix}_TaskDetails";
                tplTask.Item1.objTask = _memoryCache.Get<ProjectTasksDTO>(taskDetailsKey);
                if (tplTask.Item1.objTask != null && tplTask.Item1.objTask.TaskId == tId)
                {
                    if (tplTask.Item1.objTask?.EndDate.HasValue == true && tplTask.Item1.objTask?.StartDate.HasValue == true)
                        tplTask.Item1.Duration = (tplTask.Item1.objTask.EndDate.Value - tplTask.Item1.objTask.StartDate.Value).Days;
                    tplTask.Item1.UserListForSubTask = _memoryCache.Get<List<UserKeyValues>>($"{cachePrefix}_SubTaskUsers") ?? new List<UserKeyValues>();
                }
                else
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiTaskUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync($"GetTaskById?pId={tId}");
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            tplTask.Item1.objTask = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjectTasksDTO>(data) ?? new ProjectTasksDTO();
                            tplTask.Item1.objTask.FilePath = "/TasksFile/" + tplTask.Item1.objTask.FileName ?? string.Empty;
                            tplTask.Item1.objTask.FileExt = Path.GetExtension(tplTask.Item1.objTask.FileName) ?? string.Empty;

                            _memoryCache.Set(taskDetailsKey, tplTask.Item1.objTask, TimeSpan.FromMinutes(30));
                            if (tplTask.Item1.objTask?.EndDate.HasValue == true && tplTask.Item1.objTask?.StartDate.HasValue == true)
                                tplTask.Item1.Duration = (tplTask.Item1.objTask.EndDate.Value - tplTask.Item1.objTask.StartDate.Value).Days;
                            int safeProjectId = tplTask.Item1.objTask?.ProjectId ?? 0;
                            int safeTaskId = tplTask.Item1.objTask?.TaskId ?? 0;
                            tplTask.Item1.UserListForSubTask = (safeProjectId > 0 && safeTaskId > 0)
                                ? (await GetSubTaskUsers(safeProjectId, safeTaskId) ?? new List<UserKeyValues>())
                                : new List<UserKeyValues>();
                            _memoryCache.Set($"{cachePrefix}_SubTaskUsers", tplTask.Item1.UserListForSubTask, TimeSpan.FromMinutes(30));
                        }
                    }
                }
                tplTask.Item1.AccessList = empSession?.AccessList ?? new List<AccessMasterDTO>();
                tplTask.Item1.UserType = HttpContext.Session.GetInt32("UserType");
                //var inputs = new CommentsDashboardDTO
                //{
                //    IsComment = false,
                //    ProjectId = 0,
                //    SubTaskId = 0,
                //    TaskId = tId
                //};
                // Cache comments with composite key
                string commentsKey = $"{cachePrefix}_Comments";
                var cachedComments = _memoryCache.Get<List<CommentsDashboardDTO>>(commentsKey);
                if (cachedComments != null && cachedComments.Any(c => c.TaskId == tId))
                {
                    tplTask.Item1.ActivityList = cachedComments;
                }
                else
                {
                    var inputs = new CommentsDashboardDTO
                    {
                        IsComment = false,
                        ProjectId = 0,
                        SubTaskId = 0,
                        TaskId = tId
                    };
                    List<CommentsDashboardDTO> comments = new List<CommentsDashboardDTO>();
                    var result = await GetComments(inputs);
                    if (result != null)
                        comments = result;
                    tplTask.Item1.ActivityList = comments;
                    _memoryCache.Set(commentsKey, tplTask.Item1.ActivityList, TimeSpan.FromMinutes(30));
                }
            }
            catch (SystemException)
            {
                tplTask.Item1.DisplayMessage = "Task failed!";
            }
            return View(tplTask);
        }

        [HttpGet]
        [Route("Task/TaskManagement/{pId}")]
        //[ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> TaskManagement(int? pId)
        {
            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (string.IsNullOrEmpty(strMenuSession))
                return RedirectToAction("Index", "Home");

            AccessMasterDTO? empSession = JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession);
            if (empSession == null)
                return RedirectToAction("Index", "Home");

            int? userType = HttpContext.Session.GetInt32("UserType");
            int? userId = HttpContext.Session.GetInt32("UserId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            if (!userType.HasValue || !userId.HasValue || !unitId.HasValue)
                return RedirectToAction("Index", "Home");

            int safeUserId = userId.Value;
            string cachePrefix = $"TaskMgmt_{unitId}_{pId}";
            int projectId = pId ?? 0;

            HttpContext.Session.SetInt32("PID", projectId);

            var userListTask = GetOrSetCache($"{cachePrefix}_UserList", async () => (await GetUsers()) ?? new List<UserKeyValues>());
            var projectListTask = GetOrSetCache($"{cachePrefix}_ProjectList", async () => (await GetProject()) ?? new List<ProjectKeyValues>());
            var vendorListTask = GetOrSetCache($"{cachePrefix}_VendorList", async () => (await GetUsersList(3)) ?? new List<UserKeyValues>());
            var phaseListTask = GetOrSetCache($"{cachePrefix}_PhaseList", async () => (await GetUnitPhaseList()) ?? new List<UnitPhaseKeyValues>());
            var priorityListTask = GetOrSetCache($"{cachePrefix}_PriorityList", async () => (await GetUnitPriorityList()) ?? new List<UnitPriorityKeyValues>());
            var statusListTask = GetOrSetCache($"{cachePrefix}_StatusList", async () => (await GetUnitStatusList()) ?? new List<UnitStatusKeyValues>());
            var dashboardTask = GetOrSetCache($"{cachePrefix}_TaskDashboard", async () =>
            {
                var result = await GetTaskDashboard(safeUserId, projectId);
                return result ?? new List<TaskDashboardDTO>();
            });

            await Task.WhenAll(userListTask, projectListTask, vendorListTask, phaseListTask, priorityListTask, statusListTask, dashboardTask);

            var objOutPut = new ProjectTasksDTO
            {
                AccessList = empSession.AccessList ?? new List<AccessMasterDTO>(),
                UserType = userType,
                ProjectId = pId,
                UserList = userListTask.Result ?? new List<UserKeyValues>(),
                ProjectList = projectListTask.Result ?? new List<ProjectKeyValues>(),
                VendorList = vendorListTask.Result ?? new List<UserKeyValues>(),
                PhaseList = phaseListTask.Result ?? new List<UnitPhaseKeyValues>(),
                PriorityList = priorityListTask.Result ?? new List<UnitPriorityKeyValues>(),
                StatusList = statusListTask.Result ?? new List<UnitStatusKeyValues>(),
                ProjectTaskList = dashboardTask.Result ?? new List<TaskDashboardDTO>()
            };

            return View(objOutPut);
        }

        [HttpGet]
        [Route("Task/TaskDetails/{pId}")]
        //[ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> TaskDetails(int? pId)
        {
            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (string.IsNullOrEmpty(strMenuSession))
                return RedirectToAction("Index", "Home");

         

            AccessMasterDTO? empSession = JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession);
            if (empSession == null)
                return RedirectToAction("Index", "Home");

            int? userType = HttpContext.Session.GetInt32("UserType");
            int? userId = HttpContext.Session.GetInt32("UserId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            if (!userType.HasValue || !userId.HasValue || !unitId.HasValue)
                return RedirectToAction("Index", "Home");

            int safeUserId = userId.Value;
            string cachePrefix = $"TaskMgmt_{unitId}_{pId}";
            int projectId = pId ?? 0;

            string dashboardKey = userId != null ? $"ProjectDashboard_{pId}" : "ProjectDashboard";
            HttpContext.Session.SetInt32("PID", projectId);

            var projectListTask = GetOrSetCache(dashboardKey, async () =>
            {
                var dashboardList = await GetProjectDashboard(safeUserId);
                var result = dashboardList?.Select(x => new ProjectKeyValues { ProjectId = x.ProjectId, ProjectName = x.ProjectName }).ToList();
                return result ?? new List<ProjectKeyValues>();
            });

            var userListTask = GetOrSetCache($"{cachePrefix}_UserList", async () => (await GetUsers()) ?? new List<UserKeyValues>());
          //  var projectListTask = GetOrSetCache($"{cachePrefix}_ProjectList", async () => (await GetProject()) ?? new List<ProjectKeyValues>());
            var vendorListTask = GetOrSetCache($"{cachePrefix}_VendorList", async () => (await GetUsersList(3)) ?? new List<UserKeyValues>());
            var phaseListTask = GetOrSetCache($"{cachePrefix}_PhaseList", async () => (await GetUnitPhaseList()) ?? new List<UnitPhaseKeyValues>());
            var priorityListTask = GetOrSetCache($"{cachePrefix}_PriorityList", async () => (await GetUnitPriorityList()) ?? new List<UnitPriorityKeyValues>());
            var statusListTask = GetOrSetCache($"{cachePrefix}_StatusList", async () => (await GetUnitStatusList()) ?? new List<UnitStatusKeyValues>());
            var dashboardTask = GetOrSetCache($"{cachePrefix}_TaskDashboard", async () =>
            {
                var result = await GetTaskDashboard(safeUserId, projectId);
                return result ?? new List<TaskDashboardDTO>();
            });

            await Task.WhenAll(userListTask, projectListTask, vendorListTask, phaseListTask, priorityListTask, statusListTask, dashboardTask);

            var objOutPut = new ProjectTasksDTO
            {
                AccessList = empSession.AccessList ?? new List<AccessMasterDTO>(),
                UserType = userType,
                ProjectId = pId,
                UserList = userListTask.Result ?? new List<UserKeyValues>(),
                ProjectList = projectListTask.Result ?? new List<ProjectKeyValues>(),
                VendorList = vendorListTask.Result ?? new List<UserKeyValues>(),
                PhaseList = phaseListTask.Result ?? new List<UnitPhaseKeyValues>(),
                PriorityList = priorityListTask.Result ?? new List<UnitPriorityKeyValues>(),
                StatusList = statusListTask.Result ?? new List<UnitStatusKeyValues>(),
                ProjectTaskList = dashboardTask.Result ?? new List<TaskDashboardDTO>()
            };

            if (objOutPut.AccessList.Where(x => x.ModuleCode.Trim() == "T" && x.IsApproval == false).Count() > 0)
            {
                var itemToRemove = objOutPut.StatusList.SingleOrDefault(x => x.Status == "Approved");
                if (itemToRemove != null)
                {
                    objOutPut.StatusList.Remove(itemToRemove);
                }

            }
            return View(objOutPut);
        }

        //[ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> TaskManagementWithFilter(int? pId, int? priorityId, int? phaseId, int? statusId)
        {
            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (string.IsNullOrEmpty(strMenuSession))
                return RedirectToAction("Index", "Home");

            AccessMasterDTO? empSession = JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession);



            int? userType = HttpContext.Session.GetInt32("UserType");
            int? userId = HttpContext.Session.GetInt32("UserId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");

           // int? projectId = HttpContext.Session.GetInt32("PID");

            HttpContext.Session.SetInt32("priorityId", (int)priorityId);
            HttpContext.Session.SetInt32("statusId", (int)statusId);

            string dashboardKey = userId != null ? $"ProjectDashboard_{pId}" : "ProjectDashboard";

            if (!userType.HasValue || !userId.HasValue || !unitId.HasValue)
                return RedirectToAction("Index", "Home");

            int safeUserId = userId.Value;
            string cachePrefix = $"TaskMgmt_{unitId}_{pId}";
            int projectId = pId ?? 0;

            HttpContext.Session.SetInt32("PID", projectId);
            var userListTask = GetOrSetCache($"{cachePrefix}_UserList", async () => (await GetUsers()) ?? new List<UserKeyValues>());
          // var projectListTask = GetOrSetCache($"{cachePrefix}_ProjectList", async () => (await GetProject()) ?? new List<ProjectKeyValues>());
            var vendorListTask = GetOrSetCache($"{cachePrefix}_VendorList", async () => (await GetUsersList(3)) ?? new List<UserKeyValues>());
            var phaseListTask = GetOrSetCache($"{cachePrefix}_PhaseList", async () => (await GetUnitPhaseList()) ?? new List<UnitPhaseKeyValues>());
            var priorityListTask = GetOrSetCache($"{cachePrefix}_PriorityList", async () => (await GetUnitPriorityList()) ?? new List<UnitPriorityKeyValues>());
            var statusListTask = GetOrSetCache($"{cachePrefix}_StatusList", async () => (await GetUnitStatusList()) ?? new List<UnitStatusKeyValues>());

            var projectListTask = GetOrSetCache(dashboardKey, async () =>
            {
                var dashboardList = await GetProjectDashboard(userId);
                var result = dashboardList?.Select(x => new ProjectKeyValues { ProjectId = x.ProjectId, ProjectName = x.ProjectName }).ToList();
                return result ?? new List<ProjectKeyValues>();
            });

            var dashboardTask = GetOrSetCache($"{cachePrefix}_TaskDashboard_{pId}", async () =>
            {
                var result = await GetTaskDashboard(safeUserId, projectId);
                return result ?? new List<TaskDashboardDTO>();
            });

            await Task.WhenAll(userListTask, projectListTask, vendorListTask, phaseListTask, priorityListTask, statusListTask, dashboardTask);

            var objOutPut = new ProjectTasksDTO
            {
                AccessList = empSession.AccessList ?? new List<AccessMasterDTO>(),
                ProjectId = pId,
                EncProjectId = CommonHelper.EncryptURLHTML(pId?.ToString() ?? "0"),
                FPhaseId = phaseId,
                FPriorityId = priorityId,
                FStatusId = statusId,
                UserType = userType,
                UserList = userListTask.Result ?? new List<UserKeyValues>(),
                ProjectList = projectListTask.Result ?? new List<ProjectKeyValues>(),
                VendorList = vendorListTask.Result ?? new List<UserKeyValues>(),
                PhaseList = phaseListTask.Result ?? new List<UnitPhaseKeyValues>(),
                PriorityList = priorityListTask.Result ?? new List<UnitPriorityKeyValues>(),
                StatusList = statusListTask.Result ?? new List<UnitStatusKeyValues>(),
                ProjectTaskList = dashboardTask.Result ?? new List<TaskDashboardDTO>()
            };

            // Filtering
            var list = objOutPut.ProjectTaskList ?? new List<TaskDashboardDTO>();
            if(projectId > 0)
            list = list.Where(p => p.ProjectId == projectId).ToList();

            if (priorityId > 0 && phaseId > 0 && statusId > 0)
                list = list.Where(p => p.PriorityId == priorityId && p.PhaseId == phaseId && p.StatusId == statusId).ToList();
            else if (priorityId > 0 && phaseId > 0)
                list = list.Where(p => p.PriorityId == priorityId && p.PhaseId == phaseId).ToList();
            else if (priorityId > 0 && statusId > 0)
                list = list.Where(p => p.PriorityId == priorityId && p.StatusId == statusId).ToList();
            else if (phaseId > 0 && statusId > 0)
                list = list.Where(p => p.PhaseId == phaseId && p.StatusId == statusId).ToList();
            else if (phaseId > 0)
                list = list.Where(p => p.PhaseId == phaseId).ToList();
            else if (priorityId > 0)
                list = list.Where(p => p.PriorityId == priorityId).ToList();
            else if (statusId > 0)
                list = list.Where(p => p.StatusId == statusId).ToList();

          //  phaseId = 0;

            //if (priorityId > 0 && statusId > 0)
            //    list = list.Where(p => p.PriorityId == priorityId && p.StatusId == statusId).ToList();
            //else if (priorityId > 0 && phaseId > 0)
            //    list = list.Where(p => p.PriorityId == priorityId).ToList();
            //else if (priorityId > 0 && statusId > 0)
            //    list = list.Where(p => p.PriorityId == priorityId && p.StatusId == statusId).ToList();
            //else if (statusId > 0)
            //    list = list.Where(p => p.PhaseId == phaseId && p.StatusId == statusId).ToList();
            //else if (phaseId > 0)
            //    list = list.Where(p => p.PhaseId == phaseId).ToList();
            //else if (priorityId > 0)
            //    list = list.Where(p => p.PriorityId == priorityId).ToList();
            //else if (statusId > 0)
            //    list = list.Where(p => p.StatusId == statusId).ToList();

            objOutPut.ProjectTaskList = list;

            return View("TaskDetails", objOutPut);
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

        public async Task<List<UnitPhaseKeyValues>> GetUnitPhaseList()
        {
            // DataTable dt = new DataTable();
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            List<UnitPhaseKeyValues> PriorityLst = new List<UnitPhaseKeyValues>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetUnitPhases?unitId=" + unitId);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    PriorityLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UnitPhaseKeyValues>>(data) ?? new List<UnitPhaseKeyValues>();

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

                HttpResponseMessage response = await client.GetAsync("GetUnitStatus?unitId=" + unitId + "&statusType=T");
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

        public async Task<List<UserKeyValues>> GetUsersList(int? userType)
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

        [HttpPost]
        public async Task<IActionResult> SaveTask(TasksDTO inputs, List<IFormFile> profileImageInput)
        {
            ProjectTasksDTO OutPut = new ProjectTasksDTO();
            ClientMail obj = new ClientMail();
            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (string.IsNullOrEmpty(strMenuSession))
                return RedirectToAction("Index", "Home");


            if (profileImageInput.Count > 0)
            {
                foreach (IFormFile postedFile in profileImageInput)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "TasksFile");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    //var fileName = Guid.NewGuid().ToString() + Path.GetExtension(postedFile.FileName);
                    var fileName = postedFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await postedFile.CopyToAsync(stream);
                    }
                    inputs.FileName = fileName;

                   // inputs.ProfileImage = null;

                }
            }
            else
            {
                inputs.FileName = null;
               // inputs.ProfileName = null;
            }


            AccessMasterDTO? empSession = JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession);
            OutPut.AccessList = empSession?.AccessList ?? new List<AccessMasterDTO>();
            OutPut.UserType = HttpContext.Session.GetInt32("UserType");
            inputs.IsActive = true;
            inputs.CreatedBy = HttpContext.Session.GetInt32("UserId");
            inputs.CreatedOn = DateTime.Now;
            inputs.UnitId = HttpContext.Session.GetInt32("UnitId");
            OutPut.ProjectId = inputs.ProjectId;
            inputs.EncryptedId = "";

            int? userId = HttpContext.Session.GetInt32("UserId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            if (userId == null || unitId == null)
                return RedirectToAction("Index", "Home");
            int safeUserId = userId.Value;
            int safeProjectId = inputs.ProjectId ?? 0;
            // Use composite cache keys as in TaskManagement
            string cachePrefix = $"TaskMgmt_{unitId}_{inputs.ProjectId}";
            string summeryKey = $"ProjectSummery";
            //string dashboardKey = userId != null ? $"ProjectDashboard" : string.Empty;
          //  string dashboardKey = userId != null ? $"ProjectDashboard_{projectId}" : "ProjectDashboard";
            string dashboardKey = userId != null ? $"ProjectDashboard_{inputs.ProjectId}" : "ProjectDashboard";

            //if(inputs.TaskId == 0)
            //    inputs.IsCreated = true; // Ensure TaskId is initialized for new tasks
            //  else
            //    inputs.IsCreated = false; // For updates, set to false


            // string summeryKey = userId != null ? $"ProjectSummery" : string.Empty;
            // string roleKey = (unitId != null && inputs.ProjectId != 0) ? $"ProjectRole" : string.Empty;
            // string taskDashboardKey = userId != null && unitId != null ? $"TskDashboard_{unitId}":string.Empty;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiTaskUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("SaveTask", inputs);
                    if (!response.IsSuccessStatusCode)
                        return RedirectToAction("TaskManagement", "Task");

                    if (_memoryCache is MemoryCache memCache)
                    {
                        memCache.Clear();
                    }

                    var data = await response.Content.ReadAsStringAsync();
                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);
                    OutPut.HttpStatusCode = result?.HttpStatusCode ?? 0;
                    OutPut.DisplayMessage = result?.DisplayMessage ?? string.Empty;

                    var projectListTask = GetOrSetCache(dashboardKey, async () =>
                    {
                        var dashboardList = await GetProjectDashboard(userId);
                        var result = dashboardList?.Select(x => new ProjectKeyValues { ProjectId = x.ProjectId, ProjectName = x.ProjectName }).ToList();
                        return result ?? new List<ProjectKeyValues>();
                    });
                    // Add user comments (async, fire-and-forget)
                    var commentsInputs = new UserCommentsDTO
                    {
                        TaskId = result?.RespId ?? inputs.TaskId,
                        ProjectId = inputs.ProjectId,
                        SubTaskId = 0,
                        Comments = (inputs.TaskId == 0 ? "Created a task : " : "Updated the task :") + inputs.TaskName,
                        Summary = "Task Management"
                    };
                    _ = AddUserComments(commentsInputs);


                    if (inputs.TaskId == 0)
                    {
                        OutPut.IsCreated = true; // Ensure TaskId is initialized for new tasks
                        inputs.TaskId = result?.RespId ?? inputs.TaskId; // Set TaskId from response or inputs
                    }
                    else
                    {
                        OutPut.IsCreated = false; // For updates, set to false
                    }

                    //commentsInputs.TaskId = result?.RespId ?? inputs.TaskId;
                    //commentsInputs.ProjectId = inputs.ProjectId;
                    //commentsInputs.SubTaskId = 0;
                    //commentsInputs.Comments = (inputs.TaskId == 0 ? "Created a task : " : "Updated the task :") + inputs.TaskName;
                    //commentsInputs.Summary = "Task Management";



                    //OutPut.ProjectList = await projectListTask;
                    //// Use cached lists if available, otherwise fetch and cache
                    ////OutPut.ProjectList = _memoryCache.Get<List<ProjectKeyValues>>($"{cachePrefix}_ProjectList") ?? await GetOrSetCache($"{cachePrefix}_ProjectList", async () => (await GetProject()) ?? new List<ProjectKeyValues>());
                    //OutPut.UserList = _memoryCache.Get<List<UserKeyValues>>($"{cachePrefix}_UserList") ?? await GetOrSetCache($"{cachePrefix}_UserList", async () => (await GetUsers()) ?? new List<UserKeyValues>());
                    //OutPut.VendorList = _memoryCache.Get<List<UserKeyValues>>($"{cachePrefix}_VendorList") ?? await GetOrSetCache($"{cachePrefix}_VendorList", async () => (await GetUsersList(3)) ?? new List<UserKeyValues>());
                    //OutPut.PhaseList = _memoryCache.Get<List<UnitPhaseKeyValues>>($"{cachePrefix}_PhaseList") ?? await GetOrSetCache($"{cachePrefix}_PhaseList", async () => (await GetUnitPhaseList()) ?? new List<UnitPhaseKeyValues>());
                    //OutPut.PriorityList = _memoryCache.Get<List<UnitPriorityKeyValues>>($"{cachePrefix}_PriorityList") ?? await GetOrSetCache($"{cachePrefix}_PriorityList", async () => (await GetUnitPriorityList()) ?? new List<UnitPriorityKeyValues>());
                    //OutPut.StatusList = _memoryCache.Get<List<UnitStatusKeyValues>>($"{cachePrefix}_StatusList") ?? await GetOrSetCache($"{cachePrefix}_StatusList", async () => (await GetUnitStatusList()) ?? new List<UnitStatusKeyValues>());

                    OutPut.ProjectTaskList = await GetTaskDashboard(safeUserId, safeProjectId) ?? new List<TaskDashboardDTO>();
                    OutPut.TaskRoleSummeryList = await GetTaskRoleSummery(unitId, inputs.TaskId);
                    OutPut.ProjectTaskList = OutPut.ProjectTaskList.Where(x => x.TaskId == inputs.TaskId).ToList();
                    await obj.SendTaskMail(OutPut);
                    return RedirectToAction("TaskDetails", "Task");
                   // return View("TaskDetails", OutPut);
                }
            }
            catch (SystemException)
            {
                OutPut.DisplayMessage = "Project failed!";
                return RedirectToAction("TaskDetails", "Task");
            }
        }


        public async Task<int> AddUserComments(UserCommentsDTO inputs)
        {
            try
            {
                UserNotificationsDTO objNotification = new UserNotificationsDTO();
                inputs.IsActive = true;
                inputs.UserId = HttpContext.Session.GetInt32("UserId");
                objNotification.UserId = HttpContext.Session.GetInt32("UserId");
                int? unitId = HttpContext.Session.GetInt32("UnitId");
                objNotification.ProjectId = inputs.ProjectId;
                objNotification.TaskId = inputs.TaskId;
                objNotification.SubTaskId = inputs.SubTaskId;
                objNotification.Heading = inputs.Summary;
                objNotification.NotifyMessage = inputs.Comments;
                inputs.CreationDate = DateTime.Now;

                //int safeUserId = inputs.UserId.Value;
                //int safeUnitId = unitId.Value;
                //string cachePrefix = $"TaskMgmt_{safeUnitId}_{safeUserId}";

                //string commentsKey = $"{cachePrefix}_GetComments_{inputs.TaskId}";
                //var cachedComments = _memoryCache.Get<List<CommentsDashboardDTO>>(commentsKey);

                using (HttpClient client = new HttpClient())
                {
                  //  _memoryCache.Remove(commentsKey);

                    client.BaseAddress = new Uri(EnvironmentUrl.apiCommentsUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("UsersComments", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        if (_memoryCache is MemoryCache memCache)
                        {
                            memCache.Clear();
                        }
                        var data = await response.Content.ReadAsStringAsync();
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);
                        await AddNotification(objNotification);
                    }
                }
            }
            catch (SystemException)
            {
            }
            return 1;
        }

        public async Task<int> AddNotification(UserNotificationsDTO inputs)
        {
            try
            {
                inputs.UserId = HttpContext.Session.GetInt32("UserId");             
                int? unitId = HttpContext.Session.GetInt32("UnitId");
                inputs.Base64ProfileImage = "";
                //int safeUserId = inputs.UserId.Value;
                //int safeUnitId = unitId.Value;
                //string cachePrefix = $"TaskMgmt_{safeUnitId}_{safeUserId}";
                inputs.CreationDate = DateTime.Now;

                using (HttpClient client = new HttpClient())
                {
                  //  _memoryCache.Remove($"{cachePrefix}_GetNotifications_{inputs.UserId}");

                    client.BaseAddress = new Uri(EnvironmentUrl.apiCommentsUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("SaveNotification", inputs);

                    if (response.IsSuccessStatusCode)
                    {
                        if (_memoryCache is MemoryCache memCache)
                        {
                            memCache.Clear();
                        }
                        var data = await response.Content.ReadAsStringAsync();
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<int>(data);
                    }

                }


            }
            catch (SystemException ex)
            {

            }

            return 1;
        }
        public async Task<List<ProjectKeyValues>>? GetProject()
        {
            // DataTable dt = new DataTable();
            List<ProjectKeyValues> UserLst = new List<ProjectKeyValues>();
            int? UnitId = HttpContext.Session.GetInt32("UnitId");
            int? userId = HttpContext.Session.GetInt32("UserId");
            // int countryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ecountryId));
            //  inputs.CountryId = countryId;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiProjectUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                //HttpResponseMessage response = await client.GetAsync("GetUserListing?userType=" + userType + "&logInUser="+ logInUser);
                HttpResponseMessage response = await client.GetAsync("GetProjects?unitId=" + userId);
                //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    UserLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProjectKeyValues>>(data) ?? new List<ProjectKeyValues>();

                    //foreach (var item in UserLst)
                    //{

                    //    item.EncryptedId = CommonHelper.EncryptURLHTML(item.ProjectId.ToString());
                    //    // if (item.ProfileImage != null)
                    //    //    item.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);

                    //}



                }

                return UserLst;
            }






        }

        [HttpGet]
        [Route("Task/GetTaskId/{epId}")]
        public async Task<IActionResult>? GetTaskId(string epId)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            ProjectTasksDTO objOutPut = new ProjectTasksDTO();
          
            int pId = Convert.ToInt32(CommonHelper.DecryptURLHTML(epId));
             int unitId = HttpContext.Session.GetInt32("UnitId") ?? 0;
            int? userId = HttpContext.Session.GetInt32("UserId");
            int? projectId = HttpContext.Session.GetInt32("PID");
            if (pId > 0)
            {
                string cachePrefix = $"TaskMgmt_{unitId}_{projectId}";
               
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiTaskUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("GetTaskById?pId=" + pId);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            objOutPut = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjectTasksDTO>(data) ?? new ProjectTasksDTO();
                           // string dashboardKey = userId != null ? $"ProjectDashboard" : string.Empty;

                            string dashboardKey = userId != null ? $"ProjectDashboard_{pId}" : "ProjectDashboard";

                            var projectListTask = GetOrSetCache(dashboardKey, async () =>
                            {
                                var dashboardList = await GetProjectDashboard(userId);
                                var result = dashboardList?.Select(x => new ProjectKeyValues { ProjectId = x.ProjectId, ProjectName = x.ProjectName }).ToList();
                                return result ?? new List<ProjectKeyValues>();
                            });

                           objOutPut.ProjectList = await projectListTask;
                           // objOutPut.ProjectList = _memoryCache.Get<List<ProjectKeyValues>>(dashboardKey) ?? await GetOrSetCache(dashboardKey, async () => (await GetProject()) ?? new List<ProjectKeyValues>());
                            objOutPut.UserList = _memoryCache.Get<List<UserKeyValues>>($"{cachePrefix}_UserList") ?? await GetOrSetCache($"{cachePrefix}_UserList", async () => (await GetUsers()) ?? new List<UserKeyValues>());
                            objOutPut.VendorList = _memoryCache.Get<List<UserKeyValues>>($"{cachePrefix}_VendorList") ?? await GetOrSetCache($"{cachePrefix}_VendorList", async () => (await GetUsersList(3)) ?? new List<UserKeyValues>());
                            objOutPut.PhaseList = _memoryCache.Get<List<UnitPhaseKeyValues>>($"{cachePrefix}_PhaseList") ?? await GetOrSetCache($"{cachePrefix}_PhaseList", async () => (await GetUnitPhaseList()) ?? new List<UnitPhaseKeyValues>());
                            objOutPut.PriorityList = _memoryCache.Get<List<UnitPriorityKeyValues>>($"{cachePrefix}_PriorityList") ?? await GetOrSetCache($"{cachePrefix}_PriorityList", async () => (await GetUnitPriorityList()) ?? new List<UnitPriorityKeyValues>());
                            objOutPut.StatusList = _memoryCache.Get<List<UnitStatusKeyValues>>($"{cachePrefix}_StatusList") ?? await GetOrSetCache($"{cachePrefix}_StatusList", async () => (await GetUnitStatusList()) ?? new List<UnitStatusKeyValues>());

                           // objOutPut.ProjectTaskList = await GetTaskDashboard(safeUserId, safeProjectId) ?? new List<TaskDashboardDTO>();

                            //objOutPut.ProjectList = await GetProject() ?? new List<ProjectKeyValues>();
                            //objOutPut.UserList = await GetUsers() ?? new List<UserKeyValues>();
                            //objOutPut.VendorList = await GetUsersList(3) ?? new List<UserKeyValues>();
                            //objOutPut.PhaseList = await GetUnitPhaseList() ?? new List<UnitPhaseKeyValues>();
                            //objOutPut.PriorityList = await GetUnitPriorityList() ?? new List<UnitPriorityKeyValues>();
                            //objOutPut.StatusList = await GetUnitStatusList() ?? new List<UnitStatusKeyValues>();
                            objOutPut.ProjectTaskList = await GetTaskDashboard(HttpContext.Session.GetInt32("UserId"), projectId);

                            string? strMenuSession = HttpContext.Session.GetString("Menu");
                            if (strMenuSession == null)
                                return RedirectToAction("Index", "Home");

                            AccessMasterDTO? empSession = (AccessMasterDTO?)(JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession));
                            objOutPut.AccessList = empSession.AccessList;
                            objOutPut.UserType = HttpContext.Session.GetInt32("UserType");
                            objOutPut.ProjectId = projectId;
                            objOutPut.IsView = false;
                        }

                    }

                }
                catch (SystemException ex)
                {
                    objOutPut.DisplayMessage = "Task failed!" + ex.Message;
                }

            }
            return View("TaskDetails", objOutPut);
        }

        [HttpGet]
        [Route("Task/GetTaskView/{epId}")]
        public async Task<IActionResult>? GetTaskView(string epId)
        {
            // CommentsDashboardDTO inputs = new CommentsDashboardDTO();
            ProjectTasksDTO objOutPut = new ProjectTasksDTO();
            int? userId = HttpContext.Session.GetInt32("UserId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");

             int? projectId= HttpContext.Session.GetInt32("PID"); 
            int tId = Convert.ToInt32(CommonHelper.DecryptURLHTML(epId));
            if (tId > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string? strMenuSession = HttpContext.Session.GetString("Menu");
                        if (strMenuSession == null)
                            return RedirectToAction("Index", "Home");


                        client.BaseAddress = new Uri(EnvironmentUrl.apiTaskUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("GetTaskById?pId=" + tId);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            objOutPut = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjectTasksDTO>(data) ?? new ProjectTasksDTO();
                            

                            int safeUserId = userId.Value;
                            int safeUnitId = unitId.Value;
                            string cachePrefix = $"TaskMgmt_{safeUnitId}_{projectId}";
                            // Use composite cache keys for all lists
                            //objOutPut.UserList = _memoryCache.Get<List<UserKeyValues>>($"{cachePrefix}_UserList") ?? await GetOrSetCache($"{cachePrefix}_UserList", async () => (await GetUsers()) ?? new List<UserKeyValues>());
                            //objOutPut.ProjectList = _memoryCache.Get<List<ProjectKeyValues>>($"{cachePrefix}_ProjectList") ?? await GetOrSetCache($"{cachePrefix}_ProjectList", async () => (await GetProject()) ?? new List<ProjectKeyValues>());
                            //objOutPut.VendorList = _memoryCache.Get<List<UserKeyValues>>($"{cachePrefix}_VendorList") ?? await GetOrSetCache($"{cachePrefix}_VendorList", async () => (await GetUsersList(3)) ?? new List<UserKeyValues>());
                            //objOutPut.PhaseList = _memoryCache.Get<List<UnitPhaseKeyValues>>($"{cachePrefix}_PhaseList") ?? await GetOrSetCache($"{cachePrefix}_PhaseList", async () => (await GetUnitPhaseList()) ?? new List<UnitPhaseKeyValues>());
                            //objOutPut.PriorityList = _memoryCache.Get<List<UnitPriorityKeyValues>>($"{cachePrefix}_PriorityList") ?? await GetOrSetCache($"{cachePrefix}_PriorityList", async () => (await GetUnitPriorityList()) ?? new List<UnitPriorityKeyValues>());
                            //objOutPut.StatusList = _memoryCache.Get<List<UnitStatusKeyValues>>($"{cachePrefix}_StatusList") ?? await GetOrSetCache($"{cachePrefix}_StatusList", async () => (await GetUnitStatusList()) ?? new List<UnitStatusKeyValues>());
                            //objOutPut.UserListForSubTask = _memoryCache.Get<List<UserKeyValues>>($"{cachePrefix}_SubTaskUsers") ?? await GetOrSetCache($"{cachePrefix}_SubTaskUsers", async () => (await GetSubTaskUsers(objOutPut.ProjectId, objOutPut.TaskId)) ?? new List<UserKeyValues>());
                            //objOutPut.ProjectTaskList = _memoryCache.Get<List<TaskDashboardDTO>>($"{cachePrefix}_TaskDashboard") ?? await GetOrSetCache($"{cachePrefix}_TaskDashboard", async () => (await GetTaskDashboard(objOutPut.ProjectId, objOutPut.TaskId)) ?? new List<TaskDashboardDTO>());
                            //var dashboardTask = GetOrSetCache($"{cachePrefix}_TaskDashboard_{projectId}", async () =>
                            //{
                            //    var result = await GetTaskDashboard(safeUserId, projectId);
                            //    return result ?? new List<TaskDashboardDTO>();
                            //});


                            string dashboardKey = userId != null ? $"ProjectDashboard_{projectId}" : "ProjectDashboard";

                            var projectListTask = GetOrSetCache(dashboardKey, async () =>
                            {
                                var dashboardList = await GetProjectDashboard(userId);
                                var result = dashboardList?.Select(x => new ProjectKeyValues { ProjectId = x.ProjectId, ProjectName = x.ProjectName }).ToList();
                                return result ?? new List<ProjectKeyValues>();
                            });

                            objOutPut.ProjectList = await projectListTask;
                            // objOutPut.ProjectList = _memoryCache.Get<List<ProjectKeyValues>>(dashboardKey) ?? await GetOrSetCache(dashboardKey, async () => (await GetProject()) ?? new List<ProjectKeyValues>());
                            objOutPut.UserList = _memoryCache.Get<List<UserKeyValues>>($"{cachePrefix}_UserList") ?? await GetOrSetCache($"{cachePrefix}_UserList", async () => (await GetUsers()) ?? new List<UserKeyValues>());
                            objOutPut.VendorList = _memoryCache.Get<List<UserKeyValues>>($"{cachePrefix}_VendorList") ?? await GetOrSetCache($"{cachePrefix}_VendorList", async () => (await GetUsersList(3)) ?? new List<UserKeyValues>());
                            objOutPut.PhaseList = _memoryCache.Get<List<UnitPhaseKeyValues>>($"{cachePrefix}_PhaseList") ?? await GetOrSetCache($"{cachePrefix}_PhaseList", async () => (await GetUnitPhaseList()) ?? new List<UnitPhaseKeyValues>());
                            objOutPut.PriorityList = _memoryCache.Get<List<UnitPriorityKeyValues>>($"{cachePrefix}_PriorityList") ?? await GetOrSetCache($"{cachePrefix}_PriorityList", async () => (await GetUnitPriorityList()) ?? new List<UnitPriorityKeyValues>());
                            objOutPut.StatusList = _memoryCache.Get<List<UnitStatusKeyValues>>($"{cachePrefix}_StatusList") ?? await GetOrSetCache($"{cachePrefix}_StatusList", async () => (await GetUnitStatusList()) ?? new List<UnitStatusKeyValues>());

                            // objOutPut.ProjectTaskList = await GetTaskDashboard(safeUserId, safeProjectId) ?? new List<TaskDashboardDTO>();

                            //objOutPut.ProjectList = await GetProject() ?? new List<ProjectKeyValues>();
                            //objOutPut.UserList = await GetUsers() ?? new List<UserKeyValues>();
                            //objOutPut.VendorList = await GetUsersList(3) ?? new List<UserKeyValues>();
                            //objOutPut.PhaseList = await GetUnitPhaseList() ?? new List<UnitPhaseKeyValues>();
                            //objOutPut.PriorityList = await GetUnitPriorityList() ?? new List<UnitPriorityKeyValues>();
                            //objOutPut.StatusList = await GetUnitStatusList() ?? new List<UnitStatusKeyValues>();
                            objOutPut.ProjectTaskList = await GetTaskDashboard(HttpContext.Session.GetInt32("UserId"), 0);

                            objOutPut.UserListForSubTask = _memoryCache.Get<List<UserKeyValues>>($"{cachePrefix}_SubTaskUsers") ?? new List<UserKeyValues>();

                            if (objOutPut.UserListForSubTask == null || objOutPut.UserListForSubTask.Count == 0)
                            {
                                objOutPut.UserListForSubTask = (projectId>= 0 && tId > 0)
                                                  ? (await GetSubTaskUsers(projectId, tId) ?? new List<UserKeyValues>())
                                                  : new List<UserKeyValues>();
                                _memoryCache.Set($"{cachePrefix}_SubTaskUsers", objOutPut.UserListForSubTask, TimeSpan.FromMinutes(30));
                            }

                            AccessMasterDTO? empSession = (AccessMasterDTO?)(JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession));


                           

                            string commentsKey = $"{cachePrefix}_Comments";
                            var cachedComments = _memoryCache.Get<List<CommentsDashboardDTO>>(commentsKey);
                            if (cachedComments != null && cachedComments.Any(c => c.TaskId == tId))
                            {
                                objOutPut.ActivityList = cachedComments;
                            }
                            else
                            {
                                var inputs = new CommentsDashboardDTO
                                {
                                    IsComment = false,
                                    ProjectId = 0,
                                    SubTaskId = 0,
                                    TaskId = tId
                                };

                                List<CommentsDashboardDTO> comments = new List<CommentsDashboardDTO>();
                                var result = await GetComments(inputs);
                                if (result != null)
                                    comments = result;
                                objOutPut.ActivityList = comments;
                                _memoryCache.Set(commentsKey, objOutPut.ActivityList, TimeSpan.FromMinutes(30));
                            }
                            objOutPut.AccessList = empSession.AccessList ?? new List<AccessMasterDTO>();
                            objOutPut.EncryptedId = epId;
                        }

                    }

                }
                catch (SystemException ex)
                {
                    objOutPut.DisplayMessage = "Task failed!" + ex.Message;
                }

            }
            return View("TaskDetails", objOutPut);
        }

        [HttpGet]
        [Route("Task/GetTaskView1/{epId}")]
        public async Task<IActionResult>? GetTaskView1(string epId)
        {
            ProjectTasksDTO objOutPut = new ProjectTasksDTO();
            int pId = 0;
            if (!string.IsNullOrEmpty(epId))
            {
                var decrypted = CommonHelper.DecryptURLHTML(epId);
                if (!string.IsNullOrEmpty(decrypted) && int.TryParse(decrypted, out int parsedId))
                    pId = parsedId;
            }
            if (pId <= 0)
                return RedirectToAction("Index", "Home");
            try
            {
                objOutPut.EncryptedId = epId;
                string? strMenuSession = HttpContext.Session.GetString("Menu");
                if (string.IsNullOrEmpty(strMenuSession))
                    return RedirectToAction("Index", "Home");
                AccessMasterDTO? empSession = JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession);
                int? userId = HttpContext.Session.GetInt32("UserId");
                int? unitId = HttpContext.Session.GetInt32("UnitId");
                if (!userId.HasValue || !unitId.HasValue || empSession == null)
                    return RedirectToAction("Index", "Home");
                int safeUserId = userId.Value;
                int safeUnitId = unitId.Value;
                string cachePrefix = $"TaskMgmt_{safeUnitId}_{pId}";
                // Use composite cache keys for all lists
                objOutPut.UserList = _memoryCache.Get<List<UserKeyValues>>($"{cachePrefix}_UserList") ?? await GetOrSetCache($"{cachePrefix}_UserList", async () => (await GetUsers()) ?? new List<UserKeyValues>());
                objOutPut.ProjectList = _memoryCache.Get<List<ProjectKeyValues>>($"{cachePrefix}_ProjectList") ?? await GetOrSetCache($"{cachePrefix}_ProjectList", async () => (await GetProject()) ?? new List<ProjectKeyValues>());
                objOutPut.VendorList = _memoryCache.Get<List<UserKeyValues>>($"{cachePrefix}_VendorList") ?? await GetOrSetCache($"{cachePrefix}_VendorList", async () => (await GetUsersList(3)) ?? new List<UserKeyValues>());
                objOutPut.PhaseList = _memoryCache.Get<List<UnitPhaseKeyValues>>($"{cachePrefix}_PhaseList") ?? await GetOrSetCache($"{cachePrefix}_PhaseList", async () => (await GetUnitPhaseList()) ?? new List<UnitPhaseKeyValues>());
                objOutPut.PriorityList = _memoryCache.Get<List<UnitPriorityKeyValues>>($"{cachePrefix}_PriorityList") ?? await GetOrSetCache($"{cachePrefix}_PriorityList", async () => (await GetUnitPriorityList()) ?? new List<UnitPriorityKeyValues>());
                objOutPut.StatusList = _memoryCache.Get<List<UnitStatusKeyValues>>($"{cachePrefix}_StatusList") ?? await GetOrSetCache($"{cachePrefix}_StatusList", async () => (await GetUnitStatusList()) ?? new List<UnitStatusKeyValues>());
                // Cache task details with composite key
                string taskDetailsKey = $"{cachePrefix}_TaskDetails";
                objOutPut.objTask = _memoryCache.Get<ProjectTasksDTO>(taskDetailsKey);
                if (objOutPut.objTask == null || objOutPut.objTask.TaskId != pId)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiTaskUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync($"GetTaskById?pId={pId}");
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            objOutPut.objTask = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjectTasksDTO>(data) ?? new ProjectTasksDTO();
                            _memoryCache.Set(taskDetailsKey, objOutPut.objTask, TimeSpan.FromMinutes(30));
                        }
                        else
                        {
                            objOutPut.objTask = new ProjectTasksDTO();
                        }
                    }
                }
                if (objOutPut.objTask?.EndDate.HasValue == true && objOutPut.objTask?.StartDate.HasValue == true)
                    objOutPut.Duration = (objOutPut.objTask.EndDate.Value - objOutPut.objTask.StartDate.Value).Days;
                // Cache comments with composite key
                string commentsKey = $"{cachePrefix}_Comments";
                var cachedComments = _memoryCache.Get<List<CommentsDashboardDTO>>(commentsKey);
                if (cachedComments != null && cachedComments.Any(c => c.TaskId == pId))
                {
                    objOutPut.ActivityList = cachedComments;
                }
                else
                {
                    var inputs = new CommentsDashboardDTO
                    {
                        IsComment = false,
                        ProjectId = 0,
                        SubTaskId = 0,
                        TaskId = pId
                    };

                    List<CommentsDashboardDTO> comments = new List<CommentsDashboardDTO>();
                    var result = await GetComments(inputs);
                    if (result != null)
                        comments = result;
                    objOutPut.ActivityList = comments;
                    _memoryCache.Set(commentsKey, objOutPut.ActivityList, TimeSpan.FromMinutes(30));
                }
                objOutPut.AccessList = empSession.AccessList ?? new List<AccessMasterDTO>();
                objOutPut.UserType = HttpContext.Session.GetInt32("UserType");
                objOutPut.IsView = true;
            }
            catch (SystemException ex)
            {
                objOutPut.DisplayMessage = "Task failed!" + ex.Message;
            }
            return View("TaskDetails", objOutPut);
        }

        [HttpGet]
       // [Route("Task/DeleteTask/{epId}")]
        public async Task<bool>? DeleteTask(string epId)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            ProjectTasksDTO OutPut = new ProjectTasksDTO();

            int pId = Convert.ToInt32(CommonHelper.DecryptURLHTML(epId));
            int? userType = HttpContext.Session.GetInt32("UserType");
            int? userId = HttpContext.Session.GetInt32("UserId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            int? projectId = HttpContext.Session.GetInt32("PID");
            int? priorityId = HttpContext.Session.GetInt32("priorityId");
            int? statusId = HttpContext.Session.GetInt32("statusId");

            string? strMenuSession = HttpContext.Session.GetString("Menu");
            string cachePrefix = $"TaskMgmt_{unitId}_{pId}";

            string dashboardKey = userId != null ? $"ProjectDashboard" : string.Empty;
            string summeryKey = userId != null ? $"ProjectSummery" : string.Empty;
            string roleKey = (unitId != null && projectId != 0) ? $"ProjectRole" : string.Empty;
            string taskKey = $"{cachePrefix}_TaskDashboard";

            if (pId > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiTaskUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("DeleteTask?pId=" + pId);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                            OutPut.HttpStatusCode = result.HttpStatusCode;
                            OutPut.DisplayMessage = result.DisplayMessage;

                            if (_memoryCache is MemoryCache memCache)
                            {
                                memCache.Clear();
                            }

                          
                            // OutPut.ProjectList = await GetProject();
                        }
                        //AccessMasterDTO? empSession = JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession);
                        //// OutPut.AccessList = empSession?.AccessList ?? new List<AccessMasterDTO>();
                        //int safeUserId = userId.Value;
                        //var userListTask = GetOrSetCache($"{cachePrefix}_UserList", async () => (await GetUsers()) ?? new List<UserKeyValues>());
                        //var projectListTask = GetOrSetCache($"{cachePrefix}_ProjectList", async () => (await GetProject()) ?? new List<ProjectKeyValues>());
                        //var vendorListTask = GetOrSetCache($"{cachePrefix}_VendorList", async () => (await GetUsersList(3)) ?? new List<UserKeyValues>());
                        //var phaseListTask = GetOrSetCache($"{cachePrefix}_PhaseList", async () => (await GetUnitPhaseList()) ?? new List<UnitPhaseKeyValues>());
                        //var priorityListTask = GetOrSetCache($"{cachePrefix}_PriorityList", async () => (await GetUnitPriorityList()) ?? new List<UnitPriorityKeyValues>());
                        //var statusListTask = GetOrSetCache($"{cachePrefix}_StatusList", async () => (await GetUnitStatusList()) ?? new List<UnitStatusKeyValues>());
                        //var dashboardTask = GetOrSetCache($"{cachePrefix}_TaskDashboard", async () =>
                        //{
                        //    var result = await GetTaskDashboard(safeUserId, projectId);
                        //    return result ?? new List<TaskDashboardDTO>();
                        //});

                        //await Task.WhenAll(userListTask, projectListTask, vendorListTask, phaseListTask, priorityListTask, statusListTask, dashboardTask);

                        //var objOutPut = new ProjectTasksDTO
                        //{
                        //    AccessList = empSession.AccessList ?? new List<AccessMasterDTO>(),
                        //    ProjectId = projectId,
                        //    //  PhaseId = phaseId,
                        //    PriorityId = priorityId,
                        //    StatusId = statusId,
                        //    UserType = userType,
                        //    UserList = userListTask.Result ?? new List<UserKeyValues>(),
                        //    ProjectList = projectListTask.Result ?? new List<ProjectKeyValues>(),
                        //    VendorList = vendorListTask.Result ?? new List<UserKeyValues>(),
                        //    PhaseList = phaseListTask.Result ?? new List<UnitPhaseKeyValues>(),
                        //    PriorityList = priorityListTask.Result ?? new List<UnitPriorityKeyValues>(),
                        //    StatusList = statusListTask.Result ?? new List<UnitStatusKeyValues>(),
                        //    ProjectTaskList = dashboardTask.Result ?? new List<TaskDashboardDTO>()
                        //};

                        //// Filtering
                        //var list = objOutPut.ProjectTaskList ?? new List<TaskDashboardDTO>();


                        //int phaseId = 0;

                        //if (priorityId > 0 && statusId > 0)
                        //    list = list.Where(p => p.PriorityId == priorityId && p.StatusId == statusId).ToList();

                        //else if (priorityId > 0)
                        //    list = list.Where(p => p.PriorityId == priorityId).ToList();
                        //else if (statusId > 0)
                        //    list = list.Where(p => p.StatusId == statusId).ToList();

                        //objOutPut.ProjectTaskList = list;

                        return true;
                    }

                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "Clinic failed!" + ex.Message;
                    return false;
                }

            }
            return true;
        }

        public async Task<List<TaskDashboardDTO>>? GetTaskDashboard(int? unitId, int? projectId)
        {
            // DataTable dt = new DataTable();
            int isVandor = 0;
            List<TaskDashboardDTO> ProjectLst = new List<TaskDashboardDTO>();

            // int countryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ecountryId));
            //  inputs.CountryId = countryId;
            int? vendor = HttpContext.Session.GetInt32("UserType");
            if (vendor == 3)
                isVandor = 1;
            else
                isVandor = 0;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiTaskUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                //HttpResponseMessage response = await client.GetAsync("GetUserListing?userType=" + userType + "&logInUser="+ logInUser);
                HttpResponseMessage response = await client.GetAsync("GetTaskDashboard?unitId=" + unitId + "&projectId=" + projectId + "&IsVandor=" + isVandor);
                //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    ProjectLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TaskDashboardDTO>>(data) ?? new List<TaskDashboardDTO>();

                    foreach (var item in ProjectLst)
                    {

                        item.eTaskId = CommonHelper.EncryptURLHTML(item.TaskId.ToString());

                    }



                }

                return ProjectLst;
            }






        }

        public async Task<IActionResult> ProjectDocuments()
        {
            ProjectTasksDTO objOutPut = new ProjectTasksDTO();


            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (strMenuSession == null)
                return RedirectToAction("Index", "Home");
            //AccessMasterDTO? empSession = (AccessMasterDTO?)(JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession));

            //objOutPut.AccessList = empSession.AccessList.Where(p => p.ModuleCode.Trim() == "T").ToList();


            //objOutPut.UserList = await GetUsers();
            //objOutPut.ProjectList = await GetProject();
            //objOutPut.VendorList = await GetUsersList(3);
            //objOutPut.PhaseList = await GetUnitPhaseList();
            //objOutPut.PriorityList = await GetUnitPriorityList();
            //objOutPut.StatusList = await GetUnitStatusList();
            //objOutPut.ProjectTaskList = await GetTaskDashboard(HttpContext.Session.GetInt32("UserId"), 0);
            return View(objOutPut);
        }

        public async Task<IActionResult> ProjectDrawings()
        {
            ProjectTasksDTO objOutPut = new ProjectTasksDTO();


            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (strMenuSession == null)
                return RedirectToAction("Index", "Home");

            //AccessMasterDTO? empSession = (AccessMasterDTO?)(JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession));

            //objOutPut.AccessList = empSession.AccessList.Where(p => p.ModuleCode.Trim() == "T").ToList();


            //objOutPut.UserList = await GetUsers();
            //objOutPut.ProjectList = await GetProject();
            //objOutPut.VendorList = await GetUsersList(3);
            //objOutPut.PhaseList = await GetUnitPhaseList();
            //objOutPut.PriorityList = await GetUnitPriorityList();
            //objOutPut.StatusList = await GetUnitStatusList();
            //objOutPut.ProjectTaskList = await GetTaskDashboard(HttpContext.Session.GetInt32("UserId"), 0);
            return View(objOutPut);
        }

        public async Task<IActionResult> ProjectPhotos()
        {
            ProjectTasksDTO objOutPut = new ProjectTasksDTO();


            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (strMenuSession == null)
                return RedirectToAction("Index", "Home");

            //AccessMasterDTO? empSession = (AccessMasterDTO?)(JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession));

            //objOutPut.AccessList = empSession.AccessList.Where(p => p.ModuleCode.Trim() == "T").ToList();


            //objOutPut.UserList = await GetUsers();
            //objOutPut.ProjectList = await GetProject();
            //objOutPut.VendorList = await GetUsersList(3);
            //objOutPut.PhaseList = await GetUnitPhaseList();
            //objOutPut.PriorityList = await GetUnitPriorityList();
            //objOutPut.StatusList = await GetUnitStatusList();
            //objOutPut.ProjectTaskList = await GetTaskDashboard(HttpContext.Session.GetInt32("UserId"), 0);
            return View(objOutPut);
        }

        [HttpGet]
        [Route("Task/ProjectDocuments/{pId}")]
        public async Task<IActionResult> ProjectDocuments(int? pid)
        {
            ProjectTasksDTO objOutPut = new ProjectTasksDTO();


            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (strMenuSession == null)
                return RedirectToAction("Index", "Home");

            AccessMasterDTO? empSession = (AccessMasterDTO?)(JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession));
            objOutPut.AccessList = empSession.AccessList;
            return View(objOutPut);
        }

        [HttpGet]
        [Route("Task/ProjectDrawings/{pId}")]
        public async Task<IActionResult> ProjectDrawings(int? pid)
        {
            ProjectTasksDTO objOutPut = new ProjectTasksDTO();

            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (strMenuSession == null)
                return RedirectToAction("Index", "Home");

            AccessMasterDTO? empSession = (AccessMasterDTO?)(JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession));
            objOutPut.AccessList = empSession.AccessList;

            return View(objOutPut);
        }

        [HttpGet]
        [Route("Task/ProjectPhotos/{pId}")]
        public async Task<IActionResult> ProjectPhotos(int? pid)
        {
            ProjectTasksDTO objOutPut = new ProjectTasksDTO();

            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (strMenuSession == null)
                return RedirectToAction("Index", "Home");

            AccessMasterDTO? empSession = (AccessMasterDTO?)(JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession));
            objOutPut.AccessList = empSession.AccessList;

            return View(objOutPut);
        }

        public async Task<IActionResult> TaskKanban()
        {
            return View();
        }

        [HttpPost]
        public async Task<UserCommentsDTO>? SaveComments(UserCommentsDTO inputs)
        {
            UserNotificationsDTO objNotification = new UserNotificationsDTO();
            UserCommentsDTO OutPut = new UserCommentsDTO();
            ProjectTasksDTO objOutput = new ProjectTasksDTO();
            inputs.IsActive = true;
            inputs.UserId = HttpContext.Session.GetInt32("UserId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            objNotification.UserId = HttpContext.Session.GetInt32("UserId");
            objNotification.ProjectId = inputs.ProjectId;
            objNotification.TaskId = inputs.TaskId;
            objNotification.SubTaskId = inputs.SubTaskId;
            objNotification.Heading = inputs.Summary;
            objNotification.NotifyMessage = inputs.Comments;
            inputs.CreationDate = DateTime.Now;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiCommentsUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("UsersComments", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        if (_memoryCache is MemoryCache memCache)
                        {
                            memCache.Clear();
                        }
                        //  _memoryCache.Remove("Notifications");
                        var data = await response.Content.ReadAsStringAsync();

                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);
                        OutPut.HttpStatusCode = result.HttpStatusCode;
                        OutPut.DisplayMessage = result.DisplayMessage;
                        if (result.HttpStatusCode == 200)
                        {
                            //  ProjectTasksDTO objOutPut = new ProjectTasksDTO();
                            ClientMail obj = new ClientMail();


                            objOutput.ActivityList = await GetCommentsDetails(inputs.ProjectId, inputs.TaskId);
                            objOutput.TaskRoleSummeryList = await GetTaskRoleSummery(unitId, inputs.TaskId);
                            objOutput.IsCreated = true;
                            objOutput.Description = inputs.Comments;
                            objOutput.CreatedOn = inputs.CreationDate;
                            //  objOutput.
                            AddNotification(objNotification);
                            //  ClientMail objTask = new ClientMail();
                            // await objTask.SendCommentsMail(objOutput);
                        }
                        return OutPut;

                    }

                }


            }
            catch (SystemException ex)
            {
                OutPut.DisplayMessage = "Project failed!" + ex.Message;
            }
            return OutPut;

        }


        [HttpPost]
        public async Task<List<CommentsDashboardDTO>>? GetComments(CommentsDashboardDTO inputs)
        {
            // UserCommentsDTO inputs = new UserCommentsDTO();
            List<CommentsDashboardDTO> CommentsLst = new List<CommentsDashboardDTO>();
            inputs.IsActive = true;
            inputs.UserId = HttpContext.Session.GetInt32("UserId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");

            int safeUserId = inputs.UserId.Value;
            int safeUnitId = unitId.Value;
            string cachePrefix = $"TaskMgmt_{safeUnitId}";

            string commentsKey = $"{cachePrefix}_GetComments";
            var cachedComments = _memoryCache.Get<List<CommentsDashboardDTO>>(commentsKey);
            if (cachedComments != null)
            {
                CommentsLst = cachedComments;
                return CommentsLst;
            }
            else
            {
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

                            CommentsLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CommentsDashboardDTO>>(data) ?? new List<CommentsDashboardDTO>();

                            _memoryCache.Set(commentsKey, CommentsLst, TimeSpan.FromMinutes(30));
                            return CommentsLst;

                        }

                    }


                }
                catch (SystemException ex)
                {

                }
            }
            return null;

        }


        [HttpPost]
        public async Task<UserCommentsDTO>? DeleteComments(UserCommentsDTO inputs)
        {
            // UserCommentsDTO inputs = new UserCommentsDTO();
            UserCommentsDTO OutPut = new UserCommentsDTO();
            inputs.IsActive = true;
            inputs.UserId = HttpContext.Session.GetInt32("UserId");

            // inputs.Comments = comments;
            // inputs.IsAdmin = false;
            inputs.CreationDate = DateTime.Now;

            inputs.UserId = HttpContext.Session.GetInt32("UserId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");

            int safeUserId = inputs.UserId.Value;
            int safeUnitId = unitId.Value;
         //   string cachePrefix = $"TaskMgmt_{safeUnitId}";
           // string commentsKey = $"{cachePrefix}_GetComments";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiCommentsUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("DeleteComments", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        if (_memoryCache is MemoryCache memCache)
                        {
                            memCache.Clear();
                        }
                        var data = await response.Content.ReadAsStringAsync();

                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);
                        OutPut.HttpStatusCode = result.HttpStatusCode;
                        OutPut.DisplayMessage = result.DisplayMessage;
                        return OutPut;

                    }

                }


            }
            catch (SystemException ex)
            {
                OutPut.DisplayMessage = "Project failed!" + ex.Message;
            }
            return OutPut;

        }

        [HttpGet]
        // [Route("Task/GetCommentById/{epId}")]
        public async Task<UserCommentsDTO>? GetCommentById(int? Id)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            UserCommentsDTO objOutPut = new UserCommentsDTO();

            //  int pId = Convert.ToInt32(CommonHelper.DecryptURLHTML(epId));
            if (Id > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiCommentsUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("GetCommentsById?Id=" + Id);

                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            objOutPut = Newtonsoft.Json.JsonConvert.DeserializeObject<UserCommentsDTO>(data) ?? new UserCommentsDTO();

                        }

                    }

                }
                catch (SystemException ex)
                {
                    objOutPut.DisplayMessage = "Task failed!" + ex.Message;
                }

            }
            return objOutPut;
        }

        public async Task<List<TaskDashboardDTO>>? GetTaskDetailsForMail(int? taskId)
        {
            // DataTable dt = new DataTable();
            int isVandor = 0;
            List<TaskDashboardDTO> ProjectLst = new List<TaskDashboardDTO>();


            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiTaskUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetTaskDetailsForTemplate?taskId=" + taskId);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    ProjectLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TaskDashboardDTO>>(data) ?? new List<TaskDashboardDTO>();

                    foreach (var item in ProjectLst)
                    {
                        item.eTaskId = CommonHelper.EncryptURLHTML(item.TaskId.ToString());
                    }
                }

                return ProjectLst;
            }






        }


        public async Task<List<ProjectRoleSummeryDTO>>? GetTaskRoleSummery(int? unitId, int? taskId)
        {
            // DataTable dt = new DataTable();
            List<ProjectRoleSummeryDTO> ProjectLst = new List<ProjectRoleSummeryDTO>();

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiTaskUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("GetTaskRoleSummery?unitId=" + unitId + "&taskId=" + taskId);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    ProjectLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProjectRoleSummeryDTO>>(data) ?? new List<ProjectRoleSummeryDTO>();

                }
                foreach (var item in ProjectLst)
                {


                    if (item.ProfilePic != null)
                        item.Base64ProfileImage = item.ProfilePic; //"data:image/png;base64," + Convert.ToBase64String(item.ProfilePic, 0, item.ProfilePic.Length);

                }

                return ProjectLst;
            }

        }

        public async Task<List<CommentsDashboardDTO>>? GetCommentsDetails(int? projectId, int? taskId)
        {
            // DataTable dt = new DataTable();
            List<CommentsDashboardDTO> ProjectLst = new List<CommentsDashboardDTO>();
            int? UserId = HttpContext.Session.GetInt32("UserId");

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiCommentsUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("GetCommentsDetails?ProjectId=" + projectId + "&TaskId=" + taskId + "&UserId=" + UserId);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    ProjectLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CommentsDashboardDTO>>(data) ?? new List<CommentsDashboardDTO>();

                }
                foreach (var item in ProjectLst)
                {
                    item.eTaskId = CommonHelper.EncryptURLHTML(item.TaskId.ToString());
                    //if (item.ProfileName != null)
                    //    item.ProfileName = item.ProfileName;  //"data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);
                }

                return ProjectLst;
            }

        }


        public async Task<List<UserKeyValues>> GetSubTaskUsers(int? projectId, int taskId)
        {
            // DataTable dt = new DataTable();
            int? userId = HttpContext.Session.GetInt32("UserId");
            List<UserKeyValues> PriorityLst = new List<UserKeyValues>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiSubTaskUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetSubTaskUsers?userId=" + userId + "&taskId=" + taskId + "&projectId=" + projectId);
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


        public async Task<List<SubTaskDashboardDTO>>? GetSubTaskDashboard(int? taskId)
        {
            // DataTable dt = new DataTable();

            List<SubTaskDashboardDTO> ProjectLst = new List<SubTaskDashboardDTO>();

            // int countryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ecountryId));
            //  inputs.CountryId = countryId;
            int? userId = HttpContext.Session.GetInt32("UserId");


            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiSubTaskUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                //HttpResponseMessage response = await client.GetAsync("GetUserListing?userType=" + userType + "&logInUser="+ logInUser);
                HttpResponseMessage response = await client.GetAsync("GetSubTaskDashboard?userId=" + userId + "&taskId=" + taskId);
                //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    ProjectLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SubTaskDashboardDTO>>(data) ?? new List<SubTaskDashboardDTO>();

                    foreach (var item in ProjectLst)
                    {

                        item.eSubTaskId = CommonHelper.EncryptURLHTML(item.SubTaskId.ToString());

                    }



                }

                return ProjectLst;
            }






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
                HttpResponseMessage response = await client.GetAsync("GetProjectDashboard?unitId=" + unitId);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    ProjectLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProjectDashboardDTO>>(data) ?? new List<ProjectDashboardDTO>();
                    foreach (var item in ProjectLst ?? new List<ProjectDashboardDTO>())
                    {
                        item.EncProjectId = CommonHelper.EncryptURLHTML(item.ProjectId.ToString());
                    }
                }
                return ProjectLst;
            }






        }

        public IActionResult Download(string fileName)
        {

            string folderpath = Path.Combine(this._environment.WebRootPath, "TasksFile");

            string[] paths = fileName.Split('/').Select(a => a.Trim()).ToArray();
            var filePath = Path.Combine(folderpath,paths[2]);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            var contentType = GetContentType(filePath);
            var fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, contentType, paths[2]);
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