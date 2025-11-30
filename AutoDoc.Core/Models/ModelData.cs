namespace AutoDoc.Core.Models
{
    public class ModelData
    {
        public ModelData()
        {
            Context = string.Empty;
            TemplateMessage = string.Empty;
        }

        public string Context { get; set; }

        public string TemplateMessage { get; set; }
    }
}
