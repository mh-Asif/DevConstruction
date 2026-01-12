

namespace Construction.Infrastructure.Models
{
    public class ItemsDTO
    {
        public int ItemId { get; set; }
        public string? Item_Name { get; set; }
        public string? Item_Unit { get; set; }
        public int? Item_Reorder_Level { get; set; }
        public DateTime? Item_Created_At { get; set; }
        public bool? IsActive { get; set; } = true;
        public HttpResponseMessage? HttpMessage { get; set; }
        public string DisplayMessage { get; set; } = string.Empty;
        public int? HttpStatusCode { get; set; } = 200;
        public List<ItemsDTO>? ItemsList { get; set; }
    }
}
