using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;

namespace FaceApiMvcWebApp.Infrastructure
{
    public static class ConnectionString
    {
        //static string account = CloudConfigurationManager.GetSetting("StorageAccountName");
        //static string key = CloudConfigurationManager.GetSetting("StorageAccountKey");
        static string contianer = CloudConfigurationManager.GetSetting("StorageAccountBlobContainerName");
        public static CloudStorageAccount GetConnectionString()
        {
            //string connectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", account, key);
            return CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
        }
        public static string GetCloudStorageBlobContainerName()
        {
            return contianer;
        }
        public static string GetCloudServiceBusConnectionString()
        {
            return CloudConfigurationManager.GetSetting("ServiceBusConnectionString");
        }
        public static string GetCloudServiceBusQueueName()
        {
            return CloudConfigurationManager.GetSetting("ServiceBusQueueName");
        }
    }
}