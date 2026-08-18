<template>
  <TarModal centered :close="t('actions.close')" fade ref="modal" :title="title">
    <p class="mb-0" v-html="help"></p>
    <template #footer>
      <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
      <TarButton
        :disabled="isLoading"
        icon="fas fa-xmark"
        :loading="isLoading"
        :status="t('loading')"
        :text="t('actions.remove')"
        variant="danger"
        @click="confirm"
      />
    </template>
  </TarModal>
</template>

<script setup lang="ts">
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import TarButton from "@/components/tar/TarButton.vue";
import TarModal from "@/components/tar/TarModal.vue";
import type { Character } from "@/types/characters";
import type { Customization } from "@/types/customizations";
import { removeCharacterCustomization } from "@/api/characters";

const { t } = useI18n();

const props = defineProps<{
  character: Character;
  customization: Customization;
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Character): void;
}>();

const isLoading = ref<boolean>(false);
const modal = ref<InstanceType<typeof TarModal> | null>(null);

const help = computed<string>(() => t(`characters.customizations.remove.${props.customization.kind.toLowerCase()}.help`, { name: props.customization.name }));
const title = computed<string>(() => t(`characters.customizations.remove.${props.customization.kind.toLowerCase()}.lead`));

function cancel(): void {
  modal.value?.hide();
}

async function confirm(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const character: Character = await removeCharacterCustomization(props.character.id, props.customization.id);
      emit("updated", character);
      modal.value?.hide();
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

function open(): void {
  modal.value?.show();
}
defineExpose({ open });
</script>
