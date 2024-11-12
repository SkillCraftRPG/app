<script setup lang="ts">
import { parsingUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import AppInput from "@/components/shared/AppInput.vue";
import type { AgeCategory } from "@/types/lineages";

const { parseNumber } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    category: AgeCategory;
    min?: number | string;
    modelValue?: number;
  }>(),
  {
    min: 1,
  },
);

const unit = computed<string>(() => (typeof props.modelValue === "number" && props.modelValue > 1 ? "years" : "year"));

defineEmits<{
  (e: "update:model-value", value?: number): void;
}>();
</script>

<template>
  <AppInput
    floating
    :id="`ages-${category}`"
    :label="`lineages.ages.categories.${category}`"
    :min="min"
    :model-value="modelValue?.toString()"
    :placeholder="`lineages.ages.categories.${category}`"
    step="1"
    type="number"
    @update:model-value="$emit('update:model-value', $event === '' ? undefined : parseNumber($event))"
  >
    <template #append>
      <span class="input-group-text">{{ t(`game.units.${unit}`) }}</span>
    </template>
  </AppInput>
</template>
