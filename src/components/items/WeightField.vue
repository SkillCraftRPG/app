<template>
  <FormInput :id="id" :label="t(label)" :min="min" :model-value="modelValue?.toString() ?? ''" :step="step" type="number" @update:model-value="onUpdate" />
</template>

<script setup lang="ts">
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";

const { parseNumber } = parsingUtils;
const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    min?: number | string;
    modelValue?: number | null;
    step?: number | string;
  }>(),
  {
    id: "weight",
    label: "items.weight",
    min: 0,
    step: 0.01,
  },
);

const emit = defineEmits<{
  (e: "update:model-value", value: number | undefined): void;
}>();

function onUpdate(value: string): void {
  emit("update:model-value", value ? parseNumber(value) : undefined);
}
</script>
