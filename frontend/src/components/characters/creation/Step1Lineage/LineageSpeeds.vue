<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import SpeedInput from "@/components/lineages/SpeedInput.vue";
import type { LineageModel, SpeedsModel } from "@/types/lineages";

const { t } = useI18n();

const props = defineProps<{
  lineage: LineageModel;
}>();

const speeds = computed<SpeedsModel>(() => {
  const speeds: SpeedsModel = { ...props.lineage.speeds };
  const species: LineageModel | undefined = props.lineage.species;
  if (species) {
    if (props.lineage.speeds.walk === 0) {
      speeds.walk = species.speeds.walk;
    }
    if (props.lineage.speeds.climb === 0) {
      speeds.climb = species.speeds.climb;
    }
    if (props.lineage.speeds.swim === 0) {
      speeds.swim = species.speeds.swim;
    }
    if (props.lineage.speeds.fly === 0) {
      speeds.fly = species.speeds.fly;
    }
    if (props.lineage.speeds.hover === 0) {
      speeds.hover = species.speeds.hover;
    }
    if (props.lineage.speeds.burrow === 0) {
      speeds.burrow = species.speeds.burrow;
    }
  }
  return speeds;
});
</script>

<template>
  <div>
    <h5>{{ t("lineages.speeds") }}</h5>
    <div class="row">
      <SpeedInput class="col" disabled :model-value="speeds.walk" speed="Walk" validation="server" />
      <SpeedInput class="col" disabled :model-value="speeds.climb" speed="Climb" validation="server" />
      <SpeedInput class="col" disabled :model-value="speeds.swim" speed="Swim" validation="server" />
      <SpeedInput class="col" disabled :model-value="speeds.fly" speed="Fly" validation="server" />
      <SpeedInput class="col" disabled :model-value="speeds.hover" speed="Hover" validation="server" />
      <SpeedInput class="col" disabled :model-value="speeds.burrow" speed="Burrow" validation="server" />
    </div>
  </div>
</template>
