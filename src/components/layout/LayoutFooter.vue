<template>
  <footer class="bg-dark">
    <div class="container-fluid text-center py-3">
      <RouterLink class="text-light" :to="{ name: 'Home' }">
        <img src="@/assets/img/logo.png" :alt="`${t('brand')} Logo`" height="32" />&nbsp;@&nbsp;{{ t("brand") }}&nbsp;{{ version }}&nbsp;{{ year }}
      </RouterLink>
    </div>
  </footer>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

const version = import.meta.env.VITE_APP_VERSION;
const year: number = new Date().getUTCFullYear();
const { t } = useI18n();
</script>
