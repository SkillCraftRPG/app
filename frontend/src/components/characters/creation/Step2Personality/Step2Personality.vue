<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import PersonalitySelect from "@/components/personalities/PersonalitySelect.vue";
import type { PersonalityModel } from "@/types/personalities";
import type { Step2 } from "@/types/characters";
import MarkdownText from "@/components/shared/MarkdownText.vue";

const { t } = useI18n();

const personality = ref<PersonalityModel>();

const emit = defineEmits<{
  (e: "back"): void;
  (e: "continue", value: Step2): void;
  (e: "error", value: unknown): void;
}>();

const { handleSubmit } = useForm();
const onSubmit = handleSubmit(() => emit("continue", { personality: personality.value, customizations: [] }));
</script>

<template>
  <div>
    <h3>{{ t("characters.steps.personality") }}</h3>
    <form @submit="onSubmit">
      <PersonalitySelect :model-value="personality?.id" required @error="$emit('error', $event)" @selected="personality = $event" />
      <MarkdownText v-if="personality?.description" :text="personality.description" />
      <!-- TODO(fpion): Customizations -->
      <TarButton class="me-1" icon="fas fa-arrow-left" :text="t('actions.back')" variant="secondary" @click="$emit('back')" />
      <TarButton class="ms-1" icon="fas fa-arrow-right" :text="t('actions.continue')" type="submit" />
    </form>
  </div>
</template>
