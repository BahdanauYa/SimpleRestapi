namespace SimpleRestapi.Models
{
    public class Todo
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public int PercentComplete { get; set; }
        public bool IsDone { get; set; }
    }
}
