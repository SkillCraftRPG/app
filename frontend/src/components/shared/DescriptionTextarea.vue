<script setup lang="ts">
import AppTextarea from "./AppTextarea.vue";

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    rows?: number | string;
  }>(),
  {
    id: "description",
    label: "description",
    placeholder: "description",
    rows: 15,
  },
);

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <AppTextarea
    floating
    :id="id"
    :label="label"
    :model-value="modelValue"
    :placeholder="placeholder"
    :rows="rows"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
