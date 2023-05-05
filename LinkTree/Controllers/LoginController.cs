using LinkTree.Data;
using LinkTree.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace LinkTree.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        //Gerekli Tanımlamaların Yapılması
        private readonly Context _db;

        private readonly UserManager<AppUser> _userManager;

        private readonly SignInManager<AppUser> _signInManager;

        private readonly IEmailSender _emailSender;

        //Yapıcı Metot
        public LoginController(Context db, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailSender emailSender)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [AllowAnonymous]
        [HttpGet]

        //Kayıt Sayfası GET
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]

		//Kayıt Sayfası POST
		public async Task<IActionResult> SignUp(UserRegisterViewModel p)
        {
            //Identity Kütüphanesindeki AppUser sınıfından bir obje üretiyoruz
            AppUser appUser = new AppUser()
            {
                UserName = p.UserName,
                Email = p.Mail,
            };
            //Şifre Eşleşme Kontrolü
            if (p.Password == p.ConfirmPassword)
            {
                var result = await _userManager.CreateAsync(appUser, p.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("SignIn", "Login");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            return View(p);
        }
        //Giriş Yapma GET
        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }
		//Giriş Yapma POST
		[HttpPost]
        public async Task<IActionResult> SignIn(UserSignInModel p)
        {   
            if (ModelState.IsValid)
            {   //Kullanıcı Adı ve Şifre Doğrulaması
                var results = await _signInManager.PasswordSignInAsync(p.username, p.password, false, true);
                if (results.Succeeded)
                {   
                    return RedirectToAction("SeeProfile", "Profile");
                }
                else
                {
                    return RedirectToAction("SignIn", "Login");
                }
            }
            return View();
        }
        //Şifremi Unuttum GET
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
		//Şifremi Unuttum POST
		[HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel p)
        {
            //Mail Adresine göre user bulma
            var user = await _userManager.FindByEmailAsync(p.Mail);
			//bulunan user'a şifre değiştirme token üretimi
			string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
			//bulunan user'a şifre değiştirme token url üretimi
			var passwordResetTokenLink = Url.Action("ResetPassword", "Login", new
            {
                userId = user.Id,
                token = passwordResetToken
            }, HttpContext.Request.Scheme);
            //mail gönderme işlemi
            _emailSender.SendEmailAsync(user.Email, "Şifre Değişikliği", passwordResetTokenLink);
            return View();
        }
        //şifre değiştirme GET
        [HttpGet]
        public IActionResult ResetPassword(string userid,string token)
        {
            TempData["userid"] = userid;
            TempData["token"] = token;
            return View();
        }
		//şifre değiştirme POST
		[HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel p)
        {   //user id ve tokenin doğrulanması
            var userid = TempData["userid"];
            var token = TempData["token"];
            if(userid==null || token == null)
            {
                //hata mesajı
            }
            //şifre değiştirme işlemlerinin yapılması
            var user = await _userManager.FindByIdAsync(userid.ToString());
            var result = await _userManager.ResetPasswordAsync(user, token.ToString(), p.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("SignIn", "Login");
            }
            return View();
        }
    }
}
