<template>
  <LinkCard
    :subtitle="t(`customizations.kind.options.${customization.kind}`)"
    :title="customization.name"
    :to="{ name: 'Customization', params: { id: customization.id } }"
  >
    <div class="card-text">
      <span v-if="customization.summary">{{ customization.summary }}</span>
      <span class="text-muted" v-else>&mdash;</span>
    </div>
    <StatusBlock :actor="customization.updatedBy" class="card-text mt-2 small text-secondary" :date="customization.updatedOn" relative />
  </LinkCard>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import LinkCard from "@/components/shared/LinkCard.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Customization } from "@/types/customizations";

const { t } = useI18n();

defineProps<{
  customization: Customization;
}>();
</script>
