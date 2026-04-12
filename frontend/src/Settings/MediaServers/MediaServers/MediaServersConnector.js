import PropTypes from 'prop-types';
import React, { Component } from 'react';
import { connect } from 'react-redux';
import { createSelector } from 'reselect';
import { deleteMediaServer, fetchMediaServers } from 'Store/Actions/settingsActions';
import createSortedSectionSelector from 'Store/Selectors/createSortedSectionSelector';
import createTagsSelector from 'Store/Selectors/createTagsSelector';
import sortByProp from 'Utilities/Array/sortByProp';
import MediaServers from './MediaServers';

function createMapStateToProps() {
  return createSelector(
    createSortedSectionSelector('settings.mediaServers', sortByProp('name')),
    createTagsSelector(),
    (mediaServers, tagList) => {
      return {
        ...mediaServers,
        tagList
      };
    }
  );
}

const mapDispatchToProps = {
  fetchMediaServers,
  deleteMediaServer
};

class MediaServersConnector extends Component {

  //
  // Lifecycle

  componentDidMount() {
    this.props.fetchMediaServers();
  }

  //
  // Listeners

  onConfirmDeleteMediaServer = (id) => {
    this.props.deleteMediaServer({ id });
  };

  //
  // Render

  render() {
    return (
      <MediaServers
        {...this.props}
        onConfirmDeleteMediaServer={this.onConfirmDeleteMediaServer}
      />
    );
  }
}

MediaServersConnector.propTypes = {
  fetchMediaServers: PropTypes.func.isRequired,
  deleteMediaServer: PropTypes.func.isRequired
};

export default connect(createMapStateToProps, mapDispatchToProps)(MediaServersConnector);
