import PropTypes from 'prop-types';
import React, { Component } from 'react';
import Card from 'Components/Card';
import ConfirmModal from 'Components/Modal/ConfirmModal';
import TagList from 'Components/TagList';
import { kinds } from 'Helpers/Props';
import translate from 'Utilities/String/translate';
import EditArrClientModalConnector from './EditArrClientModalConnector';
import styles from './ArrClient.css';

class ArrClient extends Component {

  //
  // Lifecycle

  constructor(props, context) {
    super(props, context);

    this.state = {
      isEditArrClientModalOpen: false,
      isDeleteArrClientModalOpen: false
    };
  }

  //
  // Listeners

  onEditArrClientPress = () => {
    this.setState({ isEditArrClientModalOpen: true });
  };

  onEditArrClientModalClose = () => {
    this.setState({ isEditArrClientModalOpen: false });
  };

  onDeleteArrClientPress = () => {
    this.setState({
      isEditArrClientModalOpen: false,
      isDeleteArrClientModalOpen: true
    });
  };

  onDeleteArrClientModalClose = () => {
    this.setState({ isDeleteArrClientModalOpen: false });
  };

  onConfirmDeleteArrClient = () => {
    this.props.onConfirmDeleteArrClient(this.props.id);
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
        className={styles.arrClient}
        overlayContent={true}
        onPress={this.onEditArrClientPress}
      >
        <div className={styles.name}>
          {name}
        </div>

        <TagList
          tags={tags}
          tagList={tagList}
        />

        <EditArrClientModalConnector
          id={id}
          isOpen={this.state.isEditArrClientModalOpen}
          onModalClose={this.onEditArrClientModalClose}
          onDeleteArrClientPress={this.onDeleteArrClientPress}
        />

        <ConfirmModal
          isOpen={this.state.isDeleteArrClientModalOpen}
          kind={kinds.DANGER}
          title={translate('DeleteArrClient')}
          message={translate('DeleteArrClientMessageText', { name })}
          confirmLabel={translate('Delete')}
          onConfirm={this.onConfirmDeleteArrClient}
          onCancel={this.onDeleteArrClientModalClose}
        />
      </Card>
    );
  }
}

ArrClient.propTypes = {
  id: PropTypes.number.isRequired,
  name: PropTypes.string.isRequired,
  tags: PropTypes.arrayOf(PropTypes.number).isRequired,
  tagList: PropTypes.arrayOf(PropTypes.object).isRequired,
  onConfirmDeleteArrClient: PropTypes.func.isRequired
};

export default ArrClient;
