# Define settings
$resourceGroupName = "[your-resource-group-name]"
$iotHubName = "[your-iot-hub-name]"
$deviceId = "[your-mioty-gateway-device-id]"
$functionappname = "[your-key-management-function-name]"

# Log into Azure account.
Login-AzAccount

# Add Device
Add-AzIotHubDevice -ResourceGroupName $resourceGroupName -IotHubName $iotHubName -DeviceId $deviceId -AuthMethod "shared_private_key"

# Get device connection string
$deviceConnectionString = (Get-AzIotHubDCS -ResourceGroupName $resourceGroupName -IotHubName $iotHubName -DeviceId $deviceId -KeyType primary).ConnectionString

# Get IoTHub connection string
$iotHubConnectionString = (Get-AzIotHubConnectionString -ResourceGroupName $resourceGroupName -Name $iotHubName -KeyName "iothubowner").PrimaryConnectionString

# Output connection strings to user
Write-Output ""
Write-Output "Listing iot hub connection string"
Write-Output "======================================"
Write-Output $iotHubConnectionString

Write-Output ""
Write-Output "Listing device connection string"
Write-Output "======================================"
Write-Output $deviceConnectionString

# Publish Azure function
Publish-AzWebapp -ResourceGroupName $resourceGroupName -Name $functionappname -ArchivePath "keyManagementFunction.zip"