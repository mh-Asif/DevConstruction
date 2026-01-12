
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace ConstructionApp.Core.Entities
{
    [Table("Items")]
    public partial class Items
    {
       [Key]
        public int ItemId { get; set; }
        public string? Item_Name { get; set; }
        public string? Item_Unit { get; set; }
        public int? Item_Reorder_Level { get; set; }       
        public DateTime? Item_Created_At { get; set; }
        public bool? IsActive { get; set; }
       
     
    }
}
