using System.ComponentModel.DataAnnotations;

namespace LinkTree.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }
        public string UserName { get; set; }

        public string UserMail { get; set; }

        public string UserAvatar { get; set; }

    }
}
