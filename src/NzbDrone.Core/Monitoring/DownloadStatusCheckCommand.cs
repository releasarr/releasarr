using NzbDrone.Core.Messaging.Commands;

namespace NzbDrone.Core.Monitoring
{
    public class DownloadStatusCheckCommand : Command
    {
        public override bool SendUpdatesToClient => true;

        public override string CompletionMessage => "Download status check completed";
    }
}
