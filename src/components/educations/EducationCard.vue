<template>
  <LinkCard :subtitle="education.feature?.name" :title="education.name" :to="{ name: 'Education', params: { id: education.id } }">
    <div v-if="education.skill || education.wealthMultiplier" class="card-text d-flex justify-content-between align-items-center gap-2 mb-2">
      <div class="text-start">
        <template v-if="education.skill">
          <font-awesome-icon icon="fas fa-kitchen-set" aria-hidden="true" />&nbsp;{{ t(`game.skill.options.${education.skill}`) }}
        </template>
      </div>
      <div v-if="education.wealthMultiplier" class="text-end">
        <font-awesome-icon icon="fas fa-coins" aria-hidden="true" />&nbsp;×{{ education.wealthMultiplier }}
      </div>
    </div>
    <div v-if="education.summary" class="card-text">{{ education.summary }}</div>
    <StatusBlock :actor="education.updatedBy" class="card-text mt-2 small text-secondary" :date="education.updatedOn" relative />
  </LinkCard>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import LinkCard from "@/components/shared/LinkCard.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Education } from "@/types/educations";

const { t } = useI18n();

defineProps<{
  education: Education;
}>();
</script>
