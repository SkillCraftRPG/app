<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import AppInput from "@/components/shared/AppInput.vue";
import * as gameUtils from "@/helpers/gameUtils";

const { parseNumber } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  height?: number;
  modelValue?: number;
  roll?: string;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value?: number): void;
}>();

function onRoll(): void {
  if (props.height && props.roll) {
    const bmi: number = gameUtils.roll(props.roll);
    const weight: number = Math.round(bmi * (props.height / 100.0) * (props.height / 100.0) * 10) / 10.0;
    emit("update:model-value", weight);
  }
}
</script>

<template>
  <AppInput
    floating
    id="weight"
    label="characters.weight"
    min="1"
    :model-value="modelValue?.toString()"
    placeholder="characters.weight"
    required
    step="1"
    type="number"
    @update:model-value="$emit('update:model-value', $event === '' ? undefined : parseNumber($event))"
  >
    <template #prepend>
      <TarButton v-if="roll" icon="fas fa-dice" :text="roll" @click="onRoll" />
    </template>
    <template #append>
      <span class="input-group-text">{{ t("game.units.kilograms") }}</span>
    </template>
  </AppInput>
</template>
