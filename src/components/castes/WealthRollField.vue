<template>
  <FormInput
    :id="id"
    :label="t(label)"
    :max="max"
    :model-value="modelValue"
    :placeholder="placeholder"
    @update:model-value="$emit('update:model-value', $event ?? '')"
  />
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    max?: number | string;
    modelValue?: string;
    placeholder?: string;
  }>(),
  {
    id: "wealth-roll",
    label: "castes.wealthRoll",
    max: 16,
    placeholder: "8d6",
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>
