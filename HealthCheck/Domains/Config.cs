using System.Collections.Generic;

namespace HealthCheck.Domains
{
    public class Config
    {
        public string WebSiteName { get; }
        public int CheckingTimeInterval { get; }
        public Dictionary<string,string> TargetServers { get; }
        public string SlackLoggerEndPoint { get; }
        public string SlackLoggerWebhookUrl { get; }

        public Config(string websiteName, Dictionary<string, string> targetServers, string slackLoggerEndPoint,
            string slackLoggerWebhookUrl, int checkingTimeInterval = 10)
        {
            WebSiteName = websiteName;
            CheckingTimeInterval = checkingTimeInterval;
            TargetServers = targetServers;
            SlackLoggerEndPoint = slackLoggerEndPoint;
            SlackLoggerWebhookUrl = slackLoggerWebhookUrl;

        }
    }
}
