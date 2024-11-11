<script setup lang="ts">
import AppTextarea from "@/components/shared/AppTextarea.vue";

defineProps<{
  id?: string;
  modelValue?: string;
}>();

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <AppTextarea
    floating
    :id="id"
    label="lineages.names.label"
    :model-value="modelValue"
    placeholder="lineages.names.label"
    rows="10"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
