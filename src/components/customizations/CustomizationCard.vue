<template>
  <LinkCard :title="customization.name" :to="{ name: 'Customization', params: { id: customization.id } }">
    <template #subtitle-override>
      <h6 class="card-subtitle mb-2 text-body-secondary">
        <template v-if="icon"><font-awesome-icon :icon="icon" />&nbsp;</template>{{ t(`customizations.kind.options.${customization.kind}`) }}
      </h6>
    </template>
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
import { computed } from "vue";

const { t } = useI18n();

const props = defineProps<{
  customization: Customization;
}>();

const icon = computed<string | undefined>(() => {
  switch (props.customization.kind) {
    case "Disability":
      return "fas fa-wheelchair";
    case "Gift":
      return "fas fa-trophy";
  }
});
</script>
