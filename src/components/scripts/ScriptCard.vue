<template>
  <LinkCard :title="script.name" :to="{ name: 'Script', params: { id: script.id } }">
    <div class="card-text">
      <span v-if="script.summary">{{ script.summary }}</span>
      <span class="text-muted" v-else>&mdash;</span>
    </div>
    <!-- TODO(fpion): updated by & on -->
  </LinkCard>
</template>

<script setup lang="ts">
import LinkCard from "@/components/shared/LinkCard.vue";
import type { Script } from "@/types/scripts";

defineProps<{
  script: Script;
}>();
</script>
