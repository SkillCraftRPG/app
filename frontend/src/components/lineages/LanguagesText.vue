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
    id="languages-text"
    label="lineages.languages.text"
    max="65535"
    :model-value="modelValue"
    placeholder="lineages.languages.text"
    rows="5"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
