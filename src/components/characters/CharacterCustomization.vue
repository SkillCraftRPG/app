<template>
  <div>
    <TarCard class="clickable text-center" @click="open">
      <div class="fw-semibold">{{ customization.name }}</div>
      <div class="text-body-secondary">
        <CustomizationKindDisplay :kind="customization.kind" />
      </div>
    </TarCard>
    <TarModal centered :close="t('actions.close')" fade ref="modal" size="large" :title="title">
      <div v-if="customization.summary" class="fst-italic text-body-secondary mb-3">{{ customization.summary }}</div>
      <MarkdownContent v-if="customization.content" class="mb-3" :text="customization.content"></MarkdownContent>
      <template #footer>
        <TarButton icon="fas fa-xmark" :text="t('actions.close')" variant="secondary" @click="close" />
      </template>
    </TarModal>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import CustomizationKindDisplay from "@/components/customizations/CustomizationKindDisplay.vue";
import MarkdownContent from "@/components/shared/MarkdownContent.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarCard from "@/components/tar/TarCard.vue";
import TarModal from "@/components/tar/TarModal.vue";
import type { Customization } from "@/types/customizations";

const { t } = useI18n();

const props = defineProps<{
  customization: Customization;
}>();

const modal = ref<InstanceType<typeof TarModal> | null>(null);

const title = computed<string>(() => t(`customizations.format.${props.customization.kind.toLowerCase()}`, { name: props.customization.name }));

function close(): void {
  modal.value?.hide();
}

function open(): void {
  modal.value?.show();
}
</script>
