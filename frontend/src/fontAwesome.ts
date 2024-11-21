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
  faBrain,
  faCheck,
  faChevronLeft,
  faCircleInfo,
  faClipboard,
  faCodeBranch,
  faDice,
  faDungeon,
  faEdit,
  faEnvelope,
  faEquals,
  faEye,
  faGamepad,
  faGraduationCap,
  faHome,
  faIdCard,
  faKey,
  faLanguage,
  faList,
  faLock,
  faMinus,
  faPalette,
  faPaw,
  faPhone,
  faPlus,
  faQuestion,
  faRotate,
  faSave,
  faScrewdriverWrench,
  faShoppingCart,
  faSpaghettiMonsterFlying,
  faTimes,
  faTrash,
  faTrashArrowUp,
  faTriangleExclamation,
  faUser,
  faUsers,
  faVial,
  faWandSparkles,
} from "@fortawesome/free-solid-svg-icons";

library.add(
  faArrowLeft,
  faArrowRight,
  faArrowRightFromBracket,
  faArrowRightToBracket,
  faAt,
  faBan,
  faBrain,
  faCheck,
  faChevronLeft,
  faCircleInfo,
  faClipboard,
  faCodeBranch,
  faDice,
  faDungeon,
  faEdit,
  faEnvelope,
  faEquals,
  faEye,
  faGamepad,
  faGraduationCap,
  faHome,
  faIdCard,
  faKey,
  faLanguage,
  faList,
  faLock,
  faMinus,
  faPalette,
  faPaw,
  faPhone,
  faPlus,
  faQuestion,
  faRotate,
  faSave,
  faScrewdriverWrench,
  faShoppingCart,
  faSpaghettiMonsterFlying,
  faTimes,
  faTrash,
  faTrashArrowUp,
  faTriangleExclamation,
  faUser,
  faUsers,
  faVial,
  faWandSparkles,
);

export default function (app: App) {
  app.component("font-awesome-icon", FontAwesomeIcon);
}
