import AppSectionState, {
  AppSectionDeleteState,
  AppSectionItemState,
  AppSectionSaveState,
} from 'App/State/AppSectionState';
import Notification from 'typings/Notification';
import General from 'typings/Settings/General';
import UiSettings from 'typings/Settings/UiSettings';

export interface GeneralAppState
  extends AppSectionItemState<General>,
    AppSectionSaveState {}

export interface NotificationAppState
  extends AppSectionState<Notification>,
    AppSectionDeleteState {}

export type UiSettingsAppState = AppSectionItemState<UiSettings>;

interface SettingsAppState {
  general: GeneralAppState;
  notifications: NotificationAppState;
  ui: UiSettingsAppState;
}

export default SettingsAppState;
