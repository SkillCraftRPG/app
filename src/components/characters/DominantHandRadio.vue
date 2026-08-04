<template>
  <div>
    <div class="form-label">{{ t(label) }}</div>
    <div class="d-flex flex-wrap gap-3">
      <div v-for="option in options" :key="option" class="form-check">
        <input
          :checked="modelValue === option"
          class="form-check-input"
          :id="`${id}-${option}`"
          :name="id"
          type="radio"
          :value="option"
          @change="$emit('update:model-value', option)"
        />
        <label class="form-check-label" :for="`${id}-${option}`">{{ t(`characters.dominantHand.options.${option}`) }}</label>
      </div>
      <div class="form-check">
        <input
          :checked="modelValue === null"
          class="form-check-input"
          :id="`${id}-unspecified`"
          :name="id"
          type="radio"
          @change="$emit('update:model-value', null)"
        />
        <label class="form-check-label" :for="`${id}-unspecified`">{{ t(`characters.dominantHand.options.Unspecified`) }}</label>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import type { DominantHand } from "@/types/characters";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue: DominantHand | null;
  }>(),
  {
    id: "dominant-hand",
    label: "characters.dominantHand.label",
  },
);

defineEmits<{
  (e: "update:model-value", value: DominantHand | null): void;
}>();

const options: DominantHand[] = ["Right", "Left"];
</script>
