<template>
  <TarInput
    floating
    :id="id"
    :label="t(label)"
    :max="max"
    :model-value="modelValue"
    :placeholder="t(label)"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import TarInput from "@/components/tar/TarInput.vue";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    max?: number | string;
    modelValue?: string;
    required?: boolean | string;
  }>(),
  {
    id: "first-name",
    max: 50,
    label: "account.name.first",
  },
);

defineEmits<{
  (e: "update:model-value", value: string | undefined): void;
}>();
</script>
