<template>
  <BooleanSelect :id="id" :label="label" :model-value="modelValue" @update:model-value="$emit('update:model-value', $event)" />
</template>

<script setup lang="ts">
import BooleanSelect from "@/components/shared/BooleanSelect.vue";

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: boolean;
  }>(),
  {
    id: "allow-multiple-purchases",
    label: "talents.allowMultiplePurchases.label",
  },
);

defineEmits<{
  (e: "update:model-value", value: boolean | undefined): void;
}>();
</script>
