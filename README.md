# Sending order receipt file to user through mail using Azure Functions once the payment done

<b>Key Features:<b/>

Azure HttpTrigger, QueueTrigger, BlobTrigger, TableStorage, SendGrid

<b>Requirement:<b/>

Visual Studio 2017+, .net core 2.1, Azure access

<b>local.settings.json file changes:<b/>
 
 1. Add Azure Storage connection string in "AzureWebJobsStorage" section.
 2. Add Send Grid Api Key in "SendGridApiKey" section.
 3. Add From Email Id in "EmailSender" section.
 
 ```
 {
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "EmailSender": "FromEmailId",
    "SendGridApiKey": "YourSendGridApiKey"
  }
}
 ```

![header image](https://github.com/iamchandanys/azure_function_app/blob/master/Images/AzureFunctionDemo.png)

<b>Steps:<b/>

Step A - Receives Order Details like orderid, emailid, orderamount etc,. through HTTP trigger.

1. HTTP Triggers once the payment is done.
2. Reads the request body and save the order data in Table Storage.
3. Send a Queue Message to generate Receipt File.

Step 2 - Receives Queue message and generates the Receipt File.

4. Queue triggers once it receives the message.
5. Generates Receipt File with the name orderid and store it in Blob Storage.

Step C - Sends Receipt File to the user.

6. Blob triggers when any new file is saved in it.
7. It reads the data from Table Storage by file name (orderid) & then sends Receipt file to the user through mail using Send Grid.
