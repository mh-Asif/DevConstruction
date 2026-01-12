using Construction.Infrastructure.Helper;
using Construction.Infrastructure.KeyValues;
using Construction.Infrastructure.Models;
using ConstructionApp.WebUI.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace ConstructionApp.WebUI.Controllers
{
    public class SubTaskController : Controller
    {
        private readonly ILogger<SubTaskController> _logger;
        private readonly IMemoryCache _memoryCache;

        public SubTaskController(ILogger<SubTaskController> logger,IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache;
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
                    PriorityLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UnitPriorityKeyValues>>(data);

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
                    PriorityLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UnitPhaseKeyValues>>(data);

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
                    PriorityLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UnitStatusKeyValues>>(data);

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
        public async Task<OutPutResponse>? SaveSubTask(ProjectSubTasksDTO inputs)
        {
            ProjectSubTasksDTO OutPut = new ProjectSubTasksDTO();
            OutPutResponse outResponse = new OutPutResponse();
        //    ClientMail obj = new ClientMail();
          //  string? strMenuSession = HttpContext.Session.GetString("Menu");
          //  if (strMenuSession == null)
            //    return RedirectToAction("Index", "Home");

          //  AccessMasterDTO? empSession = (AccessMasterDTO?)(JsonConvert.DeserializeObject<AccessMasterDTO?>(strMenuSession));
           // OutPut.AccessList = empSession.AccessList;
            OutPut.UserType = HttpContext.Session.GetInt32("UserType");
            inputs.IsActive = true;
            inputs.CreatedBy = HttpContext.Session.GetInt32("UserId");
            // inputs.IsAdmin = false;
            inputs.CreatedOn = DateTime.Now;
            //inputs.StartDate = inputs.SubTaskStartDate;
            //inputs.EndDate = inputs.SubTaskEndDate;
            //inputs.Collaborators = inputs.SubCollaborators;
            inputs.UnitId = HttpContext.Session.GetInt32("UnitId");
            // inputs.LoginPassword = CommonHelper.Encrypt(CommonHelper.RandomString());
            //  inputs.FileDetails = "";
            OutPut.ProjectId = inputs.ProjectId;
            inputs.EncryptedId = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiSubTaskUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("SaveSubTask", inputs);
                    if (response.IsSuccessStatusCode)
                    {

                       // _memoryCache.Remove("Comments");
                        //_memoryCache.Remove("Notifications");

                        UserCommentsDTO commentsInputs = new UserCommentsDTO();
                        var data = await response.Content.ReadAsStringAsync();

                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);
                        outResponse.HttpStatusCode = result.HttpStatusCode;
                        outResponse.DisplayMessage = result.DisplayMessage;
                        // OutPut.TaskId = result.RespId;
                        if (inputs.SubTaskId == 0)
                        {
                           
                            commentsInputs.TaskId = result.RespId;
                            commentsInputs.ProjectId = inputs.ProjectId;
                            commentsInputs.SubTaskId = inputs.SubTaskId;
                            commentsInputs.Comments = "Created Sub Task :" + inputs.SubTaskName;
                            commentsInputs.Summary = "Sub Task Management";
                             AddUserComments(commentsInputs);

                          OutPut.IsCreated = true;
                          //  OutPut.ProjectTaskList = await GetTaskDetailsForMail(result.RespId);
                          //  OutPut.TaskRoleSummeryList = await GetTaskRoleSummery(HttpContext.Session.GetInt32("UnitId"), result.RespId);
                           // await obj.SendTaskMail(OutPut);

                        }
                        else
                        {

                            commentsInputs.TaskId = inputs.TaskId;
                            commentsInputs.ProjectId = inputs.ProjectId;
                            commentsInputs.SubTaskId = inputs.SubTaskId;
                            commentsInputs.Comments = "Updated Sub Task :" + inputs.SubTaskName;
                            commentsInputs.Summary = "Sub Task Management";
                            AddUserComments(commentsInputs);

                            OutPut.IsCreated = false;
                            //OutPut.ProjectTaskList = await GetTaskDetailsForMail(inputs.TaskId);
                            //OutPut.TaskRoleSummeryList = await GetTaskRoleSummery(HttpContext.Session.GetInt32("UnitId"), inputs.TaskId);
                            //await obj.SendTaskMail(OutPut);
                        }

                      //  OutPut.ProjectList = await GetProject();
                       // OutPut.UserList = await GetUsers();
                       // OutPut.VendorList = await GetUsersList(3);
                       //// OutPut.PhaseList = await GetUnitPhaseList();
                       // OutPut.PriorityList = await GetUnitPriorityList();
                       // OutPut.StatusList = await GetUnitStatusList();
                       // OutPut.ProjectTaskList = await GetTaskDashboard(HttpContext.Session.GetInt32("UserId"), inputs.ProjectId);
                        // if (OutPut.HttpStatusCode != 200)
                       
                        // else
                        // return RedirectToAction("TaskManagement", "Task");

                    }

                }


            }
            catch (SystemException ex)
            {
                OutPut.DisplayMessage = "Project failed!" + ex.Message;
            }
            return outResponse;
        }

        public async Task<int> AddUserComments(UserCommentsDTO inputs)
        {
            try
            {
                UserNotificationsDTO objNotification = new UserNotificationsDTO();
                inputs.IsActive = true;
                inputs.UserId = HttpContext.Session.GetInt32("UserId");
                objNotification.UserId = HttpContext.Session.GetInt32("UserId");
                objNotification.ProjectId = inputs.ProjectId;
                objNotification.TaskId = inputs.TaskId;
                objNotification.SubTaskId = inputs.SubTaskId;
                objNotification.Heading = inputs.Summary;
                objNotification.NotifyMessage = inputs.Comments;
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
                        AddNotification(objNotification);
                    }

                }


            }
            catch (SystemException ex)
            {

            }

            return 1;
        }

        public async Task<int> AddNotification(UserNotificationsDTO inputs)
        {
            try
            {
                inputs.Base64ProfileImage = "";
                // inputs.IsActive = true;
                // inputs.UserId = HttpContext.Session.GetInt32("UserId");

                // inputs.Comments = comments;
                // inputs.IsAdmin = false;
                inputs.CreationDate = DateTime.Now;

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiCommentsUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("SaveNotification", inputs);

                    if (response.IsSuccessStatusCode)
                    {
                       // _memoryCache.Remove("Comments");
                        _memoryCache.Remove("Notifications");
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

        [HttpGet]
        //public async Task<ProjectSubTasksDTO>? GetSubTaskView(string epId)
        public async Task<ProjectSubTasksDTO>? GetSubTaskView(int? pId)
        {
            //  CommentsDashboardDTO inputs = new CommentsDashboardDTO();
            ProjectSubTasksDTO objOutPut = new ProjectSubTasksDTO();
                
           // int pId = Convert.ToInt32(CommonHelper.DecryptURLHTML(epId));
            if (pId > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {


                        client.BaseAddress = new Uri(EnvironmentUrl.apiSubTaskUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("GetSubTaskById?pId=" + pId);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            objOutPut = Newtonsoft.Json.JsonConvert.DeserializeObject<ProjectSubTasksDTO>(data);

                        }

                    }

                }
                catch (SystemException ex)
                {
                    objOutPut.DisplayMessage = "Task failed!" + ex.Message;
                }

            }
            return  objOutPut;
        }


        [HttpGet]
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
                    ProjectLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SubTaskDashboardDTO>>(data);

                    foreach (var item in ProjectLst)
                    {

                        item.eSubTaskId = CommonHelper.EncryptURLHTML(item.SubTaskId.ToString());

                    }



                }

                return ProjectLst;
            }






        }

        public async Task<bool>? DeleteSubTask(int pId)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
           // SubTaskDashboardDTO OutPut = new SubTaskDashboardDTO();
            bool isDeleted = false;
            if (pId > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiSubTaskUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("DeleteSubTask?pId=" + pId);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                           
                            var data = await response.Content.ReadAsStringAsync();
                            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);
                            isDeleted = true;
                          
                        }

                    }
                }
                catch (SystemException ex)
                {
                    isDeleted = false;
                }

            }
            return isDeleted;
        }
    }
}
