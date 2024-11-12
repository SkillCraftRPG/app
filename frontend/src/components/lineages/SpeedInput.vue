<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import AppInput from "@/components/shared/AppInput.vue";
import type { Speed } from "@/types/game";

const { parseNumber } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  modelValue?: number;
  speed: Speed;
}>();

const unit = computed<string>(() => (typeof props.modelValue === "number" && props.modelValue > 1 ? "squares" : "square"));

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
  >
    <template #append>
      <span class="input-group-text">{{ t(`game.units.${unit}`) }}</span>
    </template>
  </AppInput>
</template>
