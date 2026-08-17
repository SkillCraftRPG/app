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

const props = withDefaults(
  defineProps<{
    id?: string;
    min?: number | string;
    modelValue: number;
    step?: number | string;
    unit?: "cm" | "m";
  }>(),
  {
    id: "height",
    min: 0,
    unit: "m",
  },
);

defineEmits<{
  (e: "update:model-value", value: number): void;
}>();

const label = computed<string>(() => `${t("characters.physical.height")} (${t(`game.unit.${props.unit}`)})`);
const max = computed<number | undefined>(() => {
  switch (props.unit) {
    case "cm":
      return 9999;
    case "m":
      return 99.99;
  }
});
const step = computed<number | undefined>(() => {
  switch (props.unit) {
    case "cm":
      return 1;
    case "m":
      return 0.01;
  }
});
</script>
