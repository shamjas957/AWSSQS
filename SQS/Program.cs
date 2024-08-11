using Amazon.SQS;
using Amazon.SQS.Model;
using SQS;
using System.Text.Json;

var customerCreated = new CustomerCreated
{
    ID = Guid.NewGuid(),
    FullName = "Shamjas P V",
    Age = 32,
    UserName = "Shamjas",
    DOB = new DateTime(1990, 1, 1)
};

var sqsClient = new AmazonSQSClient("AccessKey", "SecretKey");

var queueUrlResponse = await sqsClient.GetQueueUrlAsync("customers");

var sendMessageRequest = new SendMessageRequest
{
    QueueUrl = queueUrlResponse.QueueUrl,
    MessageBody = JsonSerializer.Serialize(customerCreated),
    MessageAttributes = new Dictionary<string, MessageAttributeValue>
    {
        {
        "MessageType", new MessageAttributeValue
            {
                DataType="String",
                StringValue= nameof(CustomerCreated)
            }
        }
    }
};
var response = await sqsClient.SendMessageAsync(sendMessageRequest);

Console.ReadLine();