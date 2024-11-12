<script setup lang="ts">
import { parsingUtils } from "logitar-js";

import AppInput from "@/components/shared/AppInput.vue";
import type { AgeCategory } from "@/types/lineages";

const { parseNumber } = parsingUtils;

withDefaults(
  defineProps<{
    category: AgeCategory;
    min?: number | string;
    modelValue?: number;
  }>(),
  {
    min: 1,
  },
);

defineEmits<{
  (e: "update:model-value", value?: number): void;
}>();

// TODO(fpion): prepend years/années
</script>

<template>
  <AppInput
    floating
    :id="`ages-${category}`"
    :label="`lineages.ages.categories.${category}`"
    :min="min"
    :model-value="modelValue?.toString()"
    :placeholder="`lineages.ages.categories.${category}`"
    step="1"
    type="number"
    @update:model-value="$emit('update:model-value', $event === '' ? undefined : parseNumber($event))"
  />
</template>
