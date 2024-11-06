<script setup lang="ts">
import { parsingUtils } from "logitar-js";

import AppInput from "@/components/shared/AppInput.vue";

const { parseNumber } = parsingUtils;

defineProps<{
  id?: string;
  label: string;
  modelValue?: number;
  placeholder?: string;
}>();

defineEmits<{
  (e: "update:model-value", value?: number): void;
}>();
</script>

<template>
  <AppInput
    floating
    :id="id"
    :label="label"
    min="1"
    :model-value="modelValue?.toString()"
    :placeholder="placeholder"
    step="1"
    type="number"
    @update:model-value="$emit('update:model-value', $event === '' ? undefined : parseNumber($event))"
  />
</template>
