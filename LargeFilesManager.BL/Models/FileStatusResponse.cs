namespace LargeFilesManager.BL.Models
{
    public record FileStatusResponse
    {
        public string FileName { get; set; }

        public int Status { get; set; }
    }
}
