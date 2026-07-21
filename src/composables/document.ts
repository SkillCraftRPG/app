import { useI18n } from "vue-i18n";

export function useDocument() {
  const { t } = useI18n();

  function setTitle(title: string): void {
    document.title = `${title} · ${t("brand")}`;
  }

  return { setTitle };
}
