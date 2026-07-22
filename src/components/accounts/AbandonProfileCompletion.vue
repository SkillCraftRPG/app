<template>
  <div>
    <TarButton icon="fas fa-xmark" outline :text="t('actions.abandon')" type="button" variant="danger" data-bs-toggle="modal" :data-bs-target="`#${id}`" />
    <TarModal centered :close="t('actions.close')" fade :id="id" ref="modal" :title="t('account.profile.completion.abandon.lead')">
      <p>{{ t("account.profile.completion.abandon.help") }}</p>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
        <TarButton icon="fas fa-xmark" :text="t('actions.abandon')" variant="danger" @click="abandon" />
      </template>
    </TarModal>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import TarButton from "@/components/tar/TarButton.vue";
import TarModal from "@/components/tar/TarModal.vue";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
  }>(),
  {
    id: "abandon-modal",
  },
);

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
</script>
