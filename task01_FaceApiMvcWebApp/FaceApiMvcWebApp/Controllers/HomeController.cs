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
            {   //iterating through multiple file collection   
                foreach (HttpPostedFileBase file in files)
                //Parallel.ForEach(files, (file) =>
                {
                    //Checking file is available to save.  
                    if (file != null)
                    {
                        /*
                        var InputFileName = Path.GetFileName(file.FileName);
                        var ServerSavePath = Path.Combine(Server.MapPath("~/UploadedFiles/") + InputFileName);
                        //Save file to server folder  
                        file.SaveAs(ServerSavePath);
                        //assigning file uploaded status to ViewBag for showing message to user.  
                        ViewBag.UploadStatus = files.Count().ToString() + " files uploaded successfully.";
                        */
                        var imageUrl = await imageService.UploadImageAsync(file);
                        TempData["LatestImage"] = imageUrl.ToString();
                    }
                };//);
            }
            //return RedirectToAction("LatestImage");
            return await Task.Run<ActionResult>(() =>
            {
                return RedirectToAction("LatestImage");
            });
        }


        /*
var imageUrl= await imageService.UploadImageAsync(photo);  
            TempData["LatestImage"] = imageUrl.ToString();  
            return RedirectToAction("LatestImage");         
        */

        public ActionResult LatestImage()
        {
            var latestImage = string.Empty;
            if (TempData["LatestImage"] != null)
            {
                ViewBag.LatestImage = Convert.ToString(TempData["LatestImage"]);
            }

            return View();
        }
    }
}