namespace LinkTree.Models
{
    public class UserEditViewModel
    {

        public string Username { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public IFormFile UserAvatar { get; set; }

        public string? UserBackgroundColor { get; set; }

        public string Mail { get; set; }
    }
}
