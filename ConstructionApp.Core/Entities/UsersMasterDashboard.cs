using ConstructionApp.Core.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionApp.Core.Entities
{
   // [Table("UsersMaster")]
    public partial class UsersMasterDashboard
    {
      //  [Key]
        public int UserId { get; set; }
        public string? CompanyName { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactNumber { get; set; }
        public string? EmailAddress { get; set; }
        public string? BusinessEmail { get; set; }
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
        public int? BusinessType { get; set; }
        public string? GSTN { get; set; }
        public string? PaymentTerm { get; set; }
        public string? GSTNFile { get; set; }
        public string? Address { get; set; }
        public int? CityId { get; set; }
        public string? CityName { get; set; }
        public int? StateId { get; set; }
        public int? StateName { get; set; }
        public int? CountryId { get; set; }
        public int? CountryName { get; set; }
        public string? PostalOrZipCode { get; set; }
        public int? UserType { get; set; }
        public int? DepartmentId { get; set; }
        public int? JobTitleId { get; set; }
        public int? RoleId { get; set; }
        public int? MgrId { get; set; }
        public int? HODId { get; set; }
        public string? WorkLocation { get; set; }
        public DateTime? DOJ { get; set; }
        public DateTime? DOB { get; set; }
        public string? EmpTypeId { get; set; }
        public int? EmpStatus { get; set; }
        public string? BankName { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public string? IFSCOrShiftCode { get; set; }
        public string? Description { get; set; }
        public string? ProfileName { get; set; }
        //[MaxLength]
        //public byte[]? ProfileImage { get; set; }
        public bool? IsActive { get; set; }
        public int? UnitId { get; set; }
        public bool? IsAdmin { get; set; }
        public string? PanCard { get; set; }
        public string? RoleName { get; set; }       
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        
        //[NavigationProperty]
        //public virtual DepartmentMaster Department { get; set; } = null!;
      
        //[NavigationProperty]
        //public virtual JobTitleMaster JobTitle { get; set; } = null!;

      
    }
}
