<template>
  <TarModal centered :close="t('actions.close')" fade ref="modal" :title="t('account.profile.completion.abandon.lead')">
    <p>{{ t("account.profile.completion.abandon.help") }}</p>
    <template #footer>
      <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
      <TarButton icon="fas fa-xmark" :text="t('actions.abandon')" variant="danger" @click="abandon" />
    </template>
  </TarModal>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import TarButton from "@/components/tar/TarButton.vue";
import TarModal from "@/components/tar/TarModal.vue";

const { t } = useI18n();

const emit = defineEmits<{
  (e: "abandon"): void;
}>();

const modal = ref<InstanceType<typeof TarModal> | null>(null);

function abandon(): void {
  modal.value?.hide();
  emit("abandon");
}

function cancel(): void {
  modal.value?.hide();
}

function open(): void {
  modal.value?.show();
}
defineExpose({ open });
</script>
