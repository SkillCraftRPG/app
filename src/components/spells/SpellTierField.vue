<template>
  <SelectField
    :id="id"
    :label="t(label)"
    :model-value="modelValue?.toString()"
    :options="options"
    :placeholder="t(placeholder)"
    :required="required"
    @update:model-value="onModelValueUpdate"
  />
</template>

<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import SelectField from "@/components/forms/SelectField.vue";
import type { SelectOption } from "@/types/tar/select";

const { parseNumber } = parsingUtils;
const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: number;
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    id: "tier",
    label: "spells.tier.label",
    placeholder: "spells.tier.placeholder",
  },
);

const emit = defineEmits<{
  (e: "update:model-value", value: number | undefined): void;
}>();

const options = computed<SelectOption[]>(() => [0, 1, 2, 3].map((value) => ({ text: value.toString(), value: value.toString() })));

function onModelValueUpdate(value: string): void {
  emit("update:model-value", parseNumber(value || undefined));
}
</script>
