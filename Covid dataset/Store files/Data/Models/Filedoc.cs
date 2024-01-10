namespace Store_files.Data.Models
{
    public class Filedoc
    {
        public int Id { get; set; }
        public DateTime UploadedDateTime { get; set; }
        public required byte[] FileBytes { get; set; }
    }
}
