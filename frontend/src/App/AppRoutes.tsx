import React from 'react';
import { Redirect, Route } from 'react-router-dom';
import NotFound from 'Components/NotFound';
import Switch from 'Components/Router/Switch';
import DashboardConnector from 'Dashboard/DashboardConnector';
import HistoryConnector from 'History/HistoryConnector';
import ArrClientSettings from 'Settings/ArrClients/ArrClientSettings';
import DevelopmentSettingsConnector from 'Settings/Development/DevelopmentSettingsConnector';
import GeneralSettingsConnector from 'Settings/General/GeneralSettingsConnector';
import MediaServerSettings from 'Settings/MediaServers/MediaServerSettings';
import NotificationSettings from 'Settings/Notifications/NotificationSettings';
import Settings from 'Settings/Settings';
import TagSettings from 'Settings/Tags/TagSettings';
import UISettingsConnector from 'Settings/UI/UISettingsConnector';
import BackupsConnector from 'System/Backup/BackupsConnector';
import LogsTableConnector from 'System/Events/LogsTableConnector';
import Logs from 'System/Logs/Logs';
import Status from 'System/Status/Status';
import Tasks from 'System/Tasks/Tasks';
import Updates from 'System/Updates/Updates';
import SearchConnector from 'Search/SearchConnector';
import TrackedContentConnector from 'TrackedContent/TrackedContentConnector';
import getPathWithUrlBase from 'Utilities/getPathWithUrlBase';

function RedirectWithUrlBase() {
  return <Redirect to={getPathWithUrlBase('/')} />;
}

function AppRoutes() {
  return (
    <Switch>
      {/*
        Dashboard
      */}

      <Route exact={true} path="/" component={DashboardConnector} />

      {window.Releasarr.urlBase && (
        <Route
          exact={true}
          path="/"
          // eslint-disable-next-line @typescript-eslint/ban-ts-comment
          // @ts-ignore
          addUrlBase={false}
          render={RedirectWithUrlBase}
        />
      )}

      {/*
        Search
      */}

      <Route path="/search" component={SearchConnector} />

      {/*
        Tracked Content
      */}

      <Route path="/trackedcontent" component={TrackedContentConnector} />

      {/*
        Activity
      */}

      <Route path="/history" component={HistoryConnector} />

      {/*
        Settings
      */}

      <Route exact={true} path="/settings" component={Settings} />

      <Route path="/settings/mediaservers" component={MediaServerSettings} />

      <Route path="/settings/arrclients" component={ArrClientSettings} />

      <Route path="/settings/connect" component={NotificationSettings} />

      <Route path="/settings/tags" component={TagSettings} />

      <Route path="/settings/general" component={GeneralSettingsConnector} />

      <Route path="/settings/ui" component={UISettingsConnector} />

      <Route
        path="/settings/development"
        component={DevelopmentSettingsConnector}
      />

      {/*
        System
      */}

      <Route path="/system/status" component={Status} />

      <Route path="/system/tasks" component={Tasks} />

      <Route path="/system/backup" component={BackupsConnector} />

      <Route path="/system/updates" component={Updates} />

      <Route path="/system/events" component={LogsTableConnector} />

      <Route path="/system/logs/files" component={Logs} />

      {/*
        Not Found
      */}

      <Route path="*" component={NotFound} />
    </Switch>
  );
}

export default AppRoutes;
