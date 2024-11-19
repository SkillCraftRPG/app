<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { AgeCategory } from "@/types/game";

const { orderBy } = arrayUtils;
const { rt, tm } = useI18n();

defineProps<{
  modelValue?: AgeCategory;
}>();

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("game.age.categories"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

defineEmits<{
  (e: "update:model-value", value?: AgeCategory): void;
}>();
</script>

<template>
  <AppSelect
    floating
    id="age-category"
    label="game.age.category"
    :model-value="modelValue"
    :options="options"
    placeholder="game.age.category"
    required
    @update:model-value="$emit('update:model-value', $event === '' ? undefined : ($event as AgeCategory))"
  />
</template>
