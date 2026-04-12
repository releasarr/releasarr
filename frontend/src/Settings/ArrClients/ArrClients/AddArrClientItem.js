import PropTypes from 'prop-types';
import React, { Component } from 'react';
import Button from 'Components/Link/Button';
import Link from 'Components/Link/Link';
import Menu from 'Components/Menu/Menu';
import MenuContent from 'Components/Menu/MenuContent';
import { sizes } from 'Helpers/Props';
import translate from 'Utilities/String/translate';
import AddArrClientPresetMenuItem from './AddArrClientPresetMenuItem';
import styles from './AddArrClientItem.css';

class AddArrClientItem extends Component {

  //
  // Listeners

  onArrClientSelect = () => {
    const {
      implementation
    } = this.props;

    this.props.onArrClientSelect({ implementation });
  };

  //
  // Render

  render() {
    const {
      implementation,
      implementationName,
      infoLink,
      presets,
      onArrClientSelect
    } = this.props;

    const hasPresets = !!presets && !!presets.length;

    return (
      <div
        className={styles.arrClient}
      >
        <Link
          className={styles.underlay}
          onPress={this.onArrClientSelect}
        />

        <div className={styles.overlay}>
          <div className={styles.name}>
            {implementationName}
          </div>

          <div className={styles.actions}>
            {
              hasPresets &&
                <span>
                  <Button
                    size={sizes.SMALL}
                    onPress={this.onArrClientSelect}
                  >
                    Custom
                  </Button>

                  <Menu className={styles.presetsMenu}>
                    <Button
                      className={styles.presetsMenuButton}
                      size={sizes.SMALL}
                    >
                      Presets
                    </Button>

                    <MenuContent>
                      {
                        presets.map((preset) => {
                          return (
                            <AddArrClientPresetMenuItem
                              key={preset.name}
                              name={preset.name}
                              implementation={implementation}
                              onPress={onArrClientSelect}
                            />
                          );
                        })
                      }
                    </MenuContent>
                  </Menu>
                </span>
            }

            <Button
              to={infoLink}
              size={sizes.SMALL}
            >
              {translate('MoreInfo')}
            </Button>
          </div>
        </div>
      </div>
    );
  }
}

AddArrClientItem.propTypes = {
  implementation: PropTypes.string.isRequired,
  implementationName: PropTypes.string.isRequired,
  infoLink: PropTypes.string.isRequired,
  presets: PropTypes.arrayOf(PropTypes.object),
  onArrClientSelect: PropTypes.func.isRequired
};

export default AddArrClientItem;
