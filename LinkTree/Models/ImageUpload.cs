namespace LinkTree.Models
{
    public class ImageUpload
    {
        public int UserID { get; set; }
        public string UserName { get; set; }

        public string UserMail { get; set; }

        public IFormFile UserAvatar { get; set; }
    }
}
