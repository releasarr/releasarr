import { createThunk, handleThunks } from 'Store/thunks';
import createFetchHandler from 'Store/Actions/Creators/createFetchHandler';
import createRemoveItemHandler from 'Store/Actions/Creators/createRemoveItemHandler';
import createHandleActions from './Creators/createHandleActions';

//
// Variables

export const section = 'trackedContent';

//
// Actions Types

export const FETCH_TRACKED_CONTENT = 'trackedContent/fetchTrackedContent';
export const DELETE_TRACKED_ITEM = 'trackedContent/deleteTrackedItem';

//
// Action Creators

export const fetchTrackedContent = createThunk(FETCH_TRACKED_CONTENT);
export const deleteTrackedItem = createThunk(DELETE_TRACKED_ITEM);

//
// State

export const defaultState = {
  isFetching: false,
  isPopulated: false,
  error: null,
  items: [],
  isDeleting: false,
  deleteError: null
};

//
// Action Handlers

export const actionHandlers = handleThunks({
  [FETCH_TRACKED_CONTENT]: createFetchHandler(section, '/trackedcontent'),
  [DELETE_TRACKED_ITEM]: createRemoveItemHandler(section, '/trackedcontent')
});

//
// Reducers

export const reducers = createHandleActions({}, defaultState, section);
