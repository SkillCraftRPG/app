<script setup lang="ts">
import AppInput from "@/components/shared/AppInput.vue";

defineProps<{
  disabled?: boolean | string;
  id?: string;
  modelValue?: string;
  required?: boolean | string;
}>();

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <AppInput
    :disabled="disabled"
    floating
    :id="id"
    label="lineages.names.category.label"
    max="255"
    :model-value="modelValue"
    placeholder="lineages.names.category.label"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
