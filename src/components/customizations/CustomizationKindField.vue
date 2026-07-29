<template>
  <FormSelect
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
import { useI18n } from "vue-i18n";

import FormSelect from "@/components/forms/FormSelect.vue";
import { computed } from "vue";
import type { SelectOption } from "@/types/tar/select";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue: string;
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    id: "kind",
    label: "customizations.kind.label",
    placeholder: "customizations.kind.placeholder",
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

const options = computed<SelectOption[]>(() => ["Gift", "Disability"].map((value) => ({ text: t(`customizations.kind.options.${value}`), value })));
</script>
