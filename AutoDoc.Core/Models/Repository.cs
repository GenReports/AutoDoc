namespace AutoDoc.Core.Models
{
    public class Repository
    {
        public Repository()
        {
            Path = string.Empty;
        }

        public Guid Id { get; set; } = Guid.NewGuid();

        public string Path { get; set; }
    }
}
