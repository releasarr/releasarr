import PropTypes from 'prop-types';
import React, { Component } from 'react';
import { connect } from 'react-redux';
import { clearPendingChanges } from 'Store/Actions/baseActions';
import { cancelSaveMediaServer, cancelTestMediaServer } from 'Store/Actions/settingsActions';
import EditMediaServerModal from './EditMediaServerModal';

function createMapDispatchToProps(dispatch, props) {
  const section = 'settings.mediaServers';

  return {
    dispatchClearPendingChanges() {
      dispatch(clearPendingChanges({ section }));
    },

    dispatchCancelTestMediaServer() {
      dispatch(cancelTestMediaServer({ section }));
    },

    dispatchCancelSaveMediaServer() {
      dispatch(cancelSaveMediaServer({ section }));
    }
  };
}

class EditMediaServerModalConnector extends Component {

  //
  // Listeners

  onModalClose = () => {
    this.props.dispatchClearPendingChanges();
    this.props.dispatchCancelTestMediaServer();
    this.props.dispatchCancelSaveMediaServer();
    this.props.onModalClose();
  };

  //
  // Render

  render() {
    const {
      dispatchClearPendingChanges,
      dispatchCancelTestMediaServer,
      dispatchCancelSaveMediaServer,
      ...otherProps
    } = this.props;

    return (
      <EditMediaServerModal
        {...otherProps}
        onModalClose={this.onModalClose}
      />
    );
  }
}

EditMediaServerModalConnector.propTypes = {
  onModalClose: PropTypes.func.isRequired,
  dispatchClearPendingChanges: PropTypes.func.isRequired,
  dispatchCancelTestMediaServer: PropTypes.func.isRequired,
  dispatchCancelSaveMediaServer: PropTypes.func.isRequired
};

export default connect(null, createMapDispatchToProps)(EditMediaServerModalConnector);
