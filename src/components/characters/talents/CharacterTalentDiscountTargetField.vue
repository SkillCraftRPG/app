<template>
  <SelectField
    v-if="Array.isArray(options)"
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder)"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  />
  <InputField
    v-else
    :id="id"
    :label="t(label)"
    :max="max"
    :model-value="modelValue"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import InputField from "@/components/forms/InputField.vue";
import SelectField from "@/components/forms/SelectField.vue";
import type { SelectOption } from "@/types/tar/select";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    max?: number | string;
    modelValue: string;
    options?: SelectOption[];
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    id: "target",
    label: "characters.talents.discount.target",
    max: 50,
    placeholder: "characters.talents.discount.source.placeholder",
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

// TODO(fpion): required does not seem to work?
</script>
