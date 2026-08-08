<template>
  <InputField
    :id="id"
    :label="t(label)"
    :max="max"
    :min="min"
    :model-value="modelValue?.toString() ?? ''"
    :step="step"
    type="number"
    @update:model-value="onUpdate"
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
    modelValue: number;
    step?: number;
  }>(),
  {
    id: "age",
    label: "lineages.physical.age.label",
    max: 9999,
    min: 0,
    step: 1,
  },
);

const emit = defineEmits<{
  (e: "update:model-value", value: number): void;
}>();

function onUpdate(value: string): void {
  emit("update:model-value", parseNumber(value) ?? 0);
}
</script>
