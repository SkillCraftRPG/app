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
    id="wealth-roll"
    label="castes.wealthRoll"
    :model-value="modelValue?.toString()"
    placeholder="castes.wealthRoll"
    :rules="{ roll: true }"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
