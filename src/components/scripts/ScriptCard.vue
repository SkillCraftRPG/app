<template>
  <LinkCard :title="script.name" :to="{ name: 'Script', params: { id: script.id } }">
    <div class="card-text">
      <span v-if="script.summary">{{ script.summary }}</span>
      <span class="text-muted" v-else>&mdash;</span>
    </div>
    <StatusBlock :actor="script.updatedBy" class="card-text mt-2 small text-secondary" :date="script.updatedOn" relative />
  </LinkCard>
</template>

<script setup lang="ts">
import LinkCard from "@/components/shared/LinkCard.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Script } from "@/types/scripts";

defineProps<{
  script: Script;
}>();
</script>
