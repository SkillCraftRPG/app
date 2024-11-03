import type { App } from "vue";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";

import { library } from "@fortawesome/fontawesome-svg-core";
import {
  faArrowLeft,
  faArrowRight,
  faArrowRightFromBracket,
  faArrowRightToBracket,
  faAt,
  faBan,
  faCheck,
  faChevronLeft,
  faCircleInfo,
  faDice,
  faDungeon,
  faEdit,
  faEnvelope,
  faEye,
  faHome,
  faIdCard,
  faKey,
  faLock,
  faPalette,
  faPhone,
  faPlus,
  faRotate,
  faSave,
  faSpaghettiMonsterFlying,
  faTimes,
  faUser,
  faVial,
} from "@fortawesome/free-solid-svg-icons";

library.add(
  faArrowLeft,
  faArrowRight,
  faArrowRightFromBracket,
  faArrowRightToBracket,
  faAt,
  faBan,
  faCheck,
  faChevronLeft,
  faCircleInfo,
  faDice,
  faDungeon,
  faEdit,
  faEnvelope,
  faEye,
  faHome,
  faIdCard,
  faKey,
  faLock,
  faPalette,
  faPhone,
  faPlus,
  faRotate,
  faSave,
  faSpaghettiMonsterFlying,
  faTimes,
  faUser,
  faVial,
);

export default function (app: App) {
  app.component("font-awesome-icon", FontAwesomeIcon);
}
