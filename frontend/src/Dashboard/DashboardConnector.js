import PropTypes from 'prop-types';
import React, { Component } from 'react';
import { connect } from 'react-redux';
import { createSelector } from 'reselect';
import { fetchDashboard } from 'Store/Actions/dashboardActions';
import Dashboard from './Dashboard';

function createMapStateToProps() {
  return createSelector(
    (state) => state.dashboard,
    (dashboard) => {
      return {
        ...dashboard
      };
    }
  );
}

const mapDispatchToProps = {
  fetchDashboard
};

class DashboardConnector extends Component {

  //
  // Lifecycle

  componentDidMount() {
    this.props.fetchDashboard();
  }

  //
  // Render

  render() {
    return (
      <Dashboard {...this.props} />
    );
  }
}

DashboardConnector.propTypes = {
  fetchDashboard: PropTypes.func.isRequired
};

export default connect(createMapStateToProps, mapDispatchToProps)(DashboardConnector);
