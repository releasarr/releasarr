using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using NzbDrone.Core.Messaging.Events;

namespace NzbDrone.Core.TrackedContent
{
    public interface ITrackedItemService
    {
        List<TrackedItem> GetAll();
        TrackedItem Get(int id);
        TrackedItem FindByPlexGuid(string plexGuid);
        List<TrackedItem> GetByStatus(TrackedItemStatus status);
        List<TrackedItem> GetByStatuses(params TrackedItemStatus[] statuses);
        TrackedItem Add(TrackedItem item);
        TrackedItem Update(TrackedItem item);
        void Delete(int id);
        TrackedItem UpdateStatus(int id, TrackedItemStatus status);
        DashboardStats GetDashboardStats();
    }

    public class TrackedItemService : ITrackedItemService
    {
        private readonly ITrackedItemRepository _repository;
        private readonly IEventAggregator _eventAggregator;
        private readonly Logger _logger;

        public TrackedItemService(ITrackedItemRepository repository,
                                   IEventAggregator eventAggregator,
                                   Logger logger)
        {
            _repository = repository;
            _eventAggregator = eventAggregator;
            _logger = logger;
        }

        public List<TrackedItem> GetAll()
        {
            return _repository.All().ToList();
        }

        public TrackedItem Get(int id)
        {
            return _repository.Get(id);
        }

        public TrackedItem FindByPlexGuid(string plexGuid)
        {
            return _repository.FindByPlexGuid(plexGuid);
        }

        public List<TrackedItem> GetByStatus(TrackedItemStatus status)
        {
            return _repository.GetByStatus(status);
        }

        public List<TrackedItem> GetByStatuses(params TrackedItemStatus[] statuses)
        {
            return _repository.GetByStatuses(statuses);
        }

        public TrackedItem Add(TrackedItem item)
        {
            item.AddedAt = DateTime.UtcNow;

            _logger.Info("Adding tracked item: {0} ({1})", item.Title, item.ContentType);
            return _repository.Insert(item);
        }

        public TrackedItem Update(TrackedItem item)
        {
            return _repository.Update(item);
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
        }

        public TrackedItem UpdateStatus(int id, TrackedItemStatus status)
        {
            var item = _repository.Get(id);
            item.Status = status;

            if (status == TrackedItemStatus.Available)
            {
                item.AvailableAt = DateTime.UtcNow;
            }

            if (status == TrackedItemStatus.Notified)
            {
                item.NotifiedAt = DateTime.UtcNow;
            }

            return _repository.Update(item);
        }

        public DashboardStats GetDashboardStats()
        {
            var all = _repository.All().ToList();
            return new DashboardStats
            {
                Watchlisted = all.Count(x => x.Status == TrackedItemStatus.Watchlisted),
                Monitored = all.Count(x => x.Status == TrackedItemStatus.Monitored),
                Downloading = all.Count(x => x.Status == TrackedItemStatus.Downloading),
                Available = all.Count(x => x.Status == TrackedItemStatus.Available),
                Notified = all.Count(x => x.Status == TrackedItemStatus.Notified),
                Total = all.Count
            };
        }
    }

    public class DashboardStats
    {
        public int Watchlisted { get; set; }
        public int Monitored { get; set; }
        public int Downloading { get; set; }
        public int Available { get; set; }
        public int Notified { get; set; }
        public int Total { get; set; }
    }
}
