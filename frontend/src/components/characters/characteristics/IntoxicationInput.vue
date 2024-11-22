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
    id="intoxication"
    label="characters.intoxication"
    min="0"
    max="99"
    :model-value="modelValue.toString()"
    placeholder="characters.intoxication"
    required
    step="1"
    type="number"
    @update:model-value="$emit('update:model-value', parseNumber($event) ?? 0)"
  />
</template>
