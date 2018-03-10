using FaceApiMvcWebApp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FaceApiMvcWebApp.Controllers
{
    public class HomeController : Controller
    {
        ImageService imageService = new ImageService();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult UploadFiles()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> UploadFiles(HttpPostedFileBase[] files)
        {
            //Ensure model state is valid  
            if (ModelState.IsValid)
            {
                var start = DateTime.Now;
                var imageUrl = string.Empty;
                imageService.CloudInit();
                //iterating through multiple file collection   
                foreach (HttpPostedFileBase file in files)                
                {
                    //Checking file is available to save.  
                    if (file != null)
                        imageUrl = await imageService.UploadImageAsync(file);                        
                };
                TempData["LatestImage"] = imageUrl;                
                TempData["Duration"] = DateTime.Now.Subtract(start).Milliseconds;
            }            
            return await Task.Run<ActionResult>(() =>
            {
                return RedirectToAction("LatestImage");
            });
        }

        public ActionResult LatestImage()
        {
            var latestImage = string.Empty;
            if (TempData["LatestImage"] != null)
            {
                ViewBag.LatestImage = Convert.ToString(TempData["LatestImage"]);
                if (TempData["Duration"] != null)
                    ViewBag.Duration = Convert.ToInt32(TempData["Duration"]);
            }

            return View();
        }
    }
}