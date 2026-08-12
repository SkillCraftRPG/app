<template>
  <InputField
    :id="id"
    :label="t(label)"
    :max="max"
    :min="minimumValue"
    :model-value="modelValue?.toString() ?? ''"
    :required="isRequired"
    type="number"
    @update:model-value="onUpdate"
  />
</template>

<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import InputField from "@/components/forms/InputField.vue";

const { parseBoolean, parseNumber } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    max?: number | string;
    modelValue?: number | null;
    required?: boolean | string;
  }>(),
  {
    id: "maximum-charges",
    label: "items.charges.maximum",
    max: 999,
  },
);

const emit = defineEmits<{
  (e: "update:model-value", value: number | undefined): void;
}>();

const isRequired = computed<boolean>(() => parseBoolean(props.required) ?? false);
const minimumValue = computed<number>(() => (isRequired.value ? 1 : 0));

function onUpdate(value: string): void {
  emit("update:model-value", value ? parseNumber(value) : undefined);
}
</script>
