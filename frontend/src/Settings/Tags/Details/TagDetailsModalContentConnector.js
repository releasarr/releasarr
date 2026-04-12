import { connect } from 'react-redux';
import { createSelector } from 'reselect';
import TagDetailsModalContent from './TagDetailsModalContent';

function findMatchingItems(ids, items) {
  return items.filter((s) => {
    return ids.includes(s.id);
  });
}

function createMatchingNotificationsSelector() {
  return createSelector(
    (state, { notificationIds }) => notificationIds,
    (state) => state.settings.notifications.items,
    findMatchingItems
  );
}

function createMapStateToProps() {
  return createSelector(
    createMatchingNotificationsSelector(),
    (notifications) => {
      return {
        notifications
      };
    }
  );
}

export default connect(createMapStateToProps)(TagDetailsModalContent);
