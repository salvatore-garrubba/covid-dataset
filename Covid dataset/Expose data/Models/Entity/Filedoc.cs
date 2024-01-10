namespace Expose_data.Models.Entity
{
    public class Filedoc
    {
        public int Id { get; set; }
        public DateTime UploadedDateTime { get; set; }
        public required byte[] FileBytes { get; set; }
    }
}
