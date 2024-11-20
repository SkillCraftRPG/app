<script setup lang="ts">
import { useI18n } from "vue-i18n";

import WeightRollInput from "./WeightRollInput.vue";
import type { WeightCategory, WeightModel } from "@/types/lineages";

const { t } = useI18n();

const props = defineProps<{
  modelValue: WeightModel;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: WeightModel): void;
}>();

function setRoll(category: WeightCategory, value?: string): void {
  const weight: WeightModel = { ...props.modelValue };
  switch (category) {
    case "Starved":
      weight.starved = value ?? undefined;
      break;
    case "Skinny":
      weight.skinny = value ?? undefined;
      break;
    case "Normal":
      weight.normal = value ?? undefined;
      break;
    case "Overweight":
      weight.overweight = value ?? undefined;
      break;
    case "Obese":
      weight.obese = value ?? undefined;
      break;
    default:
      throw new Error(`The weight category "${category}" is not supported.`);
  }
  emit("update:model-value", weight);
}
</script>

<template>
  <div>
    <h3>{{ t("game.weight.label") }}</h3>
    <div class="row">
      <WeightRollInput category="Starved" class="col" :model-value="modelValue.starved" @update:model-value="setRoll('Starved', $event)" />
      <WeightRollInput category="Skinny" class="col" :model-value="modelValue.skinny" @update:model-value="setRoll('Skinny', $event)" />
      <WeightRollInput category="Normal" class="col" :model-value="modelValue.normal" @update:model-value="setRoll('Normal', $event)" />
      <WeightRollInput category="Overweight" class="col" :model-value="modelValue.overweight" @update:model-value="setRoll('Overweight', $event)" />
      <WeightRollInput category="Obese" class="col" :model-value="modelValue.obese" @update:model-value="setRoll('Obese', $event)" />
    </div>
  </div>
</template>
