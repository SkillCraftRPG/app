<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { WeightCategory } from "@/types/lineages";

const { orderBy } = arrayUtils;
const { rt, tm } = useI18n();

defineProps<{
  modelValue?: WeightCategory;
}>();

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("game.weight.categories"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

defineEmits<{
  (e: "update:model-value", value?: WeightCategory): void;
}>();
</script>

<template>
  <AppSelect
    floating
    id="weight-category"
    label="game.weight.category"
    :model-value="modelValue"
    :options="options"
    placeholder="game.weight.category"
    required
    @update:model-value="$emit('update:model-value', $event === '' ? undefined : ($event as WeightCategory))"
  />
</template>
