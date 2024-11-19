<script setup lang="ts">
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import AppInput from "@/components/shared/AppInput.vue";
import type { Speed } from "@/types/game";
import type { ValidationType } from "@/types/validation";

const { parseNumber } = parsingUtils;
const { t } = useI18n();

defineProps<{
  disabled?: boolean | string;
  modelValue?: number;
  required?: boolean | string;
  speed: Speed;
  validation?: ValidationType;
}>();

defineEmits<{
  (e: "update:model-value", value?: number): void;
}>();
</script>

<template>
  <AppInput
    :disabled="disabled"
    floating
    :id="`speed-${speed}`"
    :label="`game.speeds.${speed}`"
    :min="0"
    :max="8"
    :model-value="modelValue?.toString()"
    :placeholder="`game.speeds.${speed}`"
    :required="required"
    step="1"
    type="number"
    :validation="validation"
    @update:model-value="$emit('update:model-value', $event === '' ? undefined : parseNumber($event))"
  >
    <template #append>
      <span class="input-group-text">{{ t("game.units.square", modelValue ?? 0) }}</span>
    </template>
  </AppInput>
</template>
