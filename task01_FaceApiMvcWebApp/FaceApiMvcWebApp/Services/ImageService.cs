using FaceApiMvcWebApp.Infrastructure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace FaceApiMvcWebApp.Services
{
    public class ImageService
    {
        private CloudBlobContainer cloudBlobContainer;
        public void CloudInit()
        {
            CloudStorageAccount cloudStorageAccount = ConnectionString.GetConnectionString();
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            cloudBlobContainer = cloudBlobClient.GetContainerReference(ConnectionString.GetCloudStorageBlobContainerName());

            if (cloudBlobContainer.CreateIfNotExists())
            {
                cloudBlobContainer.SetPermissions(
                    new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    }
                    );
            }
        }
        public async Task<string> UploadImageAsync(HttpPostedFileBase imageToUpload)
        {
            string imageFullPath = null;
            if (imageToUpload == null || imageToUpload.ContentLength == 0)
            {
                return null;
            }
            try
            {
                string imageName = Guid.NewGuid().ToString();
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(imageName);
                cloudBlockBlob.Properties.ContentType = imageToUpload.ContentType;
                await cloudBlockBlob.UploadFromStreamAsync(imageToUpload.InputStream);

                imageFullPath = cloudBlockBlob.Uri.ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return imageFullPath;
        }
    }
}