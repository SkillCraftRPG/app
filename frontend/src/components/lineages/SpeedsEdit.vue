<script setup lang="ts">
import { useI18n } from "vue-i18n";

import SpeedInput from "./SpeedInput.vue";
import type { Speed } from "@/types/game";
import type { SpeedsModel } from "@/types/lineages";

const { t } = useI18n();

const props = defineProps<{
  modelValue: SpeedsModel;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: SpeedsModel): void;
}>();

function setSpeed(type: Speed, value?: number): void {
  const speeds: SpeedsModel = { ...props.modelValue };
  switch (type) {
    case "Walk":
      speeds.walk = value ?? 0;
      break;
    case "Climb":
      speeds.climb = value ?? 0;
      break;
    case "Swim":
      speeds.swim = value ?? 0;
      break;
    case "Fly":
      speeds.fly = value ?? 0;
      break;
    case "Hover":
      speeds.hover = value ?? 0;
      break;
    case "Burrow":
      speeds.burrow = value ?? 0;
      break;
    default:
      throw new Error(`The weight category "${type}" is not supported.`);
  }
  emit("update:model-value", speeds);
}
</script>

<template>
  <div>
    <h3>{{ t("lineages.speeds") }}</h3>
    <div class="row">
      <SpeedInput class="col-lg-2" :model-value="modelValue.walk" speed="Walk" @update:model-value="setSpeed('Walk', $event)" />
      <SpeedInput class="col-lg-2" :model-value="modelValue.climb" speed="Climb" @update:model-value="setSpeed('Climb', $event)" />
      <SpeedInput class="col-lg-2" :model-value="modelValue.swim" speed="Swim" @update:model-value="setSpeed('Swim', $event)" />
      <SpeedInput class="col-lg-2" :model-value="modelValue.fly" speed="Fly" @update:model-value="setSpeed('Fly', $event)" />
      <SpeedInput class="col-lg-2" :model-value="modelValue.hover" speed="Hover" @update:model-value="setSpeed('Hover', $event)" />
      <SpeedInput class="col-lg-2" :model-value="modelValue.burrow" speed="Burrow" @update:model-value="setSpeed('Burrow', $event)" />
    </div>
  </div>
</template>

<style scoped>
@media (min-width: 992px) {
  .col-weight {
    flex: 0 0 auto;
    width: 20%;
  }
}
</style>
