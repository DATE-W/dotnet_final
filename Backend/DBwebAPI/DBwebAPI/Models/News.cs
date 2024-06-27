namespace DBwebAPI.Models
{
    public class News
    {
        public int news_id { get; set; }
        public string title { get; set; }
        public DateTime publishDateTime { get; set; }
        public string summary { get; set; }
        public string contains { get; set; }
        public string matchTag { get; set; }
        public string propertyTag { get; set; }
        //public List<string>? pictureRoutes { get; set; }
    }
    public class NewsWithIndex
    {
        public int news_id { get; set; }
        public string title { get; set; }
        public DateTime publishDateTime { get; set; }
        public string summary { get; set; }
        public string contains { get; set; }
        public string matchTag { get; set; }
        public string propertyTag { get; set; }
        public int index { get; set; }
        //public List<string>? pictureRoutes { get; set; }
    }
}
