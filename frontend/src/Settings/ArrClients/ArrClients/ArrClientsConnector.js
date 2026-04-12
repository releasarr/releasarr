import PropTypes from 'prop-types';
import React, { Component } from 'react';
import { connect } from 'react-redux';
import { createSelector } from 'reselect';
import { deleteArrClient, fetchArrClients } from 'Store/Actions/settingsActions';
import createSortedSectionSelector from 'Store/Selectors/createSortedSectionSelector';
import createTagsSelector from 'Store/Selectors/createTagsSelector';
import sortByProp from 'Utilities/Array/sortByProp';
import ArrClients from './ArrClients';

function createMapStateToProps() {
  return createSelector(
    createSortedSectionSelector('settings.arrClients', sortByProp('name')),
    createTagsSelector(),
    (arrClients, tagList) => {
      return {
        ...arrClients,
        tagList
      };
    }
  );
}

const mapDispatchToProps = {
  fetchArrClients,
  deleteArrClient
};

class ArrClientsConnector extends Component {

  //
  // Lifecycle

  componentDidMount() {
    this.props.fetchArrClients();
  }

  //
  // Listeners

  onConfirmDeleteArrClient = (id) => {
    this.props.deleteArrClient({ id });
  };

  //
  // Render

  render() {
    return (
      <ArrClients
        {...this.props}
        onConfirmDeleteArrClient={this.onConfirmDeleteArrClient}
      />
    );
  }
}

ArrClientsConnector.propTypes = {
  fetchArrClients: PropTypes.func.isRequired,
  deleteArrClient: PropTypes.func.isRequired
};

export default connect(createMapStateToProps, mapDispatchToProps)(ArrClientsConnector);
