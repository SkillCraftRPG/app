<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import type { CharacterModel } from "@/types/characters";
import { cancelCharacterLevelUp } from "@/api/characters";
import { useToastStore } from "@/stores/toast";

const toasts = useToastStore();
const { t } = useI18n();

const props = defineProps<{
  character: CharacterModel;
}>();

const isLoading = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);

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

async function onCancelLevelUp(): Promise<void> {
  try {
    const character: CharacterModel = await cancelCharacterLevelUp(props.character.id);
    toasts.success("characters.levelUpCancelled");
    emit("updated", character);
  } catch (e: unknown) {
    emit("error", e);
  }
}

// TODO(fpion): Cancel --> other text
</script>

<template>
  <span>
    <TarButton
      :disabled="character.levelUps.length < 1"
      icon="fas fa-ban"
      :text="t('actions.cancel')"
      variant="danger"
      data-bs-toggle="modal"
      data-bs-target="#cancel-level-up"
    />
    <TarModal :close="t('actions.close')" id="cancel-level-up" ref="modalRef" :title="t('actions.cancel')">
      <p>
        {{ t("characters.levelUp.cancel") }}
        <br />
        <span class="text-danger">TODO</span>
      </p>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="onCancel" />
        <TarButton
          :disabled="isLoading"
          icon="fas fa-ban"
          :loading="isLoading"
          :status="t('loading')"
          :text="t('actions.cancel')"
          variant="danger"
          @click="onCancelLevelUp"
        />
      </template>
    </TarModal>
  </span>
</template>
