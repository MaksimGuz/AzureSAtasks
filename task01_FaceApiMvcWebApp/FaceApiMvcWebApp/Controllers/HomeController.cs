using FaceApiMvcWebApp.Infrastructure;
using FaceApiMvcWebApp.Services;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FaceApiMvcWebApp.Controllers
{
    public class HomeController : Controller
    {
        ImageService imageService = new ImageService();
        string ServiceBusConnectionString = ConnectionString.GetCloudServiceBusConnectionString();
        string QueueName = ConnectionString.GetCloudServiceBusQueueName();

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
                var imageName = string.Empty;
                imageService.CloudInit();                
                // Create MessagingFactory object
                var messagingFactory = MessagingFactory.CreateFromConnectionString(ServiceBusConnectionString);
                // Create MessageSender object
                var messageSender = await messagingFactory.CreateMessageSenderAsync(QueueName);
                //iterating through multiple file collection   
                foreach (HttpPostedFileBase file in files)                
                {
                    //Checking file is available to save.  
                    if (file != null)
                    {
                        imageName = await imageService.UploadImageAsync(file);
                        // Create message payload
                        var faceServiceBusMessage = new FaceServiceBusMessage
                        {
                            DeviceId = "0x1111",
                            BlobName = imageName
                        };
                        // Create BrokeredMessage object
                        using (var brokeredMessage = new BrokeredMessage(faceServiceBusMessage, new DataContractSerializer(typeof(FaceServiceBusMessage))))
                        {
                            //Send message
                            messageSender.SendAsync(brokeredMessage).Wait();
                        }
                    }
                };                
                TempData["LatestImage"] = imageName;
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

    [DataContract]
    public class FaceServiceBusMessage
    {
        [DataMember]
        public string DeviceId { get; set; }

        [DataMember]
        public string BlobName { get; set; }
    }
}