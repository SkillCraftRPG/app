<template>
  <LinkableCard :title="item.name" :to="{ name: 'Item', params: { id: item.id } }">
    <div v-if="item.price || item.weight" class="card-text d-flex justify-content-between align-items-center gap-2 mb-2">
      <div class="text-start">
        <template v-if="item.price"><font-awesome-icon icon="fas fa-coins" aria-hidden="true" />&nbsp;{{ item.price }}</template>
      </div>
      <div v-if="item.weight" class="text-end"><font-awesome-icon icon="fas fa-weight-hanging" aria-hidden="true" />&nbsp;{{ item.weight }}</div>
    </div>
    <div v-if="item.summary" class="card-text">{{ item.summary }}</div>
    <StatusBlock :actor="item.updatedBy" class="card-text mt-2 small text-secondary" :date="item.updatedOn" relative />
  </LinkableCard>
</template>

<script setup lang="ts">
import LinkableCard from "@/components/shared/LinkableCard.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Item } from "@/types/items";

defineProps<{
  item: Item;
}>();
</script>
