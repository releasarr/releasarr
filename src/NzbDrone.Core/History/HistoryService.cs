using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using NzbDrone.Core.Configuration;
using NzbDrone.Core.Datastore;
using NzbDrone.Core.Messaging.Commands;

namespace NzbDrone.Core.History
{
    public interface IHistoryService
    {
        PagingSpec<History> Paged(PagingSpec<History> pagingSpec);
        History MostRecentForIndexer(int indexerId);
        History MostRecentForDownloadId(string downloadId);
        History Get(int historyId);
        List<History> Find(string downloadId, HistoryEventType eventType);
        List<History> FindByDownloadId(string downloadId);
        List<History> GetByIndexerId(int indexerId, HistoryEventType? eventType);
        void UpdateMany(List<History> toUpdate);
        List<History> Between(DateTime start, DateTime end);
        List<History> Since(DateTime date, HistoryEventType? eventType);
        int CountSince(int indexerId, DateTime date, List<HistoryEventType> eventTypes);
        History FindFirstForIndexerSince(int indexerId, DateTime date, List<HistoryEventType> eventTypes, int limit);
    }

    public class HistoryService : IHistoryService,
                                  IExecute<CleanUpHistoryCommand>,
                                  IExecute<ClearHistoryCommand>
    {
        private readonly IHistoryRepository _historyRepository;
        private readonly IConfigService _configService;
        private readonly Logger _logger;

        public HistoryService(IHistoryRepository historyRepository, IConfigService configService, Logger logger)
        {
            _historyRepository = historyRepository;
            _configService = configService;
            _logger = logger;
        }

        public PagingSpec<History> Paged(PagingSpec<History> pagingSpec)
        {
            return _historyRepository.GetPaged(pagingSpec);
        }

        public History MostRecentForIndexer(int indexerId)
        {
            return _historyRepository.MostRecentForIndexer(indexerId);
        }

        public History MostRecentForDownloadId(string downloadId)
        {
            return _historyRepository.MostRecentForDownloadId(downloadId);
        }

        public History Get(int historyId)
        {
            return _historyRepository.Get(historyId);
        }

        public List<History> Find(string downloadId, HistoryEventType eventType)
        {
            return _historyRepository.FindByDownloadId(downloadId).Where(c => c.EventType == eventType).ToList();
        }

        public List<History> FindByDownloadId(string downloadId)
        {
            return _historyRepository.FindByDownloadId(downloadId);
        }

        public List<History> GetByIndexerId(int indexerId, HistoryEventType? eventType)
        {
            return _historyRepository.GetByIndexerId(indexerId, eventType);
        }

        public void UpdateMany(List<History> toUpdate)
        {
            _historyRepository.UpdateMany(toUpdate);
        }

        public List<History> Between(DateTime start, DateTime end)
        {
            return _historyRepository.Between(start, end);
        }

        public List<History> Since(DateTime date, HistoryEventType? eventType)
        {
            return _historyRepository.Since(date, eventType);
        }

        public void Cleanup()
        {
            var cleanupDays = _configService.HistoryCleanupDays;

            if (cleanupDays == 0)
            {
                _logger.Info("Automatic cleanup of History is disabled");
                return;
            }

            _logger.Info("Removing items older than {0} days from history", cleanupDays);

            _historyRepository.Cleanup(cleanupDays);

            _logger.Debug("History has been cleaned up.");
        }

        public void Execute(CleanUpHistoryCommand message)
        {
            Cleanup();
        }

        public void Execute(ClearHistoryCommand message)
        {
            _historyRepository.Purge(vacuum: true);
        }

        public int CountSince(int indexerId, DateTime date, List<HistoryEventType> eventTypes)
        {
            return _historyRepository.CountSince(indexerId, date, eventTypes);
        }

        public History FindFirstForIndexerSince(int indexerId, DateTime date, List<HistoryEventType> eventTypes, int limit)
        {
            return _historyRepository.FindFirstForIndexerSince(indexerId, date, eventTypes, limit);
        }
    }
}
