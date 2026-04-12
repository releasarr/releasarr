import PropTypes from 'prop-types';
import React, { Component } from 'react';
import Label from 'Components/Label';
import IconButton from 'Components/Link/IconButton';
import ProgressBar from 'Components/ProgressBar';
import RelativeDateCell from 'Components/Table/Cells/RelativeDateCell';
import TableRowCell from 'Components/Table/Cells/TableRowCell';
import TableRow from 'Components/Table/TableRow';
import { icons, kinds, sizes } from 'Helpers/Props';
import translate from 'Utilities/String/translate';
import styles from './TrackedContentRow.css';

function getStatusKind(status) {
  switch (status) {
    case 'watchlisted':
      return kinds.DEFAULT;
    case 'monitored':
      return kinds.PRIMARY;
    case 'downloading':
      return kinds.WARNING;
    case 'available':
      return kinds.SUCCESS;
    case 'notified':
      return kinds.INFO;
    default:
      return kinds.DEFAULT;
  }
}

function getSeriesStatusKind(seriesStatus) {
  if (!seriesStatus) {
    return kinds.DEFAULT;
  }

  switch (seriesStatus.toLowerCase()) {
    case 'continuing':
      return kinds.PRIMARY;
    case 'ended':
      return kinds.DEFAULT;
    case 'upcoming':
      return kinds.WARNING;
    default:
      return kinds.DEFAULT;
  }
}

class TrackedContentRow extends Component {

  //
  // Listeners

  onDeletePress = () => {
    this.props.onDeletePress(this.props.id);
  };

  //
  // Render

  render() {
    const {
      title,
      posterUrl,
      contentType,
      status,
      addedAt,
      episodeFileCount,
      totalEpisodeCount,
      network,
      studio,
      seriesStatus,
      nextEpisodeAirDateUtc,
      queueStatus,
      queueTimeleft,
      columns
    } = this.props;

    return (
      <TableRow>
        {
          columns.map((column) => {
            const {
              name,
              isVisible
            } = column;

            if (!isVisible) {
              return null;
            }

            if (name === 'title') {
              return (
                <TableRowCell
                  key={name}
                  className={styles.title}
                >
                  <div className={styles.titleContent}>
                    {
                      posterUrl ?
                        <img
                          className={styles.poster}
                          src={posterUrl}
                          alt={title}
                        /> :
                        null
                    }
                    <span>{title}</span>
                  </div>
                </TableRowCell>
              );
            }

            if (name === 'contentType') {
              return (
                <TableRowCell
                  key={name}
                  className={styles.contentType}
                >
                  <Label kind={kinds.INFO}>
                    {contentType}
                  </Label>
                </TableRowCell>
              );
            }

            if (name === 'status') {
              return (
                <TableRowCell
                  key={name}
                  className={styles.status}
                >
                  <Label kind={getStatusKind(status)}>
                    {status}
                  </Label>
                </TableRowCell>
              );
            }

            if (name === 'progress') {
              if (contentType === 'series' && totalEpisodeCount != null && totalEpisodeCount > 0) {
                const fileCount = episodeFileCount || 0;
                const percent = Math.round((fileCount / totalEpisodeCount) * 100);

                return (
                  <TableRowCell
                    key={name}
                    className={styles.progress}
                  >
                    <ProgressBar
                      progress={percent}
                      kind={percent === 100 ? kinds.SUCCESS : kinds.PRIMARY}
                      size={sizes.SMALL}
                      showText={true}
                      text={`${fileCount}/${totalEpisodeCount}`}
                    />
                  </TableRowCell>
                );
              }

              return (
                <TableRowCell
                  key={name}
                  className={styles.progress}
                >
                  {contentType === 'movie' ? '\u2014' : ''}
                </TableRowCell>
              );
            }

            if (name === 'network') {
              return (
                <TableRowCell
                  key={name}
                  className={styles.network}
                >
                  {network || studio || '\u2014'}
                </TableRowCell>
              );
            }

            if (name === 'seriesStatus') {
              return (
                <TableRowCell
                  key={name}
                  className={styles.seriesStatus}
                >
                  {
                    seriesStatus ?
                      <Label kind={getSeriesStatusKind(seriesStatus)}>
                        {seriesStatus}
                      </Label> :
                      '\u2014'
                  }
                </TableRowCell>
              );
            }

            if (name === 'nextAiring') {
              return nextEpisodeAirDateUtc ?
                <RelativeDateCell
                  key={name}
                  className={styles.date}
                  date={nextEpisodeAirDateUtc}
                /> :
                <TableRowCell
                  key={name}
                  className={styles.date}
                >
                  {'\u2014'}
                </TableRowCell>;
            }

            if (name === 'queue') {
              return (
                <TableRowCell
                  key={name}
                  className={styles.queue}
                >
                  {
                    queueStatus ?
                      <span>
                        <Label kind={kinds.WARNING}>{queueStatus}</Label>
                        {queueTimeleft ? ` ${queueTimeleft}` : ''}
                      </span> :
                      '\u2014'
                  }
                </TableRowCell>
              );
            }

            if (name === 'addedAt') {
              return (
                <RelativeDateCell
                  key={name}
                  className={styles.date}
                  date={addedAt}
                />
              );
            }

            if (name === 'actions') {
              return (
                <TableRowCell
                  key={name}
                  className={styles.actions}
                >
                  <IconButton
                    name={icons.DELETE}
                    title={translate('Delete')}
                    onPress={this.onDeletePress}
                  />
                </TableRowCell>
              );
            }

            return null;
          })
        }
      </TableRow>
    );
  }
}

TrackedContentRow.propTypes = {
  id: PropTypes.number.isRequired,
  title: PropTypes.string.isRequired,
  posterUrl: PropTypes.string,
  contentType: PropTypes.string.isRequired,
  status: PropTypes.string.isRequired,
  addedAt: PropTypes.string,
  availableAt: PropTypes.string,
  episodeFileCount: PropTypes.number,
  totalEpisodeCount: PropTypes.number,
  network: PropTypes.string,
  studio: PropTypes.string,
  seriesStatus: PropTypes.string,
  nextEpisodeAirDateUtc: PropTypes.string,
  queueStatus: PropTypes.string,
  queueTimeleft: PropTypes.string,
  columns: PropTypes.arrayOf(PropTypes.object).isRequired,
  onDeletePress: PropTypes.func.isRequired
};

export default TrackedContentRow;
