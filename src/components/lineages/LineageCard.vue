<template>
  <LinkCard :title="lineage.name" :to="{ name: 'Lineage', params: { id: lineage.id } }">
    <div v-if="lineage.summary" class="card-text">{{ lineage.summary }}</div>
    <StatusBlock :actor="lineage.updatedBy" class="card-text mt-2 small text-secondary" :date="lineage.updatedOn" relative />
  </LinkCard>
</template>

<script setup lang="ts">
import LinkCard from "@/components/shared/LinkCard.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Lineage } from "@/types/lineages";

defineProps<{
  lineage: Lineage;
}>();
</script>
