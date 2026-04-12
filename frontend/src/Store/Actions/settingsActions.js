import { createAction } from 'redux-actions';
import { handleThunks } from 'Store/thunks';
import createHandleActions from './Creators/createHandleActions';
import arrClients from './Settings/arrClients';
import development from './Settings/development';
import general from './Settings/general';
import mediaServers from './Settings/mediaServers';
import notifications from './Settings/notifications';
import ui from './Settings/ui';

export * from './Settings/arrClients';
export * from './Settings/general';
export * from './Settings/mediaServers';
export * from './Settings/notifications';
export * from './Settings/development';
export * from './Settings/ui';

//
// Variables

export const section = 'settings';

//
// State

export const defaultState = {
  advancedSettings: false,

  arrClients: arrClients.defaultState,
  general: general.defaultState,
  mediaServers: mediaServers.defaultState,
  notifications: notifications.defaultState,
  development: development.defaultState,
  ui: ui.defaultState
};

export const persistState = [
  'settings.advancedSettings'
];

//
// Actions Types

export const TOGGLE_ADVANCED_SETTINGS = 'settings/toggleAdvancedSettings';

//
// Action Creators

export const toggleAdvancedSettings = createAction(TOGGLE_ADVANCED_SETTINGS);

//
// Action Handlers

export const actionHandlers = handleThunks({
  ...arrClients.actionHandlers,
  ...general.actionHandlers,
  ...mediaServers.actionHandlers,
  ...notifications.actionHandlers,
  ...development.actionHandlers,
  ...ui.actionHandlers
});

//
// Reducers

export const reducers = createHandleActions({

  [TOGGLE_ADVANCED_SETTINGS]: (state, { payload }) => {
    return Object.assign({}, state, { advancedSettings: !state.advancedSettings });
  },

  ...arrClients.reducers,
  ...general.reducers,
  ...mediaServers.reducers,
  ...notifications.reducers,
  ...development.reducers,
  ...ui.reducers

}, defaultState, section);
