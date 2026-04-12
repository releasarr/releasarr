import PropTypes from 'prop-types';
import React, { Component } from 'react';
import Card from 'Components/Card';
import FieldSet from 'Components/FieldSet';
import Icon from 'Components/Icon';
import PageSectionContent from 'Components/Page/PageSectionContent';
import { icons } from 'Helpers/Props';
import translate from 'Utilities/String/translate';
import AddMediaServerModal from './AddMediaServerModal';
import EditMediaServerModalConnector from './EditMediaServerModalConnector';
import MediaServer from './MediaServer';
import styles from './MediaServers.css';

class MediaServers extends Component {

  //
  // Lifecycle

  constructor(props, context) {
    super(props, context);

    this.state = {
      isAddMediaServerModalOpen: false,
      isEditMediaServerModalOpen: false
    };
  }

  //
  // Listeners

  onAddMediaServerPress = () => {
    this.setState({ isAddMediaServerModalOpen: true });
  };

  onAddMediaServerModalClose = ({ mediaServerSelected = false } = {}) => {
    this.setState({
      isAddMediaServerModalOpen: false,
      isEditMediaServerModalOpen: mediaServerSelected
    });
  };

  onEditMediaServerModalClose = () => {
    this.setState({ isEditMediaServerModalOpen: false });
  };

  //
  // Render

  render() {
    const {
      items,
      tagList,
      onConfirmDeleteMediaServer,
      ...otherProps
    } = this.props;

    const {
      isAddMediaServerModalOpen,
      isEditMediaServerModalOpen
    } = this.state;

    return (
      <FieldSet legend={translate('MediaServers')}>
        <PageSectionContent
          errorMessage={translate('UnableToLoadMediaServers')}
          {...otherProps}
        >
          <div className={styles.mediaServers}>
            {
              items.map((item) => {
                return (
                  <MediaServer
                    key={item.id}
                    {...item}
                    tagList={tagList}
                    onConfirmDeleteMediaServer={onConfirmDeleteMediaServer}
                  />
                );
              })
            }

            <Card
              className={styles.addMediaServer}
              onPress={this.onAddMediaServerPress}
            >
              <div className={styles.center}>
                <Icon
                  name={icons.ADD}
                  size={45}
                />
              </div>
            </Card>
          </div>

          <AddMediaServerModal
            isOpen={isAddMediaServerModalOpen}
            onModalClose={this.onAddMediaServerModalClose}
          />

          <EditMediaServerModalConnector
            isOpen={isEditMediaServerModalOpen}
            onModalClose={this.onEditMediaServerModalClose}
          />
        </PageSectionContent>
      </FieldSet>
    );
  }
}

MediaServers.propTypes = {
  isFetching: PropTypes.bool.isRequired,
  error: PropTypes.object,
  items: PropTypes.arrayOf(PropTypes.object).isRequired,
  tagList: PropTypes.arrayOf(PropTypes.object).isRequired,
  onConfirmDeleteMediaServer: PropTypes.func.isRequired
};

export default MediaServers;
