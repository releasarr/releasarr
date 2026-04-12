using FluentMigrator;
using NzbDrone.Core.Datastore.Migration.Framework;

namespace NzbDrone.Core.Datastore.Migration
{
    [Migration(044)]
    public class releasarr_tables : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Create.TableForModel("MediaServers")
                  .WithColumn("Name").AsString()
                  .WithColumn("Implementation").AsString()
                  .WithColumn("Settings").AsString().Nullable()
                  .WithColumn("ConfigContract").AsString().Nullable()
                  .WithColumn("Enable").AsBoolean().NotNullable().WithDefaultValue(true)
                  .WithColumn("Tags").AsString().Nullable();

            Create.TableForModel("MediaServerStatus")
                  .WithColumn("ProviderId").AsInt32().NotNullable().Unique()
                  .WithColumn("InitialFailure").AsDateTimeOffset().Nullable()
                  .WithColumn("MostRecentFailure").AsDateTimeOffset().Nullable()
                  .WithColumn("EscalationLevel").AsInt32().NotNullable()
                  .WithColumn("DisabledTill").AsDateTimeOffset().Nullable();

            Create.TableForModel("ArrClients")
                  .WithColumn("Name").AsString()
                  .WithColumn("Implementation").AsString()
                  .WithColumn("Settings").AsString().Nullable()
                  .WithColumn("ConfigContract").AsString().Nullable()
                  .WithColumn("Enable").AsBoolean().NotNullable().WithDefaultValue(true)
                  .WithColumn("Tags").AsString().Nullable();

            Create.TableForModel("ArrClientStatus")
                  .WithColumn("ProviderId").AsInt32().NotNullable().Unique()
                  .WithColumn("InitialFailure").AsDateTimeOffset().Nullable()
                  .WithColumn("MostRecentFailure").AsDateTimeOffset().Nullable()
                  .WithColumn("EscalationLevel").AsInt32().NotNullable()
                  .WithColumn("DisabledTill").AsDateTimeOffset().Nullable();

            Create.TableForModel("TrackedItems")
                  .WithColumn("Title").AsString()
                  .WithColumn("ContentType").AsInt32()
                  .WithColumn("PlexGuid").AsString().Nullable()
                  .WithColumn("TmdbId").AsInt32().Nullable()
                  .WithColumn("TvdbId").AsInt32().Nullable()
                  .WithColumn("ImdbId").AsString().Nullable()
                  .WithColumn("ArrClientId").AsInt32().Nullable()
                  .WithColumn("ArrItemId").AsInt32().Nullable()
                  .WithColumn("Status").AsInt32()
                  .WithColumn("MediaServerId").AsInt32()
                  .WithColumn("AddedAt").AsDateTimeOffset()
                  .WithColumn("AvailableAt").AsDateTimeOffset().Nullable()
                  .WithColumn("NotifiedAt").AsDateTimeOffset().Nullable()
                  .WithColumn("Metadata").AsString().Nullable()
                  .WithColumn("Year").AsInt32().Nullable()
                  .WithColumn("PosterUrl").AsString().Nullable();

            // Add OnContentAvailable to Notifications table
            Alter.Table("Notifications")
                 .AddColumn("OnContentAvailable").AsBoolean().NotNullable().WithDefaultValue(false);
        }
    }
}
