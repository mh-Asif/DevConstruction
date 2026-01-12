using AutoMapper;
using Construction.Infrastructure.Helper;
using Construction.Infrastructure.KeyValues;
using Construction.Infrastructure.Models;
using ConstructionApp.Core.Entities;
using ConstructionApp.Core.Repository;
using ConstructionApp.Services.DBContext;
using Dapper;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Linq.Expressions;
using System.Net;
using System.Text;

namespace ConstructionApp.EndPoints.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterAPIsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MasterAPIsController> _logger;
        private readonly IMapper _mapper;
        private readonly ConstDbContext _constDbContext;

        public MasterAPIsController(IUnitOfWork unitOfWork, ILogger<MasterAPIsController> logger, IMapper mapper, ConstDbContext constDbContext)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _constDbContext = constDbContext;
        }

        [HttpGet]
        [Route("GetCountry")]
        [Produces("application/json", Type = typeof(CountryKeyValues))]
        public IEnumerable<CountryKeyValues> GetCountry()
        {
            try
            {
                return (_unitOfWork.CountryMaster.GetAll(p => p.IsActive == true).Result
                               .Select(p => new CountryKeyValues()
                               {
                                   CountryId = p.CountryId,
                                   CountryCode = p.CountryCode,
                                   CountryName = p.CountryName
                               })).ToList();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving country {nameof(GetCountry)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetState")]
        [Produces("application/json", Type = typeof(StateMasterDTO))]
        public IEnumerable<StateMasterDTO> GetState()
        {
            try
            {
                return (_unitOfWork.StateMaster.GetAll(p => p.IsActive == true).Result
                               .Select(p => new StateMasterDTO()
                               {
                                   StateId = p.StateId,
                                   StateName = p.StateName,
                                   CountryId = p.CountryId,
                                   CountryName = p.Country.CountryName
                               })).ToList();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving State {nameof(GetState)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetCountryState")]
        [Produces("application/json", Type = typeof(StateKeyValues))]
        public IEnumerable<StateKeyValues> GetCountryState(int countryId)
        {
            try
            {
                return (_unitOfWork.StateMaster.GetAll(p => p.IsActive == true && p.CountryId == countryId).Result
                               .Select(p => new StateKeyValues()
                               {
                                   StateId = p.StateId,
                                   StateName = p.StateName
                               })).ToList();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving State {nameof(GetCountryState)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetCity")]
        [Produces("application/json", Type = typeof(CityMasterDTO))]
        public IEnumerable<CityMasterDTO> GetCity()
        {
            try
            {
                return (_unitOfWork.CityMaster.GetAll(p => p.IsActive == true).Result
                               .Select(p => new CityMasterDTO()
                               {
                                   CityId = p.CityId,
                                   CityName = p.CityName,
                                   CountryName = p.Country.CountryName,
                                   StateName = p.State.StateName
                               })).ToList();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving City {nameof(GetCity)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetStateCity")]
        [Produces("application/json", Type = typeof(CityKeyValues))]
        public IEnumerable<CityKeyValues> GetStateCity(int stateId)
        {
            try
            {
                return (_unitOfWork.CityMaster.GetAll(p => p.IsActive == true && p.StateId == stateId).Result
                               .Select(p => new CityKeyValues()
                               {
                                   CityId = p.CityId,
                                   CityName = p.CityName
                               })).ToList();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving State {nameof(GetStateCity)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetStatus")]
        [Produces("application/json", Type = typeof(StatusKeyValues))]
        public IEnumerable<StatusKeyValues> GetStatus()
        {
            try
            {
                return (_unitOfWork.StatusMaster.GetAll(p => p.IsActive == true).Result
                               .Select(p => new StatusKeyValues()
                               {
                                   StatusId = p.StatusId,
                                   Status = p.Status
                               })).ToList();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving GetStatus {nameof(GetStatus)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetPriority")]
        [Produces("application/json", Type = typeof(PriorityMasterDTO))]
        public IEnumerable<PriorityMasterDTO> GetPriority()
        {
            try
            {
                return (_unitOfWork.PriorityMaster.GetAll(p => p.IsActive == true).Result
                               .Select(p => new PriorityMasterDTO()
                               {
                                   PriorityId = p.PriorityId,
                                   Priority = p.Priority
                               })).ToList();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving GetPriority {nameof(GetPriority)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetUnitPriority")]
        [Produces("application/json", Type = typeof(CompanyPriorityMasterDTO))]
        public IEnumerable<CompanyPriorityMasterDTO> GetUnitPriority(int? unitId)
        {
            try
            {
                return (_unitOfWork.CompanyPriorityMaster.GetAll(p => p.IsActive == true && p.CompanyId == unitId).Result
                               .Select(p => new CompanyPriorityMasterDTO()
                               {
                                   CompanyPriority = p.CompanyPriority,
                                   CompanyId = p.CompanyId,
                                   UnitPriorityId = p.UnitPriorityId,
                                   CompanyPriorityId = p.CompanyPriorityId
                               })).ToList();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving GetUnitPriority {nameof(GetUnitPriority)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetUnitStatus")]
        [Produces("application/json", Type = typeof(UnitStatusMasterDTO))]
        public IEnumerable<UnitStatusMasterDTO> GetUnitStatus(int? unitId, string? statusType)
        {
            try
            {
                if (statusType == "B")
                {
                    return (_unitOfWork.UnitStatusMaster.GetAll(p => p.IsActive == true && p.UnitId == unitId).Result
                                   .Select(p => new UnitStatusMasterDTO()
                                   {
                                       Status = p.Status,
                                       UnitId = p.UnitId,
                                       MasterStatusId = p.MasterStatusId,
                                       StatusType = p.StatusType,
                                       ID = p.ID
                                   })).ToList();
                }
                else
                {
                    return (_unitOfWork.UnitStatusMaster.GetAll(p => p.IsActive == true && p.UnitId == unitId && p.StatusType.Trim() == statusType).Result
                                                     .Select(p => new UnitStatusMasterDTO()
                                                     {
                                                         Status = p.Status,
                                                         UnitId = p.UnitId,
                                                         MasterStatusId = p.MasterStatusId,
                                                         StatusType = p.StatusType,
                                                         ID = p.ID
                                                     })).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving GetUnitPriority {nameof(GetUnitStatus)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetUnitPhases")]
        [Produces("application/json", Type = typeof(UnitPhasesMasterDTO))]
        public IEnumerable<UnitPhasesMasterDTO> GetUnitPhases(int? unitId)
        {
            try
            {
                return (_unitOfWork.UnitPhasesMaster.GetAll(p => p.IsActive == true && p.UnitId == unitId).Result
                               .Select(p => new UnitPhasesMasterDTO()
                               {
                                   Phase = p.Phase,
                                   UnitId = p.UnitId,
                                   MasterPhaseId = p.MasterPhaseId,
                                   ID = p.ID
                               })).ToList();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving GetUnitPhases {nameof(GetUnitPhases)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetPhases")]
        [Produces("application/json", Type = typeof(PhasesMasterDTO))]
        public IEnumerable<PhasesMasterDTO> GetPhases()
        {
            try
            {
                return (_unitOfWork.PhasesMaster.GetAll(p => p.IsActive == true).Result
                               .Select(p => new PhasesMasterDTO()
                               {
                                   PhaseID = p.PhaseID,
                                   Phases = p.Phases
                               })).ToList();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving GetPhases {nameof(GetPhases)}");
                throw;
            }
        }

        //[HttpGet]
        //[Route("GetUnitStatus")]
        //[Produces("application/json", Type = typeof(PhasesMasterDTO))]
        //public IEnumerable<PhasesMasterDTO> GetUnitStatus(int? unitId)
        //{
        //    try
        //    {
        //        return (_unitOfWork.PhasesMaster.GetAll(p => p.IsActive == true).Result
        //                       .Select(p => new PhasesMasterDTO()
        //                       {
        //                           PhaseID = p.PhaseID,
        //                           Phases = p.Phases
        //                       })).ToList();

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Error in retriving GetPhases {nameof(GetPhases)}");
        //        throw;
        //    }
        //}

        [HttpGet]
        [Route("GetDepartment")]
        [Produces("application/json", Type = typeof(DepartmentKeyValues))]
        public IEnumerable<DepartmentKeyValues> GetDepartment()
        {
            try
            {
                return (_unitOfWork.DepartmentMaster.GetAll(p => p.IsActive == true).Result
                               .Select(p => new DepartmentKeyValues()
                               {
                                   DepartmentId = p.DepartmentId,
                                   DepartmentName = p.DepartmentName,
                                   DepartmentCode = p.DepartmentCode
                               })).ToList();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving Get Department {nameof(GetDepartment)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetJobTitle")]
        [Produces("application/json", Type = typeof(JobKeyValues))]
        public IEnumerable<JobKeyValues> GetJobTitle()
        {
            try
            {
                return (_unitOfWork.JobTitleMaster.GetAll(p => p.IsActive == true).Result
                               .Select(p => new JobKeyValues()
                               {
                                   JobTitleId = p.JobTitleId,
                                   JobTitle = p.JobTitle
                               })).ToList();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving Get JobTitle {nameof(GetJobTitle)}");
                throw;
            }
        }


        [HttpGet]
        [Route("GetRole")]
        [Produces("application/json", Type = typeof(RoleKeyValues))]
        public IEnumerable<RoleKeyValues> GetRole()
        {
            try
            {
                return (_unitOfWork.RoleMaster.GetAll(p => p.IsActive == true).Result
                               .Select(p => new RoleKeyValues()
                               {
                                   RoleId = p.RoleId,
                                   RoleName = p.RoleName
                               })).ToList();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving Get role {nameof(GetRole)}");
                throw;
            }
        }


        [HttpGet]
        [Route("GetDocumentCategory")]
        [Produces("application/json", Type = typeof(DocumentCategoryDTO))]
        public IEnumerable<DocumentCategoryDTO> GetDocumentCategory()
        {
            try
            {
                return (_unitOfWork.DocumentCategory.GetAll(p => p.IsActive == true).Result
                               .Select(p => new DocumentCategoryDTO()
                               {
                                    Id = p.Id,
                                    Category = p.Category,
                                    IsDisable=p.IsDisable
                               })).ToList();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving Get Document Category {nameof(GetDocumentCategory)}");
                throw;
            }
        }

        //[HttpGet]
        //[Route("GetDocumentCategory")]
        //[Produces("application/json", Type = typeof(DocumentCategoryDTO))]
        //public IEnumerable<DocumentCategoryDTO> GetDocumentCategory(int? id)
        //{
        //    try
        //    {
        //        return (_unitOfWork.DocumentCategory.GetAll(p => p.IsActive == true && p.IsDisable == true && p.Id==id).Result
        //                       .Select(p => new DocumentCategoryDTO()
        //                       {
        //                           Id = p.Id,
        //                           Category = p.Category,
        //                           IsDisable = p.IsDisable
        //                       })).ToList();

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Error in retriving Get Document Category {nameof(GetDocumentCategory)}");
        //        throw;
        //    }
        //}

        [HttpGet]
        [Route("GetDrawingCategory")]
        [Produces("application/json", Type = typeof(DrawingCategoryDTO))]
        public IEnumerable<DrawingCategoryDTO> GetDrawingCategory()
        {
            try
            {
                return (_unitOfWork.DrawingCategory.GetAll(p => p.IsActive == true).Result
                               .Select(p => new DrawingCategoryDTO()
                               {
                                   Id = p.Id,
                                   Category = p.Category,
                                   IsActive=p.IsActive,
                                   IsDisable=p.IsDisable
                               })).ToList();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving Get Document Category {nameof(GetDrawingCategory)}");
                throw;
            }
        }


        [HttpGet]
        [Route("GetPhotoCategory")]
        [Produces("application/json", Type = typeof(PhotoCategoryDTO))]
        public IEnumerable<PhotoCategoryDTO> GetPhotoCategory()
        {
            try
            {
                return (_unitOfWork.PhotoCategory.GetAll(p => p.IsActive == true).Result
                               .Select(p => new PhotoCategoryDTO()
                               {
                                   Id = p.Id,
                                   Category = p.Category,
                                   IsDisable = p.IsDisable
                               })).ToList();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving Get Document Category {nameof(GetPhotoCategory)}");
                throw;
            }
        }

        [HttpPost]
        [Route("CountryMaster")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> CountryMaster(CountryKeyValues inputDTO)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {


                if (ModelState.IsValid)
                {
                    if (Convert.ToInt32(inputDTO.CountryId) == 0)
                    {
                        CountryMasterDTO inputData = new CountryMasterDTO();
                        inputData.CountryName = inputDTO.CountryName;
                        inputData.CountryId = inputDTO.CountryId;
                        inputData.CountryCode = inputDTO.CountryCode;
                        inputData.CreatedOn = DateTime.Now;
                        inputData.IsActive = true;


                        Expression<Func<CountryMaster, bool>> expression = a => a.CountryName.Trim().Replace(" ", "") == inputDTO.CountryName.Trim().Replace(" ", "") && a.IsActive == true;
                        if (!_unitOfWork.CountryMaster.Exists(expression))
                        {
                            _unitOfWork.CountryMaster.AddAsync(_mapper.Map<CountryMaster>(inputData));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Country saved successfully!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "Duplicate Entry Found!";
                            return Ok(outPut);
                        }


                    }
                    else
                    {
                        CountryMaster outputDetails = _mapper.Map<CountryMaster>(await _unitOfWork.CountryMaster.GetByIdAsync(Convert.ToInt32(inputDTO.CountryId)));
                        if (outputDetails != null)
                        {
                            outputDetails.CountryName = inputDTO.CountryName;
                            outputDetails.CountryCode = inputDTO.CountryCode;
                            outputDetails.ModifiedOn = DateTime.Now;

                        }
                        Expression<Func<CountryMaster, bool>> expression = a => a.CountryId != Convert.ToInt32(inputDTO.CountryId) && a.IsActive == true;
                        if (_unitOfWork.CountryMaster.Exists(expression))
                        {
                            _unitOfWork.CountryMaster.Update(_mapper.Map<CountryMaster>(outputDetails));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Country updated successfully!";
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
                outPut.HttpStatusCode = 201;
                outPut.DisplayMessage = "Transaction failed!";
                return Ok(outPut);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving country {nameof(CountryMaster)}");
                outPut.HttpStatusCode = 400;
                outPut.DisplayMessage = "Registration failed!";
                return Ok(outPut);
            }
        }



        [HttpPost]
        [Route("StateMaster")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> StateMaster(StateKeyValues inputDTO)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {


                if (ModelState.IsValid)
                {
                    if (Convert.ToInt32(inputDTO.StateId) == 0)
                    {
                        StateMasterDTO inputData = new StateMasterDTO();
                        inputData.StateName = inputDTO.StateName;
                        inputData.CountryId = inputDTO.CountryId;
                        inputData.CreatedOn = DateTime.Now;
                        inputData.IsActive = true;

                        Expression<Func<StateMaster, bool>> expression = a => a.StateName.Trim().Replace(" ", "") == inputDTO.StateName.Trim().Replace(" ", "") && a.IsActive == true;
                        if (!_unitOfWork.StateMaster.Exists(expression))
                        {
                            _unitOfWork.StateMaster.AddAsync(_mapper.Map<StateMaster>(inputData));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "State successfully saved!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "Duplicate Entry Found!";
                            return Ok(outPut);
                        }


                    }
                    else
                    {
                        StateMaster outputDetails = _mapper.Map<StateMaster>(await _unitOfWork.StateMaster.GetByIdAsync(Convert.ToInt32(inputDTO.StateId)));
                        if (outputDetails != null)
                        {
                            outputDetails.StateName = inputDTO.StateName;
                            outputDetails.CountryId = inputDTO.CountryId;
                            outputDetails.ModifiedOn = DateTime.Now;
                            //outputDetails.ModifiedBy= inputDTO.ModifiedBy;

                        }
                        Expression<Func<StateMaster, bool>> expression = a => a.StateId != Convert.ToInt32(inputDTO.StateId) && a.IsActive == true;
                        if (_unitOfWork.StateMaster.Exists(expression))
                        {
                            _unitOfWork.StateMaster.Update(_mapper.Map<StateMaster>(outputDetails));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "State updated successfully!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "State updated failed!";
                            return Ok(outPut);
                        }
                    }
                }
                outPut.HttpStatusCode = 201;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving State {nameof(StateMaster)}");
                outPut.HttpStatusCode = 400;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);
            }
        }

        [HttpPost]
        [Route("CityMaster")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> CityMaster(CityKeyValues inputDTO)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {


                if (ModelState.IsValid)
                {
                    if (Convert.ToInt32(inputDTO.CityId) == 0)
                    {
                        CityMasterDTO inputData = new CityMasterDTO();
                        inputData.CityName = inputDTO.CityName;
                        inputData.CountryId = inputDTO.CountryId;
                        inputData.StateId = inputDTO.StateId;
                        inputData.CreatedOn = DateTime.Now;
                        inputData.IsActive = true;


                        Expression<Func<CityMaster, bool>> expression = a => a.CityName.Trim().Replace(" ", "") == inputDTO.CityName.Trim().Replace(" ", "") && a.IsActive == true;
                        if (!_unitOfWork.CityMaster.Exists(expression))
                        {
                            _unitOfWork.CityMaster.AddAsync(_mapper.Map<CityMaster>(inputData));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "State successfully saved!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "Duplicate Entry Found!";
                            return Ok(outPut);
                        }

                        //_unitOfWork.CityMaster.AddAsync(_mapper.Map<CityMaster>(inputData));
                        //_unitOfWork.Save();
                        //outPut.HttpStatusCode = 200;
                        //outPut.DisplayMessage = "City successfully saved!";
                        //return Ok(outPut);

                    }
                    else
                    {
                        CityMaster outputDetails = _mapper.Map<CityMaster>(await _unitOfWork.CityMaster.GetByIdAsync(Convert.ToInt32(inputDTO.CityId)));
                        if (outputDetails != null)
                        {
                            outputDetails.CityName = inputDTO.CityName;
                            outputDetails.StateId = inputDTO.StateId;
                            outputDetails.CountryId = inputDTO.CountryId;
                            outputDetails.ModifiedOn = DateTime.Now;
                            //outputDetails.ModifiedBy= inputDTO.ModifiedBy;

                        }
                        Expression<Func<CityMaster, bool>> expression = a => a.CityId != Convert.ToInt32(inputDTO.CityId) && a.IsActive == true;
                        if (_unitOfWork.CityMaster.Exists(expression))
                        {
                            _unitOfWork.CityMaster.Update(_mapper.Map<CityMaster>(outputDetails));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "City updated successfully!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "City updated failed!";
                            return Ok(outPut);
                        }
                    }
                }
                outPut.HttpStatusCode = 201;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving City Master {nameof(CityMaster)}");
                outPut.HttpStatusCode = 400;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);
            }
        }

        [HttpPost]
        [Route("StatusMaster")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> StatusMaster(StatusKeyValues inputDTO)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {


                if (ModelState.IsValid)
                {
                    if (Convert.ToInt32(inputDTO.StatusId) == 0)
                    {
                        StatusMasterDTO inputData = new StatusMasterDTO();
                        inputData.Status = inputDTO.Status;
                        //  inputData.CreatedOn = DateTime.Now;
                        inputData.IsActive = true;

                        Expression<Func<StatusMaster, bool>> expression = a => a.Status.Trim().Replace(" ", "") == inputDTO.Status.Trim().Replace(" ", "") && a.IsActive == true;
                        if (!_unitOfWork.StatusMaster.Exists(expression))
                        {
                            _unitOfWork.StatusMaster.AddAsync(_mapper.Map<StatusMaster>(inputData));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Status successfully saved!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "Duplicate Entry Found!";
                            return Ok(outPut);
                        }



                    }
                    else
                    {
                        StatusMaster outputDetails = _mapper.Map<StatusMaster>(await _unitOfWork.StatusMaster.GetByIdAsync(Convert.ToInt32(inputDTO.StatusId)));
                        if (outputDetails != null)
                        {
                            outputDetails.Status = inputDTO.Status;
                            // outputDetails.ModifiedOn = DateTime.Now;
                            //outputDetails.ModifiedBy= inputDTO.ModifiedBy;

                        }
                        Expression<Func<StatusMaster, bool>> expression = a => a.StatusId != Convert.ToInt32(inputDTO.StatusId) && a.IsActive == true;
                        if (_unitOfWork.StatusMaster.Exists(expression))
                        {
                            _unitOfWork.StatusMaster.Update(_mapper.Map<StatusMaster>(outputDetails));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Status updated successfully!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "Status updated failed!";
                            return Ok(outPut);
                        }
                    }
                }
                outPut.HttpStatusCode = 201;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving Status Master {nameof(StatusMaster)}");
                outPut.HttpStatusCode = 400;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);
            }
        }

        [HttpPost]
        [Route("PriorityMaster")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> PriorityMaster(PriorityKeyValues inputDTO)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {


                if (ModelState.IsValid)
                {
                    if (Convert.ToInt32(inputDTO.PriorityId) == 0)
                    {
                        PriorityMasterDTO inputData = new PriorityMasterDTO();
                        inputData.Priority = inputDTO.Priority;
                        // inputData.CreatedOn = DateTime.Now;
                        inputData.IsActive = true;

                        Expression<Func<PriorityMaster, bool>> expression = a => a.Priority.Trim().Replace(" ", "") == inputDTO.Priority.Trim().Replace(" ", "") && a.IsActive == true;
                        if (!_unitOfWork.PriorityMaster.Exists(expression))
                        {
                            _unitOfWork.PriorityMaster.AddAsync(_mapper.Map<PriorityMaster>(inputData));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Priority successfully saved!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "Duplicate Entry Found!";
                            return Ok(outPut);
                        }



                    }
                    else
                    {
                        PriorityMaster outputDetails = _mapper.Map<PriorityMaster>(await _unitOfWork.PriorityMaster.GetByIdAsync(Convert.ToInt32(inputDTO.PriorityId)));
                        if (outputDetails != null)
                        {
                            outputDetails.Priority = inputDTO.Priority;
                            // outputDetails.ModifiedOn = DateTime.Now;
                            //outputDetails.ModifiedBy= inputDTO.ModifiedBy;

                        }
                        Expression<Func<PriorityMaster, bool>> expression = a => a.PriorityId != Convert.ToInt32(inputDTO.PriorityId) && a.IsActive == true;
                        if (_unitOfWork.PriorityMaster.Exists(expression))
                        {
                            _unitOfWork.PriorityMaster.Update(_mapper.Map<PriorityMaster>(outputDetails));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Priority updated successfully!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "Priority updated failed!";
                            return Ok(outPut);
                        }
                    }
                }
                outPut.HttpStatusCode = 201;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving Status Master {nameof(StatusMaster)}");
                outPut.HttpStatusCode = 400;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);
            }
        }

        [HttpPost]
        [Route("DepartmentMaster")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DepartmentMaster(DepartmentKeyValues inputDTO)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {


                if (ModelState.IsValid)
                {
                    if (Convert.ToInt32(inputDTO.DepartmentId) == 0)
                    {
                        DepartmentMasterDTO inputData = new DepartmentMasterDTO();
                        inputData.DepartmentName = inputDTO.DepartmentName;
                        inputData.DepartmentCode = inputDTO.DepartmentCode;
                        inputData.CreatedOn = DateTime.Now;
                        inputData.IsActive = true;

                        Expression<Func<DepartmentMaster, bool>> expression = a => a.DepartmentName.Trim().Replace(" ", "") == inputDTO.DepartmentName.Trim().Replace(" ", "") && a.IsActive == true;
                        if (!_unitOfWork.DepartmentMaster.Exists(expression))
                        {
                            _unitOfWork.DepartmentMaster.AddAsync(_mapper.Map<DepartmentMaster>(inputData));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Department successfully saved!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "Duplicate Entry Found!";
                            return Ok(outPut);
                        }



                    }
                    else
                    {
                        DepartmentMaster outputDetails = _mapper.Map<DepartmentMaster>(await _unitOfWork.DepartmentMaster.GetByIdAsync(Convert.ToInt32(inputDTO.DepartmentId)));
                        if (outputDetails != null)
                        {
                            outputDetails.DepartmentName = inputDTO.DepartmentName;
                            outputDetails.DepartmentCode = inputDTO.DepartmentCode;
                            outputDetails.ModifiedOn = DateTime.Now;

                        }
                        Expression<Func<DepartmentMaster, bool>> expression = a => a.DepartmentId != Convert.ToInt32(inputDTO.DepartmentId) && a.IsActive == true;
                        if (_unitOfWork.DepartmentMaster.Exists(expression))
                        {
                            _unitOfWork.DepartmentMaster.Update(_mapper.Map<DepartmentMaster>(outputDetails));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Department updated successfully!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "Department updated failed!";
                            return Ok(outPut);
                        }
                    }
                }
                outPut.HttpStatusCode = 201;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving Department Master {nameof(DepartmentMaster)}");
                outPut.HttpStatusCode = 400;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);
            }
        }

        [HttpPost]
        [Route("JobTitle")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> JobTitle(JobKeyValues inputDTO)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {


                if (ModelState.IsValid)
                {
                    if (Convert.ToInt32(inputDTO.JobTitleId) == 0)
                    {
                        JobTitleMasterDTO inputData = new JobTitleMasterDTO();
                        inputData.JobTitle = inputDTO.JobTitle;
                        inputData.CreatedOn = DateTime.Now;
                        inputData.IsActive = true;

                        Expression<Func<JobTitleMaster, bool>> expression = a => a.JobTitle.Trim().Replace(" ", "") == inputDTO.JobTitle.Trim().Replace(" ", "") && a.IsActive == true;
                        if (!_unitOfWork.JobTitleMaster.Exists(expression))
                        {
                            _unitOfWork.JobTitleMaster.AddAsync(_mapper.Map<JobTitleMaster>(inputData));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Job Title successfully saved!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "Duplicate Entry Found!";
                            return Ok(outPut);
                        }


                    }
                    else
                    {
                        JobTitleMaster outputDetails = _mapper.Map<JobTitleMaster>(await _unitOfWork.JobTitleMaster.GetByIdAsync(Convert.ToInt32(inputDTO.JobTitleId)));
                        if (outputDetails != null)
                        {
                            outputDetails.JobTitle = inputDTO.JobTitle;
                            outputDetails.ModifiedOn = DateTime.Now;

                        }
                        Expression<Func<JobTitleMaster, bool>> expression = a => a.JobTitleId != Convert.ToInt32(inputDTO.JobTitleId) && a.IsActive == true;
                        if (_unitOfWork.JobTitleMaster.Exists(expression))
                        {

                            _unitOfWork.JobTitleMaster.Update(_mapper.Map<JobTitleMaster>(outputDetails));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Job Title updated successfully!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "Job Title updated failed!";
                            return Ok(outPut);
                        }
                    }
                }
                outPut.HttpStatusCode = 201;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving Job Title {nameof(JobTitle)}");
                outPut.HttpStatusCode = 400;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);
            }
        }


        [HttpPost]
        [Route("RoleMaster")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> RoleMaster(RoleKeyValues inputDTO)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {


                if (ModelState.IsValid)
                {
                    if (Convert.ToInt32(inputDTO.RoleId) == 0)
                    {
                        RoleMasterDTO inputData = new RoleMasterDTO();
                        inputData.RoleName = inputDTO.RoleName;
                        inputData.CreatedOn = DateTime.Now;
                        //  inputData.CreatedBy = inputDTO.CreatedBy;
                        inputData.IsActive = true;

                        Expression<Func<RoleMaster, bool>> expression = a => a.RoleName.Trim().Replace(" ", "") == inputDTO.RoleName.Trim().Replace(" ", "") && a.IsActive == true;
                        if (!_unitOfWork.RoleMaster.Exists(expression))
                        {
                            _unitOfWork.RoleMaster.AddAsync(_mapper.Map<RoleMaster>(inputData));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Role successfully saved!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "Duplicate Entry Found!";
                            return Ok(outPut);
                        }


                    }
                    else
                    {
                        RoleMaster outputDetails = _mapper.Map<RoleMaster>(await _unitOfWork.RoleMaster.GetByIdAsync(Convert.ToInt32(inputDTO.RoleId)));
                        if (outputDetails != null)
                        {
                            outputDetails.RoleName = inputDTO.RoleName;
                            outputDetails.modifiedOn = DateTime.Now;

                        }
                        Expression<Func<RoleMaster, bool>> expression = a => a.RoleId != Convert.ToInt32(inputDTO.RoleId) && a.IsActive == true;
                        if (_unitOfWork.RoleMaster.Exists(expression))
                        {

                            _unitOfWork.RoleMaster.Update(_mapper.Map<RoleMaster>(outputDetails));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Role updated successfully!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "Role updated failed!";
                            return Ok(outPut);
                        }
                    }
                }
                outPut.HttpStatusCode = 201;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving role {nameof(RoleMaster)}");
                outPut.HttpStatusCode = 400;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);
            }
        }

        [HttpPost]
        [Route("DocumentCategory")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DocumentCategory(DocumentCategoryDTO inputDTO)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {


                if (ModelState.IsValid)
                {
                    if (Convert.ToInt32(inputDTO.Id) == 0)
                    {
                        //DocumentCategoryDTO inputData = new DocumentCategoryDTO();
                        //inputData.RoleName = inputDTO.RoleName;
                        inputDTO.CreationDate = DateTime.Now;
                        //  inputData.CreatedBy = inputDTO.CreatedBy;
                        inputDTO.IsActive = true;

                        Expression<Func<DocumentCategory, bool>> expression = a => a.Category.Trim().Replace(" ", "") == inputDTO.Category.Trim().Replace(" ", "") && a.IsActive == true;
                        if (!_unitOfWork.DocumentCategory.Exists(expression))
                        {
                            _unitOfWork.DocumentCategory.AddAsync(_mapper.Map<DocumentCategory>(inputDTO));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Category successfully saved!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "Duplicate Entry Found!";
                            return Ok(outPut);
                        }


                    }
                    else
                    {
                        DocumentCategory outputDetails = _mapper.Map<DocumentCategory>(await _unitOfWork.DocumentCategory.GetByIdAsync(Convert.ToInt32(inputDTO.Id)));
                        if (outputDetails != null)
                        {
                            outputDetails.Category = inputDTO.Category;
                          //  outputDetails.modifiedOn = DateTime.Now;

                        }
                        Expression<Func<DocumentCategory, bool>> expression = a => a.Id != Convert.ToInt32(inputDTO.Id) && a.IsActive == true;
                        if (_unitOfWork.DocumentCategory.Exists(expression))
                        {

                            _unitOfWork.DocumentCategory.Update(_mapper.Map<DocumentCategory>(outputDetails));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Category updated successfully!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "failed!";
                            return Ok(outPut);
                        }
                    }
                }
                outPut.HttpStatusCode = 201;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving document {nameof(DocumentCategory)}");
                outPut.HttpStatusCode = 400;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);
            }
        }

        [HttpPost]
        [Route("DrawingCategory")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DrawingCategory(DrawingCategoryDTO inputDTO)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {


                if (ModelState.IsValid)
                {
                    if (Convert.ToInt32(inputDTO.Id) == 0)
                    {
                        inputDTO.CreationDate = DateTime.Now;                     
                        inputDTO.IsActive = true;

                        Expression<Func<DrawingCategory, bool>> expression = a => a.Category.Trim().Replace(" ", "") == inputDTO.Category.Trim().Replace(" ", "") && a.IsActive == true;
                        if (!_unitOfWork.DrawingCategory.Exists(expression))
                        {
                            _unitOfWork.DrawingCategory.AddAsync(_mapper.Map<DrawingCategory>(inputDTO));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Category successfully saved!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "Duplicate Entry Found!";
                            return Ok(outPut);
                        }


                    }
                    else
                    {
                        DrawingCategory outputDetails = _mapper.Map<DrawingCategory>(await _unitOfWork.DrawingCategory.GetByIdAsync(Convert.ToInt32(inputDTO.Id)));
                        if (outputDetails != null)
                        {
                            outputDetails.Category = inputDTO.Category;
                            //  outputDetails.modifiedOn = DateTime.Now;

                        }
                        Expression<Func<DrawingCategory, bool>> expression = a => a.Id != Convert.ToInt32(inputDTO.Id) && a.IsActive == true;
                        if (_unitOfWork.DrawingCategory.Exists(expression))
                        {

                            _unitOfWork.DrawingCategory.Update(_mapper.Map<DrawingCategory>(outputDetails));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Category updated successfully!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "failed!";
                            return Ok(outPut);
                        }
                    }
                }
                outPut.HttpStatusCode = 201;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving document {nameof(DrawingCategory)}");
                outPut.HttpStatusCode = 400;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);
            }
        }



        [HttpPost]
        [Route("PhotoCategory")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> PhotoCategory(PhotoCategoryDTO inputDTO)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {


                if (ModelState.IsValid)
                {
                    if (Convert.ToInt32(inputDTO.Id) == 0)
                    {
                        inputDTO.CreationDate = DateTime.Now;
                        inputDTO.IsActive = true;

                        Expression<Func<PhotoCategory, bool>> expression = a => a.Category.Trim().Replace(" ", "") == inputDTO.Category.Trim().Replace(" ", "") && a.IsActive == true;
                        if (!_unitOfWork.PhotoCategory.Exists(expression))
                        {
                            _unitOfWork.PhotoCategory.AddAsync(_mapper.Map<PhotoCategory>(inputDTO));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Category successfully saved!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "Duplicate Entry Found!";
                            return Ok(outPut);
                        }


                    }
                    else
                    {
                        PhotoCategory outputDetails = _mapper.Map<PhotoCategory>(await _unitOfWork.PhotoCategory.GetByIdAsync(Convert.ToInt32(inputDTO.Id)));
                        if (outputDetails != null)
                        {
                            outputDetails.Category = inputDTO.Category;
                            //  outputDetails.modifiedOn = DateTime.Now;

                        }
                        Expression<Func<PhotoCategory, bool>> expression = a => a.Id != Convert.ToInt32(inputDTO.Id) && a.IsActive == true;
                        if (_unitOfWork.PhotoCategory.Exists(expression))
                        {

                            _unitOfWork.PhotoCategory.Update(_mapper.Map<PhotoCategory>(outputDetails));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Category updated successfully!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "failed!";
                            return Ok(outPut);
                        }
                    }
                }
                outPut.HttpStatusCode = 201;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving document {nameof(DrawingCategory)}");
                outPut.HttpStatusCode = 400;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);
            }
        }

        [HttpPost]
        [Route("CreateUser")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> CreateUser(JobKeyValues inputDTO)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {


                if (ModelState.IsValid)
                {
                    if (Convert.ToInt32(inputDTO.JobTitleId) == 0)
                    {
                        JobTitleMasterDTO inputData = new JobTitleMasterDTO();
                        inputData.JobTitle = inputDTO.JobTitle;
                        inputData.CreatedOn = DateTime.Now;
                        inputData.IsActive = true;
                        _unitOfWork.JobTitleMaster.AddAsync(_mapper.Map<JobTitleMaster>(inputData));
                        _unitOfWork.Save();
                        outPut.HttpStatusCode = 200;
                        outPut.DisplayMessage = "Job Title successfully saved!";
                        return Ok(outPut);

                    }
                    else
                    {
                        JobTitleMaster outputDetails = _mapper.Map<JobTitleMaster>(await _unitOfWork.JobTitleMaster.GetByIdAsync(Convert.ToInt32(inputDTO.JobTitleId)));
                        if (outputDetails != null)
                        {
                            outputDetails.JobTitle = inputDTO.JobTitle;
                            outputDetails.ModifiedOn = DateTime.Now;

                        }
                        Expression<Func<JobTitleMaster, bool>> expression = a => a.JobTitleId != Convert.ToInt32(inputDTO.JobTitleId) && a.IsActive == true;
                        if (_unitOfWork.JobTitleMaster.Exists(expression))
                        {
                            _unitOfWork.JobTitleMaster.Update(_mapper.Map<JobTitleMaster>(outputDetails));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Job Title updated successfully!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "Job Title updated failed!";
                            return Ok(outPut);
                        }
                    }
                }
                outPut.HttpStatusCode = 201;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving Job Title {nameof(JobTitle)}");
                outPut.HttpStatusCode = 400;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);
            }
        }

        [HttpPost]
        public string ValidationUser(UsersMasterDTO inputDTO)
        {
            string validMessage = "";
            StringBuilder strBuilder = new StringBuilder();


            if (string.IsNullOrEmpty(inputDTO.ContactPerson))
            {
                strBuilder.Append($"Contact Person name is Mandatory</br>");
            }
            if (string.IsNullOrEmpty(inputDTO.EmailAddress))
            {
                strBuilder.Append($"Email address is Mandatory</br>");
            }
            if (string.IsNullOrEmpty(inputDTO.ContactNumber))
            {
                strBuilder.Append($"Contact Number is Mandatory</br>");
            }
            if (string.IsNullOrEmpty(inputDTO.LoginPassword))
            {
                strBuilder.Append($"Password is Mandatory</br>");
            }


            if (!string.IsNullOrEmpty(strBuilder.ToString()))
                validMessage = strBuilder.ToString();

            return validMessage;
        }


        [HttpGet]
        [Route("GetCountryById")]
        [Produces("application/json", Type = typeof(CountryMasterDTO))]
        public async Task<IActionResult> GetCountryById(int countryId)
        {
            try
            {
                CountryMasterDTO outputDTO = _mapper.Map<CountryMasterDTO>(await _unitOfWork.CountryMaster.GetByIdAsync(countryId));
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
                _logger.LogError(ex, $"Error in retriving Country {nameof(GetCountry)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetDocumentById")]
        [Produces("application/json", Type = typeof(DocumentCategoryDTO))]
        public async Task<IActionResult> GetDocumentById(int docId)
        {
            try
            {
                DocumentCategoryDTO outputDTO = _mapper.Map<DocumentCategoryDTO>(await _unitOfWork.DocumentCategory.GetByIdAsync(docId));
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
                _logger.LogError(ex, $"Error in retriving Country {nameof(GetDocumentById)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetDrawingById")]
        [Produces("application/json", Type = typeof(DrawingCategoryDTO))]
        public async Task<IActionResult> GetDrawingById(int drId)
        {
            try
            {
                DrawingCategoryDTO outputDTO = _mapper.Map<DrawingCategoryDTO>(await _unitOfWork.DrawingCategory.GetByIdAsync(drId));
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
                _logger.LogError(ex, $"Error in retriving Country {nameof(GetDrawingById)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetPhotoById")]
        [Produces("application/json", Type = typeof(PhotoCategoryDTO))]
        public async Task<IActionResult> GetPhotoById(int drId)
        {
            try
            {
                PhotoCategoryDTO outputDTO = _mapper.Map<PhotoCategoryDTO>(await _unitOfWork.PhotoCategory.GetByIdAsync(drId));
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
                _logger.LogError(ex, $"Error in retriving Country {nameof(GetDrawingById)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetRoleById")]
        [Produces("application/json", Type = typeof(RoleMasterDTO))]
        public async Task<IActionResult> GetRoleById(int rId)
        {
            try
            {
                RoleMasterDTO outputDTO = _mapper.Map<RoleMasterDTO>(await _unitOfWork.RoleMaster.GetByIdAsync(rId));
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
                _logger.LogError(ex, $"Error in retriving Country {nameof(GetRoleById)}");
                throw;
            }
        }

        [HttpGet]
        [Route("DeleteRole")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DeleteRole(int rId)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    RoleMaster outputMaster = _mapper.Map<RoleMaster>(await _unitOfWork.RoleMaster.GetByIdAsync(rId));
                    outputMaster.IsActive = false;
                    _unitOfWork.RoleMaster.Update(outputMaster);
                    _unitOfWork.Save();
                    outPut.HttpStatusCode = 200;
                    outPut.DisplayMessage = "Role delete successfully!";
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
                _logger.LogError(ex, $"Error while deleting country {nameof(DeleteRole)}");
                throw;
            }
        }

        [HttpGet]
        [Route("DeleteCountry")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DeleteCountry(int countryId)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    CountryMaster outputMaster = _mapper.Map<CountryMaster>(await _unitOfWork.CountryMaster.GetByIdAsync(countryId));
                    outputMaster.IsActive = false;
                    _unitOfWork.CountryMaster.Update(outputMaster);
                    _unitOfWork.Save();
                    outPut.HttpStatusCode = 200;
                    outPut.DisplayMessage = "Country delete successfully!";
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
                _logger.LogError(ex, $"Error while deleting country {nameof(DeleteCountry)}");
                throw;
            }
        }

        [HttpGet]
        [Route("DeleteDocumentCategory")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DeleteDocumentCategory(int Id)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    DocumentCategory outputMaster = _mapper.Map<DocumentCategory>(await _unitOfWork.DocumentCategory.GetByIdAsync(Id));
                    outputMaster.IsActive = false;
                    _unitOfWork.DocumentCategory.Update(outputMaster);
                    _unitOfWork.Save();
                    outPut.HttpStatusCode = 200;
                    outPut.DisplayMessage = "Category delete successfully!";
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
                _logger.LogError(ex, $"Error while deleting country {nameof(DeleteDocumentCategory)}");
                throw;
            }
        }

        [HttpGet]
        [Route("DeleteDrawingCategory")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DeleteDrawingCategory(int Id)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    DrawingCategory outputMaster = _mapper.Map<DrawingCategory>(await _unitOfWork.DrawingCategory.GetByIdAsync(Id));
                    outputMaster.IsActive = false;
                    _unitOfWork.DrawingCategory.Update(outputMaster);
                    _unitOfWork.Save();
                    outPut.HttpStatusCode = 200;
                    outPut.DisplayMessage = "Category delete successfully!";
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
                _logger.LogError(ex, $"Error while deleting country {nameof(DeleteDrawingCategory)}");
                throw;
            }
        }

        [HttpGet]
        [Route("DeletePhotoCategory")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DeletePhotoCategory(int Id)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    PhotoCategory outputMaster = _mapper.Map<PhotoCategory>(await _unitOfWork.PhotoCategory.GetByIdAsync(Id));
                    outputMaster.IsActive = false;
                    _unitOfWork.PhotoCategory.Update(outputMaster);
                    _unitOfWork.Save();
                    outPut.HttpStatusCode = 200;
                    outPut.DisplayMessage = "Category delete successfully!";
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
                _logger.LogError(ex, $"Error while deleting country {nameof(DeleteDrawingCategory)}");
                throw;
            }
        }

        [HttpGet]
        [Route("DisableDocumentCategory")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DisableDocumentCategory(int catId, bool status)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    DocumentCategory outputMaster = _mapper.Map<DocumentCategory>(await _unitOfWork.DocumentCategory.GetByIdAsync(catId));
                    outputMaster.IsDisable = status;
                    _unitOfWork.DocumentCategory.Update(outputMaster);
                    _unitOfWork.Save();
                    outPut.HttpStatusCode = 200;
                    outPut.DisplayMessage = "Category delete successfully!";
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
                _logger.LogError(ex, $"Error while deleting country {nameof(DisableDocumentCategory)}");
                throw;
            }
        }

        [HttpGet]
        [Route("DisableDrawingCategory")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DisableDrawingCategory(int catId, bool status)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    DrawingCategory outputMaster = _mapper.Map<DrawingCategory>(await _unitOfWork.DrawingCategory.GetByIdAsync(catId));
                    outputMaster.IsDisable = status;
                    _unitOfWork.DrawingCategory.Update(outputMaster);
                    _unitOfWork.Save();
                    outPut.HttpStatusCode = 200;
                    outPut.DisplayMessage = "Category delete successfully!";
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
                _logger.LogError(ex, $"Error while deleting country {nameof(DeleteDrawingCategory)}");
                throw;
            }
        }

        [HttpGet]
        [Route("DisablePhotoCategory")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DisablePhotoCategory(int catId, bool status)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    PhotoCategory outputMaster = _mapper.Map<PhotoCategory>(await _unitOfWork.PhotoCategory.GetByIdAsync(catId));
                    outputMaster.IsDisable = status;
                    _unitOfWork.PhotoCategory.Update(outputMaster);
                    _unitOfWork.Save();
                    outPut.HttpStatusCode = 200;
                    outPut.DisplayMessage = "Category delete successfully!";
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
                _logger.LogError(ex, $"Error while deleting country {nameof(DisablePhotoCategory)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetStateById")]
        [Produces("application/json", Type = typeof(StateMasterDTO))]
        public async Task<IActionResult> GetStateById(int stateId)
        {
            try
            {
                StateMasterDTO outputDTO = _mapper.Map<StateMasterDTO>(await _unitOfWork.StateMaster.GetByIdAsync(stateId));
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
                _logger.LogError(ex, $"Error in retriving State {nameof(GetStateById)}");
                throw;
            }
        }


        [HttpGet]
        [Route("DeleteState")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DeleteState(int stateId)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    StateMaster outputMaster = _mapper.Map<StateMaster>(await _unitOfWork.StateMaster.GetByIdAsync(stateId));
                    outputMaster.IsActive = false;
                    _unitOfWork.StateMaster.Update(outputMaster);
                    _unitOfWork.Save();
                    outPut.HttpStatusCode = 200;
                    outPut.DisplayMessage = "State delete successfully!";
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
                _logger.LogError(ex, $"Error while deleting State {nameof(DeleteState)}");
                throw;
            }
        }


        [HttpGet]
        [Route("GetCityById")]
        [Produces("application/json", Type = typeof(CityMasterDTO))]
        public async Task<IActionResult> GetCityById(int cityId)
        {
            try
            {
                CityMasterDTO outputDTO = _mapper.Map<CityMasterDTO>(await _unitOfWork.CityMaster.GetByIdAsync(cityId));
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
                _logger.LogError(ex, $"Error in retriving City {nameof(GetCityById)}");
                throw;
            }
        }


        [HttpGet]
        [Route("DeleteCity")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DeleteCity(int cityId)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    CityMaster outputMaster = _mapper.Map<CityMaster>(await _unitOfWork.CityMaster.GetByIdAsync(cityId));
                    outputMaster.IsActive = false;
                    _unitOfWork.CityMaster.Update(outputMaster);
                    _unitOfWork.Save();
                    outPut.HttpStatusCode = 200;
                    outPut.DisplayMessage = "City delete successfully!";
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
                _logger.LogError(ex, $"Error while deleting City {nameof(DeleteCity)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetDepartmentById")]
        [Produces("application/json", Type = typeof(DepartmentMasterDTO))]
        public async Task<IActionResult> GetDepartmentById(int deptId)
        {
            try
            {
                DepartmentMasterDTO outputDTO = _mapper.Map<DepartmentMasterDTO>(await _unitOfWork.DepartmentMaster.GetByIdAsync(deptId));
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
                _logger.LogError(ex, $"Error in retriving Department {nameof(GetDepartmentById)}");
                throw;
            }
        }


        [HttpGet]
        [Route("DeleteDepartment")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DeleteDepartment(int deptId)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    DepartmentMaster outputMaster = _mapper.Map<DepartmentMaster>(await _unitOfWork.DepartmentMaster.GetByIdAsync(deptId));
                    outputMaster.IsActive = false;
                    _unitOfWork.DepartmentMaster.Update(outputMaster);
                    _unitOfWork.Save();
                    outPut.HttpStatusCode = 200;
                    outPut.DisplayMessage = "Department delete successfully!";
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
                _logger.LogError(ex, $"Error while deleting department {nameof(DeleteDepartment)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetDesignationById")]
        [Produces("application/json", Type = typeof(JobTitleMasterDTO))]
        public async Task<IActionResult> GetDesignationById(int desigId)
        {
            try
            {
                JobTitleMasterDTO outputDTO = _mapper.Map<JobTitleMasterDTO>(await _unitOfWork.JobTitleMaster.GetByIdAsync(desigId));
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
                _logger.LogError(ex, $"Error in retriving Department {nameof(GetDepartmentById)}");
                throw;
            }
        }


        [HttpGet]
        [Route("DeleteDesignation")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DeleteDesignation(int desigId)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    JobTitleMaster outputMaster = _mapper.Map<JobTitleMaster>(await _unitOfWork.JobTitleMaster.GetByIdAsync(desigId));
                    outputMaster.IsActive = false;
                    _unitOfWork.JobTitleMaster.Update(outputMaster);
                    _unitOfWork.Save();
                    outPut.HttpStatusCode = 200;
                    outPut.DisplayMessage = "Designation delete successfully!";
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
                _logger.LogError(ex, $"Error while deleting designation {nameof(DeleteDesignation)}");
                throw;
            }
        }


        [HttpPost]
        [Route("PhasesMaster")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> PhasesMaster(PhasesKeyValues inputDTO)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {


                if (ModelState.IsValid)
                {
                    if (Convert.ToInt32(inputDTO.PhaseID) == 0)
                    {
                        PhasesMasterDTO inputData = new PhasesMasterDTO();
                        inputData.Phases = inputDTO.Phases;
                        // inputData.CreatedOn = DateTime.Now;
                        inputData.IsActive = true;

                        Expression<Func<PhasesMaster, bool>> expression = a => a.Phases.Trim().Replace(" ", "") == inputDTO.Phases.Trim().Replace(" ", "") && a.IsActive == true;
                        if (!_unitOfWork.PhasesMaster.Exists(expression))
                        {
                            _unitOfWork.PhasesMaster.AddAsync(_mapper.Map<PhasesMaster>(inputData));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Phase successfully saved!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "Duplicate Entry Found!";
                            return Ok(outPut);
                        }



                    }
                    else
                    {
                        PhasesMaster outputDetails = _mapper.Map<PhasesMaster>(await _unitOfWork.PhasesMaster.GetByIdAsync(Convert.ToInt32(inputDTO.PhaseID)));
                        if (outputDetails != null)
                        {
                            outputDetails.Phases = inputDTO.Phases;
                            // outputDetails.ModifiedOn = DateTime.Now;
                            //outputDetails.ModifiedBy= inputDTO.ModifiedBy;

                        }
                        Expression<Func<PhasesMaster, bool>> expression = a => a.PhaseID != Convert.ToInt32(inputDTO.PhaseID) && a.IsActive == true;
                        if (_unitOfWork.PhasesMaster.Exists(expression))
                        {
                            _unitOfWork.PhasesMaster.Update(_mapper.Map<PhasesMaster>(outputDetails));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Phase updated successfully!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "Phase updated failed!";
                            return Ok(outPut);
                        }
                    }
                }
                outPut.HttpStatusCode = 201;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving Status Master {nameof(PhasesMaster)}");
                outPut.HttpStatusCode = 400;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);
            }
        }

        [HttpPost]
        [Route("UnitPriority")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> UnitPriority(UnitPriorityKeyValues inputDTO)
        {

            OutPutResponse outPut = new OutPutResponse();

            try
            {


                if (ModelState.IsValid)
                {
                    if (Convert.ToInt32(inputDTO.CompanyPriorityId) == 0)
                    {

                        Expression<Func<CompanyPriorityMaster, bool>> expression = a => a.CompanyPriority.Trim().Replace(" ", "") == inputDTO.CompanyPriority.Trim().Replace(" ", "") && a.CompanyId == inputDTO.CompanyId && a.IsActive == true;
                        if (!_unitOfWork.CompanyPriorityMaster.Exists(expression))
                        {
                            _unitOfWork.CompanyPriorityMaster.AddAsync(_mapper.Map<CompanyPriorityMaster>(inputDTO));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Priority successfully saved!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "Duplicate Entry Found!";
                            return Ok(outPut);
                        }



                    }
                    else
                    {
                        CompanyPriorityMaster outputDetails = _mapper.Map<CompanyPriorityMaster>(await _unitOfWork.CompanyPriorityMaster.GetByIdAsync(Convert.ToInt32(inputDTO.CompanyPriorityId)));
                        if (outputDetails != null)
                        {
                            outputDetails.CompanyPriority = inputDTO.CompanyPriority;
                            outputDetails.CompanyId = inputDTO.CompanyId;

                        }
                        Expression<Func<CompanyPriorityMaster, bool>> expression = a => a.CompanyPriorityId != Convert.ToInt32(inputDTO.CompanyPriorityId) && a.IsActive == true;
                        if (_unitOfWork.CompanyPriorityMaster.Exists(expression))
                        {
                            _unitOfWork.CompanyPriorityMaster.Update(_mapper.Map<CompanyPriorityMaster>(outputDetails));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Priority updated successfully!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "Priority updated failed!";
                            return Ok(outPut);
                        }
                    }
                }
                outPut.HttpStatusCode = 201;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving Status Master {nameof(StatusMaster)}");
                outPut.HttpStatusCode = 400;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);
            }
        }


        [HttpPost]
        [Route("UnitPhasesMaster")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> UnitPhasesMaster(UnitPhasesMasterDTO inputDTO)
        {

            OutPutResponse outPut = new OutPutResponse();

            try
            {


                if (ModelState.IsValid)
                {
                    if (Convert.ToInt32(inputDTO.ID) == 0)
                    {

                        Expression<Func<UnitPhasesMaster, bool>> expression = a => a.Phase.Trim().Replace(" ", "") == inputDTO.Phase.Trim().Replace(" ", "") && a.UnitId == inputDTO.UnitId && a.IsActive == true;
                        if (!_unitOfWork.UnitPhasesMaster.Exists(expression))
                        {
                            _unitOfWork.UnitPhasesMaster.AddAsync(_mapper.Map<UnitPhasesMaster>(inputDTO));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Phase successfully saved!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "Duplicate Entry Found!";
                            return Ok(outPut);
                        }



                    }
                    else
                    {
                        UnitPhasesMaster outputDetails = _mapper.Map<UnitPhasesMaster>(await _unitOfWork.UnitPhasesMaster.GetByIdAsync(Convert.ToInt32(inputDTO.ID)));
                        if (outputDetails != null)
                        {
                            outputDetails.Phase = inputDTO.Phase;
                            outputDetails.UnitId = inputDTO.UnitId;

                        }
                        Expression<Func<UnitPhasesMaster, bool>> expression = a => a.ID != Convert.ToInt32(inputDTO.ID) && a.IsActive == true;
                        if (_unitOfWork.UnitPhasesMaster.Exists(expression))
                        {
                            _unitOfWork.UnitPhasesMaster.Update(_mapper.Map<UnitPhasesMaster>(outputDetails));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Phase updated successfully!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "Phase updated failed!";
                            return Ok(outPut);
                        }
                    }
                }
                outPut.HttpStatusCode = 201;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving Status Master {nameof(StatusMaster)}");
                outPut.HttpStatusCode = 400;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);
            }
        }

        [HttpPost]
        [Route("UnitStatusMaster")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> UnitStatusMaster(UnitStatusMasterDTO inputDTO)
        {

            OutPutResponse outPut = new OutPutResponse();

            try
            {


                if (ModelState.IsValid)
                {
                    if (Convert.ToInt32(inputDTO.ID) == 0)
                    {

                        Expression<Func<UnitStatusMaster, bool>> expression = a => a.Status.Trim().Replace(" ", "") == inputDTO.Status.Trim().Replace(" ", "") && a.StatusType.Trim() == inputDTO.StatusType.Trim() && a.UnitId == inputDTO.UnitId && a.IsActive == true;
                        if (!_unitOfWork.UnitStatusMaster.Exists(expression))
                        {
                            _unitOfWork.UnitStatusMaster.AddAsync(_mapper.Map<UnitStatusMaster>(inputDTO));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Status successfully saved!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "Duplicate Entry Found!";
                            return Ok(outPut);
                        }



                    }
                    else
                    {
                        UnitStatusMaster outputDetails = _mapper.Map<UnitStatusMaster>(await _unitOfWork.UnitStatusMaster.GetByIdAsync(Convert.ToInt32(inputDTO.ID)));
                        if (outputDetails != null)
                        {
                            outputDetails.Status = inputDTO.Status;
                            outputDetails.UnitId = inputDTO.UnitId;

                        }
                        Expression<Func<UnitStatusMaster, bool>> expression = a => a.ID != Convert.ToInt32(inputDTO.ID) && a.IsActive == true;
                        if (_unitOfWork.UnitStatusMaster.Exists(expression))
                        {
                            _unitOfWork.UnitStatusMaster.Update(_mapper.Map<UnitStatusMaster>(outputDetails));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Status updated successfully!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "Status updated failed!";
                            return Ok(outPut);
                        }
                    }
                }
                outPut.HttpStatusCode = 201;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving Status Master {nameof(StatusMaster)}");
                outPut.HttpStatusCode = 400;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);
            }
        }

        [HttpGet]
        [Route("GetPhaseById")]
        [Produces("application/json", Type = typeof(PhasesMasterDTO))]
        public async Task<IActionResult> GetPhaseById(int phaseId)
        {
            try
            {
                PhasesMasterDTO outputDTO = _mapper.Map<PhasesMasterDTO>(await _unitOfWork.PhasesMaster.GetByIdAsync(phaseId));
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
                _logger.LogError(ex, $"Error in retriving Country {nameof(GetPhaseById)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetPriorityById")]
        [Produces("application/json", Type = typeof(PriorityMasterDTO))]
        public async Task<IActionResult> GetPriorityById(int pId)
        {
            try
            {
                PriorityMasterDTO outputDTO = _mapper.Map<PriorityMasterDTO>(await _unitOfWork.PriorityMaster.GetByIdAsync(pId));
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
                _logger.LogError(ex, $"Error in retriving Country {nameof(GetPhaseById)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetStatusById")]
        [Produces("application/json", Type = typeof(StatusMasterDTO))]
        public async Task<IActionResult> GetStatusById(int pId)
        {
            try
            {
                StatusMasterDTO outputDTO = _mapper.Map<StatusMasterDTO>(await _unitOfWork.StatusMaster.GetByIdAsync(pId));
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
                _logger.LogError(ex, $"Error in retriving Country {nameof(GetStatusById)}");
                throw;
            }
        }

        [HttpGet]
        [Route("DeletePhase")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DeletePhase(int phaseId)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    PhasesMaster outputMaster = _mapper.Map<PhasesMaster>(await _unitOfWork.PhasesMaster.GetByIdAsync(phaseId));
                    outputMaster.IsActive = false;
                    _unitOfWork.PhasesMaster.Update(outputMaster);
                    _unitOfWork.Save();
                    outPut.HttpStatusCode = 200;
                    outPut.DisplayMessage = "Phase delete successfully!";
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
                _logger.LogError(ex, $"Error while deleting country {nameof(DeleteCountry)}");
                throw;
            }
        }

        [HttpGet]
        [Route("DeletePriority")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DeletePriority(int pId)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    PriorityMaster outputMaster = _mapper.Map<PriorityMaster>(await _unitOfWork.PriorityMaster.GetByIdAsync(pId));
                    outputMaster.IsActive = false;
                    _unitOfWork.PriorityMaster.Update(outputMaster);
                    _unitOfWork.Save();
                    outPut.HttpStatusCode = 200;
                    outPut.DisplayMessage = "Phase delete successfully!";
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
                _logger.LogError(ex, $"Error while deleting country {nameof(DeleteCountry)}");
                throw;
            }
        }

        [HttpGet]
        [Route("DeleteStatus")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DeleteStatus(int pId)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    StatusMaster outputMaster = _mapper.Map<StatusMaster>(await _unitOfWork.StatusMaster.GetByIdAsync(pId));
                    outputMaster.IsActive = false;
                    _unitOfWork.StatusMaster.Update(outputMaster);
                    _unitOfWork.Save();
                    outPut.HttpStatusCode = 200;
                    outPut.DisplayMessage = "Status delete successfully!";
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
                _logger.LogError(ex, $"Error while deleting country {nameof(DeleteStatus)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetCategory")]
        [Produces("application/json", Type = typeof(MasterCategoryDTO))]
        public IEnumerable<MasterCategoryDTO> GetCategory()
        {
            try
            {
                return (_unitOfWork.MasterCategory.GetAll(p => p.IsActive == true).Result
                               .Select(p => new MasterCategoryDTO()
                               {
                                   CategoryId = p.CategoryId,
                                   CategoryName = p.CategoryName

                               })).ToList();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving GetUnitPriority {nameof(GetCategory)}");
                throw;
            }
        }


        [HttpGet]
        [Route("GetUnitCategory")]
        [Produces("application/json", Type = typeof(UnitCategoryMasterDTO))]
        public IEnumerable<UnitCategoryMasterDTO> GetUnitCategory(int? unitId)
        {
            try
            {
                return (_unitOfWork.UnitCategoryMaster.GetAll(p => p.IsActive == true && p.UnitId == unitId).Result
                               .Select(p => new UnitCategoryMasterDTO()
                               {
                                   MasterCategoryId = p.MasterCategoryId,
                                   UnitCategory = p.UnitCategory,
                                   UnitCategoryId = p.UnitCategoryId,
                                   UnitId = p.UnitId
                               })).ToList();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving GetUnitPriority {nameof(GetUnitCategory)}");
                throw;
            }
        }


        [HttpPost]
        [Route("UnitCategory")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> UnitCategory(UnitCategoryMasterDTO inputDTO)
        {

            OutPutResponse outPut = new OutPutResponse();

            try
            {


                if (ModelState.IsValid)
                {
                    if (Convert.ToInt32(inputDTO.UnitCategoryId) == 0)
                    {

                        Expression<Func<UnitCategoryMaster, bool>> expression = a => a.UnitCategory.Trim().Replace(" ", "") == inputDTO.UnitCategory.Trim().Replace(" ", "") && a.UnitId == inputDTO.UnitId && a.IsActive == true;
                        if (!_unitOfWork.UnitCategoryMaster.Exists(expression))
                        {
                            _unitOfWork.UnitCategoryMaster.AddAsync(_mapper.Map<UnitCategoryMaster>(inputDTO));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Category successfully saved!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "Duplicate Entry Found!";
                            return Ok(outPut);
                        }



                    }
                    else
                    {
                        UnitCategoryMaster outputDetails = _mapper.Map<UnitCategoryMaster>(await _unitOfWork.UnitCategoryMaster.GetByIdAsync(Convert.ToInt32(inputDTO.UnitCategoryId)));
                        if (outputDetails != null)
                        {
                            outputDetails.UnitCategory = inputDTO.UnitCategory;
                            outputDetails.UnitId = inputDTO.UnitId;

                        }
                        Expression<Func<UnitCategoryMaster, bool>> expression = a => a.UnitCategoryId != Convert.ToInt32(inputDTO.UnitCategoryId) && a.IsActive == true;
                        if (_unitOfWork.UnitCategoryMaster.Exists(expression))
                        {
                            _unitOfWork.UnitCategoryMaster.Update(_mapper.Map<UnitCategoryMaster>(outputDetails));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Category updated successfully!";
                            return Ok(outPut);
                        }
                        else
                        {
                            outPut.HttpStatusCode = 201;
                            outPut.DisplayMessage = "Category updated failed!";
                            return Ok(outPut);
                        }
                    }
                }
                outPut.HttpStatusCode = 201;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving Unit Category {nameof(UnitCategory)}");
                outPut.HttpStatusCode = 400;
                outPut.DisplayMessage = "transaction failed!";
                return Ok(outPut);
            }
        }

        [HttpGet]
        [Route("DeleteUnitPriority")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DeleteUnitPriority(int pId)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    CompanyPriorityMaster outputMaster = _mapper.Map<CompanyPriorityMaster>(await _unitOfWork.CompanyPriorityMaster.GetByIdAsync(pId));
                    outputMaster.IsActive = false;
                    _unitOfWork.CompanyPriorityMaster.Update(outputMaster);
                    _unitOfWork.Save();
                    outPut.HttpStatusCode = 200;
                    outPut.DisplayMessage = "Designation delete successfully!";
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
                _logger.LogError(ex, $"Error while deleting designation {nameof(DeleteDesignation)}");
                throw;
            }
        }

        [HttpGet]
        [Route("DeleteUnitCategory")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DeleteUnitCategory(int pId)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    UnitCategoryMaster outputMaster = _mapper.Map<UnitCategoryMaster>(await _unitOfWork.UnitCategoryMaster.GetByIdAsync(pId));
                    outputMaster.IsActive = false;
                    _unitOfWork.UnitCategoryMaster.Update(outputMaster);
                    _unitOfWork.Save();
                    outPut.HttpStatusCode = 200;
                    outPut.DisplayMessage = "Category delete successfully!";
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
                _logger.LogError(ex, $"Error while deleting designation {nameof(DeleteDesignation)}");
                throw;
            }
        }

        [HttpGet]
        [Route("DeleteUnitPhases")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DeleteUnitPhases(int pId)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    UnitPhasesMaster outputMaster = _mapper.Map<UnitPhasesMaster>(await _unitOfWork.UnitPhasesMaster.GetByIdAsync(pId));
                    outputMaster.IsActive = false;
                    _unitOfWork.UnitPhasesMaster.Update(outputMaster);
                    _unitOfWork.Save();
                    outPut.HttpStatusCode = 200;
                    outPut.DisplayMessage = "Phase delete successfully!";
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
                _logger.LogError(ex, $"Error while deleting designation {nameof(DeleteDesignation)}");
                throw;
            }
        }

        [HttpGet]
        [Route("DeleteUnitStatus")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DeleteUnitStatus(int pId)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    UnitStatusMaster outputMaster = _mapper.Map<UnitStatusMaster>(await _unitOfWork.UnitStatusMaster.GetByIdAsync(pId));
                    outputMaster.IsActive = false;
                    _unitOfWork.UnitStatusMaster.Update(outputMaster);
                    _unitOfWork.Save();
                    outPut.HttpStatusCode = 200;
                    outPut.DisplayMessage = "Status delete successfully!";
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
                _logger.LogError(ex, $"Error while deleting designation {nameof(DeleteDesignation)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetUnitPriorityById")]
        [Produces("application/json", Type = typeof(CompanyPriorityMasterDTO))]
        public async Task<IActionResult> GetUnitPriorityById(int pId)
        {
            try
            {
                CompanyPriorityMasterDTO outputDTO = _mapper.Map<CompanyPriorityMasterDTO>(await _unitOfWork.CompanyPriorityMaster.GetByIdAsync(pId));
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
                _logger.LogError(ex, $"Error in retriving Department {nameof(GetDepartmentById)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetUnitCategoryById")]
        [Produces("application/json", Type = typeof(UnitCategoryMasterDTO))]
        public async Task<IActionResult> GetUnitCategoryById(int pId)
        {
            try
            {
                UnitCategoryMasterDTO outputDTO = _mapper.Map<UnitCategoryMasterDTO>(await _unitOfWork.UnitCategoryMaster.GetByIdAsync(pId));
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
                _logger.LogError(ex, $"Error in retriving Department {nameof(GetDepartmentById)}");
                throw;
            }
        }


        [HttpGet]
        [Route("GetUnitPhasesById")]
        [Produces("application/json", Type = typeof(UnitPhasesMasterDTO))]
        public async Task<IActionResult> GetUnitPhasesById(int pId)
        {
            try
            {
                UnitPhasesMasterDTO outputDTO = _mapper.Map<UnitPhasesMasterDTO>(await _unitOfWork.UnitPhasesMaster.GetByIdAsync(pId));
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
                _logger.LogError(ex, $"Error in retriving Department {nameof(GetDepartmentById)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetUnitStatusById")]
        [Produces("application/json", Type = typeof(UnitStatusMasterDTO))]
        public async Task<IActionResult> GetUnitStatusById(int pId)
        {
            try
            {
                UnitStatusMasterDTO outputDTO = _mapper.Map<UnitStatusMasterDTO>(await _unitOfWork.UnitStatusMaster.GetByIdAsync(pId));
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
                _logger.LogError(ex, $"Error in retriving Department {nameof(GetDepartmentById)}");
                throw;
            }
        }


        [HttpGet]
        [Route("GetAccessDetails")]
        [Produces("application/json", Type = typeof(AccessMasterDTO))]
        public async Task<List<AccessMasterDTO>?> GetAccessDetails(int? unitId, int? roleId, int? departmentId)
        {
            AccessMasterDTO AccessList = new AccessMasterDTO();
            var parms = new DynamicParameters();
            parms.Add(@"@UnitId", unitId, DbType.Int32);
            parms.Add(@"@DepartmentId", departmentId, DbType.Int32);
           // parms.Add(@"@DesignationId", designationId, DbType.Int32);
            parms.Add(@"@RoleId", roleId, DbType.Int32);

            AccessList.AccessList = (await _unitOfWork.AccessMaster.GetSPData<AccessMasterDTO>("usp_AccessMaster", parms));
            return AccessList.AccessList;
        }

        [HttpGet]
        [Route("GetClientAccessDetails")]
        [Produces("application/json", Type = typeof(AccessClientMasterDTO))]
        public async Task<List<AccessClientMasterDTO>?> GetClientAccessDetails(int? unitId, bool? isClient)
        {
            AccessClientMasterDTO AccessList = new AccessClientMasterDTO();
            var parms = new DynamicParameters();
            parms.Add(@"@UnitId", unitId, DbType.Int32);
            parms.Add(@"@IsClient", isClient, DbType.Boolean);           

            AccessList.AccessMasterList = (await _unitOfWork.AccessClientMaster.GetSPData<AccessClientMasterDTO>("usp_ClientAccessMaster", parms));
            return AccessList.AccessMasterList;
        }

        [HttpPost]
        [Route("UserAccessMapping")]
        [Produces("application/json", Type = typeof(int))]
        public async Task<int> UserAccessMapping(AccessMasterDTO objInputs)
        {
            int response=0;
            // AccessMasterDTO AccessList = new AccessMasterDTO();
            if (objInputs.ModuleIds.Length > 0)
            {
                var moduleIds = objInputs.ModuleIds.Split(',');
                var IsEdits = objInputs.IsEdits.Split(',');
                var IsDeletes = objInputs.IsDeletes.Split(',');
                var IsViews = objInputs.IsViews.Split(',');
                var IsAdds = objInputs.IsAdds.Split(',');
                var IsApprovals = objInputs.IsApprovals.Split(',');
                for (int i=0;i< moduleIds.Length;i++)
                {
                    var parms = new DynamicParameters();
                    //parms.Add(@"@RoleId", objInputs.RoleId, DbType.Int32);
                    //parms.Add(@"@DepartmentId", objInputs.DepartmentId, DbType.Int32);
                    //parms.Add(@"@DesignationId", objInputs.DesignationId, DbType.Int32);
                    //parms.Add(@"@IsDelete", objInputs.IsDelete, DbType.Boolean);
                    //parms.Add(@"@@IsView", objInputs.IsView, DbType.Boolean);
                    //parms.Add(@"@IsAdd", objInputs.IsAdd, DbType.Boolean);
                    //parms.Add(@"@IsEdit", objInputs.IsEdit, DbType.Boolean);
                    //parms.Add(@"@UnitId", objInputs.UnitId, DbType.Int32);
                    //parms.Add(@"@ModuleId", objInputs.ModuleId, DbType.Int32);
                    parms.Add(@"@RoleId", objInputs.RoleId, DbType.Int32);
                    parms.Add(@"@DepartmentId", objInputs.DepartmentId, DbType.Int32);
                  //  parms.Add(@"@DesignationId", objInputs.DesignationId, DbType.Int32);
                    parms.Add(@"@IsDelete", IsDeletes[i], DbType.Boolean);
                    parms.Add(@"@@IsView", IsViews[i], DbType.Boolean);
                    parms.Add(@"@IsAdd", IsAdds[i], DbType.Boolean);
                    parms.Add(@"@IsEdit", IsEdits[i], DbType.Boolean);
                    parms.Add(@"@IsApproval", IsApprovals[i], DbType.Boolean);
                    parms.Add(@"@UnitId", objInputs.UnitId, DbType.Int32);
                    parms.Add(@"@ModuleId", moduleIds[i], DbType.Int32);
                    response = (await _unitOfWork.AccessMaster.GetStoredProcedure("usp_UserAccessMapping", parms));
                }
            }
           return response;
        }


        [HttpPost]
        [Route("ClientAccessMapping")]
        [Produces("application/json", Type = typeof(int))]
        public async Task<int> ClientAccessMapping(AccessClientMasterDTO objInputs)
        {
            int response = 0;
            // AccessMasterDTO AccessList = new AccessMasterDTO();
            if (objInputs.ModuleIds.Length > 0)
            {
                var moduleIds = objInputs.ModuleIds.Split(',');
                var IsEdits = objInputs.IsEdits.Split(',');
                var IsDeletes = objInputs.IsDeletes.Split(',');
                var IsViews = objInputs.IsViews.Split(',');
                var IsAdds = objInputs.IsAdds.Split(',');
                var IsApprovals = objInputs.IsApprovals.Split(',');
                for (int i = 0; i < moduleIds.Length; i++)
                {
                    var parms = new DynamicParameters();                    
                    parms.Add(@"@IsClient", objInputs.IsClient, DbType.Boolean);                             
                    parms.Add(@"@IsDelete", IsDeletes[i], DbType.Boolean);
                    parms.Add(@"@@IsView", IsViews[i], DbType.Boolean);
                    parms.Add(@"@IsAdd", IsAdds[i], DbType.Boolean);
                    parms.Add(@"@IsEdit", IsEdits[i], DbType.Boolean);
                    parms.Add(@"@IsApproval", IsApprovals[i], DbType.Boolean);
                    parms.Add(@"@UnitId", objInputs.UnitId, DbType.Int32);
                    parms.Add(@"@ModuleId", moduleIds[i], DbType.Int32);
                    response = (await _unitOfWork.AccessClientMaster.GetStoredProcedure("usp_ClientAccessMapping", parms));
                }
            }
            return response;
        }
        //[HttpGet]
        //[Route("GetDocumentCategory")]
        //[Produces("application/json", Type = typeof(DocumentCategoryDTO))]
        //public IEnumerable<DocumentCategoryDTO> GetDocumentCategory(int? unitId)
        //{
        //    try
        //    {
        //        return (_unitOfWork.DocumentCategory.GetAll(p => p.IsActive == true).Result
        //                       .Select(p => new DocumentCategoryDTO()
        //                       {
        //                           Id = p.Id,
        //                           Category = p.Category
        //                       })).ToList();

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Error in retriving GetDocumentCategory {nameof(GetDocumentCategory)}");
        //        throw;
        //    }
        //}


        //[HttpGet]
        //[Route("GetDrawingCategory")]
        //[Produces("application/json", Type = typeof(DrawingCategoryDTO))]
        //public IEnumerable<DrawingCategoryDTO> GetDrawingCategory()
        //{
        //    try
        //    {
        //        return (_unitOfWork.DrawingCategory.GetAll(p => p.IsActive == true).Result
        //                       .Select(p => new DrawingCategoryDTO()
        //                       {
        //                           Id = p.Id,
        //                           Category = p.Category
        //                       })).ToList();

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Error in retriving GetDrawingCategory {nameof(GetDrawingCategory)}");
        //        throw;
        //    }
        //}

        //[HttpGet]
        //[Route("GetPhotoCategory")]
        //[Produces("application/json", Type = typeof(DrawingCategoryDTO))]
        //public IEnumerable<DrawingCategoryDTO> GetPhotoCategory()
        //{
        //    try
        //    {
        //        return (_unitOfWork.PhotoCategory.GetAll(p => p.IsActive == true).Result
        //                       .Select(p => new DrawingCategoryDTO()
        //                       {
        //                           Id = p.Id,
        //                           Category = p.Category
        //                       })).ToList();

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Error in retriving GetPhotoCategory {nameof(GetPhotoCategory)}");
        //        throw;
        //    }
        //}
    }

}
