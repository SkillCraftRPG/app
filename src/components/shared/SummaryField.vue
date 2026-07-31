<template>
  <FormTextarea :id="id" :label="t(label)" :max="max" :model-value="modelValue" :rows="rows" @update:model-value="$emit('update:model-value', $event)" />
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import FormTextarea from "@/components/forms/FormTextarea.vue";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    max?: number | string;
    modelValue?: string;
    rows?: number | string;
  }>(),
  {
    id: "summary",
    label: "summary",
    max: 100,
    rows: 3,
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>
