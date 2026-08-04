<template>
  <section>
    <h2 class="h3">{{ t("talents.title") }}</h2>
    <LoadingSpinner v-if="isLoading" />
    <div class="d-flex justify-content-between">
      <div class="d-flex gap-2">
        <TarButton icon="fas fa-xmark" outline :text="t('actions.abandon')" variant="danger" @click="$emit('abandon')" />
        <TarButton icon="fas fa-arrow-left" outline :text="t('actions.previous')" variant="secondary" @click="character.goBack" />
      </div>
      <TarButton :disabled="!canSubmit" icon="fas fa-arrow-right" :text="t('actions.next')" @click="submit" />
    </div>
  </section>
</template>

<script setup lang="ts">
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import TarButton from "@/components/tar/TarButton.vue";
import { useCharacterStore } from "@/stores/character";

const character = useCharacterStore();
const { t } = useI18n();

defineEmits<{
  (e: "abandon"): void;
  (e: "error", value: unknown): void;
}>();

const isLoading = ref<boolean>(true);

const canSubmit = computed<boolean>(() => false);

function submit(): void {
  if (canSubmit.value) {
    console.log("Submitting…");
  }
}
</script>
