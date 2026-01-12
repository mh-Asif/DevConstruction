using Construction.Infrastructure.Helper;
using Construction.Infrastructure.KeyValues;
using Construction.Infrastructure.Models;
using ConstructionApp.WebUI.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;

namespace ConstructionApp.WebUI.Controllers
{
    public class MasterController : Controller
    {
        //string apiUrl = "https://localhost:7013/api/MasterAPIs/";
        private readonly ILogger<MasterController> _logger;
        private readonly IMemoryCache _memoryCache;

        public MasterController(ILogger<MasterController> logger,IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache;

        }
        public async Task<IActionResult> Country()
        {
            CountryMasterDTO objOutPut = new CountryMasterDTO();
            objOutPut.CountryMasterList = await GetCountryList();
            return View(objOutPut);
            // return View(objOutPut);
        }
        public async Task<IActionResult> DocumentCategory()
        {
            DocumentCategoryDTO objOutPut = new DocumentCategoryDTO();
            objOutPut.DocumentCategoryList = await GetDocumentCategoryList();
            ViewBag.EnvironmentUrl = EnvironmentUrl.apiUrl;
            return View(objOutPut);
            // return View(objOutPut);
        }
        public async Task<IActionResult> DrawingCategory()
        {
            DrawingCategoryDTO objOutPut = new DrawingCategoryDTO();
            objOutPut.DrawingCategoryList = await GetDrawingCategoryList();
            ViewBag.EnvironmentUrl = EnvironmentUrl.apiUrl;
            return View(objOutPut);
            // return View(objOutPut);
        }
        public async Task<IActionResult> PhotoCategory()
        {
            PhotoCategoryDTO objOutPut = new PhotoCategoryDTO();
            objOutPut.PhotoCategoryList = await GetPhotoCategoryList();
            ViewBag.EnvironmentUrl = EnvironmentUrl.apiUrl;
            return View(objOutPut);
            // return View(objOutPut);
        }
        public async Task<IActionResult> State()
        {
            StateMasterDTO objOutPut = new StateMasterDTO();
            objOutPut.StateMasterList = await GetStateList();
            objOutPut.CountryList = await CountryList();
            return View(objOutPut);
        }
        public async Task<IActionResult> City()
        {
            CityMasterDTO objOutPut = new CityMasterDTO();
            objOutPut.CityMasterList = await GetCityList();
            objOutPut.CountryList = await CountryList();
            return View(objOutPut);
        }
        public async Task<IActionResult> Department()
        {
            DepartmentMasterDTO objOutPut = new DepartmentMasterDTO();
            objOutPut.DepartmentMasterList = await GetDepartmentList();
            return View(objOutPut);
        }
        public async Task<IActionResult> Designation()
        {
            JobTitleMasterDTO objOutPut = new JobTitleMasterDTO();
            objOutPut.JobTitleMasterList = await GetDesignationList();
            return View(objOutPut);
        }
        public async Task<IActionResult> Role()
        {
            RoleMasterDTO objOutPut = new RoleMasterDTO();
            objOutPut.RoleMasterList = await GetRoleList();
            return View(objOutPut);
        }

        public async Task<IActionResult> Priority()
        {
            CompanyPriorityMasterDTO objOutPut = new CompanyPriorityMasterDTO();
            objOutPut.PriorityMasterList = await GetPriorityList();
            objOutPut.CompanyPriorityList = await GetUnitPriorityList();

            return View(objOutPut);
        }

        public async Task<IActionResult> UnitCategory()
        {
            UnitCategoryMasterDTO objOutPut = new UnitCategoryMasterDTO();
            objOutPut.MasterCategoryList = await GetCategoryList();
            objOutPut.UnitCategoryList = await GetUnitCategoryList();

            return View(objOutPut);
        }

