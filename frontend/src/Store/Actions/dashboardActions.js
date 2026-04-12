import { createThunk, handleThunks } from 'Store/thunks';
import createAjaxRequest from 'Utilities/createAjaxRequest';
import { set } from 'Store/Actions/baseActions';
import createHandleActions from './Creators/createHandleActions';

//
// Variables

export const section = 'dashboard';

//
// Actions Types

export const FETCH_DASHBOARD = 'dashboard/fetchDashboard';

//
// Action Creators

export const fetchDashboard = createThunk(FETCH_DASHBOARD);

//
// State

export const defaultState = {
  isFetching: false,
  isPopulated: false,
  error: null,
  item: {}
};

//
// Action Handlers

export const actionHandlers = handleThunks({
  [FETCH_DASHBOARD]: function(getState, payload, dispatch) {
    dispatch(set({ section, isFetching: true }));

    const promise = createAjaxRequest({
      url: '/dashboard'
    }).request;

    promise.then((data) => {
      dispatch(set({
        section,
        isFetching: false,
        isPopulated: true,
        error: null,
        item: data
      }));
    });

    promise.catch((xhr) => {
      dispatch(set({
        section,
        isFetching: false,
        isPopulated: false,
        error: xhr
      }));
    });
  }
});

//
// Reducers

export const reducers = createHandleActions({}, defaultState, section);
