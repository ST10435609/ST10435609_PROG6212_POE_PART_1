// start of CategoryResponse
namespace RaceDay.Api.Dtos
{
    public class CategoryResponse
    {
        public int CategoryId { get; set; }
        public int EventId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
// end of CategoryResponse