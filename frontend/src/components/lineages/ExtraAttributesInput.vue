<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";

import AppInput from "@/components/shared/AppInput.vue";
import type { AttributeBonusesModel } from "@/types/lineages";

const { parseNumber } = parsingUtils;

const props = defineProps<{
  attributes: AttributeBonusesModel;
  modelValue?: number;
}>();

const max = computed<number>(() => {
  let max: number = 7;
  Object.entries(props.attributes).forEach(([key, value]) => {
    if (key !== "extra" && value > 0) {
      max--;
    }
  });
  return max;
});

defineEmits<{
  (e: "update:model-value", value?: number): void;
}>();
</script>

<template>
  <AppInput
    floating
    id="extra-attributes"
    label="lineages.attributes.extra"
    min="0"
    :max="max"
    :model-value="modelValue?.toString()"
    placeholder="lineages.attributes.extra"
    required
    step="1"
    type="number"
    @update:model-value="$emit('update:model-value', $event === '' ? undefined : parseNumber($event))"
  />
</template>
