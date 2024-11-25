<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { computed, ref, watch } from "vue";
import { nanoid } from "nanoid";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import LanguageSelect from "@/components/languages/LanguageSelect.vue";
import type { CharacterLanguageModel, CharacterLanguagePayload, CharacterModel } from "@/types/characters";
import type { LanguageModel } from "@/types/languages";
import { saveCharacterLanguage } from "@/api/characters";
import { useToastStore } from "@/stores/toast";

const toasts = useToastStore();
const { t } = useI18n();

const props = defineProps<{
  character: CharacterModel;
  exclude?: (string | LanguageModel)[];
  language?: CharacterLanguageModel;
}>();

const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const notes = ref<string>("");
const selectedLanguage = ref<LanguageModel>();

const hasChanges = computed<boolean>(
  () => (selectedLanguage.value?.id ?? "") !== (props.language?.language.id ?? "") || notes.value !== (props.language?.notes ?? ""),
);
const id = computed<string>(() => (props.language ? `edit-language-${props.language.language.id ?? nanoid()}` : "add-language"));

function hide(): void {
  modalRef.value?.hide();
}

function setModel(model?: CharacterLanguageModel): void {
  notes.value = model?.notes ?? "";
  selectedLanguage.value = model?.language;
}

function onCancel(): void {
  setModel(props.language);
  hide();
}

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: CharacterModel): void;
}>();

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  if (selectedLanguage.value) {
    try {
      const payload: CharacterLanguagePayload = { notes: notes.value };
      const character: CharacterModel = await saveCharacterLanguage(props.character.id, selectedLanguage.value.id, payload);
      toasts.success(`characters.languages.${props.language ? "edited" : "added"}`);
      emit("updated", character);
    } catch (e: unknown) {
      emit("error", e);
    }
  }
  onCancel();
});

watch(() => props.language, setModel, { deep: true, immediate: true });
</script>

<template>
  <span>
    <TarButton
      :icon="language ? 'fas fa-edit' : 'fas fa-plus'"
      :text="t(language ? 'actions.edit' : 'actions.add')"
      :variant="language ? 'primary' : 'success'"
      data-bs-toggle="modal"
      :data-bs-target="`#${id}`"
    />
    <TarModal :close="t('actions.close')" :id="id" ref="modalRef" :title="t(language ? 'characters.languages.edit' : 'characters.languages.add')">
      <form @submit.prevent="onSubmit">
        <LanguageSelect
          :disabled="Boolean(language)"
          :exclude="exclude"
          :model-value="selectedLanguage?.id"
          :required="!language"
          :validation="language ? 'server' : 'client'"
          @selected="selectedLanguage = $event"
        />
        <DescriptionTextarea label="characters.languages.notes" placeholder="characters.languages.notes" rows="5" v-model="notes" />
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="onCancel" />
        <TarButton
          :disabled="isSubmitting || !hasChanges"
          :icon="language ? 'fas fa-save' : 'fas fa-plus'"
          :loading="isSubmitting"
          :status="t('loading')"
          :text="t(language ? 'actions.save' : 'actions.add')"
          :variant="language ? 'primary' : 'success'"
          @click="onSubmit"
        />
      </template>
    </TarModal>
  </span>
</template>
