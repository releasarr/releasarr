import PropTypes from 'prop-types';
import React, { Component } from 'react';
import Card from 'Components/Card';
import FieldSet from 'Components/FieldSet';
import Icon from 'Components/Icon';
import PageSectionContent from 'Components/Page/PageSectionContent';
import { icons } from 'Helpers/Props';
import translate from 'Utilities/String/translate';
import AddArrClientModal from './AddArrClientModal';
import EditArrClientModalConnector from './EditArrClientModalConnector';
import ArrClient from './ArrClient';
import styles from './ArrClients.css';

class ArrClients extends Component {

  //
  // Lifecycle

  constructor(props, context) {
    super(props, context);

    this.state = {
      isAddArrClientModalOpen: false,
      isEditArrClientModalOpen: false
    };
  }

  //
  // Listeners

  onAddArrClientPress = () => {
    this.setState({ isAddArrClientModalOpen: true });
  };

  onAddArrClientModalClose = ({ arrClientSelected = false } = {}) => {
    this.setState({
      isAddArrClientModalOpen: false,
      isEditArrClientModalOpen: arrClientSelected
    });
  };

  onEditArrClientModalClose = () => {
    this.setState({ isEditArrClientModalOpen: false });
  };

  //
  // Render

  render() {
    const {
      items,
      tagList,
      onConfirmDeleteArrClient,
      ...otherProps
    } = this.props;

    const {
      isAddArrClientModalOpen,
      isEditArrClientModalOpen
    } = this.state;

    return (
      <FieldSet legend={translate('ArrClients')}>
        <PageSectionContent
          errorMessage={translate('UnableToLoadArrClients')}
          {...otherProps}
        >
          <div className={styles.arrClients}>
            {
              items.map((item) => {
                return (
                  <ArrClient
                    key={item.id}
                    {...item}
                    tagList={tagList}
                    onConfirmDeleteArrClient={onConfirmDeleteArrClient}
                  />
                );
              })
            }

            <Card
              className={styles.addArrClient}
              onPress={this.onAddArrClientPress}
            >
              <div className={styles.center}>
                <Icon
                  name={icons.ADD}
                  size={45}
                />
              </div>
            </Card>
          </div>

          <AddArrClientModal
            isOpen={isAddArrClientModalOpen}
            onModalClose={this.onAddArrClientModalClose}
          />

          <EditArrClientModalConnector
            isOpen={isEditArrClientModalOpen}
            onModalClose={this.onEditArrClientModalClose}
          />
        </PageSectionContent>
      </FieldSet>
    );
  }
}

ArrClients.propTypes = {
  isFetching: PropTypes.bool.isRequired,
  error: PropTypes.object,
  items: PropTypes.arrayOf(PropTypes.object).isRequired,
  tagList: PropTypes.arrayOf(PropTypes.object).isRequired,
  onConfirmDeleteArrClient: PropTypes.func.isRequired
};

export default ArrClients;
