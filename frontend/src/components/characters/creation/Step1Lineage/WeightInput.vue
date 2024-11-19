<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";
import { watch } from "vue";

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

function updateWeight(bmi: number): void {
  if (props.height) {
    const weight: number = Math.round(bmi * (props.height / 100.0) * (props.height / 100.0) * 10) / 10.0;
    emit("update:model-value", weight);
  }
}

function onRoll(): void {
  if (props.roll) {
    const bmi: number = gameUtils.roll(props.roll);
    updateWeight(bmi);
  }
}

watch(
  () => props.height,
  (newHeight, oldHeight) => {
    if (
      typeof newHeight === "number" &&
      newHeight > 0 &&
      typeof oldHeight === "number" &&
      oldHeight > 0 &&
      typeof props.modelValue === "number" &&
      props.modelValue > 0
    ) {
      const bmi: number = Math.floor(props.modelValue / (oldHeight / 100.0) / (oldHeight / 100.0));
      updateWeight(bmi);
    } else {
      onRoll();
    }
  },
  { immediate: true },
);
watch(() => props.roll, onRoll, { immediate: true });
</script>

<template>
  <AppInput
    floating
    id="weight"
    label="game.weight.label"
    min="1"
    :model-value="modelValue?.toString()"
    placeholder="game.weight.label"
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
