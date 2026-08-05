<template>
  <TarCard :subtitle="acquisition.qualifier ?? undefined">
    <template #title-override>
      <div class="d-flex flex-wrap align-items-center gap-2">
        <h5 class="card-title">{{ talent.name }}</h5>
        <TalentTierDisplay short :tier="talent.tier" />
      </div>
    </template>
    <div v-if="skill" class="mb-2"><font-awesome-icon icon="fas fa-kitchen-set" aria-hidden="true" />&nbsp;{{ skill }}</div>
    <div v-else-if="talent.allowMultiplePurchases" class="mb-2">
      <TarBadge pill variant="secondary">
        <font-awesome-icon icon="fas fa-tag" aria-hidden="true" />&nbsp;{{ t("talents.allowMultiplePurchases.label") }}
      </TarBadge>
    </div>
    <div v-if="talent.summary" class="card-text">{{ talent.summary }}</div>
    <div class="d-flex justify-content-between mt-2">
      <div>
        <!-- TODO(fpion): effective cost -->
      </div>
      <div class="d-flex gap-2">
        <!-- TODO(fpion): edit button -->
        <RemoveCharacterTalent :acquisition="acquisition" @removed="$emit('removed')" />
      </div>
    </div>
  </TarCard>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import RemoveCharacterTalent from "./RemoveCharacterTalent.vue";
import TalentTierDisplay from "@/components/talents/TalentTierDisplay.vue";
import TarBadge from "@/components/tar/TarBadge.vue";
import TarCard from "@/components/tar/TarCard.vue";
import type { CharacterTalent } from "@/types/characters";
import type { Talent } from "@/types/talents";

const { t } = useI18n();

const props = defineProps<{
  acquisition: CharacterTalent;
}>();

defineEmits<{
  (e: "removed"): void;
}>();

const talent = computed<Talent>(() => props.acquisition.talent);
const skill = computed<string>(() => (talent.value.skill ? t(`game.skill.options.${talent.value.skill}`) : ""));
</script>
