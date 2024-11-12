<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import type { Step3 } from "@/types/characters";

const { t } = useI18n();

const emit = defineEmits<{
  (e: "back"): void;
  (e: "continue", value: Step3): void;
  (e: "error", value: unknown): void;
}>();

const { handleSubmit } = useForm();
const onSubmit = handleSubmit(() => emit("continue", { aspects: [] }));
</script>

<template>
  <div>
    <h3>{{ t("characters.steps.aspects") }}</h3>
    <form @submit="onSubmit">
      <!-- TODO(fpion): Aspects -->
      <TarButton class="me-1" icon="fas fa-arrow-left" :text="t('actions.back')" variant="secondary" @click="$emit('back')" />
      <TarButton class="ms-1" icon="fas fa-arrow-right" :text="t('actions.continue')" type="submit" />
    </form>
  </div>
</template>
