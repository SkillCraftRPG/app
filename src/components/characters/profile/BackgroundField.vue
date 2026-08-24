<template>
  <TextareaField :id="id" :label="t(label)" :model-value="modelValue" :rows="rows" @update:model-value="$emit('update:model-value', $event)" />
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import TextareaField from "@/components/forms/TextareaField.vue";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    rows?: number | string;
  }>(),
  {
    id: "background",
    label: "characters.background.label",
    rows: 20,
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>
