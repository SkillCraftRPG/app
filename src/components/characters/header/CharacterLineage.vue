<template>
  <TarCard class="clickable text-center">
    <div class="small text-body-secondary">{{ t("lineages.label") }}</div>
    <div class="fw-semibold">{{ species.name }}</div>
    <div v-if="ethnicity" class="text-body-secondary">{{ ethnicity.name }}</div>
  </TarCard>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import TarCard from "@/components/tar/TarCard.vue";
import type { Lineage } from "@/types/lineages";

const { t } = useI18n();

const props = defineProps<{
  lineage: Lineage;
}>();

const ethnicity = computed<Lineage | undefined>(() => (props.lineage.parent ? props.lineage : undefined));
const species = computed<Lineage>(() => props.lineage.parent ?? props.lineage);
</script>
