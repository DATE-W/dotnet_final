using SqlSugar;

namespace DBwebAPI.Models
{
    public class Highlight
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int highlight_id {get; set;}
        public string? photo { get;set;}
        public string? videoUrl { get;set;}
    }
}
