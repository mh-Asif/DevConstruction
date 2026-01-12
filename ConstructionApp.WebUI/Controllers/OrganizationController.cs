using Construction.Infrastructure.Helper;
using Construction.Infrastructure.KeyValues;
using Construction.Infrastructure.Models;
using ConstructionApp.WebUI.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Data;

namespace ConstructionApp.WebUI.Controllers
{
    public class OrganizationController : Controller
    {

      //  string apiUrl = "https://localhost:7013/api/UsersAPI/";
       // string apiMasterUrl = "https://localhost:7013/api/MasterAPIs/";
        private readonly ILogger<OrganizationController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly IMemoryCache _memoryCache;
        public OrganizationController(ILogger<OrganizationController> logger, IWebHostEnvironment environment, IMemoryCache memoryCache)
        {
            _logger = logger;
            _environment = environment;
            _memoryCache = memoryCache;
        }
        public async Task<IActionResult> CompanyManagement()
        {
            UsersMasterDTO objOutPut = new UsersMasterDTO();
            objOutPut.UserList = await GetCompanyList();
            objOutPut.FullName = HttpContext.Session.GetString("FullName");
            objOutPut.UnitId = HttpContext.Session.GetInt32("UnitId");
            return View(objOutPut);
        }
        public async Task<IActionResult> AddCompany()
        {
            UsersMasterDTO objOutPut = new UsersMasterDTO();

            objOutPut.FullName = HttpContext.Session.GetString("FullName");
            objOutPut.UnitId = HttpContext.Session.GetInt32("UnitId");
            // objOutPut.CountryMasterList = await GetCountryList();
            return View(objOutPut);
        }
        public async Task<IActionResult> ViewNotifications()
        {
            UserNotificationsDTO objOutPut = new UserNotificationsDTO();

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

                    //foreach (var item in objOutPut.NotificationList)
                    //{

                    //    // item. = CommonHelper.EncryptURLHTML(item.ProjectId.ToString());
                    //    //if (item.ProfileImage != null)
                    //    //    item.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);

                    //}

                    return View(objOutPut);
                }

            }

