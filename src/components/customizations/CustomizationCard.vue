<template>
  <LinkableCard :clickable="clickable" :selected="selected" :title="customization.name" :to="to">
    <template #subtitle-override>
      <h6 class="card-subtitle mb-2 text-body-secondary">
        <CustomizationKindDisplay :kind="customization.kind" />
      </h6>
    </template>
    <div v-if="customization.summary" class="card-text">{{ customization.summary }}</div>
    <slot>
      <StatusBlock :actor="customization.updatedBy" class="card-text mt-2 small text-secondary" :date="customization.updatedOn" relative />
    </slot>
  </LinkableCard>
</template>

<script setup lang="ts">
import type { RouteLocationAsPathGeneric, RouteLocationAsRelativeGeneric } from "vue-router";

import CustomizationKindDisplay from "./CustomizationKindDisplay.vue";
import LinkableCard from "@/components/shared/LinkableCard.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Customization } from "@/types/customizations";

defineProps<{
  clickable?: boolean | string;
  customization: Customization;
  selected?: boolean | string;
  to?: string | RouteLocationAsRelativeGeneric | RouteLocationAsPathGeneric;
}>();

// TODO(fpion): refactor
</script>
