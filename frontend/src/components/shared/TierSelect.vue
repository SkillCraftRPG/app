<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { parsingUtils } from "logitar-js";
import { ref } from "vue";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { ValidationType } from "@/types/validation";

const { parseNumber } = parsingUtils;

withDefaults(
  defineProps<{
    modelValue?: number;
    placeholder?: string;
    required?: boolean | string;
    validation?: ValidationType;
  }>(),
  {
    placeholder: "game.tier.placeholder",
  },
);

const options = ref<SelectOption[]>([
  { text: "0", value: "0" },
  { text: "1", value: "1" },
  { text: "2", value: "2" },
  { text: "3", value: "3" },
]);

defineEmits<{
  (e: "update:model-value", value?: number): void;
}>();
</script>

<template>
  <AppSelect
    floating
    id="tier"
    label="game.tier.label"
    :model-value="modelValue?.toString()"
    :options="options"
    :placeholder="placeholder"
    :required="required"
    :validation="validation"
    @update:model-value="$emit('update:model-value', $event === '' ? undefined : parseNumber($event))"
  />
</template>
