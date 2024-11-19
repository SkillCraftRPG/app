<script setup lang="ts">
import { useI18n } from "vue-i18n";

import AppInput from "@/components/shared/AppInput.vue";
import type { WeightCategory } from "@/types/lineages";

const { t } = useI18n();

defineProps<{
  category: WeightCategory;
  modelValue?: string;
}>();

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <AppInput
    floating
    :id="`weight-${category}-roll`"
    :label="`game.weight.categories.${category}`"
    :model-value="modelValue?.toString()"
    :placeholder="`game.weight.categories.${category}`"
    :rules="{ roll: true }"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template #append>
      <span class="input-group-text">{{ t("game.units.bodyMassIndex") }}</span>
    </template>
  </AppInput>
</template>
