import PropTypes from 'prop-types';
import React from 'react';
import Alert from 'Components/Alert';
import LoadingIndicator from 'Components/Loading/LoadingIndicator';
import PageContent from 'Components/Page/PageContent';
import PageContentBody from 'Components/Page/PageContentBody';
import PageToolbar from 'Components/Page/Toolbar/PageToolbar';
import PageToolbarButton from 'Components/Page/Toolbar/PageToolbarButton';
import PageToolbarSection from 'Components/Page/Toolbar/PageToolbarSection';
import Table from 'Components/Table/Table';
import TableBody from 'Components/Table/TableBody';
import { icons, kinds } from 'Helpers/Props';
import translate from 'Utilities/String/translate';
import TrackedContentRow from './TrackedContentRow';

const columns = [
  {
    name: 'title',
    label: () => translate('Title'),
    isVisible: true
  },
  {
    name: 'contentType',
    label: () => translate('Type'),
    isVisible: true
  },
  {
    name: 'status',
    label: () => translate('Status'),
    isVisible: true
  },
  {
    name: 'progress',
    label: () => 'Progress',
    isVisible: true
  },
  {
    name: 'network',
    label: () => 'Network',
    isVisible: true
  },
  {
    name: 'seriesStatus',
    label: () => 'Series Status',
    isVisible: true
  },
  {
    name: 'nextAiring',
    label: () => 'Next Airing',
    isVisible: true
  },
  {
    name: 'queue',
    label: () => 'Queue',
    isVisible: true
  },
  {
    name: 'addedAt',
    label: () => translate('Added'),
    isVisible: true
  },
  {
    name: 'actions',
    label: '',
    isVisible: true
  }
];

function TrackedContent({ items, isFetching, isPopulated, error, onRefreshPress, onDeletePress }) {
  return (
    <PageContent title="Tracked Content">
      <PageToolbar>
        <PageToolbarSection>
          <PageToolbarButton
            label={translate('Refresh')}
            iconName={icons.REFRESH}
            isSpinning={isFetching}
            onPress={onRefreshPress}
          />
        </PageToolbarSection>
      </PageToolbar>

      <PageContentBody>
        {
          isFetching && !isPopulated &&
            <LoadingIndicator />
        }

        {
          !isFetching && error &&
            <Alert kind={kinds.DANGER}>
              Unable to load tracked content
            </Alert>
        }

        {
          isPopulated && !error && !items.length &&
            <Alert kind={kinds.INFO}>
              No tracked content found
            </Alert>
        }

        {
          isPopulated && !error && !!items.length &&
            <Table columns={columns}>
              <TableBody>
                {
                  items.map((item) => {
                    return (
                      <TrackedContentRow
                        key={item.id}
                        columns={columns}
                        {...item}
                        onDeletePress={onDeletePress}
                      />
                    );
                  })
                }
              </TableBody>
            </Table>
        }
      </PageContentBody>
    </PageContent>
  );
}

TrackedContent.propTypes = {
  items: PropTypes.arrayOf(PropTypes.object).isRequired,
  isFetching: PropTypes.bool.isRequired,
  isPopulated: PropTypes.bool.isRequired,
  error: PropTypes.object,
  onRefreshPress: PropTypes.func.isRequired,
  onDeletePress: PropTypes.func.isRequired
};

TrackedContent.defaultProps = {
  items: []
};

export default TrackedContent;
