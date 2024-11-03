<script setup lang="ts">
import AppInput from "@/components/shared/AppInput.vue";

defineProps<{
  modelValue?: string;
}>();

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <AppInput
    floating
    id="script"
    label="languages.script.label"
    max="255"
    :model-value="modelValue"
    placeholder="languages.script.label"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
