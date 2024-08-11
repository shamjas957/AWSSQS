
using Amazon.SQS;
using Amazon.SQS.Model;
using Customers.Consumer.Messages;
using MediatR;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Customers.Consumer
{
    public class QueueConsumerService : BackgroundService
    {
        private readonly IAmazonSQS _sqs;
        private readonly IOptions<QueueSettings> _queueSettings;
        private readonly IMediator _mediator;
        private readonly ILogger<QueueConsumerService> _logger;

        public QueueConsumerService(IAmazonSQS sqs, IOptions<QueueSettings> queueSettings, IMediator mediator, ILogger<QueueConsumerService> logger)
        {
            _sqs = sqs;
            _queueSettings = queueSettings;
            _mediator = mediator;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var queueUrlResponse = await _sqs.GetQueueUrlAsync(_queueSettings.Value.Name, stoppingToken);

            var recieveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = queueUrlResponse.QueueUrl,
                AttributeNames = ["All"],
                MessageAttributeNames = ["All"],
                MaxNumberOfMessages = 1
            };
            while (!stoppingToken.IsCancellationRequested)
            {
                var response = await _sqs.ReceiveMessageAsync(recieveMessageRequest, stoppingToken);
                foreach (var message in response.Messages)
                {
                    var messageType = message.MessageAttributes["MessageType"].StringValue;
                    var type = Type.GetType($"Customers.Consumer.Messages.{messageType}");
                    if(type is null)
                    {
                        _logger.LogWarning($"Unknown message type: {messageType}");
                    }
                    var typedMessage = (ISqsMessage)JsonSerializer.Deserialize(message.Body, type)!;
                    try
                    {
                        await _mediator.Send(typedMessage,stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "message failed during processing");
                        continue;
                    }
                    await _sqs.DeleteMessageAsync(queueUrlResponse.QueueUrl, message.ReceiptHandle, stoppingToken);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
