<script setup lang="ts">
import { TarModal } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import { TarButton } from "logitar-vue3-ui";
import type { CharacterModel, CharacterTalentModel } from "@/types/characters";
import { removeCharacterTalent } from "@/api/characters";
import { useToastStore } from "@/stores/toast";

const toasts = useToastStore();
const { t } = useI18n();

const props = defineProps<{
  character: CharacterModel;
  talent: CharacterTalentModel;
}>();

const isLoading = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);

const id = computed<string>(() => `delete-talent-${props.talent.id}`);

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
      const character: CharacterModel = await removeCharacterTalent(props.character.id, props.talent.id);
      toasts.success("characters.talents.removed");
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
    <TarModal :close="t('actions.close')" :id="id" ref="modalRef" :title="t('characters.talents.remove.label')">
      <p>
        {{ t("characters.talents.remove.confirm") }}
        <br />
        <span class="text-danger">
          {{ talent.talent.name }}
          <template v-if="talent.precision"> ({{ talent.precision }})</template>
        </span>
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
