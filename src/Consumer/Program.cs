// See https://aka.ms/new-console-template for more information
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configurationBuilder = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json");

var options = configurationBuilder.Build().GetAWSOptions();

var services = new ServiceCollection()
    .AddDefaultAWSOptions(options)
    .AddAWSService<IAmazonSQS>();

var provider = services.BuildServiceProvider();

var url = "<MY_QUEUE_URL>";

var sqsClient = provider.GetService<IAmazonSQS>()!;

while (true)
{
    var receiveRequest = new ReceiveMessageRequest
    {
        QueueUrl = url,
        MaxNumberOfMessages = 10, 
        WaitTimeSeconds = 5
    };

    var result = await sqsClient.ReceiveMessageAsync(receiveRequest);

    if (result.Messages.Any())
    {
        var batchId = Guid.NewGuid().ToString();

        var total = result.Messages.Count;

        var current = 1;

        var batch = new List<DeleteMessageBatchRequestEntry>();

        foreach (var message in result.Messages)
        {
            Console.WriteLine($"Batch({batchId}) {current} of {total} with {message.Body} received");
            current++;
            batch.Add(new DeleteMessageBatchRequestEntry() { ReceiptHandle = message.ReceiptHandle, Id=message.MessageId });
            await Task.Delay(Random.Shared.Next(1000, 2500));
        }

        await sqsClient.DeleteMessageBatchAsync(url, batch);
    }
    else
    {
        Console.WriteLine("No messages available");

        await Task.Delay(TimeSpan.FromSeconds(5));
    }
}