<template>
  <SelectField
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder)"
    @update:model-value="$emit('update:model-value', $event ?? '')"
  />
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import SelectField from "@/components/forms/SelectField.vue";
import type { SelectOption } from "@/types/tar/select";
import type { SizeCategory } from "@/types/game";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue: string;
    placeholder?: string;
  }>(),
  {
    id: "size-category",
    label: "game.size.category.label",
    placeholder: "game.size.category.placeholder",
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

const categories: SizeCategory[] = ["Diminutive", "Tiny", "Small", "Medium", "Large", "Huge", "Gargantuan", "Colossal"];

const options = computed<SelectOption[]>(() => categories.map((value) => ({ text: t(`game.size.category.options.${value}`), value })));
</script>
