<script setup lang="ts">
import AppInput from "@/components/shared/AppInput.vue";

defineProps<{
  modelValue?: string;
  required?: boolean | string;
}>();

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <AppInput
    floating
    label="name"
    max="255"
    :model-value="modelValue"
    placeholder="name"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
