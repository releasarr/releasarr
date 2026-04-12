import PropTypes from 'prop-types';
import React, { Component } from 'react';
import Card from 'Components/Card';
import ConfirmModal from 'Components/Modal/ConfirmModal';
import TagList from 'Components/TagList';
import { kinds } from 'Helpers/Props';
import translate from 'Utilities/String/translate';
import EditMediaServerModalConnector from './EditMediaServerModalConnector';
import styles from './MediaServer.css';

class MediaServer extends Component {

  //
  // Lifecycle

  constructor(props, context) {
    super(props, context);

    this.state = {
      isEditMediaServerModalOpen: false,
      isDeleteMediaServerModalOpen: false
    };
  }

  //
  // Listeners

  onEditMediaServerPress = () => {
    this.setState({ isEditMediaServerModalOpen: true });
  };

  onEditMediaServerModalClose = () => {
    this.setState({ isEditMediaServerModalOpen: false });
  };

  onDeleteMediaServerPress = () => {
    this.setState({
      isEditMediaServerModalOpen: false,
      isDeleteMediaServerModalOpen: true
    });
  };

  onDeleteMediaServerModalClose = () => {
    this.setState({ isDeleteMediaServerModalOpen: false });
  };

  onConfirmDeleteMediaServer = () => {
    this.props.onConfirmDeleteMediaServer(this.props.id);
  };

  //
  // Render

  render() {
    const {
      id,
      name,
      tags,
      tagList
    } = this.props;

    return (
      <Card
        className={styles.mediaServer}
        overlayContent={true}
        onPress={this.onEditMediaServerPress}
      >
        <div className={styles.name}>
          {name}
        </div>

        <TagList
          tags={tags}
          tagList={tagList}
        />

        <EditMediaServerModalConnector
          id={id}
          isOpen={this.state.isEditMediaServerModalOpen}
          onModalClose={this.onEditMediaServerModalClose}
          onDeleteMediaServerPress={this.onDeleteMediaServerPress}
        />

        <ConfirmModal
          isOpen={this.state.isDeleteMediaServerModalOpen}
          kind={kinds.DANGER}
          title={translate('DeleteMediaServer')}
          message={translate('DeleteMediaServerMessageText', { name })}
          confirmLabel={translate('Delete')}
          onConfirm={this.onConfirmDeleteMediaServer}
          onCancel={this.onDeleteMediaServerModalClose}
        />
      </Card>
    );
  }
}

MediaServer.propTypes = {
  id: PropTypes.number.isRequired,
  name: PropTypes.string.isRequired,
  tags: PropTypes.arrayOf(PropTypes.number).isRequired,
  tagList: PropTypes.arrayOf(PropTypes.object).isRequired,
  onConfirmDeleteMediaServer: PropTypes.func.isRequired
};

export default MediaServer;
