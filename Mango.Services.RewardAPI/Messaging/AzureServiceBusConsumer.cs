using Azure.Messaging.ServiceBus;
using Mango.Services.RewardAPI.Message;
using Mango.Services.RewardAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.RewardAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string orderCreatedTopic;
        private readonly string orderCreatedRewardSubscription;
        private readonly IConfiguration _configuration;
        private readonly RewardService _rewardService;

        private ServiceBusProcessor _rewardProcessor;

        public AzureServiceBusConsumer(IConfiguration configuration, RewardService rewardService)
        {
            _configuration = configuration;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            orderCreatedTopic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            orderCreatedRewardSubscription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_RewardsSubscription");

            var client = new ServiceBusClient(serviceBusConnectionString);
            // If we want to listen to any queue or topic, we need a processor (ServiceBusProcessor)
            _rewardProcessor = client.CreateProcessor(orderCreatedTopic, orderCreatedRewardSubscription);

            _rewardService = rewardService;
        }

        public async Task Start()
        {
            _rewardProcessor.ProcessMessageAsync += OnNewOrderRewardsRequestReceived;
            _rewardProcessor.ProcessErrorAsync += ErrorHandler;
            // Signals the processor to begin processing message
            // If it's already running and is called again, an InvalidOperationException
            // is thrown
            await _rewardProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            // Signals processor to stop processing message. If called and processor isn't running, nothing happens
            // Doesn't close the underlying 'receivers', but will cause them to stop receiving. This will not return
            // until any inflight messages have been received
            await _rewardProcessor.StopProcessingAsync();
            // Cleans up resources used by the processor
            // Is equivalent to CloseAsync
            await _rewardProcessor.DisposeAsync();
        }

        private async Task OnNewOrderRewardsRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            RewardsMessage objMessage = JsonConvert.DeserializeObject<RewardsMessage>(body);

            try
            {
                // TODO - try to log email
                await _rewardService.UpdateRewards(objMessage);
                await args.CompleteMessageAsync(args.Message);

            } catch (Exception ex)
            {
                throw;
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

    }
}
