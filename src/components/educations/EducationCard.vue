<template>
  <LinkCard :subtitle="education.feature?.name" :title="education.name" :to="{ name: 'Education', params: { id: education.id } }">
    <template #subtitle-override>
      <h6 class="card-subtitle mb-2 text-body-secondary">
        <template v-if="education.feature">{{ education.feature.name }}</template>
        <template v-else>&mdash;</template>
      </h6>
    </template>
    <div class="card-text d-flex justify-content-between align-items-center gap-2 mb-2">
      <div class="text-start">
        <font-awesome-icon icon="fas fa-kitchen-set" aria-hidden="true" />&nbsp;<span v-if="education.skill">{{
          t(`game.skill.options.${education.skill}`)
        }}</span
        ><span v-else class="text-muted">&mdash;</span>
      </div>
      <div class="text-end">
        <font-awesome-icon icon="fas fa-coins" aria-hidden="true" />&nbsp;<span v-if="education.wealthMultiplier">×{{ education.wealthMultiplier }}</span
        ><span v-else class="text-muted">&mdash;</span>
      </div>
    </div>
    <div class="card-text">
      <span v-if="education.summary">{{ education.summary }}</span>
      <span class="text-muted" v-else>&mdash;</span>
    </div>
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
