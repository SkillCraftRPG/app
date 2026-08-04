<template>
  <LinkableCard :clickable="clickable" :selected="selected" :title="lineage.name" :to="to">
    <template #subtitle-override>
      <h6 class="card-subtitle mb-2 text-body-secondary">
        <font-awesome-icon icon="fas fa-ruler-vertical" aria-hidden="true" />&nbsp;{{ t(`game.size.category.options.${lineage.size.category}`) }}
      </h6>
    </template>
    <div v-if="lineage.summary" class="card-text">{{ lineage.summary }}</div>
    <slot>
      <StatusBlock :actor="lineage.updatedBy" class="card-text mt-2 small text-secondary" :date="lineage.updatedOn" relative />
    </slot>
  </LinkableCard>
</template>

<script setup lang="ts">
import type { RouteLocationAsPathGeneric, RouteLocationAsRelativeGeneric } from "vue-router";
import { useI18n } from "vue-i18n";

import LinkableCard from "@/components/shared/LinkableCard.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Lineage } from "@/types/lineages";

const { t } = useI18n();

defineProps<{
  clickable?: boolean | string;
  lineage: Lineage;
  selected?: boolean | string;
  to?: string | RouteLocationAsRelativeGeneric | RouteLocationAsPathGeneric;
}>();
</script>
