<template>
  <InputField
    :id="id"
    :label="t('characters.modifiers.value')"
    :model-value="modelValue.toString()"
    :required="required"
    :rules="rules"
    :step="step"
    type="number"
    @update:model-value="$emit('update:model-value', parseNumber($event) ?? 0)"
  />
</template>

<script setup lang="ts">
import type { ValidationRuleSet } from "logitar-validation";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import InputField from "@/components/forms/InputField.vue";

const { parseNumber } = parsingUtils;
const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    modelValue: number;
    required?: boolean | string;
    step?: number | string;
  }>(),
  {
    id: "value",
    step: 1,
  },
);

defineEmits<{
  (e: "update:model-value", value: number): void;
}>();

const rules = computed<ValidationRuleSet>(() => ({
  notEqual: "0",
}));
</script>
