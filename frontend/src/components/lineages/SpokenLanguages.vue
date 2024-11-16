<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import ExtraLanguagesInput from "./ExtraLanguagesInput.vue";
import LanguageCard from "@/components/languages/LanguageCard.vue";
import LanguageSelect from "@/components/languages/LanguageSelect.vue";
import LanguagesText from "./LanguagesText.vue";
import type { LanguageModel } from "@/types/languages";
import type { LanguagesPayload } from "@/types/lineages";

const { t } = useI18n();

const props = defineProps<{
  languages?: LanguageModel[];
  modelValue: LanguagesPayload;
}>();

const language = ref<LanguageModel>();
const selectedLanguages = ref<LanguageModel[]>(props.languages ?? []);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "update:model-value", value: LanguagesPayload): void;
}>();

function addLanguage(): void {
  if (language.value) {
    selectedLanguages.value.push(language.value);
    const payload: LanguagesPayload = { ...props.modelValue, ids: [...props.modelValue.ids] };
    payload.ids.push(language.value.id);
    emit("update:model-value", payload);
    language.value = undefined;
  }
}
function removeLanguage(language: LanguageModel): void {
  const index: number = selectedLanguages.value.findIndex(({ id }) => id === language.id);
  if (index >= 0) {
    selectedLanguages.value.splice(index, 1);
  }
  const payload: LanguagesPayload = { ...props.modelValue, ids: [...props.modelValue.ids].filter((id) => id !== language.id) };
  emit("update:model-value", payload);
}

function setExtra(extra: number): void {
  const payload: LanguagesPayload = { ...props.modelValue, extra };
  emit("update:model-value", payload);
}

function setText(text?: string): void {
  const payload: LanguagesPayload = { ...props.modelValue, text };
  emit("update:model-value", payload);
}
</script>

<template>
  <div>
    <h3>{{ t("lineages.languages.label") }}</h3>
    <div class="row">
      <LanguageSelect
        class="col-lg-6"
        :exclude="selectedLanguages"
        :model-value="language?.id"
        validation="server"
        @error="$emit('error', $event)"
        @selected="language = $event"
      >
        <template #append>
          <TarButton :disabled="!language" icon="fas fa-plus" :text="t('actions.add')" variant="success" @click="addLanguage" />
        </template>
      </LanguageSelect>
      <ExtraLanguagesInput class="col-lg-6" :model-value="modelValue.extra" @update:model-value="setExtra($event ?? 0)" />
    </div>
    <div v-if="selectedLanguages.length > 0" class="mb-3 row">
      <div v-for="language in selectedLanguages" :key="language.id" class="col-lg-3">
        <LanguageCard :language="language" remove view @removed="removeLanguage(language)" />
      </div>
    </div>
    <p v-else>{{ t("lineages.languages.empty") }}</p>
    <LanguagesText :model-value="modelValue.text" @update:model-value="setText" />
  </div>
</template>
