<template>
  <InputField
    :id="id"
    :label="t(label)"
    :min="min"
    :max="max"
    :model-value="modelValue.toString()"
    :required="required"
    :step="step"
    type="number"
    @update:model-value="$emit('update:model-value', parseNumber($event) ?? 0)"
  >
    <template #append>
      <slot name="append"></slot>
    </template>
  </InputField>
</template>

<script setup lang="ts">
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import InputField from "@/components/forms/InputField.vue";

const { parseNumber } = parsingUtils;
const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    max?: number | string;
    min?: number | string;
    modelValue: number | string;
    required?: boolean | string;
    step?: number | string;
  }>(),
  {
    id: "experience",
    label: "characters.experience.label",
    max: 999999,
    min: 0,
    step: 1,
  },
);

defineEmits<{
  (e: "update:model-value", value: number): void;
}>();
</script>
