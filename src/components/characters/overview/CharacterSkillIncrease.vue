<template>
  <div>
    <TarButton :disabled="!character.points.skills" icon="fas fa-arrow-turn-up" size="small" :text="increaseText" @click="open" />
    <TarModal centered :close="t('actions.close')" fade scrollable size="large" ref="modal" :title="t('characters.skills.increase')">
      <!-- TODO(fpion): implement -->
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
        <TarButton :disabled="true" icon="fas fa-arrow-turn-up" :loading="isLoading" :status="t('loading')" :text="t('actions.increase')" @click="submit" />
      </template>
    </TarModal>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import TarButton from "@/components/tar/TarButton.vue";
import TarModal from "@/components/tar/TarModal.vue";
import type { Character } from "@/types/characters";
import { formatSignedInteger } from "@/utils/format";

const { n, t } = useI18n();

const props = defineProps<{
  character: Character;
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Character): void;
}>();

const isLoading = ref<boolean>(false);
const modal = ref<InstanceType<typeof TarModal> | null>(null);

const increaseText = computed<string>(() => `${t("actions.increase")} (${formatSignedInteger(props.character.points.skills, n)})`);

async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      modal.value?.hide();
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

function cancel(): void {
  modal.value?.hide();
}

function open(): void {
  modal.value?.show();
}
</script>
