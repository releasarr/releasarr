using NzbDrone.Core.Messaging.Commands;

namespace NzbDrone.Core.Monitoring
{
    public class WatchlistSyncCommand : Command
    {
        public override bool SendUpdatesToClient => true;

        public override string CompletionMessage => "Watchlist sync completed";
    }
}
