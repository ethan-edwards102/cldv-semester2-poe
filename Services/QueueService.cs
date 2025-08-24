using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace ABCRetails.Services;

public class QueueService
{
    private readonly QueueServiceClient _qs;

    public QueueService(IConfiguration configuration)
    {
        var connectionString = configuration["ConnectionStrings:StorageAccount"];
        
        _qs = new QueueServiceClient(connectionString);
    }

    public async Task PushMessageAsync(string queueName, string message)
    {
        var queueClient = _qs.GetQueueClient(queueName);
        await queueClient.CreateIfNotExistsAsync();
        await queueClient.SendMessageAsync(message);
    }
    
    public async Task<List<string>> PeekMessagesAsync(string queueName, int max)
    {
        var queueClient = _qs.GetQueueClient(queueName);
        await queueClient.CreateIfNotExistsAsync();

        PeekedMessage[] peekedMessages = await queueClient.PeekMessagesAsync(max);
        
        var messages = new List<string>();
        foreach (var msg in peekedMessages)
        {
            messages.Add(msg.MessageText);
        }
        
        // Newest messages should appear on top
        messages.Reverse();
        
        return messages;
    }

    public async Task PopMessageAsync(string queueName)
    {
        try
        {
            var queueClient = _qs.GetQueueClient(queueName);

            // Pop the message from the front of the queue
            QueueMessage msg = await queueClient.ReceiveMessageAsync();

            await queueClient.DeleteMessageAsync(msg.MessageId, msg.PopReceipt);
        }
        catch (Exception ex)
        {
            Console.WriteLine("User attempted to clear empty or non-existent queue");
        }
    }
}