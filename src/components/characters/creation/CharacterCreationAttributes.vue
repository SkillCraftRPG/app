<template>
  <section>
    <div class="d-flex justify-content-between">
      <div class="d-flex gap-2">
        <TarButton icon="fas fa-xmark" outline :text="t('actions.abandon')" variant="danger" @click="$emit('abandon')" />
        <TarButton icon="fas fa-arrow-left" outline :text="t('actions.previous')" variant="secondary" @click="character.goBack" />
      </div>
      <TarButton disabled icon="fas fa-arrow-right" :text="t('actions.next')" />
    </div>
  </section>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import TarButton from "@/components/tar/TarButton.vue";
import { useCharacterStore } from "@/stores/character";

const character = useCharacterStore();
const { t } = useI18n();

defineEmits<{
  (e: "abandon"): void;
}>();
</script>
