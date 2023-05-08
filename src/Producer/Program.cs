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

for (int i = 0; i < 100; i++)
{
    var messageGroupId = Random.Shared.Next(0, 5).ToString();

    var body = $"@@{messageGroupId}@@{Guid.NewGuid()}";

    var request = new SendMessageRequest(url, body)
    {
        MessageGroupId = messageGroupId
    };

    await sqsClient.SendMessageAsync(request);

    Console.WriteLine($"{body} sent");
}