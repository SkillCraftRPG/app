<script setup lang="ts">
import { parsingUtils } from "logitar-js";

import AppInput from "@/components/shared/AppInput.vue";

const { parseNumber } = parsingUtils;

defineProps<{
  modelValue?: number;
  required?: boolean | string;
}>();

defineEmits<{
  (e: "update:model-value", value?: number): void;
}>();

// TODO(fpion): not equal 0
</script>

<template>
  <AppInput
    floating
    id="value"
    label="characters.bonuses.value"
    :model-value="modelValue?.toString()"
    placeholder="characters.bonuses.value"
    :required="required"
    step="1"
    type="number"
    @update:model-value="$emit('update:model-value', $event === '' ? undefined : parseNumber($event))"
  />
</template>
