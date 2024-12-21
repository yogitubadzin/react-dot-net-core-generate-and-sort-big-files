namespace LargeFilesManager.BL.Models
{
    public record FileSortResponse
    {
        public string FileName { get; set; }

        public int Status { get; set; }

        public int SortTimeInSeconds { get; set; }
    }
}
