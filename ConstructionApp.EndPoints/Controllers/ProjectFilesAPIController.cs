using AutoMapper;
using Construction.Infrastructure.KeyValues;
using Construction.Infrastructure.Models;
using ConstructionApp.Core.Entities;
using ConstructionApp.Core.Repository;
using ConstructionApp.EndPoints.Helper;
using ConstructionApp.Services.DBContext;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Linq.Expressions;

namespace ConstructionApp.EndPoints.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectFilesAPIController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProjectFilesAPIController> _logger;
        private readonly IMapper _mapper;

        public ProjectFilesAPIController(IUnitOfWork unitOfWork, ILogger<ProjectFilesAPIController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;

        }
        [HttpPost]
        [Route("SaveProjectFile")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> SaveProjectFile(ProjectFolderDTO inputDTO)
        {
            try
            {
                //inputDTO.FullName = inputDTO.FirstName + " " + inputDTO.LastName;
                OutPutResponse outPut = new OutPutResponse();
                if (ModelState.IsValid)
                {
                    if (inputDTO.Id == 0)
                    {
                        //Expression<Func<ProjectFolder, bool>> expression = a => a.FolderName != inputDTO.FolderName && a.ProjectId != inputDTO.ProjectId && a.IsActive == true;
                        //if (_unitOfWork.ProjectFolder.Exists(expression))
                        //{
                        var em = _mapper.Map<ProjectFolderDTO>(_unitOfWork.ProjectFolder.Insert(_mapper.Map<ProjectFolder>(inputDTO)));
                        outPut.RespId = em.Id;
                        outPut.DisplayMessage = "Project folder Saved Successfully";
                        //}
                        //else
                        //{
                        //    outPut.RespId = 0;
                        //    outPut.DisplayMessage = "Duplicate Record";
                        //}
                        outPut.HttpStatusCode = 200;
                        return Ok(outPut);
                    }
                    else
                    {
                        ProjectFolder outputDetails = _mapper.Map<ProjectFolder>(await _unitOfWork.ProjectFolder.GetByIdAsync(Convert.ToInt32(inputDTO.Id)));
                        if (outputDetails != null)
                        {

                            outputDetails.CategoryId = inputDTO.CategoryId;
                            outputDetails.ProjectId = inputDTO.ProjectId;
                            outputDetails.CreationDate = inputDTO.CreationDate;

                        }
                        Expression<Func<ProjectFolder, bool>> expression = a => a.Id != Convert.ToInt32(inputDTO.Id) && a.IsActive == true;
                        if (_unitOfWork.ProjectFolder.Exists(expression))
                        {
                            _unitOfWork.Projects.Update(_mapper.Map<Projects>(outputDetails));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Project Folder updated successfully!";
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
                    outPut.DisplayMessage = "Failed";
                    outPut.HttpStatusCode = 201;
                    return Ok(outPut);

                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving project {nameof(SaveProjectFile)}");
                throw;
            }
        }

        [HttpPost]
        [Route("EditDocumentFolder")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> EditDocumentFolder(ProjectFolderDTO inputDTO)
        {
            try
            {
                //inputDTO.FullName = inputDTO.FirstName + " " + inputDTO.LastName;
                OutPutResponse outPut = new OutPutResponse();
                if (ModelState.IsValid)
                {
                    if (inputDTO.Id > 0)
                    {
                        ProjectFolder outputDetails = _mapper.Map<ProjectFolder>(await _unitOfWork.ProjectFolder.GetByIdAsync(Convert.ToInt32(inputDTO.Id)));
                        if (outputDetails != null)
                        {
                            if (!string.IsNullOrEmpty(inputDTO.AccessIds))
                            {
                                outputDetails.AccessIds = inputDTO.AccessIds;
                                outputDetails.AccessType = inputDTO.AccessType;
                                outputDetails.OptMessage = inputDTO.OptMessage;
                            }
                            else if (!string.IsNullOrEmpty(inputDTO.ClientIds))
                            {
                                outputDetails.ClientIds = inputDTO.ClientIds;
                                outputDetails.AccessType = inputDTO.AccessType;
                                outputDetails.OptMessage = inputDTO.OptMessage;
                            }
                            else
                            {
                                outputDetails.EmailIds = inputDTO.EmailIds;
                                outputDetails.OptMessage = inputDTO.OptMessage;
                            }

                        }
                        Expression<Func<ProjectFolder, bool>> expression = a => a.Id != Convert.ToInt32(inputDTO.Id) && a.IsActive == true;
                        if (_unitOfWork.ProjectFolder.Exists(expression))
                        {
                            _unitOfWork.ProjectFolder.Update(_mapper.Map<ProjectFolder>(outputDetails));
                            _unitOfWork.Save();
                            if (!string.IsNullOrEmpty(outputDetails.EmailIds))
                            {
                                ExternalUsersDTO objEmail = new ExternalUsersDTO();
                                objEmail.ShareIds = Convert.ToString(inputDTO.Id);
                                objEmail.EmailId = outputDetails.EmailIds;
                                objEmail.OptMessage = outputDetails.OptMessage;
                                objEmail.TableId = 1;
                                objEmail.CreatedBy = "Admin";

                                await SaveExternalAccess(objEmail);
                            }
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Team invitation successfully!";
                            return Ok(outPut);
                        }
                        return Ok(outPut);
                    }
                    else
                    {
                        outPut.DisplayMessage = "Failed";
                        outPut.HttpStatusCode = 201;
                        return Ok(outPut);

                    }

                }
                else
                {
                    outPut.DisplayMessage = "Failed";
                    outPut.HttpStatusCode = 201;
                    return Ok(outPut);

                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving project {nameof(EditDocumentFolder)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetProjectCategoryFolders")]
        [Produces("application/json", Type = typeof(ProjectFolderDTO))]
        public IEnumerable<ProjectFolderDTO> GetProjectCategoryFolders(int? pId, int? cId, int? uId, int? uType)
        {
            try
            {
                // var result;
                if (uType == 1)
                {
                    var result = (from file in _unitOfWork.ProjectFolder.GetAll(p => p.IsActive == true && p.ProjectId == pId && p.CategoryId == cId).Result
                                  join user in _unitOfWork.UsersMaster.GetAll(p => p.IsActive == true).Result on file.UserId equals user.UserId
                                  where (file.ProjectId == pId && file.CategoryId == cId && file.IsFolder == true)

                                  select new ProjectFolderDTO()
                                  {
                                      Id = file.Id,
                                      FolderName = file.FolderName,
                                      AccessIds = file.AccessIds,
                                      ClientIds = file.ClientIds,
                                      AccessType = file.AccessType,
                                      EmailIds = file.EmailIds,
                                      OptMessage = file.OptMessage,
                                      CreationDate = file.CreationDate,
                                      UserName = (user.FullName == null ? user.BusinessOwner : user.FullName),
                                      UserType = uType,
                                      UserId = file.UserId,
                                      Base64ProfileImage = user.ProfileName // "data:image/png;base64," + Convert.ToBase64String(user.ProfileImage, 0, user.ProfileImage.Length)

                                  }).OrderByDescending(x => x.Id).ToList();

                    return result;

                }
                else if (uType == 2)
                {

                    var result = (from file in _unitOfWork.ProjectFolder.GetAll(p => p.IsActive == true && p.ProjectId == pId && p.CategoryId == cId && p.IsFolder == true).Result
                   .Where(file => !string.IsNullOrEmpty(file.ClientIds) && file.ClientIds.Split(',').Contains(uId.ToString()) || file.UserId == uId)
                                  join user in _unitOfWork.UsersMaster.GetAll(p => p.IsActive == true).Result
                    on file.UserId equals user.UserId

                                  select new ProjectFolderDTO
                                  {
                                      Id = file.Id,
                                      FolderName = file.FolderName,
                                      AccessIds = file.ClientIds,
                                      AccessType = file.AccessType,
                                      EmailIds = file.EmailIds,
                                      OptMessage = file.OptMessage,
                                      CreationDate = file.CreationDate,
                                      UserType = uType,
                                      UserId = file.UserId,
                                      UserName = string.IsNullOrEmpty(user.FullName) ? user.BusinessOwner : user.FullName,
                                      Base64ProfileImage = user.ProfileName
                                  }).OrderByDescending(x => x.Id).ToList();

                    return result;
                   
                }
                else if (uType == 3)
                {
                    var result = (from file in _unitOfWork.ProjectFolder.GetAll(p => p.IsActive == true && p.ProjectId == pId && p.CategoryId == cId && p.IsFolder == true).Result
                 .Where(file => !string.IsNullOrEmpty(file.AccessIds) && file.AccessIds.Split(',').Contains(uId.ToString()) || file.UserId == uId)
                                  join user in _unitOfWork.UsersMaster.GetAll(p => p.IsActive == true).Result
                    on file.UserId equals user.UserId

                                  select new ProjectFolderDTO
                                  {
                                      Id = file.Id,
                                      FolderName = file.FolderName,
                                      AccessIds = file.AccessIds,
                                      AccessType = file.AccessType,
                                      EmailIds = file.EmailIds,
                                      OptMessage = file.OptMessage,
                                      CreationDate = file.CreationDate,
                                      UserType = uType,
                                      UserId = file.UserId,
                                      UserName = string.IsNullOrEmpty(user.FullName) ? user.BusinessOwner : user.FullName,
                                      Base64ProfileImage = user.ProfileName
                                  }).OrderByDescending(x => x.Id).ToList();

                    return result;
                }

                return Enumerable.Empty<ProjectFolderDTO>();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving user {nameof(GetProjectCategoryFolders)}");
                throw;
            }
        }

        [HttpPost]
        [Route("SaveDocumentFile")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> SaveDocumentFile(ProjectFolderDTO inputDTO)
        {
            try
            {
                //inputDTO.FullName = inputDTO.FirstName + " " + inputDTO.LastName;
                OutPutResponse outPut = new OutPutResponse();
                if (ModelState.IsValid)
                {
                    if (inputDTO.Id == 0)
                    {
                        //Expression<Func<ProjectFolder, bool>> expression = a => a.FilePath != inputDTO.FilePath && a.ProjectId != inputDTO.ProjectId && a.CategoryId != inputDTO.CategoryId && a.IsActive == true;
                        //if (!_unitOfWork.ProjectFolder.Exists(expression))
                        //{
                        var em = _mapper.Map<ProjectFolderDTO>(_unitOfWork.ProjectFolder.Insert(_mapper.Map<ProjectFolder>(inputDTO)));
                        outPut.RespId = em.Id;
                        outPut.DisplayMessage = "Project file Saved Successfully";
                        //}
                        //else
                        //{
                        //    outPut.RespId = 0;
                        //    outPut.DisplayMessage = "Duplicate Record";
                        //}
                        outPut.HttpStatusCode = 200;
                        return Ok(outPut);
                    }
                    //else
                    //{
                    //    ProjectFolder outputDetails = _mapper.Map<ProjectFolder>(await _unitOfWork.ProjectFolder.GetByIdAsync(Convert.ToInt32(inputDTO.Id)));
                    //    if (outputDetails != null)
                    //    {

                    //        outputDetails.CategoryId = inputDTO.CategoryId;
                    //        outputDetails.ProjectId = inputDTO.ProjectId;
                    //        outputDetails.CreationDate = inputDTO.CreationDate;

                    //    }
                    //    Expression<Func<ProjectFolder, bool>> expression = a => a.Id != Convert.ToInt32(inputDTO.Id) && a.IsActive == true;
                    //    if (_unitOfWork.ProjectFolder.Exists(expression))
                    //    {
                    //        _unitOfWork.Projects.Update(_mapper.Map<Projects>(outputDetails));
                    //        _unitOfWork.Save();
                    //        outPut.HttpStatusCode = 200;
                    //        outPut.DisplayMessage = "Project Folder updated successfully!";
                    //        return Ok(outPut);
                    //    }
                    //    else
                    //    {
                    //        outPut.HttpStatusCode = 201;
                    //        outPut.DisplayMessage = "Duplicate Entry Found!";
                    //        return Ok(outPut);
                    //    }
                    //}

                    outPut.HttpStatusCode = 200;
                    return Ok(outPut);

                }
                else
                {
                    outPut.DisplayMessage = "Failed";
                    outPut.HttpStatusCode = 201;
                    return Ok(outPut);

                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving project {nameof(SaveProjectFile)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetProjectCategoryFiles")]
        [Produces("application/json", Type = typeof(ProjectFolderDTO))]
        public IEnumerable<ProjectFolderDTO> GetProjectCategoryFiles(int? pId, int? cId, int? uType)
        {
            try
            {

                var result = (from file in _unitOfWork.ProjectFolder.GetAll(p => p.IsActive == true && p.ProjectId == pId && p.CategoryId == cId).Result
                              join user in _unitOfWork.UsersMaster.GetAll(p => p.IsActive == true).Result on file.UserId equals user.UserId
                              where (file.ProjectId == pId && file.CategoryId == cId && file.IsFolder == false)

                              select new ProjectFolderDTO()
                              {
                                  Id = file.Id,
                                  AccessIds = file.AccessIds,
                                  AccessType = file.AccessType,
                                  EmailIds = file.EmailIds,
                                  OptMessage = file.OptMessage,
                                  FilePath = "/Documents/" + pId + "/" + cId + "/Files/" + file.FilePath,
                                  FileExt = Path.GetExtension(file.FilePath),
                                  CreationDate = file.CreationDate,
                                  FolderName = file.FolderName,
                                  UserType = uType,
                                  UserId = file.UserId,
                                  UserName = (user.FullName == null ? user.BusinessOwner : user.FullName),
                                  Base64ProfileImage = user.ProfileName // "data:image/png;base64," + Convert.ToBase64String(user.ProfileImage, 0, user.ProfileImage.Length)

                              }).OrderByDescending(x => x.Id).ToList();


                return result;



            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving user {nameof(GetProjectCategoryFiles)}");
                throw;
            }
        }


        [HttpPost]
        [Route("SaveFolderFile")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> SaveFolderFile(ProjectFolderFilesDTO inputDTO)
        {
            try
            {
                //inputDTO.FullName = inputDTO.FirstName + " " + inputDTO.LastName;
                OutPutResponse outPut = new OutPutResponse();
                if (ModelState.IsValid)
                {
                    if (inputDTO.Id == 0)
                    {
                        //Expression<Func<ProjectFolderFiles, bool>> expression = a => a.FilePath != inputDTO.FilePath && a.ProjectId != inputDTO.ProjectId && a.CategoryId != inputDTO.CategoryId && a.IsActive == true;
                        //if (!_unitOfWork.ProjectFolderFiles.Exists(expression))
                        //{
                        var em = _mapper.Map<ProjectFolderFilesDTO>(_unitOfWork.ProjectFolderFiles.Insert(_mapper.Map<ProjectFolderFiles>(inputDTO)));
                        outPut.RespId = em.Id;
                        outPut.DisplayMessage = "Project file Saved Successfully";
                        //}
                        //else
                        //{
                        //    outPut.RespId = 0;
                        //    outPut.DisplayMessage = "Duplicate Record";
                        //}
                        outPut.HttpStatusCode = 200;
                        return Ok(outPut);
                    }

                    outPut.HttpStatusCode = 200;
                    return Ok(outPut);

                }
                else
                {
                    outPut.DisplayMessage = "Failed";
                    outPut.HttpStatusCode = 201;
                    return Ok(outPut);

                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving project {nameof(SaveProjectFile)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetProjectFolderFiles")]
        [Produces("application/json", Type = typeof(ProjectFolderFilesDTO))]
        public IEnumerable<ProjectFolderFilesDTO> GetProjectFolderFiles(int? pId, int? cId, int? fId)
        {
            try
            {
                var result = (from file in _unitOfWork.ProjectFolderFiles.GetAll(p => p.IsActive == true && p.ProjectId == pId && p.CategoryId == cId).Result
                              join user in _unitOfWork.UsersMaster.GetAll(p => p.IsActive == true).Result on file.UserId equals user.UserId
                              where (file.ProjectId == pId && file.CategoryId == cId && file.FolderId == fId && file.IsActive == true)

                              select new ProjectFolderFilesDTO()
                              {
                                  Id = file.Id,
                                  AccessIds = file.AccessIds,                                  
                                  AccessType = file.AccessType,
                                  OptMessage = file.OptMessage,
                                  EmailIds = file.EmailIds,
                                  FilePath = "/Documents/" + pId + "/" + cId + "/" + fId + "/" + file.FilePath,
                                  FileExt = Path.GetExtension(file.FilePath),
                                  FileName = file.FileName,
                                  CreationDate = file.CreationDate,
                                  UserName = (user.FullName == null ? user.BusinessOwner : user.FullName),
                                  Base64ProfileImage = user.ProfileName // "data:image/png;base64," + Convert.ToBase64String(user.ProfileImage, 0, user.ProfileImage.Length)

                              }).OrderByDescending(x => x.Id).ToList();


                return result;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving user {nameof(GetProjectFolderFiles)}");
                throw;
            }
        }


        [HttpPost]
        [Route("SaveDrawingFile")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> SaveDrawingFile(ProjectDrawingsDTO inputDTO)
        {
            try
            {

                OutPutResponse outPut = new OutPutResponse();
                if (ModelState.IsValid)
                {
                    if (inputDTO.Id == 0)
                    {
                        //Expression<Func<ProjectFolder, bool>> expression = a => a.FilePath != inputDTO.FilePath && a.ProjectId != inputDTO.ProjectId && a.CategoryId != inputDTO.CategoryId && a.IsActive == true;
                        //if (!_unitOfWork.ProjectFolder.Exists(expression))
                        //{
                        var em = _mapper.Map<ProjectDrawingsDTO>(_unitOfWork.ProjectDrawings.Insert(_mapper.Map<ProjectDrawings>(inputDTO)));
                        outPut.RespId = em.Id;
                        outPut.DisplayMessage = "Project drawing Saved Successfully";
                        //}
                        //else
                        //{
                        //    outPut.RespId = 0;
                        //    outPut.DisplayMessage = "Duplicate Record";
                        //}
                        outPut.HttpStatusCode = 200;
                        return Ok(outPut);
                    }


                    outPut.HttpStatusCode = 200;
                    return Ok(outPut);

                }
                else
                {
                    outPut.DisplayMessage = "Failed";
                    outPut.HttpStatusCode = 201;
                    return Ok(outPut);

                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving project {nameof(SaveDrawingFile)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetProjectDrawingFolders")]
        [Produces("application/json", Type = typeof(ProjectDrawingsDTO))]
        public IEnumerable<ProjectDrawingsDTO> GetProjectDrawingFolders(int? pId, int? cId, int? uType,int? uId)
        {
            //try
            //{
            //    var result = (from file in _unitOfWork.ProjectDrawings.GetAll(p => p.IsActive == true && p.ProjectId == pId && p.CategoryId == cId).Result
            //                  join user in _unitOfWork.UsersMaster.GetAll(p => p.IsActive == true).Result on file.UserId equals user.UserId
            //                  where (file.ProjectId == pId && file.CategoryId == cId && file.IsFolder == true)

            //                  select new ProjectDrawingsDTO()
            //                  {
            //                      Id = file.Id,
            //                      AccessType = file.AccessType,
            //                      AccessIds = file.AccessIds,
            //                      EmailIds = file.EmailIds,
            //                      OptMessage = file.OptMessage,
            //                      FolderName = file.FolderName,
            //                      CreationDate = file.CreationDate,
            //                      UserName = (user.FullName == null ? user.BusinessOwner : user.FullName),
            //                      Base64ProfileImage = user.ProfileName // "data:image/png;base64," + Convert.ToBase64String(user.ProfileImage, 0, user.ProfileImage.Length)

            //                  }).OrderByDescending(x => x.Id).ToList();


            //    return result;

            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, $"Error in retriving user {nameof(GetProjectDrawingFolders)}");
            //    throw;
            //}

            try
            {
                // var result;
                if (uType == 1)
                {
                    var result = (from file in _unitOfWork.ProjectDrawings.GetAll(p => p.IsActive == true && p.ProjectId == pId && p.CategoryId == cId).Result
                                  join user in _unitOfWork.UsersMaster.GetAll(p => p.IsActive == true).Result on file.UserId equals user.UserId
                                  where (file.ProjectId == pId && file.CategoryId == cId && file.IsFolder == true)

                                  select new ProjectDrawingsDTO()
                                  {
                                      Id = file.Id,
                                      AccessType = file.AccessType,
                                      AccessIds = file.AccessIds,
                                      ClientIds = file.ClientIds,
                                      EmailIds = file.EmailIds,
                                      OptMessage = file.OptMessage,
                                      FolderName = file.FolderName,
                                      CreationDate = file.CreationDate,
                                      UserType = uType,
                                      UserId = file.UserId,
                                      UserName = (user.FullName == null ? user.BusinessOwner : user.FullName),
                                      Base64ProfileImage = user.ProfileName // "data:image/png;base64," + Convert.ToBase64String(user.ProfileImage, 0, user.ProfileImage.Length)

                                  }).OrderByDescending(x => x.Id).ToList();


                    return result;

                }
                else if (uType == 2)
                {

                    var result = (from file in _unitOfWork.ProjectDrawings.GetAll(p => p.IsActive == true && p.ProjectId == pId && p.CategoryId == cId && p.IsFolder == true).Result
                   .Where(file => !string.IsNullOrEmpty(file.ClientIds) && file.ClientIds.Split(',').Contains(uId.ToString()) || file.UserId == uId)
                                  join user in _unitOfWork.UsersMaster.GetAll(p => p.IsActive == true).Result
                    on file.UserId equals user.UserId

                                  select new ProjectDrawingsDTO
                                  {
                                      Id = file.Id,
                                      FolderName = file.FolderName,
                                      AccessIds = file.AccessIds,
                                      ClientIds = file.ClientIds,
                                      AccessType = file.AccessType,
                                      EmailIds = file.EmailIds,
                                      OptMessage = file.OptMessage,
                                      CreationDate = file.CreationDate,
                                      UserType = uType,
                                      UserId = file.UserId,
                                      UserName = string.IsNullOrEmpty(user.FullName) ? user.BusinessOwner : user.FullName,
                                      Base64ProfileImage = user.ProfileName
                                  }).OrderByDescending(x => x.Id).ToList();

                    return result;

                }
                else if (uType == 3)
                {
                    var result = (from file in _unitOfWork.ProjectDrawings.GetAll(p => p.IsActive == true && p.ProjectId == pId && p.CategoryId == cId && p.IsFolder == true).Result
                 .Where(file => !string.IsNullOrEmpty(file.AccessIds) && file.AccessIds.Split(',').Contains(uId.ToString()) || file.UserId == uId)
                                  join user in _unitOfWork.UsersMaster.GetAll(p => p.IsActive == true).Result
                    on file.UserId equals user.UserId

                                  select new ProjectDrawingsDTO
                                  {
                                      Id = file.Id,
                                      FolderName = file.FolderName,
                                      AccessIds = file.AccessIds,
                                      ClientIds = file.ClientIds,
                                      AccessType = file.AccessType,
                                      EmailIds = file.EmailIds,
                                      OptMessage = file.OptMessage,
                                      CreationDate = file.CreationDate,
                                      UserType = uType,
                                      UserId = file.UserId,
                                      UserName = string.IsNullOrEmpty(user.FullName) ? user.BusinessOwner : user.FullName,
                                      Base64ProfileImage = user.ProfileName
                                  }).OrderByDescending(x => x.Id).ToList();

                    return result;
                }

                return Enumerable.Empty<ProjectDrawingsDTO>();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving user {nameof(GetProjectCategoryFolders)}");
                throw;
            }

        }

        [HttpGet]
        [Route("GetProjectDrawingFiles")]
        [Produces("application/json", Type = typeof(ProjectDrawingsDTO))]
        public IEnumerable<ProjectDrawingsDTO> GetProjectDrawingFiles(int? pId, int? cId, int? uType)
        {
            try
            {
                var result = (from file in _unitOfWork.ProjectDrawings.GetAll(p => p.IsActive == true && p.ProjectId == pId && p.CategoryId == cId).Result
                              join user in _unitOfWork.UsersMaster.GetAll(p => p.IsActive == true).Result on file.UserId equals user.UserId
                              where (file.ProjectId == pId && file.CategoryId == cId && file.IsFolder == false)

                              select new ProjectDrawingsDTO()
                              {
                                  Id = file.Id,
                                  AccessType = file.AccessType,
                                  AccessIds = file.AccessIds,
                                  EmailIds = file.EmailIds,
                                  OptMessage = file.OptMessage,
                                  FilePath = "/Drawings/" + pId + "/" + cId + "/Files/" + file.FilePath,
                                  FileExt = Path.GetExtension(file.FilePath),
                                  CreationDate = file.CreationDate,
                                  FolderName = file.FolderName,
                                  UserType = uType,
                                  UserId = file.UserId,
                                  UserName = (user.FullName == null ? user.BusinessOwner : user.FullName),
                                  Base64ProfileImage = user.ProfileName //"data:image/png;base64," + Convert.ToBase64String(user.ProfileImage, 0, user.ProfileImage.Length)

                              }).OrderByDescending(x => x.Id).ToList();


                return result;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving user {nameof(GetProjectDrawingFiles)}");
                throw;
            }
        }




        [HttpPost]
        [Route("SaveDrawingFolderFile")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> SaveDrawingFolderFile(DrawingFolderFilesDTO inputDTO)
        {
            try
            {
                //inputDTO.FullName = inputDTO.FirstName + " " + inputDTO.LastName;
                OutPutResponse outPut = new OutPutResponse();
                if (ModelState.IsValid)
                {
                    if (inputDTO.Id == 0)
                    {
                        //Expression<Func<ProjectFolderFiles, bool>> expression = a => a.FilePath != inputDTO.FilePath && a.ProjectId != inputDTO.ProjectId && a.CategoryId != inputDTO.CategoryId && a.IsActive == true;
                        //if (!_unitOfWork.ProjectFolderFiles.Exists(expression))
                        //{
                        var em = _mapper.Map<DrawingFolderFilesDTO>(_unitOfWork.DrawingFolderFiles.Insert(_mapper.Map<DrawingFolderFiles>(inputDTO)));
                        outPut.RespId = em.Id;
                        outPut.DisplayMessage = "Project file Saved Successfully";
                        //}
                        //else
                        //{
                        //    outPut.RespId = 0;
                        //    outPut.DisplayMessage = "Duplicate Record";
                        //}
                        outPut.HttpStatusCode = 200;
                        return Ok(outPut);
                    }

                    outPut.HttpStatusCode = 200;
                    return Ok(outPut);

                }
                else
                {
                    outPut.DisplayMessage = "Failed";
                    outPut.HttpStatusCode = 201;
                    return Ok(outPut);

                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving project {nameof(SaveDrawingFolderFile)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetProjectDrawingFolderFiles")]
        [Produces("application/json", Type = typeof(DrawingFolderFilesDTO))]
        public IEnumerable<DrawingFolderFilesDTO> GetProjectDrawingFolderFiles(int? pId, int? cId, int? fId)
        {
            try
            {
                var result = (from file in _unitOfWork.DrawingFolderFiles.GetAll(p => p.IsActive == true && p.ProjectId == pId && p.CategoryId == cId).Result
                              join user in _unitOfWork.UsersMaster.GetAll(p => p.IsActive == true).Result on file.UserId equals user.UserId
                              where (file.ProjectId == pId && file.CategoryId == cId && file.FolderId == fId && file.IsActive == true)

                              select new DrawingFolderFilesDTO()
                              {
                                  Id = file.Id,
                                  AccessType = file.AccessType,
                                  AccessIds = file.AccessIds,
                                  EmailIds = file.EmailIds,
                                  OptMessage = file.OptMessage,
                                  FilePath = "/Drawings/" + pId + "/" + cId + "/" + fId + "/" + file.FilePath,
                                  FileExt = Path.GetExtension(file.FilePath),
                                  FileName = file.FileName,
                                  CreationDate = file.CreationDate,
                                  UserName = (user.FullName == null ? user.BusinessOwner : user.FullName),
                                  Base64ProfileImage = user.ProfileName //  "data:image/png;base64," + Convert.ToBase64String(user.ProfileImage, 0, user.ProfileImage.Length)

                              }).OrderByDescending(x => x.Id).ToList();


                return result;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving user {nameof(GetProjectDrawingFolderFiles)}");
                throw;
            }
        }

        [HttpGet]
        [Route("DeleteFolderFile")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DeleteFolderFile(int pId)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    ProjectFolderFiles outputMaster = _mapper.Map<ProjectFolderFiles>(await _unitOfWork.ProjectFolderFiles.GetByIdAsync(pId));
                    //outputMaster.IsActive = false;
                    _unitOfWork.ProjectFolderFiles.Remove(outputMaster);
                    _unitOfWork.Save();
                    outPut.HttpStatusCode = 200;
                    outPut.DisplayMessage = "File delete successfully!";
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
                _logger.LogError(ex, $"Error while deleting country {nameof(DeleteFolderFile)}");
                throw;
            }
        }

        [HttpGet]
        [Route("DeleteFolder")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DeleteFolder(int pId)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    ProjectFolder outputMaster = _mapper.Map<ProjectFolder>(await _unitOfWork.ProjectFolder.GetByIdAsync(pId));
                    //outputMaster.IsActive = false;
                    _unitOfWork.ProjectFolder.Remove(outputMaster);
                    _unitOfWork.Save();
                    outPut.HttpStatusCode = 200;
                    outPut.DisplayMessage = "File/folder delete successfully!";
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
                _logger.LogError(ex, $"Error while deleting country {nameof(DeleteFolder)}");
                throw;
            }
        }


        [HttpGet]
        [Route("DeleteDrawingFolderFile")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DeleteDrawingFolderFile(int pId)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    DrawingFolderFiles outputMaster = _mapper.Map<DrawingFolderFiles>(await _unitOfWork.DrawingFolderFiles.GetByIdAsync(pId));
                    //outputMaster.IsActive = false;
                    _unitOfWork.DrawingFolderFiles.Remove(outputMaster);
                    _unitOfWork.Save();
                    outPut.HttpStatusCode = 200;
                    outPut.DisplayMessage = "File delete successfully!";
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
                _logger.LogError(ex, $"Error while deleting country {nameof(DeleteDrawingFolderFile)}");
                throw;
            }
        }

        [HttpGet]
        [Route("DeleteDrawingFolder")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DeleteDrawingFolder(int pId)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    ProjectDrawings outputMaster = _mapper.Map<ProjectDrawings>(await _unitOfWork.ProjectDrawings.GetByIdAsync(pId));
                    //outputMaster.IsActive = false;
                    _unitOfWork.ProjectDrawings.Remove(outputMaster);
                    _unitOfWork.Save();
                    outPut.HttpStatusCode = 200;
                    outPut.DisplayMessage = "File/folder delete successfully!";
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
                _logger.LogError(ex, $"Error while deleting country {nameof(DeleteFolder)}");
                throw;
            }
        }

        [HttpPost]
        public async Task<bool>? SaveExternalAccess(ExternalUsersDTO objInputs)
        {
            ClientMail objmaill = new ClientMail();
            //   UserNotificationsDTO CommentsLst = new UserNotificationsDTO();
            try
            {
                string[] emails = objInputs.EmailId.Split(',');
                if (emails.Length > 0)
                {
                    for (int i = 0; i < emails.Length; i++)
                    {
                        var parms = new DynamicParameters();
                        parms.Add(@"@ShareIds", objInputs.ShareIds, DbType.String);
                        parms.Add(@"@TableId", objInputs.TableId, DbType.Int32);
                        parms.Add(@"@EmailId", emails[i], DbType.String);
                        parms.Add(@"@CreatedBy", objInputs.CreatedBy, DbType.String);
                        var result = (await _unitOfWork.ExternalUsers.GetSPData<ExternalUsersDTO>("usp_PublicFileShared", parms));
                        if (result.Count > 0)
                        {
                            objInputs.UniqueId = result[0].UniqueId;
                            objInputs.EmailId = emails[i];
                        }
                        await objmaill.SharedUrlOnMail(objInputs);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in Save External Access {nameof(SaveExternalAccess)}");
                return false;
            }
        }

        [HttpPost]
        [Route("EditDocumentFile")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> EditDocumentFile(ProjectFolderFilesDTO inputDTO)
        {
            try
            {
                //inputDTO.FullName = inputDTO.FirstName + " " + inputDTO.LastName;
                OutPutResponse outPut = new OutPutResponse();
                if (ModelState.IsValid)
                {
                    if (inputDTO.Id > 0)
                    {
                        ProjectFolderFiles outputDetails = _mapper.Map<ProjectFolderFiles>(await _unitOfWork.ProjectFolderFiles.GetByIdAsync(Convert.ToInt32(inputDTO.Id)));
                        if (outputDetails != null)
                        {
                            if (!string.IsNullOrEmpty(inputDTO.AccessIds))
                            {
                                outputDetails.AccessIds = inputDTO.AccessIds;
                                outputDetails.AccessType = inputDTO.AccessType;
                                outputDetails.OptMessage = inputDTO.OptMessage;
                            }
                            else
                            {
                                outputDetails.EmailIds = inputDTO.EmailIds;
                                outputDetails.OptMessage = inputDTO.OptMessage;
                            }

                        }
                        Expression<Func<ProjectFolderFiles, bool>> expression = a => a.Id != Convert.ToInt32(inputDTO.Id) && a.IsActive == true;
                        if (_unitOfWork.ProjectFolderFiles.Exists(expression))
                        {
                            _unitOfWork.ProjectFolderFiles.Update(_mapper.Map<ProjectFolderFiles>(outputDetails));
                            _unitOfWork.Save();
                            if (!string.IsNullOrEmpty(outputDetails.EmailIds))
                            {
                                ExternalUsersDTO objEmail = new ExternalUsersDTO();
                                objEmail.ShareIds = Convert.ToString(inputDTO.Id);
                                objEmail.EmailId = outputDetails.EmailIds;
                                objEmail.OptMessage = outputDetails.OptMessage;
                                objEmail.TableId = 2;
                                objEmail.CreatedBy = "Admin";

                                await SaveExternalAccess(objEmail);
                            }
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Team invitation successfully!";
                            return Ok(outPut);
                        }
                        return Ok(outPut);
                    }
                    else
                    {
                        outPut.DisplayMessage = "Failed";
                        outPut.HttpStatusCode = 201;
                        return Ok(outPut);

                    }

                }
                else
                {
                    outPut.DisplayMessage = "Failed";
                    outPut.HttpStatusCode = 201;
                    return Ok(outPut);

                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving project {nameof(EditDocumentFolder)}");
                throw;
            }
        }


        [HttpPost]
        [Route("EditDrawingFolder")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> EditDrawingFolder(ProjectDrawingsDTO inputDTO)
        {
            try
            {
                //inputDTO.FullName = inputDTO.FirstName + " " + inputDTO.LastName;
                OutPutResponse outPut = new OutPutResponse();
                if (ModelState.IsValid)
                {
                    if (inputDTO.Id > 0)
                    {
                        ProjectDrawings outputDetails = _mapper.Map<ProjectDrawings>(await _unitOfWork.ProjectDrawings.GetByIdAsync(Convert.ToInt32(inputDTO.Id)));
                        if (outputDetails != null)
                        {
                            if (!string.IsNullOrEmpty(inputDTO.AccessIds))
                            {
                                outputDetails.AccessIds = inputDTO.AccessIds;
                                outputDetails.AccessType = inputDTO.AccessType;
                                outputDetails.OptMessage = inputDTO.OptMessage;
                            }
                            else if (!string.IsNullOrEmpty(inputDTO.ClientIds))
                            {
                                outputDetails.ClientIds = inputDTO.ClientIds;
                                outputDetails.AccessType = inputDTO.AccessType;
                                outputDetails.OptMessage = inputDTO.OptMessage;
                            }
                            else
                            {
                                outputDetails.EmailIds = inputDTO.EmailIds;
                                outputDetails.OptMessage = inputDTO.OptMessage;
                            }

                        }
                        Expression<Func<ProjectDrawings, bool>> expression = a => a.Id != Convert.ToInt32(inputDTO.Id) && a.IsActive == true;
                        if (_unitOfWork.ProjectDrawings.Exists(expression))
                        {
                            _unitOfWork.ProjectDrawings.Update(_mapper.Map<ProjectDrawings>(outputDetails));
                            _unitOfWork.Save();
                            if (!string.IsNullOrEmpty(outputDetails.EmailIds))
                            {
                                ExternalUsersDTO objEmail = new ExternalUsersDTO();
                                objEmail.ShareIds = Convert.ToString(inputDTO.Id);
                                objEmail.EmailId = outputDetails.EmailIds;
                                objEmail.OptMessage = outputDetails.OptMessage;
                                objEmail.TableId = 3;
                                objEmail.CreatedBy = "Admin";

                                await SaveExternalAccess(objEmail);
                            }
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Team invitation successfully!";
                            return Ok(outPut);
                        }
                        return Ok(outPut);
                    }
                    else
                    {
                        outPut.DisplayMessage = "Failed";
                        outPut.HttpStatusCode = 201;
                        return Ok(outPut);

                    }

                }
                else
                {
                    outPut.DisplayMessage = "Failed";
                    outPut.HttpStatusCode = 201;
                    return Ok(outPut);

                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving project {nameof(EditDrawingFolder)}");
                throw;
            }
        }

        [HttpPost]
        [Route("EditDrawingFile")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> EditDrawingFile(DrawingFolderFilesDTO inputDTO)
        {
            try
            {
                //inputDTO.FullName = inputDTO.FirstName + " " + inputDTO.LastName;
                OutPutResponse outPut = new OutPutResponse();
                if (ModelState.IsValid)
                {
                    if (inputDTO.Id > 0)
                    {
                        DrawingFolderFiles outputDetails = _mapper.Map<DrawingFolderFiles>(await _unitOfWork.DrawingFolderFiles.GetByIdAsync(Convert.ToInt32(inputDTO.Id)));
                        if (outputDetails != null)
                        {
                            if (!string.IsNullOrEmpty(inputDTO.AccessIds))
                            {
                                outputDetails.AccessIds = inputDTO.AccessIds;
                                outputDetails.AccessType = inputDTO.AccessType;
                                outputDetails.OptMessage = inputDTO.OptMessage;
                            }
                            else
                            {
                                outputDetails.EmailIds = inputDTO.EmailIds;
                                outputDetails.OptMessage = inputDTO.OptMessage;
                            }

                        }
                        Expression<Func<DrawingFolderFiles, bool>> expression = a => a.Id != Convert.ToInt32(inputDTO.Id) && a.IsActive == true;
                        if (_unitOfWork.DrawingFolderFiles.Exists(expression))
                        {
                            _unitOfWork.DrawingFolderFiles.Update(_mapper.Map<DrawingFolderFiles>(outputDetails));
                            _unitOfWork.Save();
                            if (!string.IsNullOrEmpty(outputDetails.EmailIds))
                            {
                                ExternalUsersDTO objEmail = new ExternalUsersDTO();
                                objEmail.ShareIds = Convert.ToString(inputDTO.Id);
                                objEmail.EmailId = outputDetails.EmailIds;
                                objEmail.OptMessage = outputDetails.OptMessage;
                                objEmail.TableId = 4;
                                objEmail.CreatedBy = "Admin";

                                await SaveExternalAccess(objEmail);
                            }
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Team invitation successfully!";
                            return Ok(outPut);
                        }
                        return Ok(outPut);
                    }
                    else
                    {
                        outPut.DisplayMessage = "Failed";
                        outPut.HttpStatusCode = 201;
                        return Ok(outPut);

                    }

                }
                else
                {
                    outPut.DisplayMessage = "Failed";
                    outPut.HttpStatusCode = 201;
                    return Ok(outPut);

                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving project {nameof(EditDocumentFolder)}");
                throw;
            }
        }

        [HttpPost]
        [Route("EditFileName")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> EditFileName(ProjectFolderDTO inputDTO)
        {
            try
            {
                //inputDTO.FullName = inputDTO.FirstName + " " + inputDTO.LastName;
                OutPutResponse outPut = new OutPutResponse();
                if (ModelState.IsValid)
                {
                    if (inputDTO.Id > 0)
                    {
                        ProjectFolder outputDetails = _mapper.Map<ProjectFolder>(await _unitOfWork.ProjectFolder.GetByIdAsync(Convert.ToInt32(inputDTO.Id)));
                        if (outputDetails != null)
                        {
                            outputDetails.FolderName = inputDTO.FolderName;

                        }
                        Expression<Func<ProjectFolder, bool>> expression = a => a.Id != Convert.ToInt32(inputDTO.Id) && a.IsActive == true;
                        if (_unitOfWork.ProjectFolder.Exists(expression))
                        {
                            _unitOfWork.ProjectFolder.Update(_mapper.Map<ProjectFolder>(outputDetails));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Folder name has been changed successfully!";
                            return Ok(outPut);
                        }
                        return Ok(outPut);
                    }
                    else
                    {
                        outPut.DisplayMessage = "Failed";
                        outPut.HttpStatusCode = 201;
                        return Ok(outPut);

                    }

                }
                else
                {
                    outPut.DisplayMessage = "Failed";
                    outPut.HttpStatusCode = 201;
                    return Ok(outPut);

                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving project {nameof(EditDocumentFolder)}");
                throw;
            }
        }

        [HttpPost]
        [Route("EditDrawingFileName")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> EditDrawingFileName(ProjectDrawings inputDTO)
        {
            try
            {
                //inputDTO.FullName = inputDTO.FirstName + " " + inputDTO.LastName;
                OutPutResponse outPut = new OutPutResponse();
                if (ModelState.IsValid)
                {
                    if (inputDTO.Id > 0)
                    {
                        ProjectDrawings outputDetails = _mapper.Map<ProjectDrawings>(await _unitOfWork.ProjectDrawings.GetByIdAsync(Convert.ToInt32(inputDTO.Id)));
                        if (outputDetails != null)
                        {
                            outputDetails.FolderName = inputDTO.FolderName;

                        }
                        Expression<Func<ProjectDrawings, bool>> expression = a => a.Id != Convert.ToInt32(inputDTO.Id) && a.IsActive == true;
                        if (_unitOfWork.ProjectDrawings.Exists(expression))
                        {
                            _unitOfWork.ProjectDrawings.Update(_mapper.Map<ProjectDrawings>(outputDetails));
                            _unitOfWork.Save();
                            outPut.HttpStatusCode = 200;
                            outPut.DisplayMessage = "Folder name has been changed successfully!";
                            return Ok(outPut);
                        }
                        return Ok(outPut);
                    }
                    else
                    {
                        outPut.DisplayMessage = "Failed";
                        outPut.HttpStatusCode = 201;
                        return Ok(outPut);

                    }

                }
                else
                {
                    outPut.DisplayMessage = "Failed";
                    outPut.HttpStatusCode = 201;
                    return Ok(outPut);

                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving project {nameof(EditDocumentFolder)}");
                throw;
            }
        }
    }
}
