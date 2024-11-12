<script setup lang="ts">
import AppInput from "@/components/shared/AppInput.vue";
import type { WeightCategory } from "@/types/lineages";

defineProps<{
  category: WeightCategory;
  modelValue?: string;
}>();

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();

// TODO(fpion): prepend BMI
</script>

<template>
  <AppInput
    floating
    :id="`weight-${category}-roll`"
    :label="`lineages.weight.categories.${category}`"
    :model-value="modelValue?.toString()"
    :placeholder="`lineages.weight.categories.${category}`"
    :rules="{ roll: true }"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
