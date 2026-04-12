using System;
using NzbDrone.Core.TrackedContent;
using Releasarr.Http.REST;

namespace Releasarr.Api.V1.TrackedContent
{
    public class TrackedItemResource : RestResource
    {
        public string Title { get; set; }
        public ContentType ContentType { get; set; }
        public string PlexGuid { get; set; }
        public int? TmdbId { get; set; }
        public int? TvdbId { get; set; }
        public string ImdbId { get; set; }
        public int? ArrClientId { get; set; }
        public int? ArrItemId { get; set; }
        public TrackedItemStatus Status { get; set; }
        public int MediaServerId { get; set; }
        public DateTime AddedAt { get; set; }
        public DateTime? AvailableAt { get; set; }
        public DateTime? NotifiedAt { get; set; }
        public int? Year { get; set; }
        public string PosterUrl { get; set; }
    }

    public static class TrackedItemResourceMapper
    {
        public static TrackedItemResource ToResource(this TrackedItem model)
        {
            if (model == null)
            {
                return null;
            }

            return new TrackedItemResource
            {
                Id = model.Id,
                Title = model.Title,
                ContentType = model.ContentType,
                PlexGuid = model.PlexGuid,
                TmdbId = model.TmdbId,
                TvdbId = model.TvdbId,
                ImdbId = model.ImdbId,
                ArrClientId = model.ArrClientId,
                ArrItemId = model.ArrItemId,
                Status = model.Status,
                MediaServerId = model.MediaServerId,
                AddedAt = model.AddedAt,
                AvailableAt = model.AvailableAt,
                NotifiedAt = model.NotifiedAt,
                Year = model.Year,
                PosterUrl = model.PosterUrl
            };
        }

        public static TrackedItem ToModel(this TrackedItemResource resource)
        {
            if (resource == null)
            {
                return null;
            }

            return new TrackedItem
            {
                Id = resource.Id,
                Title = resource.Title,
                ContentType = resource.ContentType,
                PlexGuid = resource.PlexGuid,
                TmdbId = resource.TmdbId,
                TvdbId = resource.TvdbId,
                ImdbId = resource.ImdbId,
                ArrClientId = resource.ArrClientId,
                ArrItemId = resource.ArrItemId,
                Status = resource.Status,
                MediaServerId = resource.MediaServerId,
                Year = resource.Year,
                PosterUrl = resource.PosterUrl
            };
        }
    }
}
