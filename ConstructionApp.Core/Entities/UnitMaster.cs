using ConstructionApp.Core.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionApp.Core.Entities
{
    [Table("UnitMaster")]
    public partial class UnitMaster
    {

        [Key]
        public int UnitID { get; set; }

        public int ClientId { get; set; }

        public string? UnitName { get; set; }
        public string? EmailDisplayName { get; set; }

        public string? GSTN { get; set; }
        public string? TIN { get; set; }
        public string? PanCard { get; set; }

        public string? EmailId { get; set; }

        public long? ContactNumber { get; set; }

        public string? ContactPerson { get; set; }

        public string? Address { get; set; }

        public int? CountryId { get; set; }

        public int? StateId { get; set; }

        public int? CityId { get; set; }

        public long? Pincode { get; set; }
        public int? NoticePeriod { get; set; }
        public int? ConfirmationPeriod { get; set; }

        public string? WeeklyOff { get; set; }

        public int? PayrollStartDate { get; set; }

        public int? PayrollEndDate { get; set; }
        public bool? IsActive { get; set; }
        public int? IsBlock { get; set; }
        [MaxLength]
        public byte[]? ProfileImage { get; set; }
        public bool? AttendaceSandwichRule { get; set; }
        public bool? IsUnitLogo { get; set; }
        public int ClientUnitId { get; set; }
        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

      //  public virtual ICollection<ClientModuleMapping> ClientModuleMappings { get; set; } = new List<ClientModuleMapping>();

        [NavigationProperty]
        public virtual CountryMaster Country { get; set; } = null!;


        [NavigationProperty]
        public virtual StateMaster State { get; set; } = null!;

        [NavigationProperty]
        public virtual CityMaster City { get; set; } = null!;

        [NavigationProperty]
        public virtual Client Client { get; set; } = null!;

        //public virtual ICollection<EmployeePersonalDetail> EmployeePersonalDetails { get; set; } = new List<EmployeePersonalDetail>();
    }

}
