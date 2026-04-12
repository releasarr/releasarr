import PropTypes from 'prop-types';
import React, { Component } from 'react';
import Label from 'Components/Label';
import IconButton from 'Components/Link/IconButton';
import RelativeDateCell from 'Components/Table/Cells/RelativeDateCell';
import TableRowCell from 'Components/Table/Cells/TableRowCell';
import TableRow from 'Components/Table/TableRow';
import { icons, kinds } from 'Helpers/Props';
import translate from 'Utilities/String/translate';
import HistoryDetailsModal from './Details/HistoryDetailsModal';
import * as historyDataTypes from './historyDataTypes';
import HistoryEventTypeCell from './HistoryEventTypeCell';
import HistoryRowParameter from './HistoryRowParameter';
import styles from './HistoryRow.css';

export const historyParameters = [
  { key: historyDataTypes.IMDB_ID, title: 'IMDb' },
  { key: historyDataTypes.TMDB_ID, title: 'TMDb' },
  { key: historyDataTypes.TVDB_ID, title: 'TVDb' },
  { key: historyDataTypes.TRAKT_ID, title: 'Trakt' },
  { key: historyDataTypes.R_ID, title: 'TvRage' },
  { key: historyDataTypes.TVMAZE_ID, title: 'TvMaze' },
  {
    key: historyDataTypes.SEASON,
    get title() {
      return translate('Season');
    }
  },
  {
    key: historyDataTypes.EPISODE,
    get title() {
      return translate('Episode');
    }
  },
  {
    key: historyDataTypes.ARTIST,
    get title() {
      return translate('Artist');
    }
  },
  {
    key: historyDataTypes.ALBUM,
    get title() {
      return translate('Album');
    }
  },
  {
    key: historyDataTypes.LABEL,
    get title() {
      return translate('Label');
    }
  },
  {
    key: historyDataTypes.TRACK,
    get title() {
      return translate('Track');
    }
  },
  {
    key: historyDataTypes.YEAR,
    get title() {
      return translate('Year');
    }
  },
  {
    key: historyDataTypes.GENRE,
    get title() {
      return translate('Genre');
    }
  },
  {
    key: historyDataTypes.AUTHOR,
    get title() {
      return translate('Author');
    }
  },
  {
    key: historyDataTypes.TITLE,
    get title() {
      return translate('Title');
    }
  },
  {
    key: historyDataTypes.PUBLISHER,
    get title() {
      return translate('Publisher');
    }
  }
];

class HistoryRow extends Component {

  //
  // Lifecycle

  constructor(props, context) {
    super(props, context);

    this.state = {
      isDetailsModalOpen: false
    };
  }

  componentDidUpdate(prevProps) {
    if (
      prevProps.isMarkingAsFailed &&
      !this.props.isMarkingAsFailed &&
      !this.props.markAsFailedError
    ) {
      this.setState({ isDetailsModalOpen: false });
    }
  }

  //
  // Listeners

  onDetailsPress = () => {
    this.setState({ isDetailsModalOpen: true });
  };

  onDetailsModalClose = () => {
    this.setState({ isDetailsModalOpen: false });
  };

  //
  // Render

