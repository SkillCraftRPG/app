<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import { useCharacterStore } from "@/stores/character";

const character = useCharacterStore();
const { t } = useI18n();

import type { Step4 } from "@/types/characters";
import { onMounted } from "vue";

defineEmits<{
  (e: "error", value: unknown): void;
}>();

const { handleSubmit } = useForm();
const onSubmit = handleSubmit(() => {
  const payload: Step4 = {};
  character.setStep4(payload);
});

onMounted(() => {
  const step4: Step4 | undefined = character.creation.step4;
  if (step4) {
    // TODO(fpion): implement
  }
});
</script>

<template>
  <div>
    <h3>{{ t("characters.steps.attributes") }}</h3>
    <form @submit="onSubmit">
      <!-- TODO(fpion): Attributes -->
      <TarButton class="me-1" icon="fas fa-arrow-left" :text="t('actions.back')" variant="secondary" @click="character.goBack()" />
      <TarButton class="ms-1" icon="fas fa-arrow-right" :text="t('actions.continue')" type="submit" />
    </form>
  </div>
</template>
