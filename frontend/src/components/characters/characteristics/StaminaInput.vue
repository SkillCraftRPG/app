<script setup lang="ts">
import { parsingUtils } from "logitar-vue3-ui";

import AppInput from "@/components/shared/AppInput.vue";

const { parseNumber } = parsingUtils;

defineProps<{
  modelValue: number;
}>();

defineEmits<{
  (e: "update:model-value", value: number): void;
}>();
</script>

<template>
  <AppInput
    floating
    id="stamina"
    label="characters.stamina"
    min="0"
    max="999"
    :model-value="modelValue.toString()"
    placeholder="characters.stamina"
    required
    step="1"
    type="number"
    @update:model-value="$emit('update:model-value', parseNumber($event) ?? 0)"
  />
</template>
