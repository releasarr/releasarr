import PropTypes from 'prop-types';
import React, { Component } from 'react';
import Button from 'Components/Link/Button';
import LoadingIndicator from 'Components/Loading/LoadingIndicator';
import ModalBody from 'Components/Modal/ModalBody';
import ModalContent from 'Components/Modal/ModalContent';
import ModalFooter from 'Components/Modal/ModalFooter';
import ModalHeader from 'Components/Modal/ModalHeader';
import translate from 'Utilities/String/translate';
import AddArrClientItem from './AddArrClientItem';
import styles from './AddArrClientModalContent.css';

class AddArrClientModalContent extends Component {

  //
  // Render

  render() {
    const {
      isSchemaFetching,
      isSchemaPopulated,
      schemaError,
      schema,
      onArrClientSelect,
      onModalClose
    } = this.props;

    return (
      <ModalContent onModalClose={onModalClose}>
        <ModalHeader>
          {translate('AddArrClient')}
        </ModalHeader>

        <ModalBody>
          {
            isSchemaFetching &&
              <LoadingIndicator />
          }

          {
            !isSchemaFetching && !!schemaError &&
              <div>
                {translate('UnableToAddANewArrClientPleaseTryAgain')}
              </div>
          }

          {
            isSchemaPopulated && !schemaError &&
              <div>
                <div className={styles.arrClients}>
                  {
                    schema.map((arrClient) => {
                      return (
                        <AddArrClientItem
                          key={arrClient.implementation}
                          implementation={arrClient.implementation}
                          {...arrClient}
                          onArrClientSelect={onArrClientSelect}
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

AddArrClientModalContent.propTypes = {
  isSchemaFetching: PropTypes.bool.isRequired,
  isSchemaPopulated: PropTypes.bool.isRequired,
  schemaError: PropTypes.object,
  schema: PropTypes.arrayOf(PropTypes.object).isRequired,
  onArrClientSelect: PropTypes.func.isRequired,
  onModalClose: PropTypes.func.isRequired
};

export default AddArrClientModalContent;
