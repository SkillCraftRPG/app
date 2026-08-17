<template>
  <TarSelect
    floating
    :id="id"
    :label="t(label)"
    :model-value="modelValue?.toString()"
    :options="options"
    @update:model-value="$emit('update:model-value', parseNumber($event) ?? 0)"
  />
</template>

<script setup lang="ts">
import { parsingUtils } from "logitar-js";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import TarSelect from "../tar/TarSelect.vue";
import type { SelectOption } from "@/types/tar/select";

const { parseNumber } = parsingUtils;
const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: number;
  }>(),
  {
    id: "count",
    label: "count",
  },
);

defineEmits<{
  (e: "update:model-value", value: number): void;
}>();

const options = ref<SelectOption[]>([{ text: "12" }, { text: "24" }, { text: "48" }, { text: "96" }]);
</script>
