<template>
  <LinkCard :title="lineage.name" :to="{ name: 'Lineage', params: { id: lineage.id } }">
    <template #subtitle-override>
      <h6 class="card-subtitle mb-2 text-body-secondary"><font-awesome-icon icon="fas fa-ruler-vertical" aria-hidden="true" />&nbsp;{{ sizeText }}</h6>
    </template>
    <div v-if="lineage.summary" class="card-text">{{ lineage.summary }}</div>
    <StatusBlock :actor="lineage.updatedBy" class="card-text mt-2 small text-secondary" :date="lineage.updatedOn" relative />
  </LinkCard>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import LinkCard from "@/components/shared/LinkCard.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Lineage } from "@/types/lineages";
import { computed } from "vue";

const { t } = useI18n();

const props = defineProps<{
  lineage: Lineage;
}>();

const sizeText = computed<string>(() => t(`game.size.category.options.${props.lineage.size.category}`));
</script>
