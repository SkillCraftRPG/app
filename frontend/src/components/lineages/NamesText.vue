<script setup lang="ts">
import AppTextarea from "@/components/shared/AppTextarea.vue";

defineProps<{
  modelValue?: string;
}>();

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <AppTextarea
    floating
    id="names-text"
    label="lineages.names.text"
    max="65535"
    :model-value="modelValue"
    placeholder="lineages.names.text"
    rows="3"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
