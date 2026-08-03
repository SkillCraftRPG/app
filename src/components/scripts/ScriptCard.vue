<template>
  <LinkableCard :clickable="clickable" :selected="selected" :title="script.name" :to="to">
    <div v-if="script.summary" class="card-text">{{ script.summary }}</div>
    <slot>
      <StatusBlock :actor="script.updatedBy" class="card-text mt-2 small text-secondary" :date="script.updatedOn" relative />
    </slot>
  </LinkableCard>
</template>

<script setup lang="ts">
import type { RouteLocationAsPathGeneric, RouteLocationAsRelativeGeneric } from "vue-router";

import LinkableCard from "@/components/shared/LinkableCard.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Script } from "@/types/scripts";

defineProps<{
  clickable?: boolean | string;
  script: Script;
  selected?: boolean | string;
  to?: string | RouteLocationAsRelativeGeneric | RouteLocationAsPathGeneric;
}>();
</script>
