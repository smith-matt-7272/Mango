using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Models.Dto;
using Mango.Services.EmailAPI.Services;
using Mango.Services.EmailAPI.Services.IServices;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly string registeredUserQueue;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        private ServiceBusProcessor _emailCartProcessor;
        private ServiceBusProcessor _registeredUserQueueProcessor;

        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
            registeredUserQueue = _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue");

            var client = new ServiceBusClient(serviceBusConnectionString);
            // If we want to listen to any queue or topic, we need a processor (ServiceBusProcessor)
            _emailCartProcessor = client.CreateProcessor(emailCartQueue);
            _registeredUserQueueProcessor = client.CreateProcessor(registeredUserQueue);

            _emailService = emailService;
        }

        public async Task Start()
        {
            _emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
            _emailCartProcessor.ProcessErrorAsync += ErrorHandler;
            // Signals the processor to begin processing message
            // If it's already running and is called again, an InvalidOperationException
            // is thrown
            await _emailCartProcessor.StartProcessingAsync();

            _registeredUserQueueProcessor.ProcessMessageAsync += OnRegisteredUserRequestReceived;
            _registeredUserQueueProcessor.ProcessErrorAsync += ErrorHandler;

            await _registeredUserQueueProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            // Signals processor to stop processing message. If called and processor isn't running, nothing happens
            // Doesn't close the underlying 'receivers', but will cause them to stop receiving. This will not return
            // until any inflight messages have been received
            await _emailCartProcessor.StopProcessingAsync();
            // Cleans up resources used by the processor
            // Is equivalent to CloseAsync
            await _emailCartProcessor.DisposeAsync();

            await _registeredUserQueueProcessor.StopProcessingAsync();
            await _registeredUserQueueProcessor.DisposeAsync();
        }

        private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            CartDto objMessage = JsonConvert.DeserializeObject<CartDto>(body);

            try
            {
                // TODO - try to log email
                await _emailService.EmailCartAndLog(objMessage);
                await args.CompleteMessageAsync(args.Message);

            } catch (Exception ex)
            {
                throw;
            }
        }

        private async Task OnRegisteredUserRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            try
            {
                // TODO - try to log email
                await _emailService.RegisterUserAndLog(body);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
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
