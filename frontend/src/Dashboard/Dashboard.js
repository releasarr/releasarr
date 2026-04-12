import React from 'react';
import Alert from 'Components/Alert';
import LoadingIndicator from 'Components/Loading/LoadingIndicator';
import PageContent from 'Components/Page/PageContent';
import PageContentBody from 'Components/Page/PageContentBody';
import { kinds } from 'Helpers/Props';
import translate from 'Utilities/String/translate';
import styles from './Dashboard.css';

function Dashboard({ item, isFetching, isPopulated, error }) {
  const stats = item || {};

  return (
    <PageContent title={translate('Dashboard')}>
      <PageContentBody>
        {
          isFetching && !isPopulated &&
            <LoadingIndicator />
        }

        {
          !isFetching && error &&
            <Alert kind={kinds.DANGER}>
              Unable to load dashboard
            </Alert>
        }

        {
          isPopulated && !error &&
            <div className={styles.dashboard}>
              <div className={styles.card}>
                <div className={styles.cardValue}>{stats.watchlisted || 0}</div>
                <div className={styles.cardLabel}>Watchlisted</div>
              </div>
              <div className={styles.card}>
                <div className={styles.cardValue}>{stats.monitored || 0}</div>
                <div className={styles.cardLabel}>Monitored</div>
              </div>
              <div className={styles.card}>
                <div className={styles.cardValue}>{stats.downloading || 0}</div>
                <div className={styles.cardLabel}>Downloading</div>
              </div>
              <div className={styles.card}>
                <div className={styles.cardValue}>{stats.available || 0}</div>
                <div className={styles.cardLabel}>Available</div>
              </div>
              <div className={styles.card}>
                <div className={styles.cardValue}>{stats.notified || 0}</div>
                <div className={styles.cardLabel}>Notified</div>
              </div>
              <div className={styles.card}>
                <div className={styles.cardValue}>{stats.total || 0}</div>
                <div className={styles.cardLabel}>Total</div>
              </div>
            </div>
        }
      </PageContentBody>
    </PageContent>
  );
}

export default Dashboard;
