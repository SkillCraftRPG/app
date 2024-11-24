<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";

import AppInput from "@/components/shared/AppInput.vue";

const { parseNumber } = parsingUtils;

const props = defineProps<{
  modelValue?: number;
  tier: number | string;
}>();

const max = computed<number>(() => 2 + (parseNumber(props.tier) ?? 0));

defineEmits<{
  (e: "update:model-value", value?: number): void;
}>();
</script>

<template>
  <AppInput
    floating
    id="cost"
    label="characters.talents.cost"
    min="0"
    :max="max"
    :model-value="modelValue?.toString()"
    placeholder="characters.talents.cost"
    required
    step="1"
    type="number"
    @update:model-value="$emit('update:model-value', $event === '' ? undefined : parseNumber($event))"
  />
</template>
