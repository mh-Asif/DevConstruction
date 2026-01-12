using AutoMapper;
using Construction.Infrastructure.KeyValues;
using Construction.Infrastructure.Models;
using ConstructionApp.Core.Entities;
using ConstructionApp.Core.Repository;
using ConstructionApp.EndPoints.Helper;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Linq.Expressions;

namespace ConstructionApp.EndPoints.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectPhotoAPIController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProjectPhotoAPIController> _logger;
        private readonly IMapper _mapper;

        public ProjectPhotoAPIController(IUnitOfWork unitOfWork, ILogger<ProjectPhotoAPIController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;

        }

        [HttpPost]
        [Route("SaveProjectPhotoFile")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> SaveProjectPhotoFile(ProjectPhotosDTO inputDTO)
        {
            try
            {
                //inputDTO.FullName = inputDTO.FirstName + " " + inputDTO.LastName;
                OutPutResponse outPut = new OutPutResponse();
                if (ModelState.IsValid)
                {
                   // if (inputDTO.Id == 0)
                   // {
                       // Expression<Func<ProjectPhotos, bool>> expression = a => a.FolderName != inputDTO.FolderName && a.ProjectId != inputDTO.ProjectId && a.IsActive == true;
                       // if (_unitOfWork.ProjectPhotos.Exists(expression))
                       // {
                            var em = _mapper.Map<ProjectPhotosDTO>(_unitOfWork.ProjectPhotos.Insert(_mapper.Map<ProjectPhotos>(inputDTO)));
                            outPut.RespId = em.Id;
                            outPut.DisplayMessage = "Project folder Saved Successfully";
                      //  }
                      //  else
                      //  {
                            outPut.RespId = 0;
                            outPut.DisplayMessage = "Duplicate Record";
                       // }
                        outPut.HttpStatusCode = 200;
                        return Ok(outPut);
                   // }
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
                _logger.LogError(ex, $"Error in saving project {nameof(SaveProjectPhotoFile)}");
                throw;
            }
        }


        [HttpGet]
        [Route("GetProjectPhotoFolders")]
        [Produces("application/json", Type = typeof(ProjectPhotosDTO))]
        public IEnumerable<ProjectPhotosDTO> GetProjectPhotoFolders(int? pId, int? cId, int? uId, int? uType)
        {
            try
            {
                if (uType == 1)
                {
                    var result = (from file in _unitOfWork.ProjectPhotos.GetAll(p => p.IsActive == true && p.ProjectId == pId && p.CategoryId == cId).Result
                                  join user in _unitOfWork.UsersMaster.GetAll(p => p.IsActive == true).Result on file.UserId equals user.UserId
                                  where (file.ProjectId == pId && file.CategoryId == cId && file.IsFolder == true)

                                  select new ProjectPhotosDTO()
                                  {
                                      Id = file.Id,
                                      AccessType = file.AccessType,
                                      ClientIds = file.ClientIds,
                                      AccessIds = file.AccessIds,
                                      EmailIds = file.EmailIds,
                                      OptMessage = file.OptMessage,
                                      FolderName = file.FolderName,
                                      CreationDate = file.CreationDate,
                                      UserType = uType,
                                      UserId = file.UserId,
                                      UserName = (user.FullName == null ? user.BusinessOwner : user.FullName),
                                      Base64ProfileImage = user.ProfileName //"data:image/png;base64," + Convert.ToBase64String(user.ProfileImage, 0, user.ProfileImage.Length)

                                  }).OrderByDescending(x => x.Id).ToList();


                    return result;
                }
                else if (uType == 2)
                {

                    var result = (from file in _unitOfWork.ProjectPhotos.GetAll(p => p.IsActive == true && p.ProjectId == pId && p.CategoryId == cId && p.IsFolder == true).Result
                   .Where(file => !string.IsNullOrEmpty(file.ClientIds) && file.ClientIds.Split(',').Contains(uId.ToString()) || file.UserId == uId)
                                  join user in _unitOfWork.UsersMaster.GetAll(p => p.IsActive == true).Result
                    on file.UserId equals user.UserId

                                  select new ProjectPhotosDTO
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
                    var result = (from file in _unitOfWork.ProjectPhotos.GetAll(p => p.IsActive == true && p.ProjectId == pId && p.CategoryId == cId && p.IsFolder == true).Result
                 .Where(file => !string.IsNullOrEmpty(file.AccessIds) && file.AccessIds.Split(',').Contains(uId.ToString()) || file.UserId == uId)
                                  join user in _unitOfWork.UsersMaster.GetAll(p => p.IsActive == true).Result
                    on file.UserId equals user.UserId

                                  select new ProjectPhotosDTO
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

                return Enumerable.Empty<ProjectPhotosDTO>();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in retriving user {nameof(GetProjectPhotoFolders)}");
                throw;
            }
        }


        [HttpPost]
        [Route("SavePhotoFolderFile")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> SavePhotoFolderFile(ProjectFilePhotosDTO inputDTO)
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
                        var em = _mapper.Map<ProjectFilePhotosDTO>(_unitOfWork.ProjectFilePhotos.Insert(_mapper.Map<ProjectFilePhotos>(inputDTO)));
                        outPut.RespId = em.Id;
                        outPut.DisplayMessage = "Album photo Saved Successfully";
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
                _logger.LogError(ex, $"Error in saving project {nameof(SavePhotoFolderFile)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetProjectPhotoFolderFiles")]
        [Produces("application/json", Type = typeof(ProjectFilePhotosDTO))]
        public IEnumerable<ProjectFilePhotosDTO> GetProjectPhotoFolderFiles(int? pId, int? cId, int? fId)
        {
            try
            {
                var result = (from file in _unitOfWork.ProjectFilePhotos.GetAll(p => p.IsActive == true && p.ProjectId == pId && p.CategoryId == cId).Result
                              join user in _unitOfWork.UsersMaster.GetAll(p => p.IsActive == true).Result on file.UserId equals user.UserId
                              where (file.ProjectId == pId && file.CategoryId == cId && file.FolderId == fId && file.IsActive == true)

                              select new ProjectFilePhotosDTO()
                              {
                                  Id = file.Id,
                                  AccessType = file.AccessType,
                                  AccessIds = file.AccessIds,
                                  EmailIds = file.EmailIds,
                                  OptMessage = file.OptMessage,
                                  FilePath = "/Photos/" + pId + "/" + cId + "/" + fId + "/" + file.FilePath,
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
                _logger.LogError(ex, $"Error in retriving user {nameof(GetProjectPhotoFolderFiles)}");
                throw;
            }
        }


        [HttpGet]
        [Route("DeletePhotoAlbumFile")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DeletePhotoAlbumFile(int pId)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    ProjectFilePhotos outputMaster = _mapper.Map<ProjectFilePhotos>(await _unitOfWork.ProjectFilePhotos.GetByIdAsync(pId));
                    //outputMaster.IsActive = false;
                    _unitOfWork.ProjectFilePhotos.Remove(outputMaster);
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
                _logger.LogError(ex, $"Error while deleting country {nameof(DeletePhotoAlbumFile)}");
                throw;
            }
        }

        [HttpGet]
        [Route("DeletePhotoFolder")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DeletePhotoFolder(int pId)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (ModelState.IsValid)
                {
                    ProjectPhotos outputMaster = _mapper.Map<ProjectPhotos>(await _unitOfWork.ProjectPhotos.GetByIdAsync(pId));
                    //outputMaster.IsActive = false;
                    _unitOfWork.ProjectPhotos.Remove(outputMaster);
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
                _logger.LogError(ex, $"Error while deleting country {nameof(DeletePhotoFolder)}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetPublicFilesAccess")]
        [Produces("application/json", Type = typeof(PublicUsersDTO))]
        public async Task<List<PublicUsersDTO>?> GetPublicFilesAccess(string? uniqueId)
        {
            PublicUsersDTO ProjectLst = new PublicUsersDTO();
            var parms = new DynamicParameters();

            parms.Add(@"@UniqueId", uniqueId, DbType.String);           

            ProjectLst.PublicFileList = (await _unitOfWork.PublicUsers.GetSPData<PublicUsersDTO>("usp_GetUserFilesDetails", parms));
            return ProjectLst.PublicFileList;
        }

        [HttpPost]
        [Route("EditPhotoFolder")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> EditPhotoFolder(ProjectPhotosDTO inputDTO)
        {
            try
            {
                //inputDTO.FullName = inputDTO.FirstName + " " + inputDTO.LastName;
                OutPutResponse outPut = new OutPutResponse();
                if (ModelState.IsValid)
                {
                    if (inputDTO.Id > 0)
                    {
                        ProjectPhotos outputDetails = _mapper.Map<ProjectPhotos>(await _unitOfWork.ProjectPhotos.GetByIdAsync(Convert.ToInt32(inputDTO.Id)));
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
                        Expression<Func<ProjectPhotos, bool>> expression = a => a.Id != Convert.ToInt32(inputDTO.Id) && a.IsActive == true;
                        if (_unitOfWork.ProjectPhotos.Exists(expression))
                        {
                            _unitOfWork.ProjectPhotos.Update(_mapper.Map<ProjectPhotos>(outputDetails));
                            _unitOfWork.Save();
                            if (!string.IsNullOrEmpty(outputDetails.EmailIds))
                            {
                                ExternalUsersDTO objEmail = new ExternalUsersDTO();
                                objEmail.ShareIds = Convert.ToString(inputDTO.Id);
                                objEmail.EmailId = outputDetails.EmailIds;
                                objEmail.OptMessage = outputDetails.OptMessage;
                                objEmail.TableId = 5;
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
                _logger.LogError(ex, $"Error in saving project {nameof(EditPhotoFolder)}");
                throw;
            }
        }

        [HttpPost]
        [Route("EditPhotoFile")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> EditPhotoFile(ProjectFilePhotosDTO inputDTO)
        {
            try
            {
                
                OutPutResponse outPut = new OutPutResponse();
                if (ModelState.IsValid)
                {
                    if(!string.IsNullOrEmpty(inputDTO.Ids))
                    {
                        var allIds = inputDTO.Ids.Split(',').Select(int.Parse).ToList();
                        for(int i = 0; i < allIds.Count; i++)
                        {
                            ProjectFilePhotos outputDetails = _mapper.Map<ProjectFilePhotos>(await _unitOfWork.ProjectFilePhotos.GetByIdAsync(allIds[i]));
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
                            Expression<Func<ProjectFilePhotos, bool>> expression = a => a.Id != allIds[i] && a.IsActive == true;
                            if (_unitOfWork.ProjectFilePhotos.Exists(expression))
                            {
                                _unitOfWork.ProjectFilePhotos.Update(_mapper.Map<ProjectFilePhotos>(outputDetails));
                                _unitOfWork.Save();
                              
                            }
                        }
                        if (!string.IsNullOrEmpty(inputDTO.EmailIds))
                        {
                            ExternalUsersDTO objEmail = new ExternalUsersDTO();
                            objEmail.ShareIds = inputDTO.Ids;
                            objEmail.EmailId = inputDTO.EmailIds;
                            objEmail.OptMessage = inputDTO.OptMessage;
                            objEmail.TableId = 6;
                            objEmail.emailPath = inputDTO.emailPath;
                            objEmail.CreatedBy = "Admin";

                            await SaveExternalAccess(objEmail);
                        }
                        outPut.HttpStatusCode = 200;
                        outPut.DisplayMessage = "Team invitation successfully!";
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
                _logger.LogError(ex, $"Error in saving project {nameof(EditPhotoFile)}");
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
        [Route("EditPhotoFileName")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> EditPhotoFileName(ProjectPhotosDTO inputDTO)
        {
            try
            {
                  OutPutResponse outPut = new OutPutResponse();
                if (ModelState.IsValid)
                {
                    if (inputDTO.Id > 0)
                    {
                        ProjectPhotos outputDetails = _mapper.Map<ProjectPhotos>(await _unitOfWork.ProjectPhotos.GetByIdAsync(Convert.ToInt32(inputDTO.Id)));
                        if (outputDetails != null)
                        {
                            outputDetails.FolderName = inputDTO.FolderName;

                        }
                        Expression<Func<ProjectPhotos, bool>> expression = a => a.Id != Convert.ToInt32(inputDTO.Id) && a.IsActive == true;
                        if (_unitOfWork.ProjectPhotos.Exists(expression))
                        {
                            _unitOfWork.ProjectPhotos.Update(_mapper.Map<ProjectPhotos>(outputDetails));
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
                _logger.LogError(ex, $"Error in saving project {nameof(EditPhotoFileName)}");
                throw;
            }
        }

        [HttpGet]
        [Route("PdfFile")]
        [Produces("application/json", Type = typeof(PublicUsersDTO))]
        public async Task <List<PublicUsersDTO>>? PdfFile(string? ids)
        {
          //  string? ids = null;
           // ClientMail objmaill = new ClientMail();
            //   UserNotificationsDTO CommentsLst = new UserNotificationsDTO();
            try
            {
               
                        var parms = new DynamicParameters();
                        parms.Add(@"@ShareIds", ids, DbType.String);
                      
                        var result = (await _unitOfWork.PublicUsers.GetSPData<PublicUsersDTO>("usp_UserPdf", parms));
                        
               
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in Pdf File {nameof(PdfFile)}");
                return null;
            }
        }
    }
}
