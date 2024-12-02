<script setup lang="ts">
import { parsingUtils } from "logitar-js";

import AppInput from "@/components/shared/AppInput.vue";
import type { Skill } from "@/types/game";

const { parseNumber } = parsingUtils;

defineProps<{
  max?: number | string;
  modelValue?: number;
  skill: Skill;
}>();

defineEmits<{
  (e: "update:model-value", value?: number): void;
}>();
</script>

<template>
  <AppInput
    floating
    :id="`${skill}-rank`"
    label="characters.skills.rank.label"
    min="0"
    :max="max"
    :model-value="modelValue?.toString()"
    placeholder="characters.skills.rank.label"
    step="1"
    type="number"
    @update:model-value="$emit('update:model-value', $event === '' ? undefined : (parseNumber($event) ?? 0))"
  >
    <template #append>
      <slot name="append"></slot>
    </template>
  </AppInput>
</template>
