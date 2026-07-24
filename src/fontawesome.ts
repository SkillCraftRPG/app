import type { App } from "vue";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";

import { library } from "@fortawesome/fontawesome-svg-core";
import {
  faAdjust,
  faArrowLeft,
  faArrowRight,
  faArrowRightFromBracket,
  faArrowUp,
  faBan,
  faCheck,
  faCommentSms,
  faDice,
  faEnvelope,
  faFloppyDisk,
  faHatWizard,
  faHome,
  faMoon,
  faSun,
  faUser,
  faVial,
  faXmark,
} from "@fortawesome/free-solid-svg-icons";

library.add(
  faAdjust,
  faArrowLeft,
  faArrowRight,
  faArrowRightFromBracket,
  faArrowUp,
  faBan,
  faCheck,
  faCommentSms,
  faDice,
  faEnvelope,
  faFloppyDisk,
  faHatWizard,
  faHome,
  faMoon,
  faSun,
  faUser,
  faVial,
  faXmark,
);

export default function (app: App) {
  app.component("font-awesome-icon", FontAwesomeIcon);
}
