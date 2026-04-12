import PropTypes from 'prop-types';
import React, { Component } from 'react';
import { connect } from 'react-redux';
import { createSelector } from 'reselect';
import { fetchArrClientSchema, selectArrClientSchema } from 'Store/Actions/settingsActions';
import AddArrClientModalContent from './AddArrClientModalContent';

function createMapStateToProps() {
  return createSelector(
    (state) => state.settings.arrClients,
    (arrClients) => {
      const {
        isSchemaFetching,
        isSchemaPopulated,
        schemaError,
        schema
      } = arrClients;

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
  fetchArrClientSchema,
  selectArrClientSchema
};

class AddArrClientModalContentConnector extends Component {

  //
  // Lifecycle

  componentDidMount() {
    this.props.fetchArrClientSchema();
  }

  //
  // Listeners

  onArrClientSelect = ({ implementation, name }) => {
    this.props.selectArrClientSchema({ implementation, presetName: name });
    this.props.onModalClose({ arrClientSelected: true });
  };

  //
  // Render

  render() {
    return (
      <AddArrClientModalContent
        {...this.props}
        onArrClientSelect={this.onArrClientSelect}
      />
    );
  }
}

AddArrClientModalContentConnector.propTypes = {
  fetchArrClientSchema: PropTypes.func.isRequired,
  selectArrClientSchema: PropTypes.func.isRequired,
  onModalClose: PropTypes.func.isRequired
};

export default connect(createMapStateToProps, mapDispatchToProps)(AddArrClientModalContentConnector);
