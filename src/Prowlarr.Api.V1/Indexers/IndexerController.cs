using FluentValidation;
using NzbDrone.Core.Indexers;
using NzbDrone.Core.Validation;
using Prowlarr.Http;

namespace Prowlarr.Api.V1.Indexers
{
    [V1ApiController]
    public class IndexerController : ProviderControllerBase<IndexerResource, IndexerBulkResource, IIndexer, IndexerDefinition>
    {
        public IndexerController(IndexerFactory indexerFactory,
            IndexerResourceMapper resourceMapper,
            IndexerBulkResourceMapper bulkResourceMapper,
            AppProfileExistsValidator appProfileExistsValidator,
            DownloadClientExistsValidator downloadClientExistsValidator)
            : base(indexerFactory, "indexer", resourceMapper, bulkResourceMapper)
        {
            SharedValidator.RuleFor(c => c.AppProfileId).Cascade(CascadeMode.Stop)
                .ValidId()
                .SetValidator(appProfileExistsValidator);

            SharedValidator.RuleFor(c => c.DownloadClientId).SetValidator(downloadClientExistsValidator);
        }
    }
}
