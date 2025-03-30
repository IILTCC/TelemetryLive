using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using TelemetryLive.Dto;

namespace TelemetryLive.SignalR
{
    public class WebSocketService
    {
        public IHubContext<StatisticsHub> _websock;
        public WebSocketService(IHubContext<StatisticsHub> websock) { _websock = websock; }

        public async Task UpdateStatistics(StatisticsRo statisticsRo)
        {
            await _websock.Clients.All.SendAsync(Consts.UPDATE_STATISTICS_NAME, statisticsRo);
        }
    }
}
