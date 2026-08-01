<template>
  <InputField :id="id" :label="t(label)" :max="max" :model-value="modelValue" :rules="rules" @update:model-value="$emit('update:model-value', $event ?? '')">
    <template #append>
      <span class="input-group-text">{{ t("game.unit.cm") }}</span>
    </template>
  </InputField>
</template>

<script setup lang="ts">
import type { ValidationRuleSet } from "logitar-validation";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import InputField from "@/components/forms/InputField.vue";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    max?: number | string;
    modelValue?: string;
  }>(),
  {
    id: "height-roll",
    label: "lineages.physical.height",
    max: 16,
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

const rules = computed<ValidationRuleSet>(() => ({ roll: true }));
</script>
