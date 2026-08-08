<template>
  <InputField
    :id="id"
    :label="label"
    :min="min"
    :max="max"
    :model-value="modelValue.toString()"
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
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import InputField from "@/components/forms/InputField.vue";

const { parseNumber } = parsingUtils;
const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    max?: number | string;
    min?: number | string;
    modelValue: number;
    step?: number | string;
  }>(),
  {
    id: "height",
    max: 9999,
    min: 0,
    step: 1,
  },
);

defineEmits<{
  (e: "update:model-value", value: number): void;
}>();

const label = computed<string>(() => `${t("characters.physical.height")} (${t("game.unit.cm")})`);
</script>
