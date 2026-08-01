<template>
  <LinkableCard :title="script.name" :to="{ name: 'Script', params: { id: script.id } }">
    <div v-if="script.summary" class="card-text">{{ script.summary }}</div>
    <StatusBlock :actor="script.updatedBy" class="card-text mt-2 small text-secondary" :date="script.updatedOn" relative />
  </LinkableCard>
</template>

<script setup lang="ts">
import LinkableCard from "@/components/shared/LinkableCard.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Script } from "@/types/scripts";

defineProps<{
  script: Script;
}>();
</script>
