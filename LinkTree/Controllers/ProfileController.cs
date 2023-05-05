using Amazon.S3.Transfer;
using Amazon.S3;
using LinkTree.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Amazon;
using LinkTree.Data;
using System.Net;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Newtonsoft.Json.Linq;

namespace LinkTree.Controllers
{
	public class ProfileController : Controller
	{   //tanımlamaların yapılması
		private readonly UserManager<AppUser> _userManager;
		private readonly Context _db;

		//YAPICI METOT
		public ProfileController(UserManager<AppUser> userManager, Context db)
		{
			_userManager = userManager;
			_db = db;
		}
		//RESİM YÜKLEME 
		public void UploadFile(string path, string imageName)
		{
			//ACCESSKEY SECRETKEY VE BUCKETNAME ATAMALARI
			var accessKey = "your access key";
			var secretKey = "your secret key";
			RegionEndpoint bucketRegion = RegionEndpoint.USWest2;
			var bucketName = "your bucket name";
			//CLİENT OLUŞTURMA İŞLEMİ
			var s3Client = new AmazonS3Client(accessKey, secretKey, bucketRegion);
			var fileTransferUtility = new TransferUtility(s3Client);
			string filePath = path;
			try
			{
				//YÜKLEME İSTEĞİ YOLLAMA
				var fileTransferUtilityRequest = new TransferUtilityUploadRequest
				{
					BucketName = bucketName,
					FilePath = filePath,
					StorageClass = S3StorageClass.StandardInfrequentAccess,
					PartSize = 463,
					Key = imageName,
					CannedACL = S3CannedACL.PublicRead,
				};
				fileTransferUtility.UploadAsync(fileTransferUtilityRequest).GetAwaiter().GetResult();

			}
			catch (AmazonS3Exception amazonex)
			{

			}
		}
		//ANASAYFA
		[HttpGet]
		public async Task<IActionResult> Index()
		{

			//KULLANICI VERİLERİNİN ÇEKİLMESİ
			var values = await _userManager.FindByNameAsync(User.Identity.Name);
			UserEditViewModel userEditViewModel = new UserEditViewModel();
			userEditViewModel.Mail = values.Email;
			userEditViewModel.Username = values.UserName;
			if (values.UserAvatar == null)
			{
				ViewBag.avatar = "9f9b3be0-2fca-4c6c-9cc1-86ee3eb095b5.png";
			}
			else
			{
				ViewBag.avatar = values.UserAvatar;
			}
			return View(userEditViewModel);
		}

		//AYARLAR SAYFASI GET
		[HttpGet]
		public async Task<IActionResult> Settings()
		{
			//KULLANICI VERİLERİNİN ÇEKİLMESİ
			var values = await _userManager.FindByNameAsync(User.Identity.Name);
			UserEditViewModel userEditViewModel = new UserEditViewModel();
			userEditViewModel.Mail = values.Email;
			userEditViewModel.Username = values.UserName;
			userEditViewModel.UserBackgroundColor = values.UserBackgroundColor;
			if (values.UserAvatar == null)
			{
				ViewBag.avatar = "9f9b3be0-2fca-4c6c-9cc1-86ee3eb095b5.png";
			}
			else
			{
				ViewBag.avatar = values.UserAvatar;
			}
			return View(userEditViewModel);
		}
		//AYARLAR SAYFASI POST
		[HttpPost]
		public async Task<IActionResult> Settings(UserEditViewModel p)
		{
			//KULLANICI VERİLERİNİN ÇEKİLMESİ
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			//FOTOĞRAF YÜKLEME İŞLEMİNİN GERÇEKLEŞMESİ
			if (p.UserAvatar != null)
			{
				var extension = Path.GetExtension(p.UserAvatar.FileName);
				var newimagename = Guid.NewGuid() + extension;
				var location = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Image/", newimagename);
				var stream = new FileStream(location, FileMode.Create);
				p.UserAvatar.CopyTo(stream);
				user.UserAvatar = newimagename;
				stream.Dispose();
				UploadFile(location, newimagename);
			}
			user.UserName = p.Username;
			user.UserBackgroundColor = p.UserBackgroundColor;
			var result = await _userManager.UpdateAsync(user);
			if (result.Succeeded)
			{
				return RedirectToAction("Index", "Profile");
			}
			return View();
		}
		//LİNK EKLEME SAYFASI GET
		[HttpGet]
		public async Task<IActionResult> AddLink()
		{
			return View();
		}
		//LİNK EKLEME SAYFASI POST
		[HttpPost]
		public async Task<IActionResult> AddLink(Link p)
		{
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			p.AppUser = user;
			_db.Add(p);
			_db.SaveChanges();
			return RedirectToAction("Index", "Profile");
		}
		//PROFİL SAYFASI
		public async Task<IActionResult> SeeProfile()
		{
			//KULLANICI VERİLERİNİN ÇEKİLMESİ
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			IEnumerable<Link> Links = _db.Links.Where(x => x.AppUser == user).ToList();
			if (user.UserAvatar == null)
			{
				ViewBag.avatar = "9f9b3be0-2fca-4c6c-9cc1-86ee3eb095b5.png";
			}
			else
			{
				ViewBag.avatar = user.UserAvatar;
			}
			ViewBag.username = user.UserName;
			return View(Links);
		}
		//LİNK DÜZENLEME/LİSTELEME SAYFASI
		public async Task<IActionResult> EditLinks()
		{
			//KULLANICININ LİNKLERİNİN ÇEKİLMESİ
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			IEnumerable<Link> Links = _db.Links.Where(x => x.AppUser == user).ToList();
			if (user.UserAvatar == null)
			{
				ViewBag.avatar = "9f9b3be0-2fca-4c6c-9cc1-86ee3eb095b5.png";
			}
			else
			{
				ViewBag.avatar = user.UserAvatar;
			}
			ViewBag.username = user.UserName;
			return View(Links);
		}
		//LİNK SİLME İŞLEMİ
		public IActionResult DeleteLink(int id)
		{
			var link = _db.Links.Where(x => x.LinkID == id).FirstOrDefault();
			_db.Remove(link);
			_db.SaveChanges();
			return RedirectToAction("EditLinks", "Profile");
		}
		//LİNK GÜNCELLEME İŞLEMİ
		[HttpGet]
		public IActionResult UpdateLink(int id)
		{
			var link = _db.Links.Where(x => x.LinkID == id).FirstOrDefault();
			return View(link);
		}
		[HttpPost]
		public IActionResult UpdateLink(Link link)
		{
			_db.Update(link);
			_db.SaveChanges();
			return RedirectToAction("EditLinks", "Profile");
		}
	}
}
