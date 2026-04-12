import * as app from './appActions';
import * as captcha from './captchaActions';
import * as commands from './commandActions';
import * as customFilters from './customFilterActions';
import * as dashboard from './dashboardActions';
import * as history from './historyActions';
import * as localization from './localizationActions';
import * as oAuth from './oAuthActions';
import * as paths from './pathActions';
import * as providerOptions from './providerOptionActions';
import * as search from './searchActions';
import * as settings from './settingsActions';
import * as system from './systemActions';
import * as tags from './tagActions';
import * as trackedContent from './trackedContentActions';

export default [
  app,
  captcha,
  commands,
  customFilters,
  dashboard,
  history,
  oAuth,
  paths,
  providerOptions,
  localization,
  search,
  settings,
  system,
  tags,
  trackedContent
];
