<template>
  <InputField :id="id" :label="t(label)" :max="max" :model-value="modelValue" @update:model-value="$emit('update:model-value', $event ?? '')" />
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import InputField from "@/components/forms/InputField.vue";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    max?: number | string;
    modelValue?: string;
  }>(),
  {
    id: "skin",
    label: "characters.physical.skin",
    max: 20,
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>
