<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { ValidationType } from "@/types/validation";

const { orderBy } = arrayUtils;
const { rt, tm } = useI18n();

withDefaults(
  defineProps<{
    disabled?: boolean | string;
    modelValue?: string;
    placeholder?: string;
    required?: boolean | string;
    validation?: ValidationType;
  }>(),
  {
    placeholder: "customizations.type.placeholder",
  },
);

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
    :disabled="disabled"
    floating
    id="type"
    label="customizations.type.label"
    :model-value="modelValue"
    :options="options"
    :placeholder="placeholder"
    :required="required"
    :validation="validation"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
