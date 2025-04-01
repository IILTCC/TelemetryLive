using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using MongoConsumerLibary.KafkaConsumer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TelemetryLive.Dto;
using TelemetryLive.SignalR;

namespace TelemetryLive
{
    public class TelemetryLive : IHostedService
    {
        private readonly KafkaSettings _kafkaSettings;
        private readonly KafkaConnection _kafkaConnection;
        private readonly WebSocketService _webSocketService;
        IConsumer<Ignore, string> consumer;
        public TelemetryLive(KafkaConnection kafkaConnection, KafkaSettings kafkaSettings, WebSocketService webSocketService)
        {
            _kafkaConnection = kafkaConnection;
            _kafkaSettings = kafkaSettings;
            _webSocketService = webSocketService;
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
                    Console.WriteLine(consumerResult.Message.Value);
                    await _webSocketService.UpdateStatistics(ConvertToStatisticDocument(consumerResult.Message.Value));
                }
                catch (KafkaException e)
                {
                }
                catch (Exception e)
                {
                }
            }
        }
        private StatisticsRo ConvertToStatisticDocument(string json)
        {
            StatisticsRo temp = new StatisticsRo();
            try
            {
                JObject jsonObject = JObject.Parse(json);
                DateTime timestamp = jsonObject[Consts.STATISTICS_TIMESTAMP_NAME].ToObject<DateTime>();
                jsonObject.Remove(Consts.STATISTICS_TIMESTAMP_NAME);
                JObject wrappedJson = new JObject
                {
                    [nameof(temp.StatisticValues)] = jsonObject,
                };
                StatisticsRo statistic = JsonConvert.DeserializeObject<StatisticsRo>(wrappedJson.ToString());
                return statistic;
            }
            catch (Exception e)
            {
                return temp;
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
