<script setup lang="ts">
import AppInput from "@/components/shared/AppInput.vue";

withDefaults(
  defineProps<{
    id?: string;
    modelValue?: string;
    required?: boolean | string;
  }>(),
  {
    id: "roll",
  },
);

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <AppInput
    floating
    :id="id"
    label="game.damage.roll"
    :model-value="modelValue?.toString()"
    placeholder="game.damage.roll"
    :required="required"
    :rules="{ roll: true }"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template #prepend>
      <slot name="prepend"></slot>
    </template>
  </AppInput>
</template>
