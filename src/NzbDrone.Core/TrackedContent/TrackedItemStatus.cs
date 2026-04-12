namespace NzbDrone.Core.TrackedContent
{
    public enum TrackedItemStatus
    {
        Watchlisted = 0,
        Monitored = 1,
        Downloading = 2,
        Available = 3,
        Notified = 4
    }
}
