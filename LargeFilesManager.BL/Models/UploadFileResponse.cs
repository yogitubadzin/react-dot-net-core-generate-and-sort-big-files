namespace LargeFilesManager.BL.Models
{
    public record UploadFileResponse
    {
        public string FileName { get; set; }

        public bool Uploaded { get; set; }
    }
}
