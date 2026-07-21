import { createI18n } from "vue-i18n";

import en from "./en";
import fr from "./fr";

type MessageSchema = typeof fr;

export default createI18n<[MessageSchema], "en" | "fr">({
  legacy: false,
  locale: "fr",
  fallbackLocale: "fr",
  messages: { en, fr },
});
