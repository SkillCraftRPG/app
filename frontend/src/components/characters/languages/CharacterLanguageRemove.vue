<script setup lang="ts">
import { TarModal } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import { TarButton } from "logitar-vue3-ui";
import type { CharacterLanguageModel, CharacterModel } from "@/types/characters";
import { removeCharacterLanguage } from "@/api/characters";
import { useToastStore } from "@/stores/toast";

const toasts = useToastStore();
const { t } = useI18n();

const props = defineProps<{
  character: CharacterModel;
  language: CharacterLanguageModel;
}>();

const isLoading = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);

const id = computed<string>(() => `delete-character-language-${props.language.language.id}`);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: CharacterModel): void;
}>();

function hide(): void {
  modalRef.value?.hide();
}

function onCancel(): void {
  hide();
}

async function onRemove(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const character: CharacterModel = await removeCharacterLanguage(props.character.id, props.language.language.id);
      toasts.success("characters.languages.removed");
      emit("updated", character);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
      hide();
    }
  }
}
</script>

<template>
  <span>
    <TarButton icon="fas fa-trash" :text="t('actions.remove')" variant="danger" data-bs-toggle="modal" :data-bs-target="`#${id}`" />
    <TarModal :close="t('actions.close')" :id="id" ref="modalRef" :title="t('characters.languages.remove.label')">
      <p>
        {{ t("characters.languages.remove.confirm") }}
        <br />
        <span class="text-danger">{{ language.language.name }}</span>
      </p>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="onCancel" />
        <TarButton
          :disabled="isLoading"
          icon="fas fa-trash"
          :loading="isLoading"
          :status="t('loading')"
          :text="t('actions.remove')"
          variant="danger"
          @click="onRemove"
        />
      </template>
    </TarModal>
  </span>
</template>
