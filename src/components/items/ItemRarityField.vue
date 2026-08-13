<template>
  <SelectField
    floating
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder)"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import SelectField from "@/components/forms/SelectField.vue";
import { computed } from "vue";
import type { SelectOption } from "@/types/tar/select";

const { orderBy } = arrayUtils;
const { rt, t, tm } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue: string;
    placeholder?: string;
  }>(),
  {
    id: "rarity",
    label: "items.rarity.label",
    placeholder: "items.rarity.placeholder",
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("items.rarity.options"))).map(([value, text]) => ({ text, value })),
    "text",
  ),
);
</script>
