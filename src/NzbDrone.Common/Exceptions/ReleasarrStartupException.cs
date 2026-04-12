using System;

namespace NzbDrone.Common.Exceptions
{
    public class ReleasarrStartupException : NzbDroneException
    {
        public ReleasarrStartupException(string message, params object[] args)
            : base("Releasarr failed to start: " + string.Format(message, args))
        {
        }

        public ReleasarrStartupException(string message)
            : base("Releasarr failed to start: " + message)
        {
        }

        public ReleasarrStartupException()
            : base("Releasarr failed to start")
        {
        }

        public ReleasarrStartupException(Exception innerException, string message, params object[] args)
            : base("Releasarr failed to start: " + string.Format(message, args), innerException)
        {
        }

        public ReleasarrStartupException(Exception innerException, string message)
            : base("Releasarr failed to start: " + message, innerException)
        {
        }

        public ReleasarrStartupException(Exception innerException)
            : base("Releasarr failed to start: " + innerException.Message)
        {
        }
    }
}
