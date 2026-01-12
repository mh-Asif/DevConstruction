using AutoMapper;
using Construction.Infrastructure.Helper;
using Construction.Infrastructure.KeyValues;
using Construction.Infrastructure.Models;
using ConstructionApp.Core.Entities;
using ConstructionApp.Core.Helper;
using ConstructionApp.Core.Repository;
using ConstructionApp.Services.DBContext;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Linq.Expressions;
using System.Text;

namespace ConstructionApp.EndPoints.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersAPIController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UsersAPIController> _logger;
        private readonly IMapper _mapper;
        //  private readonly ConstDbContext _constDbContext;
        //private readonly MasterAPIsController _masterApiController;

        public UsersAPIController(IUnitOfWork unitOfWork, ILogger<UsersAPIController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            //  _constDbContext = constDbContext;
            //_masterApiController = masterApiController;
        }

        [HttpPost]
        [Route("ChangePassword")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> ChangePassword(UserChangePassword inputDTO)
        {
            try
            {

                // inputDTO.FullName = inputDTO.FirstName + " " + inputDTO.LastName;
                OutPutResponse outPut = new OutPutResponse();
                if (ModelState.IsValid)
                {
                    if (inputDTO.UserId > 0 && !string.IsNullOrEmpty(inputDTO.LoginPassword) && !string.IsNullOrEmpty(inputDTO.Password) && !string.IsNullOrEmpty(inputDTO.ConfirmPassword) && inputDTO.Password == inputDTO.ConfirmPassword)
                    {
                        UsersMaster outputDetails = _mapper.Map<UsersMaster>(await _unitOfWork.UsersMaster.GetByIdAsync(Convert.ToInt32(inputDTO.UserId)));
                        if (outputDetails != null)
                        {
                            outputDetails.LoginPassword = CommonHelper.Encrypt(inputDTO.Password);
                            _unitOfWork.UsersMaster.Update(outputDetails);
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Password changed successfully!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "User not found!";
                            return Ok(outPut);
                        }
                    }
                    else
                    {
                        outPut.HttpStatusCode = 201;
                        outPut.DisplayMessage = "Invalid input data!";
                        return Ok(outPut);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving User {nameof(ChangePassword)}");
                throw;
            }

            return null;
        }

        [HttpPost]
        [Route("ForgotPassword")]
        [Produces("application/json", Type = typeof(UsersMasterDTO))]
        public async Task<IActionResult> ForgotPassword(UserLogin inputDTO)
        {
            try
            {


                UsersMasterDTO outPut = new UsersMasterDTO();
                if (ModelState.IsValid)
                {
                    if (inputDTO.EmailAddress.Length > 0)
                    {
                        outPut = _mapper.Map<UsersMasterDTO>( _unitOfWork.UsersMaster.FindAllByExpression(x=>x.EmailAddress==inputDTO.EmailAddress || x.BusinessEmail==inputDTO.EmailAddress).FirstOrDefault());
                        // UsersMaster outputDetails = _mapper.Map<UsersMaster>(await _unitOfWork.UsersMaster.GetByIdAsync(Convert.ToInt32(inputDTO.UserId)));
                      //  outPut.HttpStatusCode = 200;
                        return Ok(outPut);
                    }
                    else
                    {
                        outPut.HttpStatusCode = 201;
                        outPut.DisplayMessage = "Invalid input data!";
                        return Ok(outPut);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving User {nameof(ForgotPassword)}");
                throw;
            }

            return null;
        }

        [HttpPost]
        [Route("SaveUser")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> SaveUser(UsersMasterDTO inputDTO)
        {
            try
            {
               
                // inputDTO.FullName = inputDTO.FirstName + " " + inputDTO.LastName;
                OutPutResponse outPut = new OutPutResponse();
                if (ModelState.IsValid)
                {
                    if (inputDTO.UserId == 0)
                    {
                        string ValidationMessageEmpty = ValidateEmployeeInfo(inputDTO);
                        if (string.IsNullOrEmpty(ValidationMessageEmpty))
                        {

                            var em = _mapper.Map<UsersMasterDTO>(_unitOfWork.UsersMaster.Insert(_mapper.Map<UsersMaster>(inputDTO)));
                            int empID = em.UserId;

                            outPut.DisplayMessage = "User Saved Successfully";
                            outPut.HttpStatusCode = 200;
                            return Ok(outPut);

                        }
                        else
                        {
                            outPut.DisplayMessage = ValidationMessageEmpty;
                            outPut.HttpStatusCode = 201;
                            return Ok(outPut);

                        }
                    }
                    else
                    {
                        UsersMaster outputDetails = _mapper.Map<UsersMaster>(await _unitOfWork.UsersMaster.GetByIdAsync(Convert.ToInt32(inputDTO.UserId)));
                        if (outputDetails != null)
                        {
                            outputDetails.RoleId = inputDTO.RoleId;
                            outputDetails.DOJ = inputDTO.DOJ;
                            outputDetails.EmpStatus = inputDTO.EmpStatus;
                            outputDetails.PanCard = inputDTO.PanCard;
                            outputDetails.BloodGroup = inputDTO.BloodGroup;
                            outputDetails.Description = inputDTO.Description;
                            outputDetails.Address = inputDTO.Address;
                            outputDetails.EmailAddress = inputDTO.EmailAddress;
                            outputDetails.UnitId = inputDTO.UnitId;
                            outputDetails.BusinessContactNumber = inputDTO.BusinessContactNumber;
                            outputDetails.BusinessEmail = inputDTO.BusinessEmail;
                            outputDetails.EmailDisplay= inputDTO.EmailDisplay;
                            outputDetails.IsActive = inputDTO.IsActive;
                            outputDetails.CountryId = inputDTO.CountryId;
                            outputDetails.StateId = inputDTO.StateId;
                            outputDetails.CityId = inputDTO.CityId;
                            outputDetails.CreatedOn = inputDTO.CreatedOn;
                            outputDetails.DOB = inputDTO.DOB;
                            outputDetails.EmpTypeId = inputDTO.EmpTypeId;
                            outputDetails.WorkLocation = inputDTO.WorkLocation;
                            outputDetails.HODId = inputDTO.HODId;
                            outputDetails.MgrId = inputDTO.MgrId;
                            outputDetails.DepartmentId = inputDTO.DepartmentId;
                            outputDetails.JobTitleId = inputDTO.JobTitleId;
                            outputDetails.EmpCode = inputDTO.EmpCode;
                            outputDetails.PostalOrZipCode = inputDTO.PostalOrZipCode;
                            outputDetails.Address = inputDTO.Address;
                            outputDetails.ContactNumber = inputDTO.ContactNumber;
                            outputDetails.ContactPerson = inputDTO.ContactPerson;
                            outputDetails.FullName = inputDTO.FullName;
                            if (inputDTO.ProfileName != null)
                                outputDetails.ProfileName = inputDTO.ProfileName;

                          //  outputDetails.ProfileName = inputDTO.ProfileName;
                            outputDetails.CompanyName = inputDTO.CompanyName;
                            outputDetails.BusinessOwner = inputDTO.BusinessOwner;
                            outputDetails.WebsiteUrl = inputDTO.WebsiteUrl;
                            outputDetails.GSTN = inputDTO.GSTN;
                            outputDetails.PaymentTerm = inputDTO.PaymentTerm;
                            outputDetails.AccountName = inputDTO.AccountName;
                            outputDetails.BankName = inputDTO.BankName;
                            outputDetails.AccountNumber = inputDTO.AccountNumber;
                            outputDetails.IFSCOrShiftCode = inputDTO.IFSCOrShiftCode;



                        }
                        Expression<Func<UsersMaster, bool>> expression = a => a.UserId != Convert.ToInt32(inputDTO.UserId) && a.IsActive == true;
                        if (_unitOfWork.UsersMaster.Exists(expression))
                        {
                            _unitOfWork.UsersMaster.Update(_mapper.Map<UsersMaster>(outputDetails));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "User updated successfully!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "Duplicate Entry Found!";
                            return Ok(outPut);
                        }
                    }
                }
                else
                {

                    outPut.HttpStatusCode = 201;
                    return Ok(outPut);

                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving User {nameof(SaveUser)}");
                throw;
            }
        }

        [HttpPost]
        [Route("UpdateUser")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public IActionResult UpdateUser(UsersMasterDTO inputDTO)
        {
            try
            {
                OutPutResponse outPut = new OutPutResponse();
                if (ModelState.IsValid)
                {
                    //  string UserValidation = Validation();
                    //  LoginDetailsDTO inputData = new LoginDetailsDTO();
                    string UserValidation = "";// ValidationUser(inputDTO);
                    if (string.IsNullOrEmpty(UserValidation))
                    {
                        string ValidationMessageEmpty = ValidateEmployeeInfo(inputDTO);
                        if (string.IsNullOrEmpty(ValidationMessageEmpty))
                        {
                            var em = _mapper.Map<UsersMasterDTO>(_unitOfWork.UsersMaster.Insert(_mapper.Map<UsersMaster>(inputDTO)));
                            int empID = em.UserId;

                            // string encempid = CommonHelper.EncryptURLHTML(em.UserId.ToString());
                            // em.EnycEmployeeId = encempid;
                            outPut.DisplayMessage = "User Saved Successfully";
                            outPut.HttpStatusCode = 200;
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.DisplayMessage = ValidationMessageEmpty;
                            outPut.HttpStatusCode = 201;
                            return Ok(outPut);

                        }
                    }
                    else
                    {
                        outPut.DisplayMessage = UserValidation;
                        outPut.HttpStatusCode = 201;
                        return Ok(outPut);

                    }
                    // return BadRequest(false);
                }
                return BadRequest("Invalid Model");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving User {nameof(SaveUser)}");
                throw;
            }
        }

        [HttpPost]
        public string ValidateEmployeeInfo(UsersMasterDTO inputDTO)
        {
            string validMessage = "";
            StringBuilder messageBuilder = new StringBuilder();
            //// inputDTO.ClientId = Convert.ToInt32(HttpContext.Session.GetString("ClientId"));
            //Expression<Func<UsersMaster, bool>> expression = (a => (a.IsActive == true) && (a.UserId != ((inputDTO.UserId == 0 || inputDTO.UserId == null) ? a.UserId : inputDTO.UserId)));

            Expression<Func<UsersMaster, bool>> expression = (a => (a.IsActive == true));

            IList<UsersMasterDTO> tableData = new List<UsersMasterDTO>();
            tableData = _mapper.Map<IList<UsersMasterDTO>>(_unitOfWork.UsersMaster.FindAllByExpression(expression));

            if (inputDTO.CompanyName != null && tableData.Where((a => (a.CompanyName != null && a.CompanyName.Trim() == inputDTO.CompanyName.Trim()))).Count() != 0)
            {
                messageBuilder.Append($"Duplicate Company({inputDTO.CompanyName.Trim()}) found</br>");
            }
            if (inputDTO.EmailAddress != null && tableData.Where((a => (a.EmailAddress != null && a.EmailAddress.Trim() == inputDTO.EmailAddress.Trim()))).Count() != 0)
            {
                messageBuilder.Append($"Duplicate Email Address({inputDTO.EmailAddress.Trim()}) found</br>");
            }
            if (inputDTO.GSTN != null && tableData.Where((a => (a.GSTN != null && a.GSTN.Trim() == inputDTO.GSTN.Trim()))).Count() != 0)
            {
                messageBuilder.Append($"Duplicate GSTN Number({inputDTO.GSTN.Trim()}) found</br>");
            }
            if (inputDTO.ContactNumber != null && tableData.Where((a => (a.ContactNumber != null && a.ContactNumber.Trim() == inputDTO.ContactNumber.Trim()))).Count() != 0)
            {
                messageBuilder.Append($"Duplicate Contact Number({inputDTO.ContactNumber.Trim()}) found</br>");
            }

            if (!string.IsNullOrEmpty(messageBuilder.ToString()))
                validMessage = messageBuilder.ToString();
            return validMessage;
        }

        [HttpGet]
        [Route("GetUsers")]
        [Produces("application/json", Type = typeof(UserKeyValues))]
        public IEnumerable<UserKeyValues> GetUsers(int UserType, int unitId)
        {
            try
            {
                var result = (_unitOfWork.UsersMaster.GetAll(p => p.IsActive == true && p.UserType == UserType && (p.UnitId == unitId || p.UserId == unitId)).Result
                               .Select(p => new UserKeyValues()
                               {
                                   UserId = p.UserId,
                                   FirstName = p.FirstName,
                                   LastName = p.LastName,
                                   FullName = p.FullName,
                                   BusinessOwner = p.BusinessOwner
                               })).ToList();
                // if(result.Count<=0)
                //{
                //     result = (_unitOfWork.UsersMaster.GetAll(p => p.IsActive == true && p.UserId == unitId).Result
                //            .Select(p => new UserKeyValues()
                //            {
                //                UserId = p.UserId,
                //                FirstName = p.FirstName,
                //                LastName = p.LastName
                //            })).ToList();
                //}

                return result;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving user {nameof(GetUsers)}");
                throw;
            }
        }

        // [HttpPost(Name = "GetEmployeeListing")]
        //[HttpGet]
        //[Route("GetUserListing1")]       
        //[Produces("application/json", Type = typeof(UsersMasterDTO))]
        //public async Task<IActionResult> GetUserListing1(Core.Helper.RequestParams requestParams,int? userType, int? logInUser)
        //{
        //    try
        //    {
        //        IList<UsersMasterDTO> outputModel = new List<UsersMasterDTO>();
        //        Expression<Func<UsersMaster, bool>> expression = a => a.UserType == userType && a.IsActive == true;
        //        outputModel = _mapper.Map<IList<UsersMasterDTO>>(await _unitOfWork.UsersMaster.GetPagedListWithExpression(requestParams, p => p.IsActive == true && p.UserType == userType));
        //        //  outputModel = await _unitOfWork.UsersMaster.GetUserListing(p => p.userType== userType && p.IsActive == true);
        //       // var  outputModel1 =  _unitOfWork.UsersMaster.FindAllByExpression(expression);
        //        //if (!isClient)
        //        //{
        //        //    outputModel = outputModel.Where(x => x.EmployeeCode != null).ToList();
        //        //}
        //        return Ok(outputModel);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Error in retriving user {nameof(GetUserListing1)}");
        //        throw;
        //    }
        //}

        [HttpGet]
        [Route("GetUserDashboard")]
        [Produces("application/json", Type = typeof(UsersMasterDTO))]
        public async Task<List<UsersMasterDTO>?> GetUserDashboard(int? userType, int? logInUser)
        {
            UsersMasterDTO EmployeeLeaveList = new UsersMasterDTO();
            var parms = new DynamicParameters();
            parms.Add(@"@UserType", userType, DbType.Int32);
            parms.Add(@"@UserId", logInUser, DbType.Int32);

            EmployeeLeaveList.UserList = (await _unitOfWork.UsersMasterDashboard.GetSPData<UsersMasterDTO>("usp_GetUserDashboard", parms));
            return EmployeeLeaveList.UserList;
        }


        [HttpGet]
        [Route("GetCVDashboard")]
        [Produces("application/json", Type = typeof(UsersMasterDTO))]
        public async Task<List<UsersMasterDTO>?> GetCVDashboard(int? userType, int? logInUser)
        {
            UsersMasterDTO UserList = new UsersMasterDTO();
            var parms = new DynamicParameters();
            parms.Add(@"@UserType", userType, DbType.Int32);
            parms.Add(@"@UserId", logInUser, DbType.Int32);

            UserList.UserList = (await _unitOfWork.UsersMasterDashboard.GetSPData<UsersMasterDTO>("usp_GetCVDashboard", parms));
            return UserList.UserList;
        }


        [HttpGet]
        [Route("GetCompanyDashboard")]
        [Produces("application/json", Type = typeof(UsersMasterDTO))]
        public async Task<List<UsersMasterDTO>?> GetCompanyDashboard(int? userId)
        {
            UsersMasterDTO UserList = new UsersMasterDTO();
            var parms = new DynamicParameters();
            parms.Add(@"@UserId", userId, DbType.Int32);

            UserList.UserList = (await _unitOfWork.UsersMasterDashboard.GetSPData<UsersMasterDTO>("usp_GetCompanyDashboard", parms));
            return UserList.UserList;
        }

        [HttpPost]
        [Route("GetLogin")]
        [Produces("application/json", Type = typeof(UsersMasterDTO))]
        public async Task<UsersMasterDTO> GetLogin()
        {
            return null;
        }

            [HttpPost]
        [Route("GetUserLogin")]
        [Produces("application/json", Type = typeof(UsersMasterDTO))]
        public async Task<UserSessionModel> GetUserLogin(UserLogin inPut)
        {
            //  var totalSw = System.Diagnostics.Stopwatch.StartNew();
            inPut.LoginPassword = CommonHelper.Encrypt(inPut.LoginPassword);
            UserSessionModel UserList = new UserSessionModel();
            var parms = new DynamicParameters();
            parms.Add(@"@UserName", inPut.EmailAddress, DbType.String);
            parms.Add(@"@UserPassword", inPut.LoginPassword, DbType.String);

            var dbSw = System.Diagnostics.Stopwatch.StartNew();
            UserList = (await _unitOfWork.UsersMasterDashboard.GetSPData<UserSessionModel>("usp_GetLogin", parms)).SingleOrDefault();
            //dbSw.Stop();
            //_logger.LogInformation($"DB call took {dbSw.ElapsedMilliseconds} ms");

            //totalSw.Stop();
            //_logger.LogInformation($"Total API call took {totalSw.ElapsedMilliseconds} ms");

            return UserList;
        }

        [HttpGet]
        [Route("GetUnitUsers")]
        [Produces("application/json", Type = typeof(UserKeyValues))]
        public IEnumerable<UserKeyValues> GetUnitUsers(int? unitId)
        {
            try
            {
                return (_unitOfWork.UsersMaster.GetAll(p => p.IsActive == true && (p.UnitId == unitId || p.UserId == unitId)).Result.Where(p => (p.UserType == 1 || p.UserType == 0))

                               .Select(p => new UserKeyValues()
                               {
                                   BusinessOwner = p.BusinessOwner,
                                   FirstName = p.FirstName,
                                   LastName = p.LastName,
                                   FullName = (p.FullName ==null? p.BusinessOwner: p.FullName ==" "? p.BusinessOwner: p.FullName),
                                   UserId = p.UserId,
                                   Email= (p.EmailAddress==null?p.BusinessEmail : p.EmailAddress),
                                   Base64ProfileImage = p.ProfileName
                               })).ToList();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving GetUnitPriority {nameof(GetUsers)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetUserList")]
        [Produces("application/json", Type = typeof(UserKeyValues))]
        public IEnumerable<UserKeyValues> GetUserList(int? unitId, int? userType)
        {
            try
            {
                return (_unitOfWork.UsersMaster.GetAll(p => p.IsActive == true && (p.UnitId == unitId || p.UserId == unitId)).Result.Where(p => (p.UserType == userType))

                               .Select(p => new UserKeyValues()
                               {
                                   BusinessOwner = p.BusinessOwner,
                                   FirstName = p.FirstName,
                                   LastName = p.LastName,
                                   UserId = p.UserId,
                                   FullName = (p.FullName == null ? p.CompanyName : p.FullName == " " ? p.CompanyName : p.FullName),
                                   Base64ProfileImage = p.ProfileName
                               })).ToList();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving GetUserList {nameof(GetUserList)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetUserById")]
        [Produces("application/json", Type = typeof(ProjectsDTO))]
        public async Task<IActionResult> GetUserById(int uId)
        {
            try
            {
                UsersMasterDTO outputDTO = _mapper.Map<UsersMasterDTO>(await _unitOfWork.UsersMaster.GetByIdAsync(uId));
                HttpResponseMessage httpMessage = new HttpResponseMessage();
                if (outputDTO == null)
                {
                    httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO);
                    outputDTO = CommonHelper.GetClassObject(outputDTO);
                }
                else
                    httpMessage = CommonHelper.GetHttpResponseMessage(outputDTO, outputDTO.IsActive);

                outputDTO.HttpMessage = httpMessage;
                return Ok(outputDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving user {nameof(GetUserById)}");
                throw;
            }
        }

        [HttpGet]
        [Route("DeleteUser")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DeleteUser(int uId)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    UsersMaster outputMaster = _mapper.Map<UsersMaster>(await _unitOfWork.UsersMaster.GetByIdAsync(uId));
                    outputMaster.IsActive = false;
                    _unitOfWork.UsersMaster.Update(outputMaster);
                    _unitOfWork.Save();
                    outPut.HttpStatusCode = 200;
                    outPut.DisplayMessage = "User delete successfully!";
                    return Ok(outPut);
                }
                else
                {
                    outPut.HttpStatusCode = 201;
                    outPut.DisplayMessage = "Failed";
                    return Ok(outPut);
                }
                // return Ok(ClientResponse.GetClientResponse(HttpStatusCode.UnprocessableEntity, "Invalid Model"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while deleting Delete {nameof(DeleteUser)}");
                throw;
            }
        }
    }
}
