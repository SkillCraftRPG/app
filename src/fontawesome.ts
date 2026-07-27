import type { App } from "vue";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";

import { library } from "@fortawesome/fontawesome-svg-core";
import {
  faAdjust,
  faArrowLeft,
  faArrowRight,
  faArrowRightFromBracket,
  faArrowRightToBracket,
  faArrowUp,
  faBan,
  faCheck,
  faClockRotateLeft,
  faCommentSms,
  faDesktop,
  faDice,
  faEnvelope,
  faFloppyDisk,
  faGlobe,
  faHatWizard,
  faHome,
  faMobile,
  faMoon,
  faNetworkWired,
  faPlus,
  faSun,
  faTablet,
  faUser,
  faVial,
  faXmark,
} from "@fortawesome/free-solid-svg-icons";

library.add(
  faAdjust,
  faArrowLeft,
  faArrowRight,
  faArrowRightFromBracket,
  faArrowRightToBracket,
  faArrowUp,
  faBan,
  faCheck,
  faClockRotateLeft,
  faCommentSms,
  faDesktop,
  faDice,
  faEnvelope,
  faFloppyDisk,
  faGlobe,
  faHatWizard,
  faHome,
  faMobile,
  faMoon,
  faNetworkWired,
  faPlus,
  faSun,
  faTablet,
  faUser,
  faVial,
  faXmark,
);

export default function (app: App) {
  app.component("font-awesome-icon", FontAwesomeIcon);
}
