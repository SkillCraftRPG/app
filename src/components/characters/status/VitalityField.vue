<template>
  <InputField
    :id="id"
    :label="t(label)"
    :min="min"
    :max="max"
    :model-value="modelValue.toString()"
    :step="step"
    type="number"
    @update:model-value="$emit('update:model-value', parseNumber($event) ?? 0)"
  />
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
    step?: number | string;
  }>(),
  {
    id: "vitality",
    label: "game.statistic.options.Vitality",
    max: 999,
    min: 0,
    step: 1,
  },
);

defineEmits<{
  (e: "update:model-value", value: number): void;
}>();
</script>