        public async Task<List<CountryMasterDTO>> GetCountryList()
        {
            DataTable dt = new DataTable();

            List<CountryMasterDTO> CountriesLst = new List<CountryMasterDTO>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetCountry");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    CountriesLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CountryMasterDTO>>(data);
                    foreach (var item in CountriesLst)
                    {
                        item.EncryptedCountryId = CommonHelper.EncryptURLHTML(item.CountryId.ToString());
                    }
                }

            }
            return CountriesLst;
        }

        public async Task<List<CountryKeyValues>> CountryList()
        {
            DataTable dt = new DataTable();

            List<CountryKeyValues> CountriesLst = new List<CountryKeyValues>();
            // CountryKeyValues CountriesLst = new CountryKeyValues();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetCountry");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    dt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(data);
                }
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            CountryKeyValues objDTO = new CountryKeyValues();

                            objDTO.CountryId = Convert.ToInt32(dr["CountryId"]);
                            // objDTO.CountryCode = Convert.ToString(dr["CountryCode"]);
                            objDTO.CountryName = Convert.ToString(dr["CountryName"]);
                            //objDTO.EncryptedCountryId = CommonHelper.EncryptURLHTML(objDTO.CountryId.ToString());
                            CountriesLst.Add(objDTO);
                        }
                    }
                }


            }
            return CountriesLst;
        }

        [HttpPost]
        public async Task<IActionResult>? CountryMaster(CountryMasterDTO inputs)
        {
            CountryMasterDTO OutPut = new CountryMasterDTO();
            // inputs.IsActive = true;
            DataTable dt = new DataTable();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("CountryMaster", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        //  dt = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Data.DataTable>(data);
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                        OutPut.HttpStatusCode = result.HttpStatusCode;
                        OutPut.DisplayMessage = result.DisplayMessage;

                        OutPut.CountryMasterList = await GetCountryList();
                    }

                }


            }
            catch (SystemException ex)
            {
                OutPut.DisplayMessage = "Clinic failed!" + ex.Message;
            }
            return View("Country", OutPut);
        }

        [HttpGet]
        [Route("Master/GetCountryId/{ecountryId}")]
        public async Task<IActionResult>? GetCountryId(string ecountryId)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            CountryMasterDTO OutPut = new CountryMasterDTO();

            int countryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ecountryId));
            if (countryId > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("GetCountryById?countryId=" + countryId);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            OutPut = Newtonsoft.Json.JsonConvert.DeserializeObject<CountryMasterDTO>(data);
                            // var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                            //OutPut.HttpStatusCode = result.HttpStatusCode;
                            // OutPut.DisplayMessage = result.DisplayMessage;

                            OutPut.CountryMasterList = await GetCountryList();
                        }

                    }


                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "Clinic failed!" + ex.Message;
                }

            }
            return View("Country", OutPut);
        }

        [HttpGet]
        [Route("Master/GetStateId/{estateId}")]
        public async Task<IActionResult>? GetStateId(string estateId)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            StateMasterDTO OutPut = new StateMasterDTO();

            int stateId = Convert.ToInt32(CommonHelper.DecryptURLHTML(estateId));
            if (stateId > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("GetStateById?stateId=" + stateId);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            OutPut = Newtonsoft.Json.JsonConvert.DeserializeObject<StateMasterDTO>(data);
                            OutPut.CountryList = await CountryList();
                            OutPut.StateMasterList = await GetStateList();
                        }

                    }


                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "Clinic failed!" + ex.Message;
                }

            }
            return View("State", OutPut);
        }

        [HttpGet]
        [Route("Master/GetCityById/{ecityId}")]
        public async Task<IActionResult>? GetCityById(string ecityId)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            CityMasterDTO OutPut = new CityMasterDTO();

            int cityId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ecityId));
            if (cityId > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("GetCityById?cityId=" + cityId);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            OutPut = Newtonsoft.Json.JsonConvert.DeserializeObject<CityMasterDTO>(data);
                            OutPut.CityMasterList = await GetCityList();
                            OutPut.CountryList = await CountryList();
                        }

                    }


                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "Clinic failed!" + ex.Message;
                }

            }
            return View("City", OutPut);
        }

        [HttpGet]
        [Route("Master/GetDeptById/{eDeptId}")]
        public async Task<IActionResult>? GetDeptById(string eDeptId)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            DepartmentMasterDTO OutPut = new DepartmentMasterDTO();

            int DeptId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eDeptId));
            if (DeptId > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("GetDepartmentById?deptId=" + DeptId);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {

                            var data = await response.Content.ReadAsStringAsync();
                            OutPut = Newtonsoft.Json.JsonConvert.DeserializeObject<DepartmentMasterDTO>(data);
                            OutPut.DepartmentMasterList = await GetDepartmentList();
                        }

                    }


                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "Clinic failed!" + ex.Message;
                }

            }
            return View("Department", OutPut);
        }

        [HttpGet]
        [Route("Master/GetDesignationById/{eDesigId}")]
        public async Task<IActionResult>? GetDesignationById(string eDesigId)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            JobTitleMasterDTO OutPut = new JobTitleMasterDTO();

            int DesigId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eDesigId));
            if (DesigId > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("GetDesignationById?desigId=" + DesigId);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            OutPut = Newtonsoft.Json.JsonConvert.DeserializeObject<JobTitleMasterDTO>(data);
                            OutPut.JobTitleMasterList = await GetDesignationList();
                        }

                    }


                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "Clinic failed!" + ex.Message;
                }

            }
            return View("Designation", OutPut);
        }

        [HttpGet]
        [Route("Master/DeleteCountry/{ecountryId}")]
        public async Task<IActionResult>? DeleteCountry(string ecountryId)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            CountryMasterDTO OutPut = new CountryMasterDTO();

            int countryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ecountryId));
            if (countryId > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("DeleteCountry?countryId=" + countryId);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                            OutPut.HttpStatusCode = result.HttpStatusCode;
                            OutPut.DisplayMessage = result.DisplayMessage;
                            OutPut.CountryMasterList = await GetCountryList();
                        }

                    }

                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "Clinic failed!" + ex.Message;
                }

            }
            return View("Country", OutPut);
        }

        [HttpGet]
       [Route("Master/DeleteState/{estateId}")]
        public async Task<IActionResult>? DeleteState(string estateId)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            StateMasterDTO OutPut = new StateMasterDTO();

            int stateId = Convert.ToInt32(CommonHelper.DecryptURLHTML(estateId));
            if (stateId > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("DeleteState?stateId=" + stateId);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                            OutPut.HttpStatusCode = result.HttpStatusCode;
                            OutPut.DisplayMessage = result.DisplayMessage;
                            OutPut.StateMasterList = await GetStateList();
                        }

                    }

                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "Clinic failed!" + ex.Message;
                }

            }
            return View("State",OutPut);
        }

        public async Task<List<StateMasterDTO>> GetStateList()
        {
            DataTable dt = new DataTable();

            List<StateMasterDTO> StateLst = new List<StateMasterDTO>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetState");
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
                            StateMasterDTO objDTO = new StateMasterDTO();

                            objDTO.CountryId = Convert.ToInt32(dr["CountryId"]);
                            objDTO.StateName = Convert.ToString(dr["StateName"]);
                            objDTO.CountryName = Convert.ToString(dr["CountryName"]);
                            objDTO.StateId = Convert.ToInt32(dr["StateId"]);
                            objDTO.EncryptedStateId = CommonHelper.EncryptURLHTML(objDTO.StateId.ToString());
                            StateLst.Add(objDTO);
                        }
                    }
                }


            }
            return StateLst;
        }


        [HttpGet]
        //[Route("Master/GetCountryId/{ecountryId}")]
        public async Task<List<StateKeyValues>>? GetCountryState(int countryId)
        {
            DataTable dt = new DataTable();
            List<StateKeyValues> StateLst = new List<StateKeyValues>();

            // int countryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ecountryId));
            //  inputs.CountryId = countryId;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("GetCountryState?countryId=" + countryId);
                //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
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
                            StateKeyValues objDTO = new StateKeyValues();

                            // objDTO.CountryId = Convert.ToInt32(dr["CountryId"]);
                            objDTO.StateName = Convert.ToString(dr["StateName"]);
                            // objDTO.CountryName = Convert.ToString(dr["CountryName"]);
                            objDTO.StateId = Convert.ToInt32(dr["StateId"]);
                            // objDTO.EncryptedStateId = CommonHelper.EncryptURLHTML(objDTO.StateId.ToString());
                            StateLst.Add(objDTO);
                        }
                    }
                }
                return StateLst;
            }






        }
        [HttpPost]
        public async Task<IActionResult>? StateMaster(StateMasterDTO inputs)
        {
            StateMasterDTO OutPut = new StateMasterDTO();
            inputs.IsActive = true;
            DataTable dt = new DataTable();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("StateMaster", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        //  dt = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Data.DataTable>(data);
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                        OutPut.HttpStatusCode = result.HttpStatusCode;
                        OutPut.DisplayMessage = result.DisplayMessage;

                        OutPut.StateMasterList = await GetStateList();
                        OutPut.CountryList = await CountryList();
                    }

                }


            }
            catch (SystemException ex)
            {
                OutPut.DisplayMessage = "State failed!" + ex.Message;
            }
            return View("State", OutPut);
        }



        public async Task<List<CityMasterDTO>> GetCityList()
        {
            DataTable dt = new DataTable();

            List<CityMasterDTO> CityLst = new List<CityMasterDTO>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetCity");
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
                            CityMasterDTO objDTO = new CityMasterDTO();

                            // objDTO.CountryId = Convert.ToInt32(dr["CountryId"]);
                            objDTO.StateName = Convert.ToString(dr["StateName"]);
                            objDTO.CityName = Convert.ToString(dr["CityName"]);
                            objDTO.CountryName = Convert.ToString(dr["CountryName"]);
                            // objDTO.StateId = Convert.ToInt32(dr["StateId"]);
                            objDTO.CityId = Convert.ToInt32(dr["CityId"]);
                            objDTO.EncryptedCityId = CommonHelper.EncryptURLHTML(objDTO.CityId.ToString());
                            CityLst.Add(objDTO);
                        }
                    }
                }


            }
            return CityLst;
        }

        [HttpPost]
        public async Task<IActionResult>? CityMaster(CityMasterDTO inputs)
        {
            CityMasterDTO OutPut = new CityMasterDTO();
            inputs.IsActive = true;
            //  DataTable dt = new DataTable();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("CityMaster", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        //  dt = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Data.DataTable>(data);
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                        OutPut.HttpStatusCode = result.HttpStatusCode;
                        OutPut.DisplayMessage = result.DisplayMessage;

                        OutPut.CityMasterList = await GetCityList();
                        OutPut.CountryList = await CountryList();
                    }

                }


            }
            catch (SystemException ex)
            {
                OutPut.DisplayMessage = "City failed!" + ex.Message;
            }
            return View("City", OutPut);
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

        [HttpPost]
        public async Task<IActionResult>? DeptMaster(DepartmentMasterDTO inputs)
        {
            DepartmentMasterDTO OutPut = new DepartmentMasterDTO();
            inputs.IsActive = true;
            //  DataTable dt = new DataTable();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("DepartmentMaster", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        //  dt = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Data.DataTable>(data);
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                        OutPut.HttpStatusCode = result.HttpStatusCode;
                        OutPut.DisplayMessage = result.DisplayMessage;

                        OutPut.DepartmentMasterList = await GetDepartmentList();
                    }

                }


            }
            catch (SystemException ex)
            {
                OutPut.DisplayMessage = "Dept failed!" + ex.Message;
            }
            return View("Department", OutPut);
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

        [HttpPost]
        public async Task<IActionResult>? DesignationMaster(JobTitleMasterDTO inputs)
        {
            JobTitleMasterDTO OutPut = new JobTitleMasterDTO();
            inputs.IsActive = true;
            //  DataTable dt = new DataTable();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("JobTitle", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        //  dt = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Data.DataTable>(data);
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                        OutPut.HttpStatusCode = result.HttpStatusCode;
                        OutPut.DisplayMessage = result.DisplayMessage;

                        OutPut.JobTitleMasterList = await GetDesignationList();
                    }

                }


            }
            catch (SystemException ex)
            {
                OutPut.DisplayMessage = "Designation failed!" + ex.Message;
            }
            return View("Designation", OutPut);
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


        [HttpPost]
        public async Task<IActionResult>? RoleMaster(RoleMasterDTO inputs)
        {
            RoleMasterDTO OutPut = new RoleMasterDTO();
            inputs.IsActive = true;
            //  DataTable dt = new DataTable();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("RoleMaster", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        //  dt = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Data.DataTable>(data);
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                        OutPut.HttpStatusCode = result.HttpStatusCode;
                        OutPut.DisplayMessage = result.DisplayMessage;

                        OutPut.RoleMasterList = await GetRoleList();
                    }

                }


            }
            catch (SystemException ex)
            {
                OutPut.DisplayMessage = "Designation failed!" + ex.Message;
            }
            return View("Role", OutPut);
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

                    if (PriorityLst.Count > 0)
                    {
                        foreach (var item in PriorityLst)
                        {
                            item.EncPriorityId = CommonHelper.EncryptURLHTML(item.PriorityId.ToString());
                        }

                    }
                }


            }
            return PriorityLst;
        }

        public async Task<List<CompanyPriorityMasterDTO>> GetUnitPriorityList()
        {
            // DataTable dt = new DataTable();
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            List<CompanyPriorityMasterDTO> PriorityLst = new List<CompanyPriorityMasterDTO>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetUnitPriority?unitId=" + unitId);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    PriorityLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CompanyPriorityMasterDTO>>(data);

                    if (PriorityLst.Count > 0)
                    {
                        foreach (var item in PriorityLst)
                        {
                            item.EncCompanyPriorityId = CommonHelper.EncryptURLHTML(item.CompanyPriorityId.ToString());
                        }

                    }
                }


            }
            return PriorityLst;
        }

        [HttpPost]
        public async Task<string> MasterPriority(UserAction userAction)
        {
            // CompanyPriorityMasterDTO OutPutData;
            // OutPutData.IsActive = true;
            string OutPut = null;
            //OutPut.Priority = inputs.Priority;
            //OutPut.PriorityId = inputs.PriorityId;
            string[] strPrioritylst = userAction.Priority.Split(',');
            string[] strPriorityIdlst = userAction.PriorityIds.Split(',');

            try
            {
                if (strPriorityIdlst.Length > 0)
                {
                    for (int id = 0; id < strPriorityIdlst.Length; id++)
                    {
                        UnitPriorityKeyValues OutPutData = new UnitPriorityKeyValues();
                        OutPutData.IsActive = true;
                        OutPutData.CompanyPriority = strPrioritylst[id];
                        OutPutData.UnitPriorityId = Convert.ToInt32(strPriorityIdlst[id]);
                        OutPutData.CompanyId = HttpContext.Session.GetInt32("UnitId");

                        //OutPutData.CompanyPriority = "Asif";
                        //OutPutData.CompanyPriorityId = 2;
                        //OutPutData.CompanyId =8;

                        using (HttpClient client = new HttpClient())
                        {
                            client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                            HttpResponseMessage response = await client.PostAsJsonAsync("UnitPriority", OutPutData);
                            if (response.IsSuccessStatusCode)
                            {
                                var data = await response.Content.ReadAsStringAsync();
                                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                                OutPut = result.DisplayMessage;

                                //  OutPut.PriorityMasterList = await GetPriorityList();
                            }

                        }
                    }
                }
            }
            catch (SystemException ex)
            {
                OutPut = "Priority failed!" + ex.Message;
            }
            return OutPut;
        }

        [HttpPost]
        public async Task<IActionResult>? UnitPriorityMaster(CompanyPriorityMasterDTO inputs)
        {
            CompanyPriorityMasterDTO OutPut = new CompanyPriorityMasterDTO();
            inputs.IsActive = true;
            inputs.CompanyId = HttpContext.Session.GetInt32("UnitId");
            //OutPut.Priority = inputs.Priority;
            //OutPut.PriorityId = inputs.PriorityId;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("UnitPriority", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        //  dt = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Data.DataTable>(data);
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                        OutPut.HttpStatusCode = result.HttpStatusCode;
                        OutPut.DisplayMessage = result.DisplayMessage;

                        OutPut.PriorityMasterList = await GetPriorityList();
                        OutPut.CompanyPriorityList = await GetUnitPriorityList();
                    }

                }


            }
            catch (SystemException ex)
            {
                OutPut.DisplayMessage = "Designation failed!" + ex.Message;
            }
            return View("Priority", OutPut);
        }


        public async Task<IActionResult> UnitPhases()
        {

            UnitPhasesMasterDTO OutPut = new UnitPhasesMasterDTO();
            OutPut.MasterPhasesList = await GetMasterPhasesList();
            OutPut.UnitPhasesList = await GetUnitPhasesList();
            return View(OutPut);
        }

        public async Task<IActionResult> UnitStatus()
        {

            UnitStatusMasterDTO OutPut = new UnitStatusMasterDTO();
            OutPut.StatusMasterList = await GetMasterStatusList();
            OutPut.UnitStatusMasterList = await GetUnitStatusList();
            return View(OutPut);
        }

        public async Task<List<PhasesMasterDTO>> GetMasterPhasesList()
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

        public async Task<List<UnitPhasesMasterDTO>> GetUnitPhasesList()
        {
            // DataTable dt = new DataTable();
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            List<UnitPhasesMasterDTO> PriorityLst = new List<UnitPhasesMasterDTO>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetUnitPhases?unitId=" + unitId);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    PriorityLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UnitPhasesMasterDTO>>(data);

                    if (PriorityLst.Count > 0)
                    {
                        foreach (var item in PriorityLst)
                        {
                            item.EncryptedId = CommonHelper.EncryptURLHTML(item.ID.ToString());
                        }

                    }
                }


            }
            return PriorityLst;
        }

        public async Task<List<StatusMasterDTO>> GetMasterStatusList()
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

        public async Task<List<UnitStatusMasterDTO>> GetUnitStatusList()
        {
            // DataTable dt = new DataTable();
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            List<UnitStatusMasterDTO> PriorityLst = new List<UnitStatusMasterDTO>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetUnitStatus?unitId=" + unitId + "&statusType=B");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    PriorityLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UnitStatusMasterDTO>>(data);

                    if (PriorityLst.Count > 0)
                    {
                        foreach (var item in PriorityLst)
                        {
                            item.EncryptedId = CommonHelper.EncryptURLHTML(item.ID.ToString());
                        }

                    }
                }


            }
            return PriorityLst;
        }

        [HttpPost]
        public async Task<string> MasterPhases(UserAction userAction)
        {
            // CompanyPriorityMasterDTO OutPutData;
            // OutPutData.IsActive = true;
            string OutPut = null;
            //OutPut.Priority = inputs.Priority;
            //OutPut.PriorityId = inputs.PriorityId;
            string[] strPrioritylst = userAction.Priority.Split(',');
            string[] strPriorityIdlst = userAction.PriorityIds.Split(',');

            try
            {
                if (strPriorityIdlst.Length > 0)
                {
                    for (int id = 0; id < strPriorityIdlst.Length; id++)
                    {
                        UnitPhasesMasterDTO OutPutData = new UnitPhasesMasterDTO();
                        OutPutData.IsActive = true;
                        OutPutData.Phase = strPrioritylst[id];
                        OutPutData.MasterPhaseId = Convert.ToInt32(strPriorityIdlst[id]);
                        OutPutData.UnitId = HttpContext.Session.GetInt32("UnitId");
                        OutPutData.EncryptedId = "";

                        //OutPutData.CompanyPriority = "Asif";
                        //OutPutData.CompanyPriorityId = 2;
                        //OutPutData.CompanyId =8;

                        using (HttpClient client = new HttpClient())
                        {
                            client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                            HttpResponseMessage response = await client.PostAsJsonAsync("UnitPhasesMaster", OutPutData);
                            if (response.IsSuccessStatusCode)
                            {
                                var data = await response.Content.ReadAsStringAsync();
                                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                                OutPut = result.DisplayMessage;

                                //   OutPut.MasterPhasesList = await GetMasterPhasesList();
                                // OutPut.UnitPhasesList = await GetUnitPhasesList();
                            }

                        }
                    }
                }
            }
            catch (SystemException ex)
            {
                OutPut = "Priority failed!" + ex.Message;
            }
            return OutPut;
        }

        [HttpPost]
        public async Task<IActionResult>? UnitPhaseMaster(UnitPhasesMasterDTO inputs)
        {
            UnitPhasesMasterDTO OutPut = new UnitPhasesMasterDTO();
            inputs.IsActive = true;
            inputs.UnitId = HttpContext.Session.GetInt32("UnitId");
            inputs.EncryptedId = "";
            //OutPut.Priority = inputs.Priority;
            //OutPut.PriorityId = inputs.PriorityId;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("UnitPhasesMaster", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        //  dt = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Data.DataTable>(data);
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                        OutPut.HttpStatusCode = result.HttpStatusCode;
                        OutPut.DisplayMessage = result.DisplayMessage;

                        OutPut.MasterPhasesList = await GetMasterPhasesList();
                        OutPut.UnitPhasesList = await GetUnitPhasesList();

                    }

                }


            }
            catch (SystemException ex)
            {
                OutPut.DisplayMessage = "Designation failed!" + ex.Message;
            }
            return View("UnitPhases", OutPut);
        }

        [HttpPost]
        public async Task<string> StatusMaster(UserAction userAction)
        {
            // CompanyPriorityMasterDTO OutPutData;
            // OutPutData.IsActive = true;
            string OutPut = null;
            //OutPut.Priority = inputs.Priority;
            //OutPut.PriorityId = inputs.PriorityId;
            string[] strPrioritylst = userAction.Priority.Split(',');
            string[] strPriorityIdlst = userAction.PriorityIds.Split(',');

            try
            {
                if (strPriorityIdlst.Length > 0)
                {
                    for (int id = 0; id < strPriorityIdlst.Length; id++)
                    {
                        UnitStatusMasterDTO OutPutData = new UnitStatusMasterDTO();
                        OutPutData.IsActive = true;
                        OutPutData.Status = strPrioritylst[id];
                        OutPutData.MasterStatusId = Convert.ToInt32(strPriorityIdlst[id]);
                        OutPutData.UnitId = HttpContext.Session.GetInt32("UnitId");
                        OutPutData.EncryptedId = "";

                        //OutPutData.CompanyPriority = "Asif";
                        //OutPutData.CompanyPriorityId = 2;
                        //OutPutData.CompanyId =8;

                        using (HttpClient client = new HttpClient())
                        {
                            client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                            HttpResponseMessage response = await client.PostAsJsonAsync("UnitStatusMaster", OutPutData);
                            if (response.IsSuccessStatusCode)
                            {
                                var data = await response.Content.ReadAsStringAsync();
                                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                                OutPut = result.DisplayMessage;

                                //   OutPut.MasterPhasesList = await GetMasterPhasesList();
                                // OutPut.UnitPhasesList = await GetUnitPhasesList();
                            }

                        }
                    }
                }
            }
            catch (SystemException ex)
            {
                OutPut = "Priority failed!" + ex.Message;
            }
            return OutPut;
        }

        [HttpPost]
        public async Task<IActionResult>? UnitStatusMaster(UnitStatusMasterDTO inputs)
        {
            UnitStatusMasterDTO OutPut = new UnitStatusMasterDTO();
            inputs.IsActive = true;
            inputs.UnitId = HttpContext.Session.GetInt32("UnitId");
            inputs.EncryptedId = "";
            //OutPut.Priority = inputs.Priority;
            //OutPut.PriorityId = inputs.PriorityId;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("UnitStatusMaster", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        //  dt = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Data.DataTable>(data);
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                        OutPut.HttpStatusCode = result.HttpStatusCode;
                        OutPut.DisplayMessage = result.DisplayMessage;

                        OutPut.StatusMasterList = await GetMasterStatusList();
                        OutPut.UnitStatusMasterList = await GetUnitStatusList();

                    }

                }


            }
            catch (SystemException ex)
            {
                OutPut.DisplayMessage = "Designation failed!" + ex.Message;
            }
            return View("UnitStatus", OutPut);
        }


        public async Task<List<UnitCategoryMasterDTO>> GetUnitCategoryList()
        {
            // DataTable dt = new DataTable();
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            List<UnitCategoryMasterDTO> PriorityLst = new List<UnitCategoryMasterDTO>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetUnitCategory?unitId=" + unitId);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    PriorityLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UnitCategoryMasterDTO>>(data);

                    if (PriorityLst.Count > 0)
                    {
                        foreach (var item in PriorityLst)
                        {
                            item.EncryptedId = CommonHelper.EncryptURLHTML(item.UnitCategoryId.ToString());
                        }

                    }
                }


            }
            return PriorityLst;
        }

        public async Task<List<MasterCategoryDTO>> GetCategoryList()
        {
            // DataTable dt = new DataTable();
            int? unitId = HttpContext.Session.GetInt32("UnitId");
            List<MasterCategoryDTO> PriorityLst = new List<MasterCategoryDTO>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetCategory");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    PriorityLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MasterCategoryDTO>>(data);

                }


            }
            return PriorityLst;
        }

        [HttpPost]
        public async Task<string> UnitCategoryMaster(UserAction userAction)
        {
            string OutPut = null;
            //  UnitCategoryMasterDTO OutPut = new UnitCategoryMasterDTO();
            string[] strPrioritylst = userAction.Priority.Split(',');
            string[] strPriorityIdlst = userAction.PriorityIds.Split(',');

            try
            {
                if (strPriorityIdlst.Length > 0)
                {
                    for (int id = 0; id < strPriorityIdlst.Length; id++)
                    {
                        UnitCategoryMasterDTO OutPutData = new UnitCategoryMasterDTO();
                        OutPutData.IsActive = true;
                        OutPutData.UnitCategory = strPrioritylst[id];
                        OutPutData.MasterCategoryId = Convert.ToInt32(strPriorityIdlst[id]);
                        OutPutData.UnitId = HttpContext.Session.GetInt32("UnitId");
                        OutPutData.EncryptedId = "";
                        using (HttpClient client = new HttpClient())
                        {
                            client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                            HttpResponseMessage response = await client.PostAsJsonAsync("UnitCategory", OutPutData);
                            if (response.IsSuccessStatusCode)
                            {
                                var data = await response.Content.ReadAsStringAsync();
                                //  dt = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Data.DataTable>(data);
                                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                                //  OutPut.HttpStatusCode = result.HttpStatusCode;
                                OutPut = result.DisplayMessage;

                                //  OutPut.MasterPhasesList = await GetMasterPhasesList();
                                // OutPut.UnitPhasesList = await GetUnitPhasesList();

                            }

                        }

                    }
                }
            }
            catch (SystemException ex)
            {
                OutPut = "Designation failed!" + ex.Message;
            }
            return OutPut;
        }

        [HttpPost]
        public async Task<IActionResult>? UnitCategory(UnitCategoryMasterDTO inputs)
        {
            UnitCategoryMasterDTO OutPut = new UnitCategoryMasterDTO();
            inputs.IsActive = true;
            inputs.UnitId = HttpContext.Session.GetInt32("UnitId");
            inputs.EncryptedId = "";
            //OutPut.Priority = inputs.Priority;
            //OutPut.PriorityId = inputs.PriorityId;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("UnitCategory", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        //  dt = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Data.DataTable>(data);
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                        OutPut.HttpStatusCode = result.HttpStatusCode;
                        OutPut.DisplayMessage = result.DisplayMessage;

                        OutPut.MasterCategoryList = await GetCategoryList();
                        OutPut.UnitCategoryList = await GetUnitCategoryList();

                    }

                }


            }
            catch (SystemException ex)
            {
                OutPut.DisplayMessage = "Designation failed!" + ex.Message;
            }
            return View("UnitCategory", OutPut);
        }

        [HttpGet]
        [Route("Master/DeleteUnitCategory/{epId}")]
        public async Task<IActionResult>? DeleteUnitCategory(string epId)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            UnitCategoryMasterDTO OutPut = new UnitCategoryMasterDTO();

            int pId = Convert.ToInt32(CommonHelper.DecryptURLHTML(epId));
            if (pId > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("DeleteUnitCategory?pId=" + pId);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                            OutPut.HttpStatusCode = result.HttpStatusCode;
                            OutPut.DisplayMessage = result.DisplayMessage;
                            OutPut.MasterCategoryList = await GetCategoryList();
                            OutPut.UnitCategoryList = await GetUnitCategoryList();
                        }

                    }

                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "Clinic failed!" + ex.Message;
                }

            }
            return View("UnitCategory", OutPut);
        }

        [HttpGet]
        [Route("Master/DeleteUnitPhases/{epId}")]
        public async Task<IActionResult>? DeleteUnitPhases(string epId)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            UnitPhasesMasterDTO OutPut = new UnitPhasesMasterDTO();

            int pId = Convert.ToInt32(CommonHelper.DecryptURLHTML(epId));
            if (pId > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("DeleteUnitPhases?pId=" + pId);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                            OutPut.HttpStatusCode = result.HttpStatusCode;
                            OutPut.DisplayMessage = result.DisplayMessage;
                            OutPut.MasterPhasesList = await GetMasterPhasesList();
                            OutPut.UnitPhasesList = await GetUnitPhasesList();
                        }

                    }

                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "Clinic failed!" + ex.Message;
                }

            }
            return View("UnitPhases", OutPut);
        }

        [HttpGet]
        [Route("Master/DeleteUnitStatus/{epId}")]
        public async Task<IActionResult>? DeleteUnitStatus(string epId)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            UnitStatusMasterDTO OutPut = new UnitStatusMasterDTO();

            int pId = Convert.ToInt32(CommonHelper.DecryptURLHTML(epId));
            if (pId > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("DeleteUnitStatus?pId=" + pId);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                            OutPut.HttpStatusCode = result.HttpStatusCode;
                            OutPut.DisplayMessage = result.DisplayMessage;
                            OutPut.StatusMasterList = await GetMasterStatusList();
                            OutPut.UnitStatusMasterList = await GetUnitStatusList();

                        }

                    }

                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "Clinic failed!" + ex.Message;
                }

            }
            return View("UnitStatus", OutPut);
        }

        [HttpGet]
        [Route("Master/DeleteUnitPriority/{epId}")]
        public async Task<IActionResult>? DeleteUnitPriority(string epId)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            CompanyPriorityMasterDTO OutPut = new CompanyPriorityMasterDTO();

            int pId = Convert.ToInt32(CommonHelper.DecryptURLHTML(epId));
            if (pId > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("DeleteUnitPriority?pId=" + pId);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                            OutPut.HttpStatusCode = result.HttpStatusCode;
                            OutPut.DisplayMessage = result.DisplayMessage;
                            OutPut.PriorityMasterList = await GetPriorityList();
                            OutPut.CompanyPriorityList = await GetUnitPriorityList();

                        }

                    }

                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "Clinic failed!" + ex.Message;
                }

            }
            return View("Priority", OutPut);
        }

        [HttpGet]
        [Route("Master/GetUnitPriorityById/{epId}")]
        public async Task<IActionResult>? GetUnitPriorityById(string epId)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            CompanyPriorityMasterDTO OutPut = new CompanyPriorityMasterDTO();

            int pId = Convert.ToInt32(CommonHelper.DecryptURLHTML(epId));
            if (pId > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("GetUnitPriorityById?pId=" + pId);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            OutPut = Newtonsoft.Json.JsonConvert.DeserializeObject<CompanyPriorityMasterDTO>(data);
                            OutPut.PriorityMasterList = await GetPriorityList();
                            OutPut.CompanyPriorityList = await GetUnitPriorityList();
                        }

                    }


                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "Clinic failed!" + ex.Message;
                }

            }
            return View("Priority", OutPut);
        }

        [HttpGet]
        [Route("Master/GetUnitCategoryById/{epId}")]
        public async Task<IActionResult>? GetUnitCategoryById(string epId)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            UnitCategoryMasterDTO OutPut = new UnitCategoryMasterDTO();

            int pId = Convert.ToInt32(CommonHelper.DecryptURLHTML(epId));
            if (pId > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("GetUnitCategoryById?pId=" + pId);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            OutPut = Newtonsoft.Json.JsonConvert.DeserializeObject<UnitCategoryMasterDTO>(data);
                            OutPut.MasterCategoryList = await GetCategoryList();
                            OutPut.UnitCategoryList = await GetUnitCategoryList();
                        }

                    }


                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "Clinic failed!" + ex.Message;
                }

            }
            return View("UnitCategory", OutPut);
        }

        [HttpGet]
        [Route("Master/GetUnitPhasesById/{epId}")]
        public async Task<IActionResult>? GetUnitPhasesById(string epId)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            UnitPhasesMasterDTO OutPut = new UnitPhasesMasterDTO();

            int pId = Convert.ToInt32(CommonHelper.DecryptURLHTML(epId));
            if (pId > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("GetUnitPhasesById?pId=" + pId);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            OutPut = Newtonsoft.Json.JsonConvert.DeserializeObject<UnitPhasesMasterDTO>(data);
                            OutPut.MasterPhasesList = await GetMasterPhasesList();
                            OutPut.UnitPhasesList = await GetUnitPhasesList();
                        }

                    }


                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "Clinic failed!" + ex.Message;
                }

            }
            return View("UnitPhases", OutPut);
        }

        [HttpGet]
        [Route("Master/GetUnitStatusById/{epId}")]
        public async Task<IActionResult>? GetUnitStatusById(string epId)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            UnitStatusMasterDTO OutPut = new UnitStatusMasterDTO();

            int pId = Convert.ToInt32(CommonHelper.DecryptURLHTML(epId));
            if (pId > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("GetUnitStatusById?pId=" + pId);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            OutPut = Newtonsoft.Json.JsonConvert.DeserializeObject<UnitStatusMasterDTO>(data);
                            OutPut.StatusMasterList = await GetMasterStatusList();
                            OutPut.UnitStatusMasterList = await GetUnitStatusList();
                        }

                    }


                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "Clinic failed!" + ex.Message;
                }

            }
            return View("UnitStatus", OutPut);
        }

        public async Task<List<DocumentCategoryDTO>> GetDocumentCategoryList()
        {
            DataTable dt = new DataTable();

            List<DocumentCategoryDTO> CountriesLst = new List<DocumentCategoryDTO>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetDocumentCategory");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    CountriesLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DocumentCategoryDTO>>(data);
                }

            }
            return CountriesLst;
        }

        public async Task<List<DrawingCategoryDTO>> GetDrawingCategoryList()
        {
            DataTable dt = new DataTable();

            List<DrawingCategoryDTO> CountriesLst = new List<DrawingCategoryDTO>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetDrawingCategory");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    CountriesLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DrawingCategoryDTO>>(data);
                }

            }
            return CountriesLst;
        }

        public async Task<List<PhotoCategoryDTO>> GetPhotoCategoryList()
        {
            DataTable dt = new DataTable();

            List<PhotoCategoryDTO> CountriesLst = new List<PhotoCategoryDTO>();
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("GetPhotoCategory");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    CountriesLst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PhotoCategoryDTO>>(data);
                }

            }
            return CountriesLst;
        }



        [HttpPost]
        public async Task<IActionResult>? DocumentCategoryMaster(DocumentCategoryDTO inputs)
        {
            DocumentCategoryDTO OutPut = new DocumentCategoryDTO();
            inputs.IsDisable = true;
            DataTable dt = new DataTable();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("DocumentCategory", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        //  dt = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Data.DataTable>(data);
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                        OutPut.HttpStatusCode = result.HttpStatusCode;
                        OutPut.DisplayMessage = result.DisplayMessage;

                        OutPut.DocumentCategoryList = await GetDocumentCategoryList();
                    }

                }


            }
            catch (SystemException ex)
            {
                OutPut.DisplayMessage = "Clinic failed!" + ex.Message;
            }
            return View("DocumentCategory", OutPut);
        }

        [HttpGet]     
        public async Task<IActionResult>? GetDocumentCategoryId(int Id)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            DocumentCategoryDTO OutPut = new DocumentCategoryDTO();

            //int Id = Convert.ToInt32(CommonHelper.DecryptURLHTML(eId));
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
                        HttpResponseMessage response = await client.GetAsync("GetDocumentById?docId=" + Id);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            OutPut = Newtonsoft.Json.JsonConvert.DeserializeObject<DocumentCategoryDTO>(data);
                            // var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                            //OutPut.HttpStatusCode = result.HttpStatusCode;
                            // OutPut.DisplayMessage = result.DisplayMessage;

                            OutPut.DocumentCategoryList = await GetDocumentCategoryList();
                        }

                    }


                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "Clinic failed!" + ex.Message;
                }

            }
            return View("DocumentCategory", OutPut);
        }


        [HttpGet]     
        public async Task<IActionResult>? DeleteDocumentCategory(int Id)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            DocumentCategoryDTO OutPut = new DocumentCategoryDTO();

            // int countryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ecountryId));
            if (Id > 0)
            {
                int? pId = HttpContext.Session.GetInt32("PID");
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("DeleteDocumentCategory?Id=" + Id);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var docCategoryCacheKey = $"DocumentCategory_{pId}";
                              _memoryCache.Remove(docCategoryCacheKey);

                            var data = await response.Content.ReadAsStringAsync();
                            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                            OutPut.HttpStatusCode = result.HttpStatusCode;
                            OutPut.DisplayMessage = result.DisplayMessage;
                            OutPut.DocumentCategoryList = await GetDocumentCategoryList();
                        }

                    }

                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "failed!" + ex.Message;
                }

            }
            return View("DocumentCategory", OutPut);
        }

        [HttpPost]
        public async Task<IActionResult>? DrawingCategoryMaster(DrawingCategoryDTO inputs)
        {
            DrawingCategoryDTO OutPut = new DrawingCategoryDTO();
            inputs.IsDisable = true;


            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("DrawingCategory", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        //  dt = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Data.DataTable>(data);
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                        OutPut.HttpStatusCode = result.HttpStatusCode;
                        OutPut.DisplayMessage = result.DisplayMessage;

                        OutPut.DrawingCategoryList = await GetDrawingCategoryList();
                    }

                }


            }
            catch (SystemException ex)
            {
                OutPut.DisplayMessage = "failed!" + ex.Message;
            }
            return View("DrawingCategory", OutPut);
        }

        [HttpGet]
        public async Task<IActionResult>? GetDrawingCategoryId(int Id)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            DrawingCategoryDTO OutPut = new DrawingCategoryDTO();

            //int Id = Convert.ToInt32(CommonHelper.DecryptURLHTML(eId));
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
                        HttpResponseMessage response = await client.GetAsync("GetDrawingById?drId=" + Id);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            OutPut = Newtonsoft.Json.JsonConvert.DeserializeObject<DrawingCategoryDTO>(data);
                           
                            OutPut.DrawingCategoryList = await GetDrawingCategoryList();
                        }

                    }


                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "failed!" + ex.Message;
                }

            }
            return View("DrawingCategory", OutPut);
        }

        [HttpGet]
        public async Task<IActionResult>? DeleteDrawingCategory(int Id)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            DrawingCategoryDTO OutPut = new DrawingCategoryDTO();

            // int countryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ecountryId));
            if (Id > 0)
            {
                int? pId = HttpContext.Session.GetInt32("PID");
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("DeleteDrawingCategory?Id=" + Id);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var drawingCategoryCacheKey = $"DrawingCategory_{pId}";
                            _memoryCache.Remove(drawingCategoryCacheKey);
                            var data = await response.Content.ReadAsStringAsync();
                            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                            OutPut.HttpStatusCode = result.HttpStatusCode;
                            OutPut.DisplayMessage = result.DisplayMessage;
                            OutPut.DrawingCategoryList = await GetDrawingCategoryList();
                        }

                    }

                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "failed!" + ex.Message;
                }

            }
            return View("DrawingCategory", OutPut);
        }

        [HttpPost]
        public async Task<IActionResult>? PhotoCategoryMaster(PhotoCategoryDTO inputs)
        {
            PhotoCategoryDTO OutPut = new PhotoCategoryDTO();
            inputs.IsDisable = true;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PostAsJsonAsync("PhotoCategory", inputs);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        //  dt = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Data.DataTable>(data);
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                        OutPut.HttpStatusCode = result.HttpStatusCode;
                        OutPut.DisplayMessage = result.DisplayMessage;

                        OutPut.PhotoCategoryList = await GetPhotoCategoryList();
                    }

                }


            }
            catch (SystemException ex)
            {
                OutPut.DisplayMessage = "failed!" + ex.Message;
            }
            return View("PhotoCategory", OutPut);
        }

        [HttpGet]
        public async Task<IActionResult>? GetPhotoCategoryId(int Id)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            PhotoCategoryDTO OutPut = new PhotoCategoryDTO();

            //int Id = Convert.ToInt32(CommonHelper.DecryptURLHTML(eId));
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
                        HttpResponseMessage response = await client.GetAsync("GetPhotoById?drId=" + Id);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            OutPut = Newtonsoft.Json.JsonConvert.DeserializeObject<PhotoCategoryDTO>(data);

                            OutPut.PhotoCategoryList = await GetPhotoCategoryList();
                        }

                    }


                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "failed!" + ex.Message;
                }

            }
            return View("PhotoCategory", OutPut);
        }

        [HttpGet]
        public async Task<IActionResult>? DeletePhotoCategory(int Id)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            PhotoCategoryDTO OutPut = new PhotoCategoryDTO();

            // int countryId = Convert.ToInt32(CommonHelper.DecryptURLHTML(ecountryId));
            if (Id > 0)
            {
                int? pId = HttpContext.Session.GetInt32("PID");
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var photoCategoryCacheKey = $"PhotoCategory_{pId}";
                        _memoryCache.Remove(photoCategoryCacheKey);
                        client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("DeletePhotoCategory?Id=" + Id);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                            OutPut.HttpStatusCode = result.HttpStatusCode;
                            OutPut.DisplayMessage = result.DisplayMessage;
                            OutPut.PhotoCategoryList = await GetPhotoCategoryList();
                        }

                    }

                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "failed!" + ex.Message;
                }

            }
            return View("PhotoCategory", OutPut);
        }

        [HttpGet]
       [Route("Master/DeleteDepartment/{eDeptId}")]
        public async Task<IActionResult>? DeleteDepartment(string eDeptId)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            DepartmentMasterDTO OutPut = new DepartmentMasterDTO();

            int deptId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eDeptId));
            if (deptId > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("DeleteDepartment?deptId=" + deptId);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                            OutPut.HttpStatusCode = result.HttpStatusCode;
                            OutPut.DisplayMessage = result.DisplayMessage;
                            OutPut.DepartmentMasterList = await GetDepartmentList();
                            //return true;
                        }

                    }

                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "Clinic failed!" + ex.Message;
                  //  return false;
                }

            }
            return View("Department",OutPut);
        }

        [HttpGet]
        [Route("Master/DeleteDesignation/{eDesigId}")]
        public async Task<IActionResult>? DeleteDesignation(string eDesigId)
        {
           
            JobTitleMasterDTO OutPut = new JobTitleMasterDTO();

            int DesigId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eDesigId));
            if (DesigId > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("DeleteDesignation?desigId=" + DesigId);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                            OutPut.HttpStatusCode = result.HttpStatusCode;
                            OutPut.DisplayMessage = result.DisplayMessage;
                            OutPut.JobTitleMasterList = await GetDesignationList();
                          //  return true;
                        }

                    }

                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "Clinic failed!" + ex.Message;
                  //  return false;
                }

            }
            return View("Designation",OutPut);
        }

        [HttpGet]
        [Route("Master/GetRoleById/{eRoleId}")]
        public async Task<IActionResult>? GetRoleById(string eRoleId)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            RoleMasterDTO OutPut = new RoleMasterDTO();

            int roleId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eRoleId));
            if (roleId > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("GetRoleById?rId=" + roleId);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            OutPut = Newtonsoft.Json.JsonConvert.DeserializeObject<RoleMasterDTO>(data);
                            // var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                            //OutPut.HttpStatusCode = result.HttpStatusCode;
                            // OutPut.DisplayMessage = result.DisplayMessage;

                           OutPut.RoleMasterList = await GetRoleList();
                        }

                    }


                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "Clinic failed!" + ex.Message;
                }

            }
            return View("Role", OutPut);
        }

        [HttpGet]
        [Route("Master/DeleteRole/{eRoleId}")]
        public async Task<IActionResult>? DeleteRole(string eRoleId)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            RoleMasterDTO OutPut = new RoleMasterDTO();

            int RoleId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eRoleId));
            if (RoleId > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("DeleteRole?rId=" + RoleId);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);

                            OutPut.HttpStatusCode = result.HttpStatusCode;
                            OutPut.DisplayMessage = result.DisplayMessage;
                            OutPut.RoleMasterList = await GetRoleList();
                            //  return true;
                        }

                    }

                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "Role failed!" + ex.Message;
                    //return false;
                }

            }
            return  View("Role",OutPut);
        }

        [HttpGet]
        [Route("Master/DeleteCity/{eCityId}")]
        public async Task<IActionResult>? DeleteCity(string eCityId)
        {
            // CountryKeyValues inputs = new CountryKeyValues();
            CityMasterDTO OutPut = new CityMasterDTO();

            int CityId = Convert.ToInt32(CommonHelper.DecryptURLHTML(eCityId));
            if (CityId > 0)
            {
                //  inputs.CountryId = countryId;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(EnvironmentUrl.apiMasterUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.GetAsync("DeleteCity?cityId=" + CityId);
                        //HttpResponseMessage response = await client.PostAsJsonAsync("GetCountryById", inputs);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OutPutResponse>(data);
                            OutPut.HttpStatusCode = result.HttpStatusCode;
                            OutPut.DisplayMessage = result.DisplayMessage;
                            OutPut.CityMasterList = await GetCityList();
                            // return true;
                        }

                    }

                }
                catch (SystemException ex)
                {
                    OutPut.DisplayMessage = "Role failed!" + ex.Message;
                   // return false;
                }

            }
            return View("City", OutPut);
        }
    }

}
