using LinkTree.Data;
using LinkTree.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon;
using Amazon.S3.Model;
using System.Configuration;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;

namespace LinkTree.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		private readonly Context _db;

        public HomeController(ILogger<HomeController> logger,Context db)
		{
			_db = db;
			_logger = logger;
		}
		
		public IActionResult Index()
		{
			
			return View();
		}


		public void UploadFile(string path,string imageName)
		{
			var accessKey = "AKIAWKRMXZOQZQX2LP4B";
			var secretKey = "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";
			RegionEndpoint bucketRegion = RegionEndpoint.USWest2;
			var bucketName = "link-tree-bucket";

			var s3Client = new AmazonS3Client(accessKey, secretKey, bucketRegion);
			var fileTransferUtility = new TransferUtility(s3Client);
			//string filePath = "C:\\Users\\kaan_\\Downloads\\hero_1.jpg";
			string filePath = path;
			try
			{
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
			catch(AmazonS3Exception amazonex) 
			{
				
			}
		}
		[HttpGet]
		public IActionResult Ekle()
		{
			return View();
		}
		[HttpPost]
		public IActionResult Ekle(ImageUpload p)
		{
			User u = new User();
			
			if (p.UserAvatar != null)
			{
				var extension=Path.GetExtension(p.UserAvatar.FileName);
				var newimagename = Guid.NewGuid() + extension;
				var location = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Image/", newimagename);
				var stream = new FileStream(location, FileMode.Create);
				p.UserAvatar.CopyTo(stream);
                u.UserAvatar =newimagename;
				stream.Dispose();
                UploadFile(location, newimagename);
            }
			u.UserName = p.UserName;
			u.UserMail = p.UserMail;
			_db.Add(u);
			_db.SaveChanges();
            
            return View();
		}


		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}