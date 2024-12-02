<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import type { Attribute } from "@/types/game";
import type { CharacterModel } from "@/types/characters";
import { cancelCharacterLevelUp } from "@/api/characters";
import { useToastStore } from "@/stores/toast";

const toasts = useToastStore();
const { t } = useI18n();

const props = defineProps<{
  attribute: Attribute;
  character: CharacterModel;
  level: number;
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
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const character: CharacterModel = await cancelCharacterLevelUp(props.character.id);
      toasts.success("characters.levelUpCancelled");
      emit("updated", character);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
    onCancel();
  }
}
</script>

<template>
  <span>
    <TarButton
      :disabled="character.levelUps.length < 1"
      icon="fas fa-trash"
      :text="t('actions.cancel')"
      variant="danger"
      data-bs-toggle="modal"
      data-bs-target="#cancel-level-up"
    />
    <TarModal :close="t('actions.close')" id="cancel-level-up" ref="modalRef" :title="t('characters.levelUp.cancel.label')">
      <p>
        {{ t("characters.levelUp.cancel.confirm") }}
        <br />
        <span class="text-danger">{{ t("characters.level.format", { level }) }} ({{ t(`game.attribute.options.${attribute}`) }} +1)</span>
      </p>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('no')" variant="secondary" @click="onCancel" />
        <TarButton
          :disabled="isLoading"
          icon="fas fa-trash"
          :loading="isLoading"
          :status="t('loading')"
          :text="t('yes')"
          variant="danger"
          @click="onCancelLevelUp"
        />
      </template>
    </TarModal>
  </span>
</template>
