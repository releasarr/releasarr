import { createAction } from 'redux-actions';
import createFetchHandler from 'Store/Actions/Creators/createFetchHandler';
import createFetchSchemaHandler from 'Store/Actions/Creators/createFetchSchemaHandler';
import createRemoveItemHandler from 'Store/Actions/Creators/createRemoveItemHandler';
import createSaveProviderHandler, { createCancelSaveProviderHandler } from 'Store/Actions/Creators/createSaveProviderHandler';
import createTestProviderHandler, { createCancelTestProviderHandler } from 'Store/Actions/Creators/createTestProviderHandler';
import createSetProviderFieldValueReducer from 'Store/Actions/Creators/Reducers/createSetProviderFieldValueReducer';
import createSetSettingValueReducer from 'Store/Actions/Creators/Reducers/createSetSettingValueReducer';
import { createThunk } from 'Store/thunks';
import selectProviderSchema from 'Utilities/State/selectProviderSchema';

//
// Variables

const section = 'settings.mediaServers';

//
// Actions Types

export const FETCH_MEDIA_SERVERS = 'settings/mediaServers/fetchMediaServers';
export const FETCH_MEDIA_SERVER_SCHEMA = 'settings/mediaServers/fetchMediaServerSchema';
export const SELECT_MEDIA_SERVER_SCHEMA = 'settings/mediaServers/selectMediaServerSchema';
export const SET_MEDIA_SERVER_VALUE = 'settings/mediaServers/setMediaServerValue';
export const SET_MEDIA_SERVER_FIELD_VALUE = 'settings/mediaServers/setMediaServerFieldValue';
export const SAVE_MEDIA_SERVER = 'settings/mediaServers/saveMediaServer';
export const CANCEL_SAVE_MEDIA_SERVER = 'settings/mediaServers/cancelSaveMediaServer';
export const DELETE_MEDIA_SERVER = 'settings/mediaServers/deleteMediaServer';
export const TEST_MEDIA_SERVER = 'settings/mediaServers/testMediaServer';
export const CANCEL_TEST_MEDIA_SERVER = 'settings/mediaServers/cancelTestMediaServer';

//
// Action Creators

export const fetchMediaServers = createThunk(FETCH_MEDIA_SERVERS);
export const fetchMediaServerSchema = createThunk(FETCH_MEDIA_SERVER_SCHEMA);
export const selectMediaServerSchema = createAction(SELECT_MEDIA_SERVER_SCHEMA);

export const saveMediaServer = createThunk(SAVE_MEDIA_SERVER);
export const cancelSaveMediaServer = createThunk(CANCEL_SAVE_MEDIA_SERVER);
export const deleteMediaServer = createThunk(DELETE_MEDIA_SERVER);
export const testMediaServer = createThunk(TEST_MEDIA_SERVER);
export const cancelTestMediaServer = createThunk(CANCEL_TEST_MEDIA_SERVER);

export const setMediaServerValue = createAction(SET_MEDIA_SERVER_VALUE, (payload) => {
  return {
    section,
    ...payload
  };
});

export const setMediaServerFieldValue = createAction(SET_MEDIA_SERVER_FIELD_VALUE, (payload) => {
  return {
    section,
    ...payload
  };
});

//
// Details

export default {

  //
  // State

  defaultState: {
    isFetching: false,
    isPopulated: false,
    error: null,
    isSchemaFetching: false,
    isSchemaPopulated: false,
    schemaError: null,
    schema: [],
    selectedSchema: {},
    isSaving: false,
    saveError: null,
    isDeleting: false,
    deleteError: null,
    isTesting: false,
    items: [],
    pendingChanges: {}
  },

  //
  // Action Handlers

  actionHandlers: {
    [FETCH_MEDIA_SERVERS]: createFetchHandler(section, '/mediaserver'),
    [FETCH_MEDIA_SERVER_SCHEMA]: createFetchSchemaHandler(section, '/mediaserver/schema'),

    [SAVE_MEDIA_SERVER]: createSaveProviderHandler(section, '/mediaserver'),
    [CANCEL_SAVE_MEDIA_SERVER]: createCancelSaveProviderHandler(section),
    [DELETE_MEDIA_SERVER]: createRemoveItemHandler(section, '/mediaserver'),
    [TEST_MEDIA_SERVER]: createTestProviderHandler(section, '/mediaserver'),
    [CANCEL_TEST_MEDIA_SERVER]: createCancelTestProviderHandler(section)
  },

  //
  // Reducers

  reducers: {
    [SET_MEDIA_SERVER_VALUE]: createSetSettingValueReducer(section),
    [SET_MEDIA_SERVER_FIELD_VALUE]: createSetProviderFieldValueReducer(section),

    [SELECT_MEDIA_SERVER_SCHEMA]: (state, { payload }) => {
      return selectProviderSchema(state, section, payload, (selectedSchema) => {
        selectedSchema.name = selectedSchema.implementationName;
        selectedSchema.enable = true;

        return selectedSchema;
      });
    }
  }

};
