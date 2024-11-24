<script setup lang="ts">
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import AppInput from "@/components/shared/AppInput.vue";

const { parseNumber } = parsingUtils;
const { t } = useI18n();

defineProps<{
  modelValue?: number;
}>();

defineEmits<{
  (e: "update:model-value", weight?: number): void;
}>();
</script>

<template>
  <AppInput
    floating
    id="weight"
    label="items.sort.options.Weight"
    min="0"
    :model-value="modelValue?.toString()"
    placeholder="items.sort.options.Weight"
    step="0.01"
    type="number"
    @update:model-value="$emit('update:model-value', $event === '' ? undefined : parseNumber($event))"
  >
    <template #append>
      <span class="input-group-text">{{ t("game.units.kilograms") }}</span>
    </template>
  </AppInput>
</template>
