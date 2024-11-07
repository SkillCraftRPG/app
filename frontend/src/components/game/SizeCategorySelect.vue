<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { SizeCategory } from "@/types/game";
import type { ValidationType } from "@/types/validation";

const { orderBy } = arrayUtils;
const { rt, tm } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: SizeCategory;
    validation?: ValidationType;
  }>(),
  {
    id: "size-category",
    label: "",
  },
);

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("game.size.categories"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

defineEmits<{
  (e: "update:model-value", value?: SizeCategory): void;
}>();
</script>

<template>
  <AppSelect
    floating
    :id="id"
    label="game.size.category"
    :model-value="modelValue"
    :options="options"
    placeholder="game.size.category"
    :validation="validation"
    @update:model-value="$emit('update:model-value', $event === '' ? undefined : ($event as SizeCategory))"
  />
</template>
