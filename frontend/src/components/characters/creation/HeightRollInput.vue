<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import AppInput from "@/components/shared/AppInput.vue";
import * as gameUtils from "@/helpers/gameUtils";

const { parseNumber } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  modelValue?: number;
  roll?: string;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value?: number): void;
}>();

function onRoll(): void {
  if (props.roll) {
    const value: number = gameUtils.roll(props.roll);
    emit("update:model-value", value);
  }
}
</script>

<template>
  <AppInput
    floating
    id="height"
    label="characters.height"
    min="1"
    :model-value="modelValue?.toString()"
    placeholder="characters.height"
    required
    step="1"
    type="number"
    @update:model-value="$emit('update:model-value', $event === '' ? undefined : parseNumber($event))"
  >
    <template #prepend>
      <TarButton v-if="roll" icon="fas fa-dice" :text="roll" @click="onRoll" />
    </template>
    <template #append>
      <span class="input-group-text">{{ t("game.units.centimeters") }}</span>
    </template>
  </AppInput>
</template>
