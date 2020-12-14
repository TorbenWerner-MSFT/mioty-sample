# MIOTY Azure Reference Architecture

This documentation is work in progress...

![azure-mioty-reference](azure-mioty-reference.png)

## Deploy this solution to Azure

This button will deploy the following resources to your Azure Subscription

* Azure IoT Hub
* Azure Stream Analytics Job for message deduplication
* Azure Event Hub Namespace and Event Hub as output for the Stream Analytics Job
* Azure Function and Consumption App Service Plan for the Key Management
* Azure Storage hosting the Azure Function and source for new MIOTY key files

[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FTorbenWerner-MSFT%2Fmioty-sample%2Fdevelop%2Fazuredeploy.json)