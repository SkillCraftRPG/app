<template>
  <InputField
    :id="id"
    :label="label"
    :min="min"
    :max="max"
    :model-value="modelValue.toString()"
    :step="step"
    type="number"
    @update:model-value="$emit('update:model-value', parseNumber($event) ?? 0)"
  >
    <template #append>
      <TarButton v-if="roll" icon="fas fa-dice" :text="roll" @click="randomize" />
    </template>
  </InputField>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import InputField from "@/components/forms/InputField.vue";
import TarButton from "@/components/tar/TarButton.vue";
import { roll as rollFn } from "@/utils/random";

const { parseNumber } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    max?: number | string;
    min?: number | string;
    modelValue: number;
    roll?: string;
    step?: number | string;
  }>(),
  {
    id: "height",
    max: 999,
    min: 0,
    step: 1,
  },
);

const emit = defineEmits<{
  (e: "update:model-value", value: number): void;
}>();

const label = computed<string>(() => `${t("characters.physical.height")} (${t("game.unit.cm")})`);

function randomize(): void {
  if (props.roll) {
    emit("update:model-value", rollFn(props.roll));
  }
}
</script>
