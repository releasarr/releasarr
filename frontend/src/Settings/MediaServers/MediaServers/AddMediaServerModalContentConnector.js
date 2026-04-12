import PropTypes from 'prop-types';
import React, { Component } from 'react';
import { connect } from 'react-redux';
import { createSelector } from 'reselect';
import { fetchMediaServerSchema, selectMediaServerSchema } from 'Store/Actions/settingsActions';
import AddMediaServerModalContent from './AddMediaServerModalContent';

function createMapStateToProps() {
  return createSelector(
    (state) => state.settings.mediaServers,
    (mediaServers) => {
      const {
        isSchemaFetching,
        isSchemaPopulated,
        schemaError,
        schema
      } = mediaServers;

      return {
        isSchemaFetching,
        isSchemaPopulated,
        schemaError,
        schema
      };
    }
  );
}

const mapDispatchToProps = {
  fetchMediaServerSchema,
  selectMediaServerSchema
};

class AddMediaServerModalContentConnector extends Component {

  //
  // Lifecycle

  componentDidMount() {
    this.props.fetchMediaServerSchema();
  }

  //
  // Listeners

  onMediaServerSelect = ({ implementation, name }) => {
    this.props.selectMediaServerSchema({ implementation, presetName: name });
    this.props.onModalClose({ mediaServerSelected: true });
  };

  //
  // Render

  render() {
    return (
      <AddMediaServerModalContent
        {...this.props}
        onMediaServerSelect={this.onMediaServerSelect}
      />
    );
  }
}

AddMediaServerModalContentConnector.propTypes = {
  fetchMediaServerSchema: PropTypes.func.isRequired,
  selectMediaServerSchema: PropTypes.func.isRequired,
  onModalClose: PropTypes.func.isRequired
};

export default connect(createMapStateToProps, mapDispatchToProps)(AddMediaServerModalContentConnector);
