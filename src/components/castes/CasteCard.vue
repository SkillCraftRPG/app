<template>
  <LinkableCard :subtitle="caste.feature?.name" :title="caste.name" :to="{ name: 'Caste', params: { id: caste.id } }">
    <div v-if="caste.skill || caste.wealthRoll" class="card-text d-flex justify-content-between align-items-center gap-2 mb-2">
      <div class="text-start">
        <template v-if="caste.skill">
          <font-awesome-icon icon="fas fa-kitchen-set" aria-hidden="true" />&nbsp;{{ t(`game.skill.options.${caste.skill}`) }}
        </template>
      </div>
      <div v-if="caste.wealthRoll" class="text-end"><font-awesome-icon icon="fas fa-coins" aria-hidden="true" />&nbsp;{{ caste.wealthRoll }}</div>
    </div>
    <div v-if="caste.summary" class="card-text">{{ caste.summary }}</div>
    <StatusBlock :actor="caste.updatedBy" class="card-text mt-2 small text-secondary" :date="caste.updatedOn" relative />
  </LinkableCard>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import LinkableCard from "@/components/shared/LinkableCard.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Caste } from "@/types/castes";

const { t } = useI18n();

defineProps<{
  caste: Caste;
}>();
</script>
