<template>
  <TarInput
    floating
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :placeholder="t(label)"
    :type="type"
    @update:model-value="$emit('update:model-value', $event ?? '')"
  />
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import TarInput from "@/components/tar/TarInput.vue";
import type { InputType } from "@/types/tar/input";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    type?: InputType;
  }>(),
  {
    id: "search",
    label: "search",
    type: "search",
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>
