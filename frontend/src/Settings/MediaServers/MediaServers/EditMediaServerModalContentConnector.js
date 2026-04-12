import PropTypes from 'prop-types';
import React, { Component } from 'react';
import { connect } from 'react-redux';
import { createSelector } from 'reselect';
import {
  saveMediaServer,
  setMediaServerFieldValue,
  setMediaServerValue,
  testMediaServer,
  toggleAdvancedSettings
} from 'Store/Actions/settingsActions';
import createProviderSettingsSelector from 'Store/Selectors/createProviderSettingsSelector';
import EditMediaServerModalContent from './EditMediaServerModalContent';

function createMapStateToProps() {
  return createSelector(
    (state) => state.settings.advancedSettings,
    createProviderSettingsSelector('mediaServers'),
    (advancedSettings, mediaServer) => {
      return {
        advancedSettings,
        ...mediaServer
      };
    }
  );
}

const mapDispatchToProps = {
  setMediaServerValue,
  setMediaServerFieldValue,
  saveMediaServer,
  testMediaServer,
  toggleAdvancedSettings
};

class EditMediaServerModalContentConnector extends Component {

  //
  // Lifecycle

  componentDidUpdate(prevProps, prevState) {
    if (prevProps.isSaving && !this.props.isSaving && !this.props.saveError) {
      this.props.onModalClose();
    }
  }

  //
  // Listeners

  onInputChange = ({ name, value }) => {
    this.props.setMediaServerValue({ name, value });
  };

  onFieldChange = ({ name, value }) => {
    this.props.setMediaServerFieldValue({ name, value });
  };

  onSavePress = () => {
    this.props.saveMediaServer({ id: this.props.id });
  };

  onTestPress = () => {
    this.props.testMediaServer({ id: this.props.id });
  };

  onAdvancedSettingsPress = () => {
    this.props.toggleAdvancedSettings();
  };

  //
  // Render

  render() {
    return (
      <EditMediaServerModalContent
        {...this.props}
        onSavePress={this.onSavePress}
        onTestPress={this.onTestPress}
        onAdvancedSettingsPress={this.onAdvancedSettingsPress}
        onInputChange={this.onInputChange}
        onFieldChange={this.onFieldChange}
      />
    );
  }
}

EditMediaServerModalContentConnector.propTypes = {
  id: PropTypes.number,
  isFetching: PropTypes.bool.isRequired,
  isSaving: PropTypes.bool.isRequired,
  saveError: PropTypes.object,
  item: PropTypes.object.isRequired,
  setMediaServerValue: PropTypes.func.isRequired,
  setMediaServerFieldValue: PropTypes.func.isRequired,
  saveMediaServer: PropTypes.func.isRequired,
  testMediaServer: PropTypes.func.isRequired,
  toggleAdvancedSettings: PropTypes.func.isRequired,
  onModalClose: PropTypes.func.isRequired
};

export default connect(createMapStateToProps, mapDispatchToProps)(EditMediaServerModalContentConnector);
