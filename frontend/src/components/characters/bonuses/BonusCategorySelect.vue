<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { BonusCategory } from "@/types/characters";

const { orderBy } = arrayUtils;
const { rt, tm } = useI18n();

defineProps<{
  disabled?: boolean | string;
  modelValue?: BonusCategory;
  required?: boolean | string;
}>();

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("characters.bonuses.category.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

defineEmits<{
  (e: "update:model-value", value?: BonusCategory): void;
}>();
</script>

<template>
  <AppSelect
    :disabled="disabled"
    floating
    id="bonus-category"
    label="characters.bonuses.category.label"
    :model-value="modelValue"
    :options="options"
    placeholder="characters.bonuses.category.placeholder"
    :required="required"
    @update:model-value="$emit('update:model-value', $event === '' ? undefined : ($event as BonusCategory))"
  />
</template>
