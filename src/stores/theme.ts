import { defineStore } from "pinia";
import { ref } from "vue";

export const useThemeStore = defineStore(
  "theme",
  () => {
    const currentTheme = ref<string>();

    function setTheme(theme: string): void {
      currentTheme.value = theme;
      applyTheme(theme);
    }

    function applyTheme(theme: string): void {
      if (theme === "auto") {
        document.documentElement.setAttribute("data-bs-theme", window.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light");
      } else {
        document.documentElement.setAttribute("data-bs-theme", theme);
      }
    }

    window.matchMedia("(prefers-color-scheme: dark)").addEventListener("change", () => {
      console.log("System theme changed");
      if (currentTheme.value !== "light" && currentTheme.value !== "dark") {
        applyTheme("auto");
      }
    });

    return { currentTheme, setTheme };
  },
  {
    persist: {
      afterHydrate(context) {
        if (context.store.currentTheme) {
          context.store.setTheme(context.store.currentTheme);
        }
      },
    },
  },
);
