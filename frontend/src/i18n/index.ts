import { createI18n } from "vue-i18n";

import en from "./en";
import fr from "./fr";

type MessageSchema = typeof en;

export default createI18n<[MessageSchema], "en" | "fr">({
  legacy: false,
  locale: "fr",
  fallbackLocale: "fr",
  messages: {
    en,
    fr,
  },
  datetimeFormats: {
    en: {
      medium: {
        year: "numeric",
        month: "short",
        day: "numeric",
        hour: "numeric",
        minute: "numeric",
        second: "numeric",
      },
    },
    fr: {
      medium: {
        year: "numeric",
        month: "long",
        day: "numeric",
        hour: "numeric",
        minute: "numeric",
        second: "numeric",
      },
    },
  },
  numberFormats: {
    en: {
      currency: {
        style: "currency",
        currency: "CAD",
        currencyDisplay: "narrowSymbol",
        notation: "standard",
      },
      decimal: {
        style: "decimal",
        minimumFractionDigits: 2,
      },
      percent: {
        style: "percent",
        minimumFractionDigits: 3,
      },
    },
    fr: {
      currency: {
        style: "currency",
        currency: "CAD",
        currencyDisplay: "narrowSymbol",
        notation: "standard",
      },
      decimal: {
        style: "decimal",
        minimumFractionDigits: 2,
      },
      percent: {
        style: "percent",
        minimumFractionDigits: 3,
      },
    },
  },
});
