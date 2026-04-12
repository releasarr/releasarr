import PropTypes from 'prop-types';
import React, { Component } from 'react';
import Alert from 'Components/Alert';
import LoadingIndicator from 'Components/Loading/LoadingIndicator';
import PageContent from 'Components/Page/PageContent';
import PageContentBody from 'Components/Page/PageContentBody';
import TextInput from 'Components/Form/TextInput';
import { icons, kinds } from 'Helpers/Props';
import SearchResult from './SearchResult';
import styles from './Search.css';

class Search extends Component {

  //
  // Lifecycle

  constructor(props) {
    super(props);

    this.state = {
      query: ''
    };

    this._searchTimeout = null;
  }

  componentWillUnmount() {
    if (this._searchTimeout) {
      clearTimeout(this._searchTimeout);
    }

    this.props.onClearSearch();
  }

  //
  // Listeners

  onQueryChange = ({ value }) => {
    this.setState({ query: value });

    if (this._searchTimeout) {
      clearTimeout(this._searchTimeout);
    }

    if (value.length >= 2) {
      this._searchTimeout = setTimeout(() => {
        this.props.onSearchPress(value);
      }, 500);
    }
  };

  //
  // Render

  render() {
    const {
      items,
      isFetching,
      isPopulated,
      error,
      isAdding,
      onAddToWatchlistPress
    } = this.props;

    const { query } = this.state;

    return (
      <PageContent title="Search">
        <PageContentBody>
          <div className={styles.searchContainer}>
            <TextInput
              className={styles.searchInput}
              name="searchQuery"
              value={query}
              placeholder="Search for movies or series..."
              onChange={this.onQueryChange}
              autoFocus={true}
            />
          </div>

          {
            isFetching &&
              <LoadingIndicator />
          }

          {
            !isFetching && error &&
              <Alert kind={kinds.DANGER}>
                Unable to search. Check your Sonarr/Radarr connections.
              </Alert>
          }

          {
            isPopulated && !error && !items.length && query.length >= 2 &&
              <Alert kind={kinds.INFO}>
                No results found
              </Alert>
          }

          {
            isPopulated && !error && !!items.length &&
              <div className={styles.results}>
                {
                  items.map((item) => {
                    return (
                      <SearchResult
                        key={item.id}
                        {...item}
                        isAdding={isAdding}
                        onAddPress={onAddToWatchlistPress}
                      />
                    );
                  })
                }
              </div>
          }
        </PageContentBody>
      </PageContent>
    );
  }
}

Search.propTypes = {
  items: PropTypes.arrayOf(PropTypes.object).isRequired,
  isFetching: PropTypes.bool.isRequired,
  isPopulated: PropTypes.bool.isRequired,
  error: PropTypes.object,
  isAdding: PropTypes.bool,
  onSearchPress: PropTypes.func.isRequired,
  onAddToWatchlistPress: PropTypes.func.isRequired,
  onClearSearch: PropTypes.func.isRequired
};

Search.defaultProps = {
  items: []
};

export default Search;
