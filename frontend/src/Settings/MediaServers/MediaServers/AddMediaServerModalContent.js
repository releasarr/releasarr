import PropTypes from 'prop-types';
import React, { Component } from 'react';
import Button from 'Components/Link/Button';
import LoadingIndicator from 'Components/Loading/LoadingIndicator';
import ModalBody from 'Components/Modal/ModalBody';
import ModalContent from 'Components/Modal/ModalContent';
import ModalFooter from 'Components/Modal/ModalFooter';
import ModalHeader from 'Components/Modal/ModalHeader';
import translate from 'Utilities/String/translate';
import AddMediaServerItem from './AddMediaServerItem';
import styles from './AddMediaServerModalContent.css';

class AddMediaServerModalContent extends Component {

  //
  // Render

  render() {
    const {
      isSchemaFetching,
      isSchemaPopulated,
      schemaError,
      schema,
      onMediaServerSelect,
      onModalClose
    } = this.props;

    return (
      <ModalContent onModalClose={onModalClose}>
        <ModalHeader>
          {translate('AddMediaServer')}
        </ModalHeader>

        <ModalBody>
          {
            isSchemaFetching &&
              <LoadingIndicator />
          }

          {
            !isSchemaFetching && !!schemaError &&
              <div>
                {translate('UnableToAddANewMediaServerPleaseTryAgain')}
              </div>
          }

          {
            isSchemaPopulated && !schemaError &&
              <div>
                <div className={styles.mediaServers}>
                  {
                    schema.map((mediaServer) => {
                      return (
                        <AddMediaServerItem
                          key={mediaServer.implementation}
                          implementation={mediaServer.implementation}
                          {...mediaServer}
                          onMediaServerSelect={onMediaServerSelect}
                        />
                      );
                    })
                  }
                </div>
              </div>
          }
        </ModalBody>
        <ModalFooter>
          <Button
            onPress={onModalClose}
          >
            {translate('Close')}
          </Button>
        </ModalFooter>
      </ModalContent>
    );
  }
}

AddMediaServerModalContent.propTypes = {
  isSchemaFetching: PropTypes.bool.isRequired,
  isSchemaPopulated: PropTypes.bool.isRequired,
  schemaError: PropTypes.object,
  schema: PropTypes.arrayOf(PropTypes.object).isRequired,
  onMediaServerSelect: PropTypes.func.isRequired,
  onModalClose: PropTypes.func.isRequired
};

export default AddMediaServerModalContent;
