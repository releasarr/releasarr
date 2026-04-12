import PropTypes from 'prop-types';
import React, { Component } from 'react';
import { connect } from 'react-redux';
import { createSelector } from 'reselect';
import { fetchSearchResults, addToWatchlist, clearSearch } from 'Store/Actions/searchActions';
import Search from './Search';

function createMapStateToProps() {
  return createSelector(
    (state) => state.search,
    (search) => {
      return {
        ...search
      };
    }
  );
}

const mapDispatchToProps = {
  fetchSearchResults,
  addToWatchlist,
  clearSearch
};

class SearchConnector extends Component {

  //
  // Listeners

  onSearchPress = (query) => {
    this.props.fetchSearchResults({ query });
  };

  onAddToWatchlistPress = ({ tmdbId, tvdbId }) => {
    // Use the first media server ID (Plex)
    // In a multi-server setup, this could be enhanced to let users pick
    this.props.addToWatchlist({
      mediaServerId: 1,
      tmdbId,
      tvdbId
    });
  };

  onClearSearch = () => {
    this.props.clearSearch();
  };

  //
  // Render

  render() {
    return (
      <Search
        {...this.props}
        onSearchPress={this.onSearchPress}
        onAddToWatchlistPress={this.onAddToWatchlistPress}
        onClearSearch={this.onClearSearch}
      />
    );
  }
}

SearchConnector.propTypes = {
  fetchSearchResults: PropTypes.func.isRequired,
  addToWatchlist: PropTypes.func.isRequired,
  clearSearch: PropTypes.func.isRequired
};

export default connect(createMapStateToProps, mapDispatchToProps)(SearchConnector);
