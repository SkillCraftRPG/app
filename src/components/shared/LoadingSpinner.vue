<template>
  <div class="d-flex flex-column align-items-center justify-content-center flex-grow-1 gap-3" aria-busy="true" aria-live="polite">
    <TarSpinner class="loading-spinner" :label="label" variant="primary" />
    <span class="text-body-secondary">{{ label }}</span>
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import TarSpinner from "@/components/tar/TarSpinner.vue";

const { t } = useI18n();

const label = computed<string>(() => t("loading"));
</script>

<style scoped>
.loading-spinner {
  width: 4rem;
  height: 4rem;
  border-width: 0.4em;
}
</style>
