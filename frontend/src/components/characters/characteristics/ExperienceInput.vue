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

// TODO(fpion): min should depend upon character current level
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
  />
</template>
