<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, watch } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import AppInput from "@/components/shared/AppInput.vue";

const { parseNumber } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  modelValue?: number;
  range?: number[];
}>();

const isRangeValid = computed<boolean>(() => props.range?.length === 2 && props.range[0] < props.range[1] && props.range.every((value) => value > 0));
const text = computed<string | undefined>(() => props.range?.join("—"));

const emit = defineEmits<{
  (e: "update:model-value", value?: number): void;
}>();

function onRoll(): void {
  if (props.range && isRangeValid.value) {
    const difference: number = props.range[1] - props.range[0] + 1;
    const age: number = props.range[0] + Math.floor(Math.random() * difference);
    emit("update:model-value", age);
  }
}

watch(() => props.range, onRoll, { immediate: true });
</script>

<template>
  <AppInput
    floating
    id="age"
    label="game.age.label"
    :min="isRangeValid && range ? range[0] : 1"
    :max="isRangeValid && range ? range[1] : undefined"
    :model-value="modelValue?.toString()"
    placeholder="game.age.label"
    required
    step="1"
    type="number"
    @update:model-value="$emit('update:model-value', $event === '' ? undefined : parseNumber($event))"
  >
    <template #prepend>
      <TarButton v-if="isRangeValid" icon="fas fa-dice" :text="text" @click="onRoll" />
    </template>
    <template #append>
      <span class="input-group-text">{{ t("game.units.year", modelValue ?? 0) }}</span>
    </template>
  </AppInput>
</template>
