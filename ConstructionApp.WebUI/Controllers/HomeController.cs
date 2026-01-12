using Construction.Infrastructure.Helper;

using Construction.Infrastructure.Models;
using ConstructionApp.WebUI.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Diagnostics;
using ConstructionApp.WebUI.Helper;
using Construction.Infrastructure.KeyValues;
using Microsoft.Extensions.Caching.Memory;
using System;
using Microsoft.AspNetCore.Authentication;

namespace ConstructionApp.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
      //  private readonly ClientMail _clientMail;

        public HomeController(ILogger<HomeController> logger, IMemoryCache memoryCache, IWebHostEnvironment environment, IHttpClientFactory httpClientFactory)
        {
            _environment = environment;
            _memoryCache = memoryCache;
            _logger = logger;
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
           // _clientMail = clientMail;
        }
        public async Task<IActionResult>? Index()
        {
            //Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            //Response.Headers["Pragma"] = "no-cache";
            //Response.Headers["Expires"] = "0";
           //ClientMail objEmail = new ClientMail();
           //await objEmail.MailSent("mh.asif@hotmail.com", "Testing", "Subject");

                UsersMasterDTO objInputs = new UsersMasterDTO();
            ViewBag.EnvironmentUrl = EnvironmentUrl.apiUrl;
            HttpContext.Session.Clear();
            Response.Cookies.Delete(".AspNetCore.Session");            
            await HttpContext.SignOutAsync();
            return View(objInputs);
        }
        public async Task<IActionResult>? ErrorMessage()        {
          
            HttpContext.Session.Clear();
            return View();
        }
        
        public async Task<IActionResult>? ForgotPassword()
        {
            //var uId = HttpContext.Session.GetString("UserId");
            //if (uId == null)
            //    return RedirectToAction("Index", "Home");

            UsersMasterDTO objInputs = new UsersMasterDTO();
            ViewBag.EnvironmentUrl = EnvironmentUrl.apiUrl;
            return View(objInputs);
        }
        [HttpPost]
       // [Route("ForgotPassword")]
      // [Produces("application/json", Type = typeof(UsersMasterDTO))]
        public async Task<OutPutResponse>? ForgotPassword(UserLogin inputs)
        {
            UsersMasterDTO OutPut = new UsersMasterDTO();
            OutPutResponse OutPutResponse = new OutPutResponse();
         
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiUserUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("ForgotPassword", inputs);
                    if (response.IsSuccessStatusCode)
                    {

                        var data = await response.Content.ReadAsStringAsync();

                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<UsersMasterDTO>(data);
                        OutPut.HttpStatusCode = result.HttpStatusCode;
                        OutPut.DisplayMessage = result.DisplayMessage;
                        OutPut.EmailAddress = inputs.EmailAddress;
                        OutPut.FullName = (result.FullName ==null?result.CompanyName:result.FullName);
                        OutPut.LoginPassword = CommonHelper.Decrypt(result.LoginPassword);
                        if (OutPut.HttpStatusCode == 200)
                        {
                           ClientMail objTask = new ClientMail();

                            // OutPut.EmailAddress = HttpContext.Session.GetString("EmailAddress");
                          await objTask.ForgotPasswordMail(OutPut);

                            OutPutResponse.DisplayMessage = "Password has been sent your registered mail.";

                        }
                        else
                        {
                            // Handle error case
                            OutPutResponse.DisplayMessage = "Failed to forgot password. Please try again.";
                        }
                        // if (OutPut.HttpStatusCode != 200)
                      
                        // else
                        //  return RedirectToAction("Index", "Home");

                    }
                    else
                    {
                        // Handle the case where the response is not successful
                        OutPutResponse.DisplayMessage = "Failed to forgot password. Please try again.";
                    }

                }
               
            }
            catch (SystemException ex)
            {
                OutPutResponse.DisplayMessage = "User failed!" + ex.Message;
            }
            return OutPutResponse;
            //  return RedirectToAction("ChangePassword", "User");
        }
