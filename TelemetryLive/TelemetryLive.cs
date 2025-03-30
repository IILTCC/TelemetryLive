using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using MongoConsumerLibary.KafkaConsumer;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TelemetryLive
{
    public class TelemetryLive : IHostedService
    {
        private readonly KafkaSettings _kafkaSettings;
        private readonly KafkaConnection _kafkaConnection;
        IConsumer<Ignore, string> consumer;
        public TelemetryLive(KafkaConnection kafkaConnection, KafkaSettings kafkaSettings)
        {
            _kafkaConnection = kafkaConnection;
            _kafkaSettings = kafkaSettings;
        }
        public List<string> InitializeTopicNames()
        {
            List<string> topicNames = new List<string>();
            foreach (string topic in _kafkaSettings.KafkaTopics)
                topicNames.Add(topic);
            return topicNames;
        }
        public async Task StartConsumer(CancellationToken cancellationToken)
        {
            _kafkaConnection.WaitForKafkaConnection();
            consumer = _kafkaConnection.Consumer(InitializeTopicNames());
            CancellationToken kafkaCancel = _kafkaConnection.CancellationToken(consumer);
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    ConsumeResult<Ignore, string> consumerResult = consumer.Consume(kafkaCancel);
                }
                catch (KafkaException e)
                {
                }
                catch (Exception e)
                {
                }
            }
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() => StartConsumer(cancellationToken), cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            consumer.Close();
            return Task.CompletedTask;
        }

    }
}
