<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

import type { Step6 } from "@/types/characters";

const emit = defineEmits<{
  (e: "back"): void;
  (e: "continue", value: Step6): void;
  (e: "error", value: unknown): void;
}>();

const { handleSubmit } = useForm();
const onSubmit = handleSubmit(() => emit("continue", { talentIds: [] }));
</script>

<template>
  <div>
    <h3>{{ t("characters.steps.talents") }}</h3>
    <form @submit="onSubmit">
      <!-- TODO(fpion): Talents -->
      <TarButton class="me-1" icon="fas fa-arrow-left" :text="t('actions.back')" variant="secondary" @click="$emit('back')" />
      <!-- TODO(fpion): <TarButton class="ms-1" icon="fas fa-arrow-right" :text="t('actions.continue')" type="submit" /> -->
    </form>
  </div>
</template>
