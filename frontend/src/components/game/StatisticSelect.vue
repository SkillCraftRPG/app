<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { Statistic } from "@/types/game";
import type { ValidationType } from "@/types/validation";

const { orderBy } = arrayUtils;
const { rt, tm } = useI18n();

withDefaults(
  defineProps<{
    disabled?: boolean | string;
    id?: string;
    label?: string;
    modelValue?: Statistic;
    placeholder?: string;
    validation?: ValidationType;
  }>(),
  {
    id: "statistic",
    label: "game.statistic.label",
    placeholder: "game.statistic.placeholder",
  },
);

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("game.statistic.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

defineEmits<{
  (e: "update:model-value", value?: Statistic): void;
}>();
</script>

<template>
  <AppSelect
    :disabled="disabled"
    floating
    :id="id"
    :label="label"
    :model-value="modelValue"
    :options="options"
    :placeholder="placeholder"
    :validation="validation"
    @update:model-value="$emit('update:model-value', $event === '' ? undefined : ($event as Statistic))"
  />
</template>
