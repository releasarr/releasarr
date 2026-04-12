import { createThunk, handleThunks } from 'Store/thunks';
import createAjaxRequest from 'Utilities/createAjaxRequest';
import { set, update } from './baseActions';
import createHandleActions from './Creators/createHandleActions';

//
// Variables

export const section = 'search';

//
// Actions Types

export const FETCH_SEARCH_RESULTS = 'search/fetchSearchResults';
export const ADD_TO_WATCHLIST = 'search/addToWatchlist';
export const CLEAR_SEARCH = 'search/clearSearch';

//
// Action Creators

export const fetchSearchResults = createThunk(FETCH_SEARCH_RESULTS);
export const addToWatchlist = createThunk(ADD_TO_WATCHLIST);

export const clearSearch = () => ({
  type: CLEAR_SEARCH
});

//
// State

export const defaultState = {
  isFetching: false,
  isPopulated: false,
  error: null,
  items: [],
  isAdding: false,
  addError: null
};

//
// Action Handlers

export const actionHandlers = handleThunks({
  [FETCH_SEARCH_RESULTS]: function(getState, payload, dispatch) {
    dispatch(set({ section, isFetching: true }));

    const { query } = payload;

    const { request, abortRequest } = createAjaxRequest({
      url: '/search',
      data: { query },
      traditional: true
    });

    request.done((data) => {
      dispatch(update({ section, data }));
      dispatch(set({
        section,
        isFetching: false,
        isPopulated: true,
        error: null
      }));
    });

    request.fail((xhr) => {
      dispatch(set({
        section,
        isFetching: false,
        isPopulated: false,
        error: xhr.aborted ? null : xhr
      }));
    });

    return abortRequest;
  },

  [ADD_TO_WATCHLIST]: function(getState, payload, dispatch) {
    dispatch(set({ section, isAdding: true }));

    const { request, abortRequest } = createAjaxRequest({
      url: '/search/addtowatchlist',
      method: 'POST',
      data: JSON.stringify(payload),
      dataType: 'json',
      contentType: 'application/json'
    });

    request.done(() => {
      dispatch(set({
        section,
        isAdding: false,
        addError: null
      }));
    });

    request.fail((xhr) => {
      dispatch(set({
        section,
        isAdding: false,
        addError: xhr.aborted ? null : xhr
      }));
    });

    return abortRequest;
  }
});

//
// Reducers

export const reducers = createHandleActions({
  [CLEAR_SEARCH]: (state) => {
    return Object.assign({}, state, defaultState);
  }
}, defaultState, section);
