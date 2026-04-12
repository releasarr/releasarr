import PropTypes from 'prop-types';
import React, { Component } from 'react';
import { connect } from 'react-redux';
import { createSelector } from 'reselect';
import { fetchTrackedContent, deleteTrackedItem } from 'Store/Actions/trackedContentActions';
import TrackedContent from './TrackedContent';

function createMapStateToProps() {
  return createSelector(
    (state) => state.trackedContent,
    (trackedContent) => {
      return {
        ...trackedContent
      };
    }
  );
}

const mapDispatchToProps = {
  fetchTrackedContent,
  deleteTrackedItem
};

class TrackedContentConnector extends Component {

  //
  // Lifecycle

  componentDidMount() {
    this.props.fetchTrackedContent();
  }

  //
  // Listeners

  onRefreshPress = () => {
    this.props.fetchTrackedContent();
  };

  onDeletePress = (id) => {
    this.props.deleteTrackedItem({ id });
  };

  //
  // Render

  render() {
    return (
      <TrackedContent
        {...this.props}
        onRefreshPress={this.onRefreshPress}
        onDeletePress={this.onDeletePress}
      />
    );
  }
}

TrackedContentConnector.propTypes = {
  fetchTrackedContent: PropTypes.func.isRequired,
  deleteTrackedItem: PropTypes.func.isRequired
};

export default connect(createMapStateToProps, mapDispatchToProps)(TrackedContentConnector);
