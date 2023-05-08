using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SQSTrigger;

public class Function
{
    public Function()
    {

    }

    public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
    {
        var batchId = Guid.NewGuid().ToString();

        var total = evnt.Records.Count;

        var current = 1;

        foreach (var message in evnt.Records)
        {
            context.Logger.LogInformation($"Batch({batchId}) {current} of {total} with {message.Body} received");

            await Task.Delay(Random.Shared.Next(1000, 2500));

            current++;
        }
    }
}