<script setup lang="ts">
import { parsingUtils } from "logitar-js";

import AppInput from "@/components/shared/AppInput.vue";
import type { Attribute } from "@/types/game";

const { parseNumber } = parsingUtils;

defineProps<{
  attribute: Attribute;
  modelValue?: number;
}>();

defineEmits<{
  (e: "update:model-value", value?: number): void;
}>();
</script>

<template>
  <AppInput
    floating
    :id="`${attribute.toLowerCase()}-bonus`"
    :label="`game.attributes.${attribute}`"
    min="0"
    max="2"
    :model-value="modelValue?.toString()"
    :placeholder="`game.attributes.${attribute}`"
    required
    step="1"
    type="number"
    @update:model-value="$emit('update:model-value', $event === '' ? undefined : parseNumber($event))"
  />
</template>
