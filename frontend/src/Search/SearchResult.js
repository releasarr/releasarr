import PropTypes from 'prop-types';
import React from 'react';
import Label from 'Components/Label';
import SpinnerButton from 'Components/Link/SpinnerButton';
import { kinds } from 'Helpers/Props';
import styles from './SearchResult.css';

function SearchResult({ title, type, year, overview, posterUrl, tmdbId, tvdbId, network, studio, isAdding, onAddPress }) {
  const onAdd = () => {
    onAddPress({ tmdbId, tvdbId });
  };

  return (
    <div className={styles.result}>
      <div className={styles.posterContainer}>
        {
          posterUrl ?
            <img
              className={styles.poster}
              src={posterUrl}
              alt={title}
            /> :
            <div className={styles.posterPlaceholder} />
        }
      </div>

      <div className={styles.info}>
        <div className={styles.titleRow}>
          <span className={styles.title}>{title}</span>
          {year ? <span className={styles.year}>({year})</span> : null}
        </div>

        <div className={styles.meta}>
          <Label kind={type === 'movie' ? kinds.WARNING : kinds.INFO}>
            {type === 'movie' ? 'Movie' : 'Series'}
          </Label>
          {network ? <span className={styles.network}>{network}</span> : null}
          {studio ? <span className={styles.network}>{studio}</span> : null}
        </div>

        {
          overview ?
            <div className={styles.overview}>
              {overview.length > 200 ? `${overview.substring(0, 200)}...` : overview}
            </div> :
            null
        }

        <div className={styles.actions}>
          <SpinnerButton
            kind={kinds.SUCCESS}
            isSpinning={isAdding}
            onPress={onAdd}
          >
            Add to Watchlist
          </SpinnerButton>
        </div>
      </div>
    </div>
  );
}

SearchResult.propTypes = {
  title: PropTypes.string.isRequired,
  type: PropTypes.string.isRequired,
  year: PropTypes.number,
  overview: PropTypes.string,
  posterUrl: PropTypes.string,
  tmdbId: PropTypes.number,
  tvdbId: PropTypes.number,
  network: PropTypes.string,
  studio: PropTypes.string,
  isAdding: PropTypes.bool,
  onAddPress: PropTypes.func.isRequired
};

export default SearchResult;
