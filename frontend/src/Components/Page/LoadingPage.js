import React from 'react';
import LoadingIndicator from 'Components/Loading/LoadingIndicator';
import LoadingMessage from 'Components/Loading/LoadingMessage';
import styles from './LoadingPage.css';

const releasarrLogo = 'data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCA1MTIgNTEyIj4KICA8ZGVmcz4KICAgIDxsaW5lYXJHcmFkaWVudCBpZD0iYmciIHgxPSIwJSIgeTE9IjAlIiB4Mj0iMTAwJSIgeTI9IjEwMCUiPgogICAgICA8c3RvcCBvZmZzZXQ9IjAlIiBzdHlsZT0ic3RvcC1jb2xvcjojMDBCNEQ4Ii8+CiAgICAgIDxzdG9wIG9mZnNldD0iMTAwJSIgc3R5bGU9InN0b3AtY29sb3I6IzAwOTZCNyIvPgogICAgPC9saW5lYXJHcmFkaWVudD4KICA8L2RlZnM+CiAgPCEtLSBCYWNrZ3JvdW5kIGNpcmNsZSAtLT4KICA8Y2lyY2xlIGN4PSIyNTYiIGN5PSIyNTYiIHI9IjI0MCIgZmlsbD0idXJsKCNiZykiLz4KICA8IS0tIEJlbGwgc2hhcGUgLS0+CiAgPHBhdGggZD0iTTI1NiAxMDBjLTU1IDAtMTAwIDQwLTEwMCA5NXY3NWwtMzAgMzB2MTVoMjYwdi0xNWwtMzAtMzB2LTc1YzAtNTUtNDUtOTUtMTAwLTk1eiIgZmlsbD0iI2ZmZiIvPgogIDwhLS0gQmVsbCBjbGFwcGVyIC0tPgogIDxjaXJjbGUgY3g9IjI1NiIgY3k9IjM5NSIgcj0iMjgiIGZpbGw9IiNmZmYiLz4KICA8IS0tIFBsYXkgdHJpYW5nbGUgb3ZlcmxheSBvbiBiZWxsIC0tPgogIDxwYXRoIGQ9Ik0yMzIgMTgwbDcwIDQ1LTcwIDQ1eiIgZmlsbD0iIzAwQjREOCIvPgo8L3N2Zz4K';

function LoadingPage() {
  return (
    <div className={styles.page}>
      <img
        className={styles.logoFull}
        src={releasarrLogo}
      />
      <LoadingMessage />
      <LoadingIndicator />
    </div>
  );
}

export default LoadingPage;