            return View(objOutPut);
        }

        public async Task<IActionResult> ViewComments()
        {
            UserNotificationsDTO objOutPut = new UserNotificationsDTO();
            int? pId = 0;
            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (strMenuSession == null)
                return RedirectToAction("Index", "Home");

            AccessMasterDTO? empSession = (AccessMasterDTO?)(JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession));
         

            if (HttpContext.Session.GetInt32("UserId") > 0)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiCommentsUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    //HttpResponseMessage response = await client.GetAsync("GetUserListing?userType=" + userType + "&logInUser="+ logInUser);
                    HttpResponseMessage response = await client.GetAsync("GetNotificationDetails?UserId=" + HttpContext.Session.GetInt32("UserId")+ "&Pid=" + pId);
                    //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                       // _memoryCache.Remove("UList");
                        _memoryCache.Remove("Notifications");
                        var data = await response.Content.ReadAsStringAsync();
                        objOutPut.NotificationList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserNotificationsDTO>>(data);
                    }

                    objOutPut.ProjectId = 0;
                    objOutPut.AccessList = empSession.AccessList.ToList();
                    objOutPut.UserType = HttpContext.Session.GetInt32("UserType");

                    return View(objOutPut);
                }

            }

            return View(objOutPut);
        }


        [HttpGet]
        [Route("Organization/ViewComments/{epId}")]
        public async Task<IActionResult> ViewComments(string epId)
        {
            UserNotificationsDTO objOutPut = new UserNotificationsDTO();
          // int? pId =CommonHelper.DecryptURLHTML(epId);
            int? pId = Convert.ToInt32(CommonHelper.DecryptURLHTML(epId));
            string? strMenuSession = HttpContext.Session.GetString("Menu");
            if (strMenuSession == null)
                return RedirectToAction("Index", "Home");

            AccessMasterDTO? empSession = (AccessMasterDTO?)(JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession));


            if (HttpContext.Session.GetInt32("UserId") > 0)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiCommentsUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    //HttpResponseMessage response = await client.GetAsync("GetUserListing?userType=" + userType + "&logInUser="+ logInUser);
                    HttpResponseMessage response = await client.GetAsync("GetNotificationDetails?UserId=" + HttpContext.Session.GetInt32("UserId") + "&Pid=" + pId);
                    //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        // _memoryCache.Remove("UList");
                        _memoryCache.Remove("Notifications");
                        var data = await response.Content.ReadAsStringAsync();
                        objOutPut.NotificationList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserNotificationsDTO>>(data);
                    }

                    objOutPut.ProjectId = 0;
                    objOutPut.AccessList = empSession.AccessList.ToList();
                    objOutPut.UserType = HttpContext.Session.GetInt32("UserType");

                    return View(objOutPut);
                }

            }

            return View(objOutPut);
        }

        [HttpPost]
        public async Task<IActionResult>? SaveCompany(UsersMasterDTO inputs, List<IFormFile> formFile)
        {
            UsersMasterDTO OutPut = new UsersMasterDTO();
            inputs.IsActive = true;
            inputs.CreatedBy = 1;
            inputs.CreatedOn = DateTime.Now;
            inputs.UnitId = HttpContext.Session.GetInt32("UserId");
            inputs.IsAdmin = true;
            inputs.LoginPassword = CommonHelper.Encrypt(CommonHelper.RandomString());
            inputs.Base64ProfileImage = "";
            //if (formFile.Count > 0)
            //{
            //    foreach (IFormFile postedFile in formFile)
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

            if (formFile.Count > 0)
            {
                foreach (IFormFile postedFile in formFile)
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
                            return View("AddCompany", OutPut);
                        else
                            return RedirectToAction("CompanyManagement", "Organization");

                    }

                }


            }
            catch (SystemException ex)
            {
                OutPut.DisplayMessage = "User failed!" + ex.Message;
            }
            return RedirectToAction("CompanyManagement", "Organization");
        }


        public async Task<List<UsersMasterDTO>>? GetCompanyList()
        {
            DataTable dt = new DataTable();
            List<UsersMasterDTO> UserLst = new List<UsersMasterDTO>();
          int? userId= HttpContext.Session.GetInt32("UserId");
           // int countryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ecountryId));
            //  inputs.CountryId = countryId;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiUserUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                //HttpResponseMessage response = await client.GetAsync("GetUserListing?userType=" + userType + "&logInUser="+ logInUser);
                HttpResponseMessage response = await client.GetAsync("GetCompanyDashboard?UserId=" + userId);
                //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    UserLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UsersMasterDTO>>(data);

                    foreach (var item in UserLst)
                    {

                        item.EnycUserId = CommonHelper.EncryptURLHTML(item.UserId.ToString());
                        //if (item.ProfileImage != null)
                        //    item.Base64ProfileImage = "data:image/png;base64," + Convert.ToBase64String(item.ProfileImage, 0, item.ProfileImage.Length);

                    }



                }

                return UserLst;
            }






        }
        public async Task<IActionResult> MasterPhases()
        {

            PhasesMasterDTO OutPut = new PhasesMasterDTO();
            OutPut.PhasesList = await GetPhasesList();
            return View(OutPut);
        }
        public async Task<IActionResult> MasterStatus()
        {
            StatusMasterDTO OutPut = new StatusMasterDTO();
            OutPut.StatusMasterList = await GetStatusList();
            return View(OutPut);
        }
        public async Task<IActionResult> MasterPriority()
        {
            PriorityMasterDTO OutPut = new PriorityMasterDTO();
            OutPut.PriorityMasterList = await GetPriorityList();
            return View(OutPut);
        }

        public async Task<List<PriorityMasterDTO>> GetPriorityList()
        {
            // DataTable dt = new DataTable();

            List<PriorityMasterDTO> PriorityLst = new List<PriorityMasterDTO>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetPriority");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    PriorityLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PriorityMasterDTO>>(data);

                    if(PriorityLst.Count>0)
                    {
                        foreach(var item in PriorityLst)
                        {
                            item.EncPriorityId = CommonHelper.EncryptURLHTML(item.PriorityId.ToString());
                        }
                       
                    }
                }


            }
            return PriorityLst;
        }

        public async Task<List<PhasesMasterDTO>> GetPhasesList()
        {
            // DataTable dt = new DataTable();

            List<PhasesMasterDTO> PhasesLst = new List<PhasesMasterDTO>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetPhases");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    PhasesLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PhasesMasterDTO>>(data);

                    if (PhasesLst.Count > 0)
                    {
                        foreach (var item in PhasesLst)
                        {
                            item.EncryptedId = CommonHelper.EncryptURLHTML(item.PhaseID.ToString());
                        }

                    }
                }

            }
            return PhasesLst;
        }

        public async Task<List<StatusMasterDTO>> GetStatusList()
        {
            // DataTable dt = new DataTable();

            List<StatusMasterDTO> StatusLst = new List<StatusMasterDTO>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetStatus");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    StatusLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<StatusMasterDTO>>(data);

                    if (StatusLst.Count > 0)
                    {
                        foreach (var item in StatusLst)
                        {
                            item.EncryptedId = CommonHelper.EncryptURLHTML(item.StatusId.ToString());
                        }

                    }
                }

            }
            return StatusLst;
        }

        [HttpPost]
        public async Task<IActionResult>? PriorityMaster(PriorityMasterDTO inputs)
        {
             PriorityMasterDTO OutPut = new PriorityMasterDTO();
            inputs.IsActive = true;
            //OutPut.Priority = inputs.Priority;
            //OutPut.PriorityId = inputs.PriorityId;
           
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("PriorityMaster", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        //  dt = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Data.DataTable>(data);
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                        OutPut.HttpStatusCode = result.HttpStatusCode;
                        OutPut.DisplayMessage = result.DisplayMessage;

                        OutPut.PriorityMasterList = await GetPriorityList();
                    }

                }


            }
            catch (SystemException ex)
            {
                OutPut.DisplayMessage = "Designation failed!" + ex.Message;
            }
            return View("MasterPriority", OutPut);
        }

        [HttpPost]
        public async Task<IActionResult>? PhasesMaster(PhasesMasterDTO inputs)
        {
            PhasesMasterDTO OutPut = new PhasesMasterDTO();
            inputs.IsActive = true;
            //OutPut.Priority = inputs.Priority;
            //OutPut.PriorityId = inputs.PriorityId;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("PhasesMaster", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        //  dt = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Data.DataTable>(data);
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                        OutPut.HttpStatusCode = result.HttpStatusCode;
                        OutPut.DisplayMessage = result.DisplayMessage;

                        OutPut.PhasesList = await GetPhasesList();
                    }

                }


            }
            catch (SystemException ex)
            {
                OutPut.DisplayMessage = "Designation failed!" + ex.Message;
            }
            return View("MasterPhases", OutPut);
        }


        [HttpPost]
        public async Task<IActionResult>? StatusMaster(StatusMasterDTO inputs)
        {
            StatusMasterDTO OutPut = new StatusMasterDTO();
            inputs.IsActive = true;
            //OutPut.Priority = inputs.Priority;
            //OutPut.PriorityId = inputs.PriorityId;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("StatusMaster", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        //  dt = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Data.DataTable>(data);
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                        OutPut.HttpStatusCode = result.HttpStatusCode;
                        OutPut.DisplayMessage = result.DisplayMessage;

                        OutPut.StatusMasterList = await GetStatusList();
                    }

                }


            }
            catch (SystemException ex)
            {
                OutPut.DisplayMessage = "Designation failed!" + ex.Message;
            }
            return View("MasterStatus", OutPut);
        }

        [HttpGet]
        [Route("Organization/GetPhasesById/{eId}")]
        public async Task<IActionResult>? GetPhasesById(string eId)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            PhasesMasterDTO OutPut = new PhasesMasterDTO();

            int Id = Convert.ToInt32(CommonHelper.DecryptURLHTML(eId));
            if (Id > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("GetPhaseById?phaseId=" + Id);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            OutPut = Newtonsoft.Json.JsonConvert.DeserializeObject<PhasesMasterDTO>(data);                          

                            OutPut.PhasesList = await GetPhasesList();
                        }
                    }
                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "Clinic failed!" + ex.Message;
                }

            }
            return View("MasterPhases", OutPut);
        }

        [HttpGet]
        [Route("Organization/GetPriorityById/{eId}")]
        public async Task<IActionResult>? GetPriorityById(string eId)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            PriorityMasterDTO OutPut = new PriorityMasterDTO();

            int Id = Convert.ToInt32(CommonHelper.DecryptURLHTML(eId));
            if (Id > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("GetPriorityById?pId=" + Id);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            OutPut = Newtonsoft.Json.JsonConvert.DeserializeObject<PriorityMasterDTO>(data);

                            OutPut.PriorityMasterList = await GetPriorityList();
                        }
                    }
                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "Clinic failed!" + ex.Message;
                }

            }
            return View("MasterPriority", OutPut);
        }

        [HttpGet]
        [Route("Organization/GetStatusById/{eId}")]
        public async Task<IActionResult>? GetStatusById(string eId)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            StatusMasterDTO OutPut = new StatusMasterDTO();

            int Id = Convert.ToInt32(CommonHelper.DecryptURLHTML(eId));
            if (Id > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("GetStatusById?pId=" + Id);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            OutPut = Newtonsoft.Json.JsonConvert.DeserializeObject<StatusMasterDTO>(data);

                            OutPut.StatusMasterList = await GetStatusList();
                        }
                    }
                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "Clinic failed!" + ex.Message;
                }

            }
            return View("MasterStatus", OutPut);
        }

        [HttpGet]
       // [Route("Organization/RemoveNotification/{Id}")]
        public async Task<bool>? RemoveNotification(int Id)
        {          
            var resp = false;         
            if (Id > 0)
            {
             
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiCommentsUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("RemoveNotification?Id=" + Id);                      
                        if (response.IsSuccessStatusCode)
                        {
                            _memoryCache.Remove("Notifications");
                            var data = await response.Content.ReadAsStringAsync();
                            resp = Newtonsoft.Json.JsonConvert.DeserializeObject<bool>(data);
                        }
                    }
                }
                catch (SystemException ex)
                {
                    resp = false;
                }

            }
            return resp;
        }
    }

   
}
