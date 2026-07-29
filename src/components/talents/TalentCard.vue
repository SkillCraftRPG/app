<template>
  <LinkCard :to="{ name: 'Talent', params: { id: talent.id } }">
    <template #title-override>
      <div class="d-flex flex-wrap align-items-center gap-2">
        <h5 class="card-title">{{ talent.name }}</h5>
        <TarBadge variant="secondary">{{ talent.tier }}</TarBadge>
      </div>
    </template>
    <template v-if="talent.requiredTalent" #subtitle-override>
      <h6 class="card-subtitle mb-2 text-body-secondary">
        <font-awesome-icon icon="fas fa-code-branch" aria-hidden="true" />&nbsp;{{ talent.requiredTalent.name }}
      </h6>
    </template>
    <div v-if="talent.skill"><font-awesome-icon icon="fas fa-kitchen-set" aria-hidden="true" />&nbsp;{{ t(`game.skill.options.${talent.skill}`) }}</div>
    <div v-else-if="talent.allowMultiplePurchases">
      <TarBadge pill variant="secondary"><font-awesome-icon icon="fas fa-tag" aria-hidden="true" />&nbsp;{{ t("talents.allowMultiplePurchases") }}</TarBadge>
    </div>
    <div v-if="talent.summary" class="card-text">{{ talent.summary }}</div>
    <StatusBlock :actor="talent.updatedBy" class="card-text mt-2 small text-secondary" :date="talent.updatedOn" relative />
  </LinkCard>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import LinkCard from "@/components/shared/LinkCard.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import TarBadge from "@/components/tar/TarBadge.vue";
import type { Talent } from "@/types/talents";

const { t } = useI18n();

defineProps<{
  talent: Talent;
}>();
</script>
