<template>
  <LinkCard :title="customization.name" :to="{ name: 'Customization', params: { id: customization.id } }">
    <template #subtitle-override>
      <h6 class="card-subtitle mb-2 text-body-secondary">
        <CustomizationKindDisplay :kind="customization.kind" />
      </h6>
    </template>
    <div v-if="customization.summary" class="card-text">{{ customization.summary }}</div>
    <StatusBlock :actor="customization.updatedBy" class="card-text mt-2 small text-secondary" :date="customization.updatedOn" relative />
  </LinkCard>
</template>

<script setup lang="ts">
import CustomizationKindDisplay from "./CustomizationKindDisplay.vue";
import LinkCard from "@/components/shared/LinkCard.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Customization } from "@/types/customizations";

defineProps<{
  customization: Customization;
}>();
</script>
