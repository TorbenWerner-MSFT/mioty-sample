using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using KeyManagementFunction.Models;

namespace KeyManagementFunction
{
    public static class KeyManagementBlobTriggerFunction
    {
        [FunctionName("KeyManagementBlobTriggerFunction")]
        public static async Task Run([BlobTrigger("mioty-keys/{name}", Connection = "AzureWebJobsStorage")] CloudBlockBlob myBlob,
            string name, ILogger log, ExecutionContext context)
        {
            // Setup configuration
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            log.LogInformation($"New keys arrived in file {name}");

            // Get Keys from Blob File
            var blobContent = await myBlob.DownloadTextAsync();
            var keyFile = JsonConvert.DeserializeObject<KeyFile>(blobContent);

            // Setup Service Client for communication with IoTHub
            var serviceClient = ServiceClient.CreateFromConnectionString(config["IotHubConnectionString"]);

            log.LogInformation($"Sending new key to device {keyFile.TargetGatewayId}");

            // Create the json payload and the cloud-to-device message
            var model = new KeyUpdateMessage(keyFile.TargetDeviceId, keyFile.ShortId, keyFile.Key);
            var json = JsonConvert.SerializeObject(model);
            var message = new Message(Encoding.ASCII.GetBytes(json));
            message.Properties.Add("type", "key.mng");

            log.LogInformation($"Sending key: {keyFile.Key}");
            log.LogInformation($"{json}");

            // Send the message
            await serviceClient.SendAsync(keyFile.TargetGatewayId, message);

            log.LogInformation("All keys have been send, exiting");
        }
    }
}
