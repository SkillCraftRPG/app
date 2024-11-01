<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import AppSelect from "@/components/shared/AppSelect.vue";

const { orderBy } = arrayUtils;
const { rt, tm } = useI18n();

defineProps<{
  modelValue?: string;
  required?: boolean | string;
}>();

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("customizations.type.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <AppSelect
    floating
    id="type"
    label="customizations.type.label"
    :model-value="modelValue"
    :options="options"
    placeholder="customizations.type.placeholder"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
