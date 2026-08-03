<template>
  <section class="import-wrapper position-relative" :aria-busy="isLoading">
    <div :class="{ 'import-content-busy': isLoading }" :inert="isLoading">
      <slot></slot>
    </div>
    <div v-if="isLoading" class="import-overlay" role="status" aria-live="polite" aria-atomic="true">
      <div class="card import-status shadow">
        <div class="card-body text-center">
          <LoadingSpinner class="mb-3" />
          <h2 class="h5">{{ t("import.progress.lead") }}</h2>
          <p class="text-body-secondary">{{ help }}</p>
          <TarProgress animated :aria-label="t('import.progress.label')" :label="`${progress} %`" striped :value="progress" />
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import TarProgress from "@/components/tar/TarProgress.vue";

const { parseNumber } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  index: number | string;
  count: number | string;
}>();

const parsedIndex = computed<number>(() => parseNumber(props.index) ?? 0);
const parsedCount = computed<number>(() => parseNumber(props.count) ?? 0);
const isLoading = computed<boolean>(() => Boolean(parsedCount.value));
const progress = computed<number>(() => Math.round((parsedIndex.value / parsedCount.value) * 100));
const help = computed<string>(() =>
  progress.value ? t("import.progress.count", { index: parsedIndex.value, count: parsedCount.value }) : t("import.progress.help"),
);
</script>

<style scoped>
.import-wrapper {
  min-height: 20rem;
}

.import-content-busy {
  opacity: 0.3;
  filter: grayscale(40%);
  pointer-events: none;
  user-select: none;
}

.import-overlay {
  position: absolute;
  inset: 0;
  z-index: 10;

  display: flex;
  align-items: flex-start;
  justify-content: center;

  padding: 3rem 1rem;
  background-color: rgb(var(--bs-body-bg-rgb) / 65%);
  backdrop-filter: blur(1px);
}

.import-status {
  position: sticky;
  top: 2rem;
  width: min(30rem, 100%);
}
</style>
