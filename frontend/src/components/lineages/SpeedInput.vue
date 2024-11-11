<script setup lang="ts">
import { parsingUtils } from "logitar-js";

import AppInput from "@/components/shared/AppInput.vue";
import type { Speed } from "@/types/game";

const { parseNumber } = parsingUtils;

defineProps<{
  modelValue?: number;
  speed: Speed;
}>();

defineEmits<{
  (e: "update:model-value", value?: number): void;
}>();
</script>

<template>
  <AppInput
    floating
    :id="`speed-${speed}`"
    :label="`game.speeds.${speed}`"
    :min="0"
    :max="8"
    :model-value="modelValue?.toString()"
    :placeholder="`game.speeds.${speed}`"
    required
    step="1"
    type="number"
    @update:model-value="$emit('update:model-value', $event === '' ? undefined : parseNumber($event))"
  />
</template>