public async Task<IActionResult> Dashboard()
{
    // Prevent browser caching of dashboard page
    Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
    Response.Headers["Pragma"] = "no-cache";
    Response.Headers["Expires"] = "0";

    // Block access if session is missing (after logout or back navigation)
    if (HttpContext.Session.GetInt32("UserId") == null)
        return RedirectToAction("Index", "Home");
            ProjectsDashboardDTO objOutput = new ProjectsDashboardDTO();
            int? userId = HttpContext.Session.GetInt32("UserId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            int isAdmin = HttpContext.Session.GetInt32("IsAdmin") ?? 0;
            var uId = HttpContext.Session.GetString("UserId");

          string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (uId == null)
                return RedirectToAction("Index", "Home");

            string projectDashboardKey = string.Empty, taskDashboardKey = string.Empty;
            if (isAdmin == 1)
            {
                 projectDashboardKey = userId != null && unitId != null ? $"Dashboard_{unitId}" : "Dashboard_Global";
                 taskDashboardKey = userId != null && unitId != null ? $"TskDashboard_{unitId}" : "TskDashboard_Global";
            }
            else
            {
                projectDashboardKey = userId != null && unitId != null ? $"Dashboard_{unitId}_{userId}" : "Dashboard_Global";
                taskDashboardKey = userId != null && unitId != null ? $"TskDashboard_{unitId}_{userId}" : "TskDashboard_Global";
            }
            if (_memoryCache.TryGetValue(projectDashboardKey, out var cachedDashboardObj) && cachedDashboardObj is List<ProjectsDashboardDTO> cachedDashboard && cachedDashboard != null)
            {
                objOutput.DashboardList = cachedDashboard;
            }
            else
            {
                var dashboardList = await ProjectDashboard();
                if (dashboardList == null)
                    dashboardList = new List<ProjectsDashboardDTO>();
                objOutput.DashboardList = dashboardList;
                _memoryCache.Set(projectDashboardKey, objOutput.DashboardList, TimeSpan.FromMinutes(30));
            }

            if (_memoryCache.TryGetValue(taskDashboardKey, out var cachedTaskDashboardObj) && cachedTaskDashboardObj is List<TasksDashboardDTO> cachedTaskDashboard && cachedTaskDashboard != null)
            {
                objOutput.TaskList = cachedTaskDashboard;
            }
            else
            {
                var taskListResult = await TaskDashboard();
                var taskList = taskListResult ?? new List<TasksDashboardDTO>();
                objOutput.TaskList = taskList;
                _memoryCache.Set(taskDashboardKey, objOutput.TaskList, TimeSpan.FromMinutes(30));
            }

            return View(objOutput);
        }

public async Task<IActionResult> ClientDashboard()
{
    // Prevent browser caching of client dashboard page
    Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
    Response.Headers["Pragma"] = "no-cache";
    Response.Headers["Expires"] = "0";

    // Block access if session is missing (after logout or back navigation)
    if (HttpContext.Session.GetInt32("UserId") == null)
        return RedirectToAction("Index", "Home");
            ProjectsDashboardDTO objOutput = new ProjectsDashboardDTO();
            int? userId = HttpContext.Session.GetInt32("UserId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            string projectDashboardKey = userId != null && unitId != null ? $"Dashboard_{unitId}_{userId}" : "Dashboard_Global";
            string taskDashboardKey = userId != null && unitId != null ? $"TskDashboard_{unitId}_{userId}" : "TskDashboard_Global";

            var uId = HttpContext.Session.GetString("UserId");

            //string? strMenuSession = HttpContext.Session.GetString("Menu");
            //if (strMenuSession == null || uId == null)
            //    return RedirectToAction("Index", "Home");

            if (_memoryCache.TryGetValue(projectDashboardKey, out var cachedDashboardObj) && cachedDashboardObj is List<ProjectsDashboardDTO> cachedDashboard && cachedDashboard != null)
            {
                objOutput.DashboardList = cachedDashboard;
            }
            else
            {
                var dashboardList = await ProjectDashboard();
                if (dashboardList == null)
                    dashboardList = new List<ProjectsDashboardDTO>();
                objOutput.DashboardList = dashboardList;
                _memoryCache.Set(projectDashboardKey, objOutput.DashboardList, TimeSpan.FromMinutes(30));
            }

            if (_memoryCache.TryGetValue(taskDashboardKey, out var cachedTaskDashboardObj) && cachedTaskDashboardObj is List<TasksDashboardDTO> cachedTaskDashboard && cachedTaskDashboard != null)
            {
                objOutput.TaskList = cachedTaskDashboard;
            }
            else
            {
                var taskListResult = await TaskDashboard();
                var taskList = taskListResult ?? new List<TasksDashboardDTO>();
                objOutput.TaskList = taskList;
                _memoryCache.Set(taskDashboardKey, objOutput.TaskList, TimeSpan.FromMinutes(30));
            }

            return View(objOutput);
        }

        [HttpPost]
        public async Task<IActionResult>? GetUserLogin(UsersMasterDTO inPut)
        {
            inPut.LoginPassword = CommonHelper.Encrypt(inPut.LoginPassword);
            inPut.Base64ProfileImage = "";
            UsersMasterDTO UserDetails = new UsersMasterDTO();
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(EnvironmentUrl.apiUserUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.PostAsJsonAsync("GetUserLogin", inPut);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                UserDetails = Newtonsoft.Json.JsonConvert.DeserializeObject<UsersMasterDTO>(data);
                if (UserDetails != null)
                {
                    HttpContext.Session.SetString("FullName", UserDetails.FullName);
                    HttpContext.Session.SetInt32("UserId", UserDetails.UserId);
                    HttpContext.Session.SetString("EncUserId", CommonHelper.EncryptURLHTML(UserDetails.UserId.ToString()));
                    HttpContext.Session.SetInt32("UserType", (int)UserDetails.UserType);
                    HttpContext.Session.SetInt32("IsAdmin", (UserDetails.IsAdmin == true ? 1 : 0));
                    if (UserDetails.UserType == 0)
                        HttpContext.Session.SetInt32("UnitId", (int)UserDetails.UserId);
                    else
                        HttpContext.Session.SetInt32("UnitId", (int)UserDetails.UnitId);
                    HttpContext.Session.SetInt32("RoleId", (int)UserDetails.RoleId);
                    HttpContext.Session.SetInt32("DeptId", (int)UserDetails.DepartmentId);
                    HttpContext.Session.SetInt32("DesiId", (int)UserDetails.JobTitleId);
                    if (UserDetails.ProfileName != null)
                    {
                        UserDetails.ProfileName = Path.Combine("\\Profile", UserDetails.ProfileName);
                        HttpContext.Session.SetString("Logo", UserDetails.ProfileName);
                    }
                    else
                        HttpContext.Session.SetString("Logo", "");
                    _memoryCache.Remove("Menu");
                    UserDetails.HttpStatusCode = 200;
                    return RedirectToAction("Dashboard", "Home");
                }
                else
                {
                    UsersMasterDTO UserResponse = new UsersMasterDTO();
                    UserResponse.DisplayMessage = "Wrong credentials";
                    UserResponse.HttpStatusCode = 201;
                    return View("Index", UserResponse);
                }
            }
            return RedirectToAction("UserDashboard", "User");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult>? AccessMaster()
        {

            var uId = HttpContext.Session.GetString("UserId");         
            if (uId == null)
                return RedirectToAction("Index", "Home");

            AccessMasterDTO objOutPut = new AccessMasterDTO();
            objOutPut.AccessList = await GetAccess(0, 0, 0);
            objOutPut.AccessClientList = await GetClientAccess(true);
            objOutPut.AccessVendorList = await GetVendorAccess(false);
            objOutPut.RoleMasterList = await GetRoleList();
           // objOutPut.JobTitleMasterList = await GetDesignationList();
            objOutPut.DepartmentMasterList = await GetDepartmentList();
            return View(objOutPut);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<List<RoleMasterDTO>> GetRoleList()
        {
            DataTable dt = new DataTable();

            List<RoleMasterDTO> RoleLst = new List<RoleMasterDTO>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetRole");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    dt = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Data.DataTable>(data);
                }
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            RoleMasterDTO objDTO = new RoleMasterDTO();

                            objDTO.RoleId = Convert.ToInt32(dr["RoleId"]);
                            objDTO.RoleName = Convert.ToString(dr["RoleName"]);
                            objDTO.EncryptedRoleId = CommonHelper.EncryptURLHTML(objDTO.RoleId.ToString());
                            RoleLst.Add(objDTO);
                        }
                    }
                }


            }
            return RoleLst;
        }

        public async Task<List<JobTitleMasterDTO>> GetDesignationList()
        {
            DataTable dt = new DataTable();

            List<JobTitleMasterDTO> DesigLst = new List<JobTitleMasterDTO>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetJobTitle");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    dt = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Data.DataTable>(data);
                }
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            JobTitleMasterDTO objDTO = new JobTitleMasterDTO();

                            objDTO.JobTitleId = Convert.ToInt32(dr["JobTitleId"]);
                            objDTO.JobTitle = Convert.ToString(dr["JobTitle"]);
                            objDTO.EncryptedJobTitleId = CommonHelper.EncryptURLHTML(objDTO.JobTitleId.ToString());
                            DesigLst.Add(objDTO);
                        }
                    }
                }


            }
            return DesigLst;
        }

        public async Task<List<DepartmentMasterDTO>> GetDepartmentList()
        {
            DataTable dt = new DataTable();

            List<DepartmentMasterDTO> DeptLst = new List<DepartmentMasterDTO>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetDepartment");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    dt = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Data.DataTable>(data);
                }
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            DepartmentMasterDTO objDTO = new DepartmentMasterDTO();

                            objDTO.DepartmentId = Convert.ToInt32(dr["DepartmentId"]);
                            objDTO.DepartmentCode = Convert.ToString(dr["DepartmentCode"]);
                            objDTO.DepartmentName = Convert.ToString(dr["DepartmentName"]);
                            objDTO.EncryptedDepartmentId = CommonHelper.EncryptURLHTML(objDTO.DepartmentId.ToString());
                            DeptLst.Add(objDTO);
                        }
                    }
                }

            }
            return DeptLst;
        }

        [HttpGet]
        public async Task<List<AccessMasterDTO>> GetAccess(int? roleId, int? DepartmentId, int? designationId)
        {
            //  DataTable dt = new DataTable();
            List<AccessMasterDTO> UserLst = new List<AccessMasterDTO>();
            // AccessMasterDTO objOutPut = new AccessMasterDTO();

            int? unitId = HttpContext.Session.GetInt32("UnitId");
            // int countryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ecountryId));
            //  inputs.CountryId = countryId;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                //HttpResponseMessage response = await client.GetAsync("GetUserListing?userType=" + userType + "&logInUser="+ logInUser);
                HttpResponseMessage response = await client.GetAsync("GetAccessDetails?unitId=" + unitId + "&roleId=" + roleId + "&departmentId=" + DepartmentId + "&designationId=" + designationId);
                //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    UserLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AccessMasterDTO>>(data);

                }

                return UserLst;
            }






        }

        [HttpGet]
        public async Task<List<AccessClientMasterDTO>> GetClientAccess(bool? IsClient)
        {
            //  DataTable dt = new DataTable();
            List<AccessClientMasterDTO> UserLst = new List<AccessClientMasterDTO>();
            // AccessMasterDTO objOutPut = new AccessMasterDTO();

            int? unitId = HttpContext.Session.GetInt32("UnitId");
            // int countryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ecountryId));
            //  inputs.CountryId = countryId;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                //HttpResponseMessage response = await client.GetAsync("GetUserListing?userType=" + userType + "&logInUser="+ logInUser);
                HttpResponseMessage response = await client.GetAsync("GetClientAccessDetails?unitId=" + unitId + "&isClient=" + IsClient);
                //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    UserLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AccessClientMasterDTO>>(data);

                }

                return UserLst;
            }






        }

        [HttpGet]
        public async Task<List<AccessClientMasterDTO>> GetVendorAccess(bool? IsClient)
        {
            //  DataTable dt = new DataTable();
            List<AccessClientMasterDTO> UserLst = new List<AccessClientMasterDTO>();
            // AccessMasterDTO objOutPut = new AccessMasterDTO();

            int? unitId = HttpContext.Session.GetInt32("UnitId");
            // int countryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ecountryId));
            //  inputs.CountryId = countryId;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                //HttpResponseMessage response = await client.GetAsync("GetUserListing?userType=" + userType + "&logInUser="+ logInUser);
                HttpResponseMessage response = await client.GetAsync("GetClientAccessDetails?unitId=" + unitId + "&isClient=" + IsClient);
                //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    UserLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AccessClientMasterDTO>>(data);

                }

                return UserLst;
            }






        }
        // [HttpGet]
        //[Route("Master/GetCountryId/{ecountryId}")]
        public async Task<IActionResult> GetUnitAccess(int? roleId, int? DepartmentId)
        {
            //  DataTable dt = new DataTable();, int? designationId
            //   List<AccessMasterDTO> UserLst = new List<AccessMasterDTO>();
            AccessMasterDTO objOutPut = new AccessMasterDTO();

            int? unitId = HttpContext.Session.GetInt32("UnitId");
            // int countryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ecountryId));
            //  inputs.CountryId = countryId;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                //HttpResponseMessage response = await client.GetAsync("GetUserListing?userType=" + userType + "&logInUser="+ logInUser);
                HttpResponseMessage response = await client.GetAsync("GetAccessDetails?unitId=" + unitId + "&roleId=" + roleId + "&departmentId=" + DepartmentId);
                //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    objOutPut.AccessList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AccessMasterDTO>>(data);

                }
                objOutPut.RoleMasterList = await GetRoleList();
                //objOutPut.JobTitleMasterList = await GetDesignationList();
                objOutPut.DepartmentMasterList = await GetDepartmentList();
                //objOutPut.DesignationId = designationId;
                objOutPut.RoleId = roleId;
                objOutPut.DepartmentId = DepartmentId;
                return View("AccessMaster", objOutPut);
            }






        }

        [HttpPost]
        public async Task<OutPutResponse>? UserAccessMapping(AccessMasterDTO userAction)
        {
            OutPutResponse OutPut = new OutPutResponse();
            userAction.IsActive = true;
            userAction.UnitId = HttpContext.Session.GetInt32("UnitId");

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("UserAccessMapping", userAction);
                    if (response.IsSuccessStatusCode)
                    {
                        _memoryCache.Remove("Menu");
                        var data = await response.Content.ReadAsStringAsync();                    
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<int>(data);

                        OutPut.HttpStatusCode = 200;
                        OutPut.DisplayMessage = "Role Mapped";
                       
                    }

                }


            }
            catch (SystemException ex)
            {
                OutPut.DisplayMessage = "Designation failed!" + ex.Message;
            }
            //return View("AccessMaster", OutPut);
            return  OutPut;
        }


        [HttpPost]
        public async Task<OutPutResponse>? ClientAccessMapping(AccessClientMasterDTO userAction)
        {
            OutPutResponse OutPut = new OutPutResponse();
            userAction.IsActive = true;
            userAction.UnitId = HttpContext.Session.GetInt32("UnitId");

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("ClientAccessMapping", userAction);
                    if (response.IsSuccessStatusCode)
                    {
                        _memoryCache.Remove("Menu");

                        var data = await response.Content.ReadAsStringAsync();
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<int>(data);

                        OutPut.HttpStatusCode = 200;
                        OutPut.DisplayMessage = "Role Mapped";

                    }

                }


            }
            catch (SystemException ex)
            {
                OutPut.DisplayMessage = "Designation failed!" + ex.Message;
            }
            //return View("AccessMaster", OutPut);
            return OutPut;
        }
        public async Task<List<ProjectsDashboardDTO>>? ProjectDashboard()
        {
            // DataTable dt = new DataTable();
            List<ProjectsDashboardDTO> ProjectLst = new List<ProjectsDashboardDTO>();

            int? UserId = HttpContext.Session.GetInt32("UserId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiProjectUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                //HttpResponseMessage response = await client.GetAsync("GetUserListing?userType=" + userType + "&logInUser="+ logInUser);
                HttpResponseMessage response = await client.GetAsync("ProjectDashboard?unitId=" + unitId + "&userId=" + UserId);
                //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    ProjectLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProjectsDashboardDTO>>(data);

                }

                return ProjectLst;
            }






        }

        public async Task<List<TasksDashboardDTO>>? TaskDashboard()
        {
            // DataTable dt = new DataTable();
            List<TasksDashboardDTO> ProjectLst = new List<TasksDashboardDTO>();

            int? UserId = HttpContext.Session.GetInt32("UserId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiProjectUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                //HttpResponseMessage response = await client.GetAsync("GetUserListing?userType=" + userType + "&logInUser="+ logInUser);
                HttpResponseMessage response = await client.GetAsync("TaskDashboard?unitId=" + unitId + "&userId=" + UserId);
                //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    ProjectLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TasksDashboardDTO>>(data);

                }

                return ProjectLst;
            }






        }

        public async Task<IActionResult> UserNotification()
        {
            UserNotificationsDTO objOutPut = new UserNotificationsDTO();
            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (strMenuSession == null)
                return RedirectToAction("Index", "Home");

            if (HttpContext.Session.GetInt32("UserId") > 0)
            {


                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiCommentsUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    //HttpResponseMessage response = await client.GetAsync("GetUserListing?userType=" + userType + "&logInUser="+ logInUser);
                    HttpResponseMessage response = await client.GetAsync("GetNotificationDetails?UserId=" + HttpContext.Session.GetInt32("UserId"));
                    //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        objOutPut.NotificationList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserNotificationsDTO>>(data);
                    }

                    foreach (var item in objOutPut.NotificationList)
                    {

                        // item. = CommonHelper.EncryptURLHTML(item.ProjectId.ToString());
                        if (item.ProfileImage != null)
                            item.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);

                    }

                    return PartialView("Notifications/Notifications", objOutPut);
                }

            }


            return PartialView("/Views/Shared/Notifications/Notifications", objOutPut);
        }

    }
}
