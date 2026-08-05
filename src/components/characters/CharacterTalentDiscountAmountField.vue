<template>
  <InputField
    :id="id"
    :label="t(label)"
    :max="max"
    :min="min"
    :model-value="modelValue.toString()"
    :required="required"
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
    modelValue: number;
    required?: boolean | string;
  }>(),
  {
    id: "amount",
    label: "characters.talents.discount.amount",
    max: 1,
    min: 1,
  },
);

defineEmits<{
  (e: "update:model-value", value: number): void;
}>();
</script>
