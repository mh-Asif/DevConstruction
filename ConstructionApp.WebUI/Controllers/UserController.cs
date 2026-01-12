using Construction.Infrastructure.Helper;
using Construction.Infrastructure.KeyValues;
using Construction.Infrastructure.Models;
using ConstructionApp.WebUI.Helper;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Data;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace ConstructionApp.WebUI.Controllers
{
    public class UserController(ILogger<UserController> logger, IWebHostEnvironment environment, IMemoryCache memoryCache) : Controller
    {
        // string apiUrl = "https://localhost:7013/api/UsersAPI/";
        private readonly ILogger<UserController> _logger = logger;
        private IWebHostEnvironment _environment = environment;
        private readonly IMemoryCache _memoryCache = memoryCache;
        //public UserController(ILogger<UserController> logger, IWebHostEnvironment environment)
        //{
        //    _logger = logger;
        //    _environment = environment;
        //}
        public async Task<IActionResult> AddUser()
        {
            UsersMasterDTO objOutPut = new UsersMasterDTO();
            objOutPut.UsersKey = await GetUsers(1);
            return View(objOutPut);
        }
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            UsersMasterDTO objOutPut = new UsersMasterDTO();

            return View(objOutPut);
        }

        [HttpPost]
        public async Task<IActionResult>? ChangePassword(UserChangePassword inputs)
        {
            UsersMasterDTO OutPut = new UsersMasterDTO();
            //  inputs.IsActive = true;
            inputs.UserId = (int)HttpContext.Session.GetInt32("UserId");
            //  inputs.CreatedOn = DateTime.Now;
            // inputs.UnitId = HttpContext.Session.GetInt32("UnitId");
            inputs.LoginPassword = CommonHelper.Encrypt(CommonHelper.RandomString());
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiUserUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("ChangePassword", inputs);
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
                        if (OutPut.HttpStatusCode == 200)
                        {
                            //if (inputs.UserId == 0)
                            //{
                                // Send email notification for new vendor
                                ClientMail objTask = new ClientMail();
                                OutPut.LoginPassword = CommonHelper.Decrypt(inputs.LoginPassword);
                                OutPut.FullName = HttpContext.Session.GetString("FullName");
                                OutPut.EmailAddress = HttpContext.Session.GetString("EmailAddress");
                                 await objTask.ChangePasswordMail(OutPut);
                           // }

                            int safeUserId = (int)HttpContext.Session.GetInt32("UserId");
                            int safeUnitId = (int)HttpContext.Session.GetInt32("UnitId");
                            string cachePrefix = $"UserMgmt_{safeUnitId}";
                            _memoryCache.Remove(cachePrefix);
                            // Update the session with the new password
                            HttpContext.Session.SetString("LoginPassword", inputs.Password);
                            // return RedirectToAction("Index", "Home");
                            OutPut.DisplayMessage = "Password changed successfully. Please login again.";
                        }
                        else
                        {
                            // Handle error case
                            OutPut.DisplayMessage = "Failed to change password. Please try again.";
                        }
                        // if (OutPut.HttpStatusCode != 200)
                        return View("ChangePassword", OutPut);
                        // else
                        //  return RedirectToAction("Index", "Home");

                    }

                }
            }
            catch (SystemException ex)
            {
                OutPut.DisplayMessage = "User failed!" + ex.Message;
            }
            return RedirectToAction("ChangePassword", "User");
        }

        [HttpGet]
        [Route("User/UserProfile/{eId}")]
        public async Task<IActionResult> UserProfile(string eId)
        {
            UsersMasterDTO OutPut = new UsersMasterDTO();
            CommentsDashboardDTO inputs = new CommentsDashboardDTO();
            int uId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eId));
            if (uId > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    //  int? userId = HttpContext.Session.GetInt32("UserId");
                    int? unitId = HttpContext.Session.GetInt32("UnitId");
                    string? strMenuSession = HttpContext.Session.GetString("Menu");
                    if (strMenuSession == null)
                        return RedirectToAction("Index", "Home");


                    int safeUserId = uId;
                    int safeUnitId = unitId.Value;
                   // string cachePrefix = $"UserMgmt_{safeUnitId}";
                    string dashboardKey = safeUserId != null ? $"ProjectProfile_{safeUnitId}" : string.Empty;
                   // var uid = userId.Value;
                    if (_memoryCache.Get(dashboardKey) == null)
                    {
                        var dashboardList = await GetProjectDashboard(safeUserId)!;
                        OutPut.ProjectDashboardList = dashboardList ?? new List<ProjectDashboardDTO>();
                        _memoryCache.Set(dashboardKey, OutPut.ProjectDashboardList);
                    }
                    else
                    {
                        OutPut.ProjectDashboardList = _memoryCache.Get<List<ProjectDashboardDTO>>(dashboardKey) ?? new List<ProjectDashboardDTO>();
                      //  OutPut.ProjectDashboardList = OutPut.ProjectDashboardList.Where(p => p == uId).ToList();
                    }


                    //var cachedUsers = _memoryCache.Get<List<UsersMasterDTO>>(cachePrefix);

                    //if (cachedUsers != null && cachedUsers.Any())
                    //{
                    //    OutPut.UserList = cachedUsers.Where(u => u.UserId == uId).ToList();
                    //}
                    //else
                    //{
                        OutPut.UserList = await GetUserListById(uId);
                    if (OutPut.UserList != null && OutPut.UserList.Any())
                    HttpContext.Session.SetString("FullName", OutPut.UserList[0].FullName);
                    // _memoryCache.Set(cachePrefix, OutPut.UserList, TimeSpan.FromMinutes(30));
                    //  }

                    //if (_memoryCache.TryGetValue("UList", out List<UsersMasterDTO> cachedUserList) && cachedUserList.Any(u => u.UserId == uId))
                    //{
                    //    OutPut.UserList = cachedUserList.Where(u => u.UserId == uId).ToList();
                    //}
                    //else
                    //{
                    //    OutPut.UserList = await GetUserListById(uId);
                    //    _memoryCache.Set("UList", OutPut.UserList, TimeSpan.FromMinutes(30));
                    //}
                    //  OutPut.UserList = await GetUserListById(uId);
                    // _memoryCache.Set("UserList", OutPut.UserList, TimeSpan.FromMinutes(30));

                    inputs.IsComment = false;
                    inputs.ProjectId = 0;
                    inputs.SubTaskId = 0;
                    inputs.TaskId = 0;

                    //if (_memoryCache.TryGetValue("ActivityList", out List<CommentsDashboardDTO> cachedActivityList))
                    //{
                    //    OutPut.ActivityList = cachedActivityList;
                    //}
                    //else
                    //{
                    OutPut.ActivityList = await GetComments(inputs);
                    //  _memoryCache.Set("ActivityList", OutPut.ActivityList, TimeSpan.FromMinutes(30));
                    // }
                    // OutPut.ActivityList = await GetComments(inputs);


                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "User failed!" + ex.Message;
                }

            }
            return View(OutPut);
        }



        [HttpGet]
        [Route("User/ClientProfile/{eId}")]
        public async Task<IActionResult> ClientProfile(string eId)
        {
            CommentsDashboardDTO inputs = new CommentsDashboardDTO();
            UsersMasterDTO OutPut = new UsersMasterDTO();
            int uId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eId));
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            if (uId > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    int safeUserId = uId;
                    int safeUnitId = unitId.Value;
                    string cachePrefix = $"ClientMgmt_{safeUnitId}";

                    // string commentsKey = $"{cachePrefix}_GetComments_{inputs.TaskId}";

                    var cachedUsers = _memoryCache.Get<List<UsersMasterDTO>>(cachePrefix);

                    if (cachedUsers != null && cachedUsers.Any())
                    {
                        OutPut.UserList = cachedUsers.Where(u => u.UserId == uId).ToList();
                    }
                    else
                    {
                        OutPut.UserList = await GetUserListById(uId);
                        _memoryCache.Set(cachePrefix, OutPut.UserList, TimeSpan.FromMinutes(30));
                    }

                    inputs.IsComment = false;
                    inputs.ProjectId = 0;
                    inputs.SubTaskId = 0;
                    inputs.TaskId = 0;
                    //if (_memoryCache.TryGetValue("ActivityList", out List<CommentsDashboardDTO> cachedActivityList) && cachedActivityList.Any(c => c.UserId == uId))
                    //{
                    //    OutPut.ActivityList = cachedActivityList.Where(c => c.UserId == uId).ToList();
                    //}
                    //else
                    //{
                    OutPut.ActivityList = await GetComments(inputs);
                    // _memoryCache.Set("ActivityList", OutPut.ActivityList, TimeSpan.FromMinutes(30));
                    // }
                    //  OutPut.ActivityList = await GetComments(inputs);



                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "User failed!" + ex.Message;
                }

            }
            return View(OutPut);
        }

        [HttpGet]
        [Route("User/VendorProfile/{eId}")]
        public async Task<IActionResult> VendorProfile(string eId)
        {
            CommentsDashboardDTO inputs = new CommentsDashboardDTO();
            UsersMasterDTO OutPut = new UsersMasterDTO();
            int uId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eId));
            if (uId > 0)
            {
                int? unitId = HttpContext.Session.GetInt32("UnitId");
                //  inputs.CountryId = countryId;
                try
                {

                    int safeUserId = uId;
                    int safeUnitId = unitId.Value;
                    string cachePrefix = $"VendorProfile_{safeUnitId}_{safeUserId}";

                    // string commentsKey = $"{cachePrefix}_GetComments_{inputs.TaskId}";

                    var cachedUsers = _memoryCache.Get<List<UsersMasterDTO>>(cachePrefix);

                    if (cachedUsers != null && cachedUsers.Any())
                    {
                        OutPut.UserList = cachedUsers.Where(u => u.UserId == uId).ToList();
                    }
                    else
                    {
                        OutPut.UserList = await GetUserListById(uId);
                        _memoryCache.Set(cachePrefix, OutPut.UserList, TimeSpan.FromMinutes(30));
                    }

                    //if (_memoryCache.TryGetValue("VendorList", out List<UsersMasterDTO> cachedUserList) && cachedUserList.Any(u => u.UserId == uId))
                    //{
                    //    OutPut.UserList = cachedUserList.Where(u => u.UserId == uId).ToList();
                    //}
                    //else
                    //{
                    //    OutPut.UserList = await GetUserListById(uId);
                    //    _memoryCache.Set("VendorList", OutPut.UserList, TimeSpan.FromMinutes(30));
                    //}
                    //  OutPut.UserList = await GetUserListById(uId);

                    inputs.IsComment = false;
                    inputs.ProjectId = 0;
                    inputs.SubTaskId = 0;
                    inputs.TaskId = 0;
                    //if (_memoryCache.TryGetValue("ActivityList", out List<CommentsDashboardDTO> cachedActivityList) && cachedActivityList.Any(c => c.UserId == uId))
                    //{
                    //    OutPut.ActivityList = cachedActivityList.Where(c => c.UserId == uId).ToList();
                    //}
                    //else
                    //{
                    OutPut.ActivityList = await GetComments(inputs);
                    //  _memoryCache.Set("ActivityList", OutPut.ActivityList, TimeSpan.FromMinutes(30));
                    // }
                    //  OutPut.ActivityList = await GetComments(inputs);

                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "User failed!" + ex.Message;
                }

            }
            return View(OutPut);
        }
        public async Task<IActionResult> AddNewUser()
        {
            UsersMasterDTO objOutPut = new UsersMasterDTO();

            // Caching for DepartmentList
            if (_memoryCache.TryGetValue("DepartmentList", out List<DepartmentMasterDTO> cachedDepartmentList))
            {
                objOutPut.DepartmentList = cachedDepartmentList;
            }
            else
            {
                objOutPut.DepartmentList = await GetDepartmentList();
                _memoryCache.Set("DepartmentList", objOutPut.DepartmentList, TimeSpan.FromMinutes(30));
            }

            // Caching for JobTitleList
            if (_memoryCache.TryGetValue("JobTitleList", out List<JobTitleMasterDTO> cachedJobTitleList))
            {
                objOutPut.JobTitleList = cachedJobTitleList;
            }
            else
            {
                objOutPut.JobTitleList = await GetDesignationList();
                _memoryCache.Set("JobTitleList", objOutPut.JobTitleList, TimeSpan.FromMinutes(30));
            }

            // Caching for RoleList
            if (_memoryCache.TryGetValue("RoleList", out List<RoleMasterDTO> cachedRoleList))
            {
                objOutPut.RoleList = cachedRoleList;
            }
            else
            {
                objOutPut.RoleList = await GetRoleList();
                _memoryCache.Set("RoleList", objOutPut.RoleList, TimeSpan.FromMinutes(30));
            }

            if (_memoryCache.TryGetValue("UList", out List<UserKeyValues> cachedUserList))
            {
                objOutPut.UsersKey = cachedUserList;
            }
            else
            {
                objOutPut.UsersKey = await GetUsers(1);
                _memoryCache.Set("UList", objOutPut.UsersKey, TimeSpan.FromMinutes(30));
            }

            ViewBag.EnvironmentUrl = EnvironmentUrl.apiUrl;
            //objOutPut.UsersKey = await GetUsers(1);
            //_memoryCache.Set("UserList", objOutPut.UsersKey, TimeSpan.FromMinutes(30));
            return View(objOutPut);
        }
        public async Task<IActionResult> AddVendorUser()
        {
            UsersMasterDTO objOutPut = new UsersMasterDTO();
           // objOutPut.UsersKey = await GetUsers(1);
            ViewBag.EnvironmentUrl = EnvironmentUrl.apiUrl;
            return View(objOutPut);
        }
        public async Task<IActionResult> UserManagement()
        {
            // ClientMail objTask = new ClientMail();
            UsersMasterDTO objOutPut = new UsersMasterDTO();
            //objOutPut.FullName = "Mohd Asif";
            //objOutPut.EmailAddress = "mh.asif@hotmail.com";
            //objOutPut.LoginPassword = "mypassword";
            //await objTask.UserLoginMail(objOutPut);
            int? userId = HttpContext.Session.GetInt32("UserId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (strMenuSession == null)
                return RedirectToAction("Index", "Home");


            int safeUserId = userId.Value;
            int safeUnitId = unitId.Value;
            string cachePrefix = $"UserMgmt_{safeUnitId}";        

            var cachedUsers = _memoryCache.Get<List<UsersMasterDTO>>(cachePrefix);


            AccessMasterDTO? empSession = (AccessMasterDTO?)(JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession));

            objOutPut.AccessList = empSession.AccessList.Where(p => p.ModuleCode.Trim() == "U").ToList();

            if (cachedUsers != null && cachedUsers.Any())
            {
                objOutPut.UserList = cachedUsers;
            }
            else
            {
                objOutPut.UserList = await GetUserList(1, safeUserId);
                _memoryCache.Set(cachePrefix, objOutPut.UserList, TimeSpan.FromMinutes(30));
            }

            //if (_memoryCache.TryGetValue("UList", out List<UsersMasterDTO> cachedUserList))
            //{
            //    if (cachedUserList.Any())
            //        objOutPut.UserList = cachedUserList;
            //    else
            //        objOutPut.UserList = await GetUserList(1, HttpContext.Session.GetInt32("UserId"));
            //   // objOutPut.UserList = cachedUserList;
            //}
            //else
            //{
            //    objOutPut.UserList = await GetUserList(1, HttpContext.Session.GetInt32("UserId"));
            //    _memoryCache.Set("UList", objOutPut.UserList, TimeSpan.FromMinutes(30));
            //}

            // objOutPut.UserList = await GetUserList(1, HttpContext.Session.GetInt32("UserId"));
            return View(objOutPut);
        }
        public async Task<IActionResult> ClientManagement()
        {
            UsersMasterDTO objOutPut = new UsersMasterDTO();
            int? userId = HttpContext.Session.GetInt32("UserId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");

            int safeUserId = userId.Value;
            int safeUnitId = unitId.Value;
            string cachePrefix = $"ClientMgmt_{safeUnitId}";

            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (strMenuSession == null)
                return RedirectToAction("Index", "Home");

            AccessMasterDTO? empSession = (AccessMasterDTO?)(JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession));

            objOutPut.AccessList = empSession.AccessList.Where(p => p.ModuleCode.Trim() == "C").ToList();
            var cachedUsers = _memoryCache.Get<List<UsersMasterDTO>>(cachePrefix);

            if (cachedUsers != null && cachedUsers.Any())
            {
                objOutPut.UserList = cachedUsers;
            }
            else
            {
                objOutPut.UserList = await GetCVList(2, HttpContext.Session.GetInt32("UserId"));
                _memoryCache.Set(cachePrefix, objOutPut.UserList, TimeSpan.FromMinutes(30));
            }

            //if (_memoryCache.TryGetValue("ClientList", out List<UsersMasterDTO> cachedUserList))
            //{
            //    if (cachedUserList.Any())
            //        objOutPut.UserList = cachedUserList;
            //    else
            //        objOutPut.UserList = await GetCVList(2, HttpContext.Session.GetInt32("UserId"));
            //   // objOutPut.UserList = cachedUserList;
            //}
            //else
            //{
            //    objOutPut.UserList = await GetCVList(2, HttpContext.Session.GetInt32("UserId"));
            //    _memoryCache.Set("ClientList", objOutPut.UserList, TimeSpan.FromMinutes(30));
            //}

            //objOutPut.UserList = await GetCVList(2, HttpContext.Session.GetInt32("UserId"));
            return View(objOutPut);
        }
        public IActionResult AddClient()
        {
            UsersMasterDTO objOutPut = new UsersMasterDTO();
            // objOutPut.CountryMasterList = await GetCountryList();
            return View(objOutPut);
        }
        public IActionResult AddNewClient()
        {
            UsersMasterDTO objOutPut = new UsersMasterDTO();
            ViewBag.EnvironmentUrl = EnvironmentUrl.apiUrl;
            //objOutPut.DisplayMessage = "Testing";
            // objOutPut.CountryMasterList = await GetCountryList();
            return View(objOutPut);
        }

        public async Task<IActionResult> VendorManagement()
        {
            UsersMasterDTO objOutPut = new UsersMasterDTO();
            int? userId = HttpContext.Session.GetInt32("UserId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");

            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (strMenuSession == null)
                return RedirectToAction("Index", "Home");

            AccessMasterDTO? empSession = (AccessMasterDTO?)(JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession));

            objOutPut.AccessList = empSession.AccessList.Where(p => p.ModuleCode.Trim() == "V").ToList();


            int safeUserId = userId.Value;
            int safeUnitId = unitId.Value;
            string cachePrefix = $"VendorMgmt_{safeUnitId}";
            var cachedUsers = _memoryCache.Get<List<UsersMasterDTO>>(cachePrefix);
            if (cachedUsers != null && cachedUsers.Any())
            {
                objOutPut.UserList = cachedUsers;
            }
            else
            {
                objOutPut.UserList = await GetCVList(3, HttpContext.Session.GetInt32("UserId"));
                _memoryCache.Set(cachePrefix, objOutPut.UserList, TimeSpan.FromMinutes(30));
            }

            //if (_memoryCache.TryGetValue("VendorList", out List<UsersMasterDTO> cachedUserList))
            //{
            //    if (cachedUserList.Any())
            //        objOutPut.UserList = cachedUserList;
            //    else
            //        objOutPut.UserList = await GetCVList(3, HttpContext.Session.GetInt32("UserId"));

            //    //objOutPut.UserList = cachedUserList;
            //}
            //else
            //{
            //    objOutPut.UserList = await GetCVList(3, HttpContext.Session.GetInt32("UserId"));
            //    _memoryCache.Set("VendorList", objOutPut.UserList, TimeSpan.FromMinutes(30));
            //}
            //  objOutPut.UserList = await GetCVList(3, HttpContext.Session.GetInt32("UserId"));
            return View(objOutPut);
        }

        public async Task<IActionResult> VendorUserManagement()
        {
            UsersMasterDTO objOutPut = new UsersMasterDTO();

            //int? userId = HttpContext.Session.GetInt32("UserId");
            //int? unitId = HttpContext.Session.GetInt32("UnitId");

            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (strMenuSession == null)
                return RedirectToAction("Index", "Home");

            AccessMasterDTO? empSession = (AccessMasterDTO?)(JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession));

            objOutPut.AccessList = empSession.AccessList.Where(p => p.ModuleCode.Trim() == "V").ToList();

            if (_memoryCache.TryGetValue("VendorUserList", out List<UsersMasterDTO> cachedUserList))
            {
                objOutPut.UserList = cachedUserList;
            }
            else
            {
                objOutPut.UserList = await GetCVList(4, HttpContext.Session.GetInt32("UserId"));
                _memoryCache.Set("VendorUserList", objOutPut.UserList, TimeSpan.FromMinutes(30));
            }
            //  objOutPut.UserList = await GetCVList(3, HttpContext.Session.GetInt32("UserId"));
            return View(objOutPut);
        }
        public IActionResult AddVendor()
        {
            UsersMasterDTO objOutPut = new UsersMasterDTO();
            // objOutPut.CountryMasterList = await GetCountryList();
            return View(objOutPut);
        }

        public IActionResult AddNewVendor()
        {
            UsersMasterDTO objOutPut = new UsersMasterDTO();
            // objOutPut.CountryMasterList = await GetCountryList();
            ViewBag.EnvironmentUrl = EnvironmentUrl.apiUrl;
            return View(objOutPut);
        }


        [HttpPost]
        public async Task<IActionResult>? SaveUser(UsersMasterDTO inputs, List<IFormFile> profileImageInput)
        {
            ViewBag.EnvironmentUrl = EnvironmentUrl.apiUrl;
            UsersMasterDTO OutPut = new UsersMasterDTO();
            inputs.IsActive = true;
            inputs.CreatedBy = HttpContext.Session.GetInt32("UserId");
            // inputs.Address = inputs.Address1;
            inputs.IsAdmin = false;
            inputs.CreatedOn = DateTime.Now;
            inputs.UnitId = HttpContext.Session.GetInt32("UnitId");
            inputs.LoginPassword = CommonHelper.Encrypt(CommonHelper.RandomString());
            inputs.Base64ProfileImage = "";


            if (profileImageInput.Count > 0)
            {
                foreach (IFormFile postedFile in profileImageInput)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "Profile");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(postedFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await postedFile.CopyToAsync(stream);
                    }
                    inputs.ProfileName = fileName;

                    inputs.ProfileImage = null;

                }
            }
            else
            {
                inputs.ProfileImage = null;
                inputs.ProfileName = null;
            }


            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiUserUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("SaveUser", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        if (_memoryCache is MemoryCache memCache)
                        {
                            memCache.Clear();
                        }
                        if (inputs.UserId == 0)
                        {
                            ClientMail objTask = new ClientMail();
                            inputs.LoginPassword = CommonHelper.Decrypt(inputs.LoginPassword);
                            await objTask.UserLoginMail(inputs);
                        }
                        int safeUserId = (int)HttpContext.Session.GetInt32("UserId");
                        int safeUnitId = inputs.UnitId.Value;
                        //string cachePrefix = $"UserMgmt_{safeUnitId}";
                        //string userListKey = $"UserList_{safeUnitId}";                    
                        //_memoryCache.Remove(cachePrefix);
                        //_memoryCache.Remove(userListKey);

                        var data = await response.Content.ReadAsStringAsync();

                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);
                        OutPut.HttpStatusCode = result.HttpStatusCode;
                        OutPut.DisplayMessage = result.DisplayMessage;
                        if (OutPut.HttpStatusCode != 200)
                            return View("AddNewUser", OutPut);
                        else
                            return RedirectToAction("UserManagement", "User");

                    }

                }


            }
            catch (SystemException ex)
            {
                OutPut.DisplayMessage = "User failed!" + ex.Message;
            }
            return RedirectToAction("UserManagement", "User");
        }


        [HttpPost]
        public async Task<IActionResult>? SaveClient(UsersMasterDTO inputs, List<IFormFile> profileImageInput)
        {
            ViewBag.EnvironmentUrl = EnvironmentUrl.apiUrl;

            UsersMasterDTO OutPut = new UsersMasterDTO();
            inputs.IsActive = true;
            inputs.CreatedBy = HttpContext.Session.GetInt32("UserId");
            inputs.CreatedOn = DateTime.Now;
            inputs.IsAdmin = false;
            //inputs.CreatedOn = DateTime.Now;
            //inputs.UnitId = HttpContext.Session.GetInt32("UserId");
            inputs.UnitId = HttpContext.Session.GetInt32("UnitId");
            inputs.LoginPassword = CommonHelper.Encrypt(CommonHelper.RandomString());
            inputs.Base64ProfileImage = "";
            int? userId = HttpContext.Session.GetInt32("UserId");

            //if (profileImageInput.Count > 0)
            //{
            //   foreach (IFormFile postedFile in profileImageInput)
            //    {

            //        using (var target = new MemoryStream())
            //        {
            //            postedFile.CopyTo(target);
            //            inputs.ProfileImage = target.ToArray();

            //        }
            //        //  inputData.ClientLogo = fileName;
            //    }
            //}
            //else
            //    inputs.ProfileImage = null;

            if (profileImageInput.Count > 0)
            {
                foreach (IFormFile postedFile in profileImageInput)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "Profile");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(postedFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await postedFile.CopyToAsync(stream);
                    }
                    inputs.ProfileName = fileName;

                    inputs.ProfileImage = null;

                }
            }
            else
            {
                inputs.ProfileImage = null;
                inputs.ProfileName = null;
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiUserUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("SaveUser", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        if (_memoryCache is MemoryCache memCache)
                        {
                            memCache.Clear();
                        }
                        if (inputs.UserId == 0)
                        {
                            // Send email notification for new client
                            ClientMail objTask = new ClientMail();
                            inputs.LoginPassword = CommonHelper.Decrypt(inputs.LoginPassword);
                            await objTask.UserLoginMail(inputs);
                        }
                      

                        int safeUserId = userId.Value;
                        int safeUnitId = inputs.UnitId.Value;
                        string clientListKey = $"ClientList_{safeUnitId}";
                        string cachePrefix = $"ClientMgmt_{safeUnitId}";
                        _memoryCache.Remove(cachePrefix);
                        _memoryCache.Remove(clientListKey);

                        var data = await response.Content.ReadAsStringAsync();

                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);
                        OutPut.HttpStatusCode = result.HttpStatusCode;
                        OutPut.DisplayMessage = result.DisplayMessage;
                        if (OutPut.HttpStatusCode != 200)
                            return View("AddNewClient", OutPut);
                        else
                            return RedirectToAction("ClientManagement", "User");
                    }

                }


            }
            catch (SystemException ex)
            {
                OutPut.DisplayMessage = "User failed!" + ex.Message;
            }
            return RedirectToAction("ClientManagement", "User");
        }



        [HttpPost]
        public async Task<IActionResult>? SaveVendor(UsersMasterDTO inputs, List<IFormFile> profileImageInput)
        {
            ViewBag.EnvironmentUrl = EnvironmentUrl.apiUrl;
            // List<IFormFile> formFile,
            UsersMasterDTO OutPut = new UsersMasterDTO();
            inputs.IsAdmin = false;
            inputs.IsActive = true;
            inputs.CreatedBy = HttpContext.Session.GetInt32("UserId");
            inputs.CreatedOn = DateTime.Now;
            inputs.UnitId = HttpContext.Session.GetInt32("UnitId");
            inputs.LoginPassword = CommonHelper.Encrypt(CommonHelper.RandomString());
            inputs.Base64ProfileImage = "";
            //if (profileImageInput.Count > 0)
            //{
            //    foreach (IFormFile postedFile in profileImageInput)
            //    {

            //        using (var target = new MemoryStream())
            //        {
            //            postedFile.CopyTo(target);
            //            inputs.ProfileImage = target.ToArray();

            //        }
            //        //  inputData.ClientLogo = fileName;
            //    }
            //}
            //else
            //    inputs.ProfileImage = null;

            if (profileImageInput.Count > 0)
            {
                foreach (IFormFile postedFile in profileImageInput)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "Profile");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(postedFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await postedFile.CopyToAsync(stream);
                    }
                    inputs.ProfileName = fileName;

                    inputs.ProfileImage = null;

                }
            }
            else
            {
                inputs.ProfileImage = null;
                inputs.ProfileName = null;
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiUserUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("SaveUser", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        if (_memoryCache is MemoryCache memCache)
                        {
                            memCache.Clear();
                        }
                        if (inputs.UserId == 0)
                        {
                            // Send email notification for new vendor
                            ClientMail objTask = new ClientMail();
                            inputs.LoginPassword = CommonHelper.Decrypt(inputs.LoginPassword);
                            await objTask.UserLoginMail(inputs);
                        }
                        //int safeUserId = (int)HttpContext.Session.GetInt32("UserId");
                        //int safeUnitId = inputs.UnitId.Value;
                        //string cachePrefix = $"VendorMgmt_{safeUnitId}";
                        //_memoryCache.Remove(cachePrefix);
                        //_memoryCache.Remove("VendorList");
                        //_memoryCache.Remove("UserList");
                        var data = await response.Content.ReadAsStringAsync();

                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);
                        OutPut.HttpStatusCode = result.HttpStatusCode;
                        OutPut.DisplayMessage = result.DisplayMessage;
                        if (OutPut.HttpStatusCode != 200)
                            return View("AddNewVendor", OutPut);
                        else
                            return RedirectToAction("VendorManagement", "User");

                    }

                }


            }
            catch (SystemException ex)
            {
                OutPut.DisplayMessage = "User failed!" + ex.Message;
            }
            return RedirectToAction("VendorManagement", "User");
        }

        [HttpPost]
        public async Task<IActionResult>? SaveVendorUser(UsersMasterDTO inputs, List<IFormFile> profileImageInput)
        {
            // List<IFormFile> formFile,
            UsersMasterDTO OutPut = new UsersMasterDTO();
            inputs.IsAdmin = false;
            inputs.IsActive = true;
            inputs.CreatedBy = HttpContext.Session.GetInt32("UserId");
            inputs.CreatedOn = DateTime.Now;
            inputs.UnitId = HttpContext.Session.GetInt32("UnitId");
            inputs.LoginPassword = CommonHelper.Encrypt(CommonHelper.RandomString());
            inputs.Base64ProfileImage = "";
            //if (profileImageInput.Count > 0)
            //{
            //    foreach (IFormFile postedFile in profileImageInput)
            //    {

            //        using (var target = new MemoryStream())
            //        {
            //            postedFile.CopyTo(target);
            //            inputs.ProfileImage = target.ToArray();

            //        }
            //        //  inputData.ClientLogo = fileName;
            //    }
            //}
            //else
            //    inputs.ProfileImage = null;

            if (profileImageInput.Count > 0)
            {
                foreach (IFormFile postedFile in profileImageInput)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "Profile");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(postedFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await postedFile.CopyToAsync(stream);
                    }
                    inputs.ProfileName = fileName;

                    inputs.ProfileImage = null;

                }
            }
            else
            {
                inputs.ProfileImage = null;
                inputs.ProfileName = null;
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiUserUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("SaveUser", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();

                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);
                        OutPut.HttpStatusCode = result.HttpStatusCode;
                        OutPut.DisplayMessage = result.DisplayMessage;
                        if (OutPut.HttpStatusCode != 200)
                            return View("AddVendorUser", OutPut);
                        else
                            return RedirectToAction("VendorUserManagement", "User");

                    }

                }


            }
            catch (SystemException ex)
            {
                OutPut.DisplayMessage = "User failed!" + ex.Message;
            }
            return RedirectToAction("VendorUserManagement", "User");
        }

        [HttpGet]
        //[Route("Master/GetCountryId/{ecountryId}")]
        public async Task<List<UsersMasterDTO>>? GetUserList(int? userType, int? unitId)
        {
            DataTable dt = new DataTable();
            List<UsersMasterDTO> UserLst = new List<UsersMasterDTO>();
            ViewBag.EnvironmentUrl = EnvironmentUrl.apiUrl;
            // int countryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ecountryId));
            //  inputs.CountryId = countryId;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiUserUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                //HttpResponseMessage response = await client.GetAsync("GetUserListing?userType=" + userType + "&logInUser="+ logInUser);
                HttpResponseMessage response = await client.GetAsync("GetUserDashboard?userType=" + userType + "&logInUser=" + unitId);
                //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    UserLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UsersMasterDTO>>(data);

                    foreach (var item in UserLst)
                    {

                        item.EnycUserId = CommonHelper.EncryptURLHTML(item.UserId.ToString());

                        //if (item.ProfileName != null)
                        //    item.ProfilePath = Path.Combine(_environment.WebRootPath, "Profile", item.ProfileName);


                        //if (item.ProfileName != null)
                        //    item.Base64ProfileImage = Path.Combine("\\Profile", item.ProfileName);

                    }



                }

                return UserLst;
            }






        }

        public async Task<List<UsersMasterDTO>>? GetCVList(int? userType, int? logInUser)
        {
            DataTable dt = new DataTable();
            List<UsersMasterDTO> UserLst = new List<UsersMasterDTO>();

            // int countryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ecountryId));
            //  inputs.CountryId = countryId;
            int? userId = HttpContext.Session.GetInt32("UserId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            int safeUserId = userId.Value;
            int safeUnitId = unitId.Value;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiUserUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                //HttpResponseMessage response = await client.GetAsync("GetUserListing?userType=" + userType + "&logInUser="+ logInUser);
                HttpResponseMessage response = await client.GetAsync("GetCVDashboard?userType=" + userType + "&logInUser=" + logInUser);
                //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                if (response.IsSuccessStatusCode)
                {
                    string cachePrefix = $"VendorMgmt_{safeUnitId}";
                    _memoryCache.Remove(cachePrefix);
                    var data = await response.Content.ReadAsStringAsync();
                    UserLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UsersMasterDTO>>(data);

                    foreach (var item in UserLst)
                    {
                        item.EnycUserId = CommonHelper.EncryptURLHTML(item.UserId.ToString());
                        //  if (item.ProfileImage != null)
                        // item.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);
                    }



                }

                return UserLst;
            }






        }

        public async Task<List<UserKeyValues>> GetUsers(int userType)
        {
            ViewBag.EnvironmentUrl = EnvironmentUrl.apiUrl;
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            List<UserKeyValues> PriorityLst = new List<UserKeyValues>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiUserUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetUsers?UserType=" + userType + "&unitId=" + unitId);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    PriorityLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserKeyValues>>(data);

                }

            }
            return PriorityLst;
        }

        [HttpPost]
        public async Task<IActionResult>? UploadFiles()
        {
            return null;
        }

        [HttpGet]
        //  [Route("User/DeleteUser/{eId}")]
        public async Task<bool>? DeleteUser(string eId)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            UsersMasterDTO OutPut = new UsersMasterDTO();
            int userType = 1;
            int? userId = HttpContext.Session.GetInt32("UserId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            int Id = Convert.ToInt32(CommonHelper.DecryptURLHTML(eId));
            if (Id > 0)
            {
                int safeUserId = userId.Value;
                int safeUnitId = unitId.Value;
                string cachePrefix = $"UserMgmt_{safeUnitId}";
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiUserUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("DeleteUser?uId=" + Id);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
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
                            // if(userType==1)
                            //  OutPut.UserList = await GetUserList(userType, HttpContext.Session.GetInt32("UserId"));
                            // else if (userType ==2)
                            // OutPut.UserList = await GetUserList(userType, HttpContext.Session.GetInt32("UserId"));

                        }

                    }
                    return true;
                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "Clinic failed!" + ex.Message;
                    return false;
                }

            }
            return true;
            // return "";
            // return View("UserManagement", OutPut);
        }

        [HttpGet]
        [Route("User/GetUserById/{eId}")]
        public async Task<IActionResult>? GetUserById(string eId)
        {
            ViewBag.EnvironmentUrl = EnvironmentUrl.apiUrl;

            UsersMasterDTO OutPut = new UsersMasterDTO();
            int userType = 1;
            int Id = Convert.ToInt32(CommonHelper.DecryptURLHTML(eId));
            if (Id > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiUserUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("GetUserById?uId=" + Id);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            OutPut = Newtonsoft.Json.JsonConvert.DeserializeObject<UsersMasterDTO>(data);
                            if (OutPut.ProfileName != null)
                                OutPut.Base64ProfileImage = OutPut.ProfileName;  //"data:image/png;base64," + Convert.ToBase64String(OutPut.ProfileImage, 0, OutPut.ProfileImage.Length);

                            if (_memoryCache.TryGetValue("DepartmentList", out List<DepartmentMasterDTO> cachedDepartmentList))
                            {
                                OutPut.DepartmentList = cachedDepartmentList;
                            }
                            else
                            {
                                OutPut.DepartmentList = await GetDepartmentList();
                                _memoryCache.Set("DepartmentList", OutPut.DepartmentList, TimeSpan.FromMinutes(30));
                            }

                            // Caching for JobTitleList
                            if (_memoryCache.TryGetValue("JobTitleList", out List<JobTitleMasterDTO> cachedJobTitleList))
                            {
                                OutPut.JobTitleList = cachedJobTitleList;
                            }
                            else
                            {
                                OutPut.JobTitleList = await GetDesignationList();
                                _memoryCache.Set("JobTitleList", OutPut.JobTitleList, TimeSpan.FromMinutes(30));
                            }

                            // Caching for RoleList
                            if (_memoryCache.TryGetValue("RoleList", out List<RoleMasterDTO> cachedRoleList))
                            {
                                OutPut.RoleList = cachedRoleList;
                            }
                            else
                            {
                                OutPut.RoleList = await GetRoleList();
                                _memoryCache.Set("RoleList", OutPut.RoleList, TimeSpan.FromMinutes(30));
                            }

                            if (_memoryCache.TryGetValue("UList", out List<UserKeyValues> cachedUserList))
                            {
                                OutPut.UsersKey = cachedUserList;
                            }
                            else
                            {
                                OutPut.UsersKey = await GetUsers(1);
                                _memoryCache.Set("UList", OutPut.UsersKey, TimeSpan.FromMinutes(30));
                            }
                            //OutPut.UserList = await GetUserList(userType, HttpContext.Session.GetInt32("UserId"));
                        }

                    }


                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "Clinic failed!" + ex.Message;
                }

            }
            return View("AddNewUser", OutPut);

        }

        [HttpGet]
        //  [Route("User/DeleteClient/{eId}")]
        public async Task<bool>? DeleteClient(string eId)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            UsersMasterDTO OutPut = new UsersMasterDTO();
            int userType = 2;
            int Id = Convert.ToInt32(CommonHelper.DecryptURLHTML(eId));
            int? userId = HttpContext.Session.GetInt32("UserId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            if (Id > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiUserUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("DeleteUser?uId=" + Id);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
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
                            // if(userType==1)
                            OutPut.UserList = await GetUserList(userType, HttpContext.Session.GetInt32("UserId"));

                        }

                    }

                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "Clinic failed!" + ex.Message;
                }

            }
            return true;
        }

        [HttpGet]
        [Route("User/GetClientById/{eId}")]
        public async Task<IActionResult>? GetClientById(string eId)
        {

            UsersMasterDTO OutPut = new UsersMasterDTO();
            ViewBag.EnvironmentUrl = EnvironmentUrl.apiUrl;
            int userType = 2;
            int Id = Convert.ToInt32(CommonHelper.DecryptURLHTML(eId));
            if (Id > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiUserUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("GetUserById?uId=" + Id);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            OutPut = Newtonsoft.Json.JsonConvert.DeserializeObject<UsersMasterDTO>(data);
                            if (OutPut.ProfileName != null)
                                OutPut.Base64ProfileImage = OutPut.ProfileName;//  "data:image/png;base64," + Convert.ToBase64String(OutPut.ProfileImage, 0, OutPut.ProfileImage.Length);

                        }

                    }


                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "Clinic failed!" + ex.Message;
                }

            }
            return View("AddNewClient", OutPut);

        }

        [HttpGet]
        //  [Route("User/DeleteVendor/{eId}")]
        public async Task<bool>? DeleteVendor(string eId)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            UsersMasterDTO OutPut = new UsersMasterDTO();
            int userType = 3;
            int? userId = HttpContext.Session.GetInt32("UserId");
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            int Id = Convert.ToInt32(CommonHelper.DecryptURLHTML(eId));
            if (Id > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        int safeUserId = userId.Value;
                        int safeUnitId = unitId.Value;
                        string cachePrefix = $"VendorMgmt_{safeUnitId}";
                        _memoryCache.Remove(cachePrefix);
                        client.BaseAddress = new Uri(EnvironmentUrl.apiUserUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("DeleteUser?uId=" + Id);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
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
                            // if(userType==1)
                            // OutPut.UserList = await GetUserList(userType, HttpContext.Session.GetInt32("UserId"));
                            // else if (userType ==2)
                            // OutPut.UserList = await GetUserList(userType, HttpContext.Session.GetInt32("UserId"));

                        }

                    }

                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "Clinic failed!" + ex.Message;
                }

            }
            return true;
        }

        [HttpGet]
        [Route("User/GetVendorById/{eId}")]
        public async Task<IActionResult>? GetVendorById(string eId)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            UsersMasterDTO OutPut = new UsersMasterDTO();
            ViewBag.EnvironmentUrl = EnvironmentUrl.apiUrl;
            int userType = 3;
            int Id = Convert.ToInt32(CommonHelper.DecryptURLHTML(eId));
            if (Id > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiUserUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("GetUserById?uId=" + Id);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            OutPut = Newtonsoft.Json.JsonConvert.DeserializeObject<UsersMasterDTO>(data);
                            if (OutPut.ProfileName != null)
                                OutPut.Base64ProfileImage = OutPut.ProfileName;//  "data:image/png;base64," + Convert.ToBase64String(OutPut.ProfileImage, 0, OutPut.ProfileImage.Length);

                        }

                    }


                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "Clinic failed!" + ex.Message;
                }

            }
            return View("AddNewVendor", OutPut);

        }


        public async Task<List<UsersMasterDTO>>? GetUserListById(int? userId)
        {
            DataTable dt = new DataTable();
            List<UsersMasterDTO> UserLst = new List<UsersMasterDTO>();

            // int countryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ecountryId));
            //  inputs.CountryId = countryId;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiUserUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                //HttpResponseMessage response = await client.GetAsync("GetUserListing?userType=" + userType + "&logInUser="+ logInUser);
                HttpResponseMessage response = await client.GetAsync("GetUserDashboard?userType=" + 0 + "&logInUser=" + userId);
                //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    UserLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UsersMasterDTO>>(data);

                    foreach (var item in UserLst)
                    {

                        item.EnycUserId = CommonHelper.EncryptURLHTML(item.UserId.ToString());
                        if (item.ProfileName != null)
                            item.Base64ProfileImage = item.ProfileName;// "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);

                    }



                }

                return UserLst;
            }






        }


        // [HttpPost]
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

                        foreach (var item in CommentsLst)
                        {
                            if (item.ProfileImage != null)
                                item.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);

                        }
                        return CommentsLst;

                    }

                }


            }
            catch (SystemException ex)
            {

            }
            return null;

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
    }
}
