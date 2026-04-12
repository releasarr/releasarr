import PropTypes from 'prop-types';
import React, { Component } from 'react';
import { connect } from 'react-redux';
import { createSelector } from 'reselect';
import {
  saveArrClient,
  setArrClientFieldValue,
  setArrClientValue,
  testArrClient,
  toggleAdvancedSettings
} from 'Store/Actions/settingsActions';
import createProviderSettingsSelector from 'Store/Selectors/createProviderSettingsSelector';
import EditArrClientModalContent from './EditArrClientModalContent';

function createMapStateToProps() {
  return createSelector(
    (state) => state.settings.advancedSettings,
    createProviderSettingsSelector('arrClients'),
    (advancedSettings, arrClient) => {
      return {
        advancedSettings,
        ...arrClient
      };
    }
  );
}

const mapDispatchToProps = {
  setArrClientValue,
  setArrClientFieldValue,
  saveArrClient,
  testArrClient,
  toggleAdvancedSettings
};

class EditArrClientModalContentConnector extends Component {

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
    this.props.setArrClientValue({ name, value });
  };

  onFieldChange = ({ name, value }) => {
    this.props.setArrClientFieldValue({ name, value });
  };

  onSavePress = () => {
    this.props.saveArrClient({ id: this.props.id });
  };

  onTestPress = () => {
    this.props.testArrClient({ id: this.props.id });
  };

  onAdvancedSettingsPress = () => {
    this.props.toggleAdvancedSettings();
  };

  //
  // Render

  render() {
    return (
      <EditArrClientModalContent
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

EditArrClientModalContentConnector.propTypes = {
  id: PropTypes.number,
  isFetching: PropTypes.bool.isRequired,
  isSaving: PropTypes.bool.isRequired,
  saveError: PropTypes.object,
  item: PropTypes.object.isRequired,
  setArrClientValue: PropTypes.func.isRequired,
  setArrClientFieldValue: PropTypes.func.isRequired,
  saveArrClient: PropTypes.func.isRequired,
  testArrClient: PropTypes.func.isRequired,
  toggleAdvancedSettings: PropTypes.func.isRequired,
  onModalClose: PropTypes.func.isRequired
};

export default connect(createMapStateToProps, mapDispatchToProps)(EditArrClientModalContentConnector);
