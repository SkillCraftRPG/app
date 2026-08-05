<template>
  <TarModal centered :close="t('actions.close')" fade ref="modal" :title="t('characters.talents.remove.lead')">
    <p v-html="help"></p>
    <template #footer>
      <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
      <TarButton icon="fas fa-xmark" :text="t('actions.remove')" variant="danger" @click="confirm" />
    </template>
  </TarModal>
</template>

<script setup lang="ts">
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import TarButton from "@/components/tar/TarButton.vue";
import TarModal from "@/components/tar/TarModal.vue";
import type { CharacterTalent } from "@/types/characters";

const { t } = useI18n();

const props = defineProps<{
  acquisition: CharacterTalent;
}>();

const emit = defineEmits<{
  (e: "confirm"): void;
}>();

const modal = ref<InstanceType<typeof TarModal> | null>(null);

const help = computed<string>(() => t("characters.talents.remove.help", { name: props.acquisition.talent.name }));

function cancel(): void {
  modal.value?.hide();
}

function confirm(): void {
  emit("confirm");
  modal.value?.hide();
}

function open(): void {
  modal.value?.show();
}
defineExpose({ open });
</script>
