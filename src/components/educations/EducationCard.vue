<template>
  <LinkableCard :clickable="clickable" :selected="selected" :subtitle="education.feature?.name" :title="education.name" :to="to">
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
    <slot>
      <StatusBlock :actor="education.updatedBy" class="card-text mt-2 small text-secondary" :date="education.updatedOn" relative />
    </slot>
  </LinkableCard>
</template>

<script setup lang="ts">
import type { RouteLocationAsPathGeneric, RouteLocationAsRelativeGeneric } from "vue-router";
import { useI18n } from "vue-i18n";

import LinkableCard from "@/components/shared/LinkableCard.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Education } from "@/types/educations";

const { t } = useI18n();

defineProps<{
  clickable?: boolean | string;
  education: Education;
  selected?: boolean | string;
  to?: string | RouteLocationAsRelativeGeneric | RouteLocationAsPathGeneric;
}>();

// TODO(fpion): implement
</script>
