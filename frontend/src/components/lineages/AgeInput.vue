<script setup lang="ts">
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import AppInput from "@/components/shared/AppInput.vue";
import type { AgeCategory } from "@/types/game";

const { parseNumber } = parsingUtils;
const { t } = useI18n();

withDefaults(
  defineProps<{
    category: AgeCategory;
    min?: number | string;
    modelValue?: number;
  }>(),
  {
    min: 1,
  },
);

defineEmits<{
  (e: "update:model-value", value?: number): void;
}>();
</script>

<template>
  <AppInput
    floating
    :id="`ages-${category}`"
    :label="`game.age.categories.${category}`"
    :min="min"
    :model-value="modelValue?.toString()"
    :placeholder="`game.age.categories.${category}`"
    step="1"
    type="number"
    @update:model-value="$emit('update:model-value', $event === '' ? undefined : parseNumber($event))"
  >
    <template #append>
      <span class="input-group-text">{{ t("game.units.year", modelValue ?? 0) }}</span>
    </template>
  </AppInput>
</template>
