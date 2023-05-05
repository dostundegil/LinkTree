using System.ComponentModel.DataAnnotations;

namespace LinkTree.Models
{
	public class UserRegisterViewModel
	{
		[Required(ErrorMessage ="Lütfen Kullanıcı Adını Boş Bırakmayınız")]
		public string UserName { get; set; }

		[Required(ErrorMessage = "Lütfen Mail Adresini Boş Bırakmayınız")]
		public string Mail { get; set; }

		[Required(ErrorMessage = "Lütfen Şifreyi Boş Bırakmayınız")]

		public string Password { get; set; }

		[Required(ErrorMessage = "Lütfen Şifreyi Tekrar Girin")]

		[Compare("Password",ErrorMessage ="Şifrelir Aynı Olmalıdır")]

		public string ConfirmPassword { get; set; }
	}
}
