using ConstructionApp.Core.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionApp.Core.Entities
{
    public partial class Client
    {

        [Key]
        public int ClientId { get; set; }

        public string? ClientName { get; set; }

        public string? CompanyName { get; set; }

        public string? GSTN { get; set; }

        public string? EmailId { get; set; }

        public long? ContactNumber { get; set; }

        public string? Address { get; set; }

        public int? CountryId { get; set; }

        public int? StateId { get; set; }

        public int? CityId { get; set; }

        public long? Pincode { get; set; }

        public string? ClientLogo { get; set; }

        public string? HeaderText { get; set; }

        public string? FooterText { get; set; }

        public string? SupportLink { get; set; }

        public string? PoliciesLink { get; set; }

        public string? DocumentLink { get; set; }

        public string? MenuStyle { get; set; }

        public string? ColorTheme { get; set; }

        public bool? IsActive { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

       // public virtual ICollection<ClientModuleMapping> ClientModuleMappings { get; set; } = new List<ClientModuleMapping>();

        [NavigationProperty]
        public virtual CountryMaster Country { get; set; } = null!;


        [NavigationProperty]
        public virtual StateMaster State { get; set; } = null!;

        [NavigationProperty]
        public virtual CityMaster City { get; set; } = null!;

        //public virtual ICollection<EmployeePersonalDetail> EmployeePersonalDetails { get; set; } = new List<EmployeePersonalDetail>();
    }
}
