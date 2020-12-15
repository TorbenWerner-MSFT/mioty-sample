# MIOTY Azure Function for Key Management

This solution contains a sample Azure Function that reads a new key file from an Azure Blob Storage and sends the containing key via **Cloud-to-Device Messaging** over the IoT Hub to the Gateway device.

## Configuration

Configuration is done via AppSettings defined in the Function inside the Azure Portal. When using the "Deploy to Azure button" those will automatically be set.

- **AzureWebJobsStorage:** Connection string to the Azure Storage Account that hosts the Function and contains the container for the new key files. The function will scan the container called **mioty-keys** on this storage account for new files.
- **IotHubConnectionString:** Connection string of the IoT Hub that allows sending of Cloud-to-Device messages.

## Workflow

**Trigger:** Blob Storage Trigger

```csharp
public static async Task Run([BlobTrigger("mioty-keys/{name}", Connection = "AzureWebJobsStorage")] CloudBlockBlob myBlob, string name, ILogger log, ExecutionContext context)
```

**Deserialization:** Custom Model via Newtonsoft.Json

```csharp
var blobContent = await myBlob.DownloadTextAsync();
var keyFile = JsonConvert.DeserializeObject<KeyFile>(blobContent);
```

**Cloud-to-Device Message:** Setup via custom Model

```csharp
var model = new KeyUpdateMessage(keyFile.TargetDeviceId, keyFile.ShortId, keyFile.Key);

var json = JsonConvert.SerializeObject(model);

var message = new Message(Encoding.ASCII.GetBytes(json));
message.Properties.Add("type", "key.mng");
```

**Send Message:** Using IoT Hub SDK, Target Device Id is included in Key File provided from Azure Storage

```csharp
await serviceClient.SendAsync(keyFile.TargetGatewayId, message);
```