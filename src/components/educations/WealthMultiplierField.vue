<template>
  <InputField :id="id" :label="t(label)" :max="max" :min="min" :model-value="modelValue?.toString() ?? ''" type="number" @update:model-value="onUpdate" />
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
    modelValue?: number | null;
  }>(),
  {
    id: "wealth-multiplier",
    label: "educations.wealthMultiplier",
    max: 999,
    min: 0,
  },
);

const emit = defineEmits<{
  (e: "update:model-value", value: number | undefined): void;
}>();

function onUpdate(value: string): void {
  emit("update:model-value", value ? parseNumber(value) : undefined);
}
</script>
