import React from 'react';
import PageContent from 'Components/Page/PageContent';
import PageContentBody from 'Components/Page/PageContentBody';
import SettingsToolbarConnector from 'Settings/SettingsToolbarConnector';
import translate from 'Utilities/String/translate';
import ArrClientsConnector from './ArrClients/ArrClientsConnector';

function ArrClientSettings() {
  return (
    <PageContent title={translate('ArrClientSettings')}>
      <SettingsToolbarConnector
        showSave={false}
      />

      <PageContentBody>
        <ArrClientsConnector />
      </PageContentBody>
    </PageContent>
  );
}

export default ArrClientSettings;
