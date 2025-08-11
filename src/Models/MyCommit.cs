namespace AutoDoc.Models
{
    internal class MyCommit
    {
        public MyCommit()
        {
            Message = string.Empty;
            Tittle = string.Empty;
        }

        public string Message { get; set; }
    
        public string Tittle { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
