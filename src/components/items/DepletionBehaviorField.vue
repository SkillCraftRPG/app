<template>
  <SelectField
    floating
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder)"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import SelectField from "@/components/forms/SelectField.vue";
import type { SelectOption } from "@/types/tar/select";

const { orderBy } = arrayUtils;
const { rt, t, tm } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue: string;
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    id: "depletion-behavior",
    label: "items.charges.depletionBehavior.label",
    placeholder: "items.charges.depletionBehavior.placeholder",
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("items.charges.depletionBehavior.options"))).map(([value, text]) => ({ text, value })),
    "text",
  ),
);
</script>