  render() {
    const {
      indexer,
      eventType,
      date,
      data,
      successful,
      isMarkingAsFailed,
      columns,
      shortDateFormat,
      timeFormat,
      onMarkAsFailedPress
    } = this.props;

    if (!indexer) {
      return null;
    }

    const parameters = historyParameters.filter((parameter) => parameter.key in data && data[parameter.key]);

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

            if (name === 'eventType') {
              return (
                <HistoryEventTypeCell
                  key={name}
                  indexer={indexer}
                  eventType={eventType}
                  data={data}
                  successful={successful}
                />
              );
            }

            if (name === 'indexer') {
              return (
                <TableRowCell
                  key={name}
                  className={styles.indexer}
                >
                  {indexer.name}
                </TableRowCell>
              );
            }

            if (name === 'query') {
              return (
                <TableRowCell
                  key={name}
                  className={styles.query}
                >
                  {data.query}
                </TableRowCell>
              );
            }

            if (name === 'parameters') {
              return (
                <TableRowCell key={name}>
                  <div className={styles.parametersContent}>
                    {parameters.map((parameter) => {
                      return (
                        <HistoryRowParameter
                          key={parameter.key}
                          title={parameter.title}
                          value={data[parameter.key]}
                          queryType={data.queryType}
                        />
                      );
                    }
                    )}
                  </div>
                </TableRowCell>
              );
            }

            if (name === 'grabTitle') {
              return (
                <TableRowCell
                  key={name}
                  className={styles.indexer}
                >
                  {
                    data.grabTitle ?
                      data.grabTitle :
                      null
                  }
                </TableRowCell>
              );
            }

            if (name === 'queryType') {
              return (
                <TableRowCell
                  key={name}
                  className={styles.query}
                >
                  {
                    data.queryType ?
                      <Label kind={kinds.INFO}>
                        {data.queryType}
                      </Label> :
                      null
                  }
                </TableRowCell>
              );
            }

            if (name === 'categories') {
              return (
                <TableRowCell
                  key={name}
                  className={styles.indexer}
                >
                  {
                    data.categories ?
                      data.categories :
                      null
                  }
                </TableRowCell>
              );
            }

            if (name === 'source') {
              return (
                <TableRowCell
                  key={name}
                  className={styles.indexer}
                >
                  {
                    data.source ?
                      data.source :
                      null
                  }
                </TableRowCell>
              );
            }

            if (name === 'host') {
              return (
                <TableRowCell
                  key={name}
                  className={styles.indexer}
                >
                  {
                    data.host ?
                      data.host :
                      null
                  }
                </TableRowCell>
              );
            }

            if (name === 'elapsedTime') {
              return (
                <TableRowCell
                  key={name}
                  className={styles.elapsedTime}
                >
                  {
                    data.elapsedTime ?
                      `${data.elapsedTime}ms` :
                      null
                  }
                  {
                    data.cached === '1' ?
                      ' (cached)' :
                      null
                  }
                </TableRowCell>
              );
            }

            if (name === 'date') {
              return (
                <RelativeDateCell
                  key={name}
                  className={styles.date}
                  date={date}
                  includeSeconds={true}
                />
              );
            }

            if (name === 'details') {
              return (
                <TableRowCell
                  key={name}
                  className={styles.details}
                >
                  <IconButton
                    name={icons.INFO}
                    onPress={this.onDetailsPress}
                    title={translate('HistoryDetails')}
                  />
                </TableRowCell>
              );
            }

            return null;
          })
        }

        <HistoryDetailsModal
          isOpen={this.state.isDetailsModalOpen}
          eventType={eventType}
          date={date}
          data={data}
          indexer={indexer}
          isMarkingAsFailed={isMarkingAsFailed}
          shortDateFormat={shortDateFormat}
          timeFormat={timeFormat}
          onMarkAsFailedPress={onMarkAsFailedPress}
          onModalClose={this.onDetailsModalClose}
        />
      </TableRow>
    );
  }

}

HistoryRow.propTypes = {
  indexerId: PropTypes.number,
  indexer: PropTypes.object.isRequired,
  eventType: PropTypes.string.isRequired,
  successful: PropTypes.bool.isRequired,
  date: PropTypes.string.isRequired,
  data: PropTypes.object.isRequired,
  isMarkingAsFailed: PropTypes.bool,
  markAsFailedError: PropTypes.object,
  columns: PropTypes.arrayOf(PropTypes.object).isRequired,
  shortDateFormat: PropTypes.string.isRequired,
  timeFormat: PropTypes.string.isRequired,
  onMarkAsFailedPress: PropTypes.func.isRequired
};

export default HistoryRow;
