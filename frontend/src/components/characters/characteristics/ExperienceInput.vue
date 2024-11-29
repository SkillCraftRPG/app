<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-vue3-ui";

import AppInput from "@/components/shared/AppInput.vue";
import { getTotalExperience } from "@/helpers/gameUtils";

const { parseNumber } = parsingUtils;

const props = defineProps<{
  level?: number | string;
  modelValue: number;
}>();

const currentTotal = computed<number>(() => (typeof parsedLevel.value === "number" ? getTotalExperience(parsedLevel.value) : 0));
const nextTotal = computed<number>(() =>
  typeof parsedLevel.value === "number" ? getTotalExperience(parsedLevel.value < 20 ? parsedLevel.value + 1 : parsedLevel.value) : 0,
);
const parsedLevel = computed<number | undefined>(() => parseNumber(props.level));
const percentage = computed<number>(() => {
  const percentage: number = ((props.modelValue - currentTotal.value) / (nextTotal.value - currentTotal.value)) * 100.0;
  return percentage > 100 ? 100 : Math.round(percentage);
});

defineEmits<{
  (e: "update:model-value", value: number): void;
}>();
</script>

<template>
  <AppInput
    floating
    id="experience"
    label="characters.experience"
    min="0"
    max="999999"
    :model-value="modelValue.toString()"
    placeholder="characters.experience"
    required
    step="1"
    type="number"
    @update:model-value="$emit('update:model-value', parseNumber($event) ?? 0)"
  >
    <template #append v-if="typeof parsedLevel === 'number'">
      <span class="input-group-text">/ {{ nextTotal }} ({{ percentage }} %)</span>
    </template>
  </AppInput>
</template>
