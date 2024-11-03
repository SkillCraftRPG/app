<script setup lang="ts">
import { parsingUtils } from "logitar-js";

import AppInput from "@/components/shared/AppInput.vue";

const { parseNumber } = parsingUtils;

defineProps<{
  modelValue?: number;
}>();

defineEmits<{
  (e: "update:model-value", value?: number): void;
}>();
</script>

<template>
  <AppInput
    floating
    id="wealth-multiplier"
    label="educations.wealthMultiplier"
    min="1"
    max="12"
    :model-value="modelValue?.toString()"
    placeholder="educations.wealthMultiplier"
    step="1"
    type="number"
    @update:model-value="$emit('update:model-value', $event === '' ? undefined : parseNumber($event))"
  >
    <template #prepend>
      <span class="input-group-text">{{ "×" }}</span>
    </template>
  </AppInput>
</template>
