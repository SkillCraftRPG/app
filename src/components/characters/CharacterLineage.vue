<template>
  <div class="h-100">
    <TarCard class="clickable d-flex flex-column h-100" @click="open">
      <template #contents>
        <div class="card-body text-center d-flex flex-column flex-grow-1 justify-content-center">
          <div class="small text-body-secondary">{{ t("lineages.label") }}</div>
          <div class="fw-semibold">{{ species.name }}</div>
          <div v-if="ethnicity" class="text-body-secondary">{{ ethnicity.name }}</div>
        </div>
      </template>
    </TarCard>
    <LineageDetailModal :lineage="lineage" ref="modal" />
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import LineageDetailModal from "@/components/lineages/LineageDetailModal.vue";
import TarCard from "@/components/tar/TarCard.vue";
import type { Lineage } from "@/types/lineages";

const { t } = useI18n();

const props = defineProps<{
  lineage: Lineage;
}>();

const modal = ref<InstanceType<typeof LineageDetailModal> | null>(null);

const ethnicity = computed<Lineage | undefined>(() => (props.lineage.parent ? props.lineage : undefined));
const species = computed<Lineage>(() => props.lineage.parent ?? props.lineage);

function open(): void {
  modal.value?.open();
}
</script>
