using Construction.Infrastructure.KeyValues;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construction.Infrastructure.Models
{
    public class UsersMasterDTO
    {
          public int UserId { get; set; }
        public string? EnycUserId { get; set; }
        public string Base64ProfileImage { get; set; }
        public string? CompanyName { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactNumber { get; set; }
        public string? EmailAddress { get; set; }
        public string? BusinessEmail { get; set; }
        public string? BusinessOwner { get; set; }        
        public string? BusinessContactNumber { get; set; }
        public string? EmpCode { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }
        public int? GenderId { get; set; }
        public string? BloodGroup { get; set; }
        public string? NationalId { get; set; }
        public string? NationalIdName { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? LoginPassword { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public int? BusinessType { get; set; }
        public string? GSTN { get; set; }
        public string? PaymentTerm { get; set; }
        public string? GSTNFile { get; set; }
        public string? Address { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public int? CityId { get; set; }
        public string? CityName { get; set; }
        public int? StateId { get; set; }
        public int? CountryId { get; set; }
        public string? PostalOrZipCode { get; set; }
        public int? UserType { get; set; }
        public int? DepartmentId { get; set; }
        public int? JobTitleId { get; set; }
        public string? Department { get; set; }
        public string? Designation { get; set; }
        public int? RoleId { get; set; }
        public int? MgrId { get; set; }
        public int? HODId { get; set; }
        public string? ManagerName { get; set; }
        public string? HODName { get; set; }
        public string? WorkLocation { get; set; }
        public DateTime? DOJ { get; set; }
        public DateTime? DOB { get; set; }
        public string? EmpTypeId { get; set; }
        public int? EmpStatus { get; set; }
        public string? EmpType { get; set; }
        public string? EmployeeStatus { get; set; }
        public int? Projects { get; set; }
        public string? BankName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public string? IFSCOrShiftCode { get; set; }
        public string? Description { get; set; }
        [MaxLength]
        public byte[]? ProfileImage { get; set; }    
        
        public string? ProfileName { get; set; }
        public string? ProfilePath { get; set; }
        public bool? IsActive { get; set; }
        public int? UnitId { get; set; }
        public bool? IsAdmin { get; set; }
        public string? PanCard { get; set; }
        public string? EmailDisplay { get; set; }

        public string? RoleName { get; set; }
        public string? CountryName { get; set; }
        public string? StateName { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public HttpResponseMessage? HttpMessage { get; set; }
        public string DisplayMessage { get; set; } = string.Empty;
         public List<UsersMasterDTO>? UserList { get; set; }
        public List<UserKeyValues>? UsersKey { get; set; }
        public List<AccessMasterDTO>? AccessList { get; set; }
        public List<CommentsDashboardDTO>? ActivityList { get; set; }
        public List<ProjectDashboardDTO>? ProjectDashboardList { get; set; }
        public List<DepartmentMasterDTO>? DepartmentList { get; set; }
        public List<JobTitleMasterDTO>? JobTitleList { get; set; }
        public List<RoleMasterDTO>? RoleList { get; set; }
        public int? HttpStatusCode { get; set; } = 200;
    }

    public class UserLogin
    {
        public string? EmailAddress { get; set; }
        public string? LoginPassword { get; set; }
    }
    public class UserChangePassword
    {
        public int? UserId { get; set; }
        public string? LoginPassword { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }
    public class ForgotPassword
    {
        public string? EmailAddress { get; set; }
        public string? LoginPassword { get; set; }
    }
    public class UserSessionModel
    {
        public int? UserId { get; set; }
        public string? FullName { get; set; }
        public int? UserType { get; set; }
        public bool? IsAdmin { get; set; }
        public int? UnitId { get; set; }
        public int? RoleId { get; set; }
        public int? DepartmentId { get; set; }
        public int? JobTitleId { get; set; }
        public string? ProfileName { get; set; }
        public string? EmailAddress { get; set; }
        // Add more as needed
    }
}
