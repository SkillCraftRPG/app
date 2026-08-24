<template>
  <div class="h-100">
    <TarCard class="clickable text-center h-100" @click="open">
      <div class="small text-body-secondary">{{ t("castes.label") }}</div>
      <div class="fw-semibold">{{ caste.name }}</div>
      <div v-if="caste.feature" class="text-body-secondary">{{ caste.feature.name }}</div>
    </TarCard>
    <CasteDetailModal :caste="caste" ref="modal" />
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import CasteDetailModal from "@/components/castes/CasteDetailModal.vue";
import TarCard from "@/components/tar/TarCard.vue";
import type { Caste } from "@/types/castes";

const { t } = useI18n();

defineProps<{
  caste: Caste;
}>();

const modal = ref<InstanceType<typeof CasteDetailModal> | null>(null);

function open(): void {
  modal.value?.open();
}
</script>
