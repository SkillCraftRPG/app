<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

import type { Step4 } from "@/types/characters";

const emit = defineEmits<{
  (e: "back"): void;
  (e: "continue", value: Step4): void;
  (e: "error", value: unknown): void;
}>();

const { handleSubmit } = useForm();
const onSubmit = handleSubmit(() =>
  emit("continue", {
    attributes: {
      agility: 0,
      coordination: 0,
      intellect: 0,
      presence: 0,
      sensitivity: 0,
      spirit: 0,
      vigor: 0,
      best: "Agility",
      worst: "Agility",
      optional: [],
      extra: [],
    },
  }),
);
</script>

<template>
  <div>
    <h3>{{ t("characters.steps.attributes") }}</h3>
    <form @submit="onSubmit">
      <!-- TODO(fpion): Attributes -->
      <TarButton class="me-1" icon="fas fa-arrow-left" :text="t('actions.back')" variant="secondary" @click="$emit('back')" />
      <TarButton class="ms-1" icon="fas fa-arrow-right" :text="t('actions.continue')" type="submit" />
    </form>
  </div>
</template>
