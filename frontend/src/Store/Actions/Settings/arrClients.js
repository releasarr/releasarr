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

const section = 'settings.arrClients';

//
// Actions Types

export const FETCH_ARR_CLIENTS = 'settings/arrClients/fetchArrClients';
export const FETCH_ARR_CLIENT_SCHEMA = 'settings/arrClients/fetchArrClientSchema';
export const SELECT_ARR_CLIENT_SCHEMA = 'settings/arrClients/selectArrClientSchema';
export const SET_ARR_CLIENT_VALUE = 'settings/arrClients/setArrClientValue';
export const SET_ARR_CLIENT_FIELD_VALUE = 'settings/arrClients/setArrClientFieldValue';
export const SAVE_ARR_CLIENT = 'settings/arrClients/saveArrClient';
export const CANCEL_SAVE_ARR_CLIENT = 'settings/arrClients/cancelSaveArrClient';
export const DELETE_ARR_CLIENT = 'settings/arrClients/deleteArrClient';
export const TEST_ARR_CLIENT = 'settings/arrClients/testArrClient';
export const CANCEL_TEST_ARR_CLIENT = 'settings/arrClients/cancelTestArrClient';

//
// Action Creators

export const fetchArrClients = createThunk(FETCH_ARR_CLIENTS);
export const fetchArrClientSchema = createThunk(FETCH_ARR_CLIENT_SCHEMA);
export const selectArrClientSchema = createAction(SELECT_ARR_CLIENT_SCHEMA);

export const saveArrClient = createThunk(SAVE_ARR_CLIENT);
export const cancelSaveArrClient = createThunk(CANCEL_SAVE_ARR_CLIENT);
export const deleteArrClient = createThunk(DELETE_ARR_CLIENT);
export const testArrClient = createThunk(TEST_ARR_CLIENT);
export const cancelTestArrClient = createThunk(CANCEL_TEST_ARR_CLIENT);

export const setArrClientValue = createAction(SET_ARR_CLIENT_VALUE, (payload) => {
  return {
    section,
    ...payload
  };
});

export const setArrClientFieldValue = createAction(SET_ARR_CLIENT_FIELD_VALUE, (payload) => {
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
    [FETCH_ARR_CLIENTS]: createFetchHandler(section, '/arrclient'),
    [FETCH_ARR_CLIENT_SCHEMA]: createFetchSchemaHandler(section, '/arrclient/schema'),

    [SAVE_ARR_CLIENT]: createSaveProviderHandler(section, '/arrclient'),
    [CANCEL_SAVE_ARR_CLIENT]: createCancelSaveProviderHandler(section),
    [DELETE_ARR_CLIENT]: createRemoveItemHandler(section, '/arrclient'),
    [TEST_ARR_CLIENT]: createTestProviderHandler(section, '/arrclient'),
    [CANCEL_TEST_ARR_CLIENT]: createCancelTestProviderHandler(section)
  },

  //
  // Reducers

  reducers: {
    [SET_ARR_CLIENT_VALUE]: createSetSettingValueReducer(section),
    [SET_ARR_CLIENT_FIELD_VALUE]: createSetProviderFieldValueReducer(section),

    [SELECT_ARR_CLIENT_SCHEMA]: (state, { payload }) => {
      return selectProviderSchema(state, section, payload, (selectedSchema) => {
        selectedSchema.name = selectedSchema.implementationName;

        return selectedSchema;
      });
    }
  }

};
