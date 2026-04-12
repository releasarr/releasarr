import React from 'react';
import PageContent from 'Components/Page/PageContent';
import PageContentBody from 'Components/Page/PageContentBody';
import SettingsToolbarConnector from 'Settings/SettingsToolbarConnector';
import translate from 'Utilities/String/translate';
import MediaServersConnector from './MediaServers/MediaServersConnector';

function MediaServerSettings() {
  return (
    <PageContent title={translate('MediaServerSettings')}>
      <SettingsToolbarConnector
        showSave={false}
      />

      <PageContentBody>
        <MediaServersConnector />
      </PageContentBody>
    </PageContent>
  );
}

export default MediaServerSettings;
