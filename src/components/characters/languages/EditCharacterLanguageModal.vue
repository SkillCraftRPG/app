<template>
  <TarModal centered :close="t('actions.close')" fade scrollable ref="modal" size="x-large" :title="title">
    <form @submit.prevent="handleSubmit(submit)">
      <SelectCharacterLanguage v-if="step === 'select'" :character="character" :languages="languages" ref="select" :selected="language" @toggle="toggle" />
      <EditCharacterDetailAcquisition v-else-if="step === 'acquisition' && language" :character="character" :language="language" :mode="mode" v-model="data" />
    </form>
    <template #footer>
      <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
      <TarButton v-if="canGoBack" icon="fas fa-arrow-left" outline :text="t('actions.previous')" @click="goBack" />
      <TarButton
        :disabled="isLoading || !canSubmit"
        :icon="submitIcon"
        :loading="isLoading"
        :status="t('loading')"
        :text="submitText"
        @click="handleSubmit(submit)"
      />
    </template>
  </TarModal>
</template>

<script setup lang="ts">
import { computed, nextTick, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import EditCharacterDetailAcquisition from "./EditCharacterDetailAcquisition.vue";
import SelectCharacterLanguage from "./SelectCharacterLanguage.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarModal from "@/components/tar/TarModal.vue";
import type {
  Character,
  CharacterLanguage,
  CharacterLanguageAcquisition,
  CharacterLanguageMode,
  CharacterLanguageSource,
  CreateOrReplaceCharacterLanguagePayload,
} from "@/types/characters";
import type { Language } from "@/types/languages";
import { createOrReplaceCharacterLanguage } from "@/api/characters";
import { useForm } from "@/forms";

type Step = "select" | "acquisition";

const { t } = useI18n();

const props = defineProps<{
  acquisition?: CharacterLanguage;
  character: Character;
  languages: Language[];
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Character): void;
}>();

const data = ref<CharacterLanguageAcquisition>({ source: "", target: "", notes: "" });
const isLoading = ref<boolean>(false);
const language = ref<Language>();
const modal = ref<InstanceType<typeof TarModal> | null>(null);
const select = ref<InstanceType<typeof SelectCharacterLanguage> | null>(null);
const step = ref<Step>();

const mode = computed<CharacterLanguageMode>(() => (props.acquisition ? "edit" : "add"));
const canGoBack = computed<boolean>(() => mode.value === "add" && step.value === "acquisition");
const canSubmit = computed<boolean>(() => Boolean(step.value === "acquisition" || language.value));
const submitIcon = computed<string>(() => {
  if (step.value === "select") {
    return "fas fa-arrow-right";
  }
  return mode.value === "edit" ? "fas fa-floppy-disk" : "fas fa-plus";
});
const submitText = computed<string>(() => {
  if (step.value === "select") {
    return t("actions.next");
  }
  return mode.value === "edit" ? t("actions.save") : t("actions.add");
});
const title = computed<string>(() => t(`characters.languages.${mode.value}`));

function toggle(value: Language): void {
  if (language.value?.id === value.id) {
    language.value = undefined;
  } else {
    language.value = value;
  }
}

function clear(): void {
  select.value?.clearFilters();
  if (props.acquisition) {
    step.value = "acquisition";
  } else {
    language.value = undefined;
    step.value = "select";
  }
  nextTick(reset);
}

function cancel(): void {
  clear();
  modal.value?.hide();
}

function goBack(): void {
  if (canGoBack.value) {
    step.value = "select";
  }
}

const { handleSubmit, reinitialize, reset } = useForm();
async function submit(): Promise<void> {
  if (canSubmit.value) {
    switch (step.value) {
      case "acquisition":
        if (!isLoading.value && language.value) {
          isLoading.value = true;
          try {
            const payload: CreateOrReplaceCharacterLanguagePayload = {
              source: data.value.source as CharacterLanguageSource,
              target: data.value.target,
              notes: data.value.notes,
            };
            const character: Character = await createOrReplaceCharacterLanguage(props.character.id, language.value.id, payload);
            emit("updated", character);
            if (!props.acquisition) {
              clear();
            }
            modal.value?.hide();
          } catch (e: unknown) {
            emit("error", e);
          } finally {
            isLoading.value = false;
          }
        }
        break;
      case "select":
        step.value = "acquisition";
        break;
    }
  }
}

watch(
  () => props.acquisition,
  (acquisition) => {
    language.value = acquisition?.language;
    data.value = {
      source: acquisition?.source ?? "",
      target: acquisition?.target ?? "",
      notes: acquisition?.notes ?? "",
    };
    nextTick(reinitialize);
    step.value = language.value ? "acquisition" : "select";
  },
  { deep: true, immediate: true },
);

function open(): void {
  modal.value?.show();
}
defineExpose({ open });
</script>
