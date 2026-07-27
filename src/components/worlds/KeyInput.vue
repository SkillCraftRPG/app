<template>
  <FormInput
    :id="id"
    :label="t(label)"
    :max="max"
    :model-value="modelValue"
    :required="required"
    :rules="rules"
    @update:model-value="$emit('update:model-value', $event ?? '')"
  />
</template>

<script setup lang="ts">
import type { ValidationRuleSet } from "logitar-validation";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    max?: number | string;
    modelValue?: string;
    required?: boolean | string;
  }>(),
  {
    id: "key",
    max: 100,
    label: "worlds.key",
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

const rules = computed<ValidationRuleSet>(() => ({ slug: true }));
</script>
