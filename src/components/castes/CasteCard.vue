<template>
  <LinkCard :subtitle="caste.feature?.name" :title="caste.name" :to="{ name: 'Caste', params: { id: caste.id } }">
    <template #subtitle-override>
      <h6 class="card-subtitle mb-2 text-body-secondary">
        <template v-if="caste.feature">{{ caste.feature.name }}</template>
        <template v-else>&mdash;</template>
      </h6>
    </template>
    <div class="card-text d-flex justify-content-between align-items-center gap-2 mb-2">
      <div class="text-start">
        <font-awesome-icon icon="fas fa-kitchen-set" aria-hidden="true" />&nbsp;<span v-if="caste.skill">{{ t(`game.skill.options.${caste.skill}`) }}</span
        ><span v-else class="text-muted">&mdash;</span>
      </div>
      <div class="text-end">
        <font-awesome-icon icon="fas fa-coins" aria-hidden="true" />&nbsp;<span v-if="caste.wealthRoll">{{ caste.wealthRoll }}</span
        ><span v-else class="text-muted">&mdash;</span>
      </div>
    </div>
    <div class="card-text">
      <span v-if="caste.summary">{{ caste.summary }}</span>
      <span class="text-muted" v-else>&mdash;</span>
    </div>
    <StatusBlock :actor="caste.updatedBy" class="card-text mt-2 small text-secondary" :date="caste.updatedOn" relative />
  </LinkCard>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import LinkCard from "@/components/shared/LinkCard.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Caste } from "@/types/castes";

const { t } = useI18n();

defineProps<{
  caste: Caste;
}>();
</script>
