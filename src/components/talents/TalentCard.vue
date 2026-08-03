<template>
  <LinkableCard :clickable="clickable" :selected="selected" :to="to">
    <template #title-override>
      <div class="d-flex flex-wrap align-items-center gap-2">
        <h5 class="card-title">{{ talent.name }}</h5>
        <TalentTierDisplay short :tier="talent.tier" />
      </div>
    </template>
    <template v-if="talent.requiredTalent" #subtitle-override>
      <h6 class="card-subtitle mb-2 text-body-secondary">
        <font-awesome-icon icon="fas fa-code-branch" aria-hidden="true" />&nbsp;{{ talent.requiredTalent.name }}
      </h6>
    </template>
    <div v-if="talent.skill" class="mb-2">
      <font-awesome-icon icon="fas fa-kitchen-set" aria-hidden="true" />&nbsp;{{ t(`game.skill.options.${talent.skill}`) }}
    </div>
    <div v-else-if="talent.allowMultiplePurchases" class="mb-2">
      <TarBadge pill variant="secondary">
        <font-awesome-icon icon="fas fa-tag" aria-hidden="true" />&nbsp;{{ t("talents.allowMultiplePurchases.label") }}
      </TarBadge>
    </div>
    <div v-if="talent.summary" class="card-text">{{ talent.summary }}</div>
    <slot>
      <StatusBlock :actor="talent.updatedBy" class="card-text mt-2 small text-secondary" :date="talent.updatedOn" relative />
    </slot>
  </LinkableCard>
</template>

<script setup lang="ts">
import type { RouteLocationAsPathGeneric, RouteLocationAsRelativeGeneric } from "vue-router";
import { useI18n } from "vue-i18n";

import LinkableCard from "@/components/shared/LinkableCard.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import TalentTierDisplay from "./TalentTierDisplay.vue";
import TarBadge from "@/components/tar/TarBadge.vue";
import type { Talent } from "@/types/talents";

const { t } = useI18n();

defineProps<{
  talent: Talent;
  clickable?: boolean | string;
  selected?: boolean | string;
  to?: string | RouteLocationAsRelativeGeneric | RouteLocationAsPathGeneric;
}>();
</script>
