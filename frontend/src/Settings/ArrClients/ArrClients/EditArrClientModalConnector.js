import PropTypes from 'prop-types';
import React, { Component } from 'react';
import { connect } from 'react-redux';
import { clearPendingChanges } from 'Store/Actions/baseActions';
import { cancelSaveArrClient, cancelTestArrClient } from 'Store/Actions/settingsActions';
import EditArrClientModal from './EditArrClientModal';

function createMapDispatchToProps(dispatch, props) {
  const section = 'settings.arrClients';

  return {
    dispatchClearPendingChanges() {
      dispatch(clearPendingChanges({ section }));
    },

    dispatchCancelTestArrClient() {
      dispatch(cancelTestArrClient({ section }));
    },

    dispatchCancelSaveArrClient() {
      dispatch(cancelSaveArrClient({ section }));
    }
  };
}

class EditArrClientModalConnector extends Component {

  //
  // Listeners

  onModalClose = () => {
    this.props.dispatchClearPendingChanges();
    this.props.dispatchCancelTestArrClient();
    this.props.dispatchCancelSaveArrClient();
    this.props.onModalClose();
  };

  //
  // Render

  render() {
    const {
      dispatchClearPendingChanges,
      dispatchCancelTestArrClient,
      dispatchCancelSaveArrClient,
      ...otherProps
    } = this.props;

    return (
      <EditArrClientModal
        {...otherProps}
        onModalClose={this.onModalClose}
      />
    );
  }
}

EditArrClientModalConnector.propTypes = {
  onModalClose: PropTypes.func.isRequired,
  dispatchClearPendingChanges: PropTypes.func.isRequired,
  dispatchCancelTestArrClient: PropTypes.func.isRequired,
  dispatchCancelSaveArrClient: PropTypes.func.isRequired
};

export default connect(null, createMapDispatchToProps)(EditArrClientModalConnector);
