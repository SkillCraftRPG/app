<script setup lang="ts">
import { parsingUtils } from "logitar-js";

import AppInput from "@/components/shared/AppInput.vue";
import type { ValidationType } from "@/types/validation";

const { parseNumber } = parsingUtils;

defineProps<{
  disabled?: boolean | string;
  modelValue?: number;
  required?: boolean | string;
  validation?: ValidationType;
}>();

defineEmits<{
  (e: "update:model-value", value?: number): void;
}>();
</script>

<template>
  <AppInput
    :disabled="disabled"
    floating
    id="extra-languages"
    label="lineages.languages.extra"
    min="0"
    max="3"
    :model-value="modelValue?.toString()"
    placeholder="lineages.languages.extra"
    :required="required"
    step="1"
    type="number"
    :validation="validation"
    @update:model-value="$emit('update:model-value', $event === '' ? undefined : parseNumber($event))"
  />
</template>
