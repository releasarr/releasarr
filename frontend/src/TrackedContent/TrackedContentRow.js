import PropTypes from 'prop-types';
import React, { Component } from 'react';
import Label from 'Components/Label';
import IconButton from 'Components/Link/IconButton';
import RelativeDateCell from 'Components/Table/Cells/RelativeDateCell';
import TableRowCell from 'Components/Table/Cells/TableRowCell';
import TableRow from 'Components/Table/TableRow';
import { icons, kinds } from 'Helpers/Props';
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
      contentType,
      status,
      addedAt,
      availableAt,
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
                  {title}
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

            if (name === 'addedAt') {
              return (
                <RelativeDateCell
                  key={name}
                  className={styles.date}
                  date={addedAt}
                />
              );
            }

            if (name === 'availableAt') {
              return (
                <RelativeDateCell
                  key={name}
                  className={styles.date}
                  date={availableAt}
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
  contentType: PropTypes.string.isRequired,
  status: PropTypes.string.isRequired,
  addedAt: PropTypes.string,
  availableAt: PropTypes.string,
  columns: PropTypes.arrayOf(PropTypes.object).isRequired,
  onDeletePress: PropTypes.func.isRequired
};

export default TrackedContentRow;
