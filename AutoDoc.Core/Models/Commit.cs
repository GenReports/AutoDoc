namespace AutoDoc.Core.Models
{
    public class Commit
    {
        public Commit()
        {
            Message = string.Empty;
            Tittle = string.Empty;
        }

        public string Message { get; set; }

        public string Tittle { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
