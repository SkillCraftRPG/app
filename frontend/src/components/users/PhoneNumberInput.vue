<script setup lang="ts">
import AppInput from "@/components/shared/AppInput.vue";
import type { ShowStatus } from "@/types/validation";

defineProps<{
  modelValue?: string;
  required?: boolean | string;
  showStatus?: ShowStatus;
}>();

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <AppInput
    floating
    id="phone-number"
    label="users.phone.number"
    max="20"
    :model-value="modelValue"
    placeholder="users.phone.number"
    :required="required"
    :show-status="showStatus"
    type="tel"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
