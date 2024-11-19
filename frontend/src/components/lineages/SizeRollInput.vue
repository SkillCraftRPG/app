<script setup lang="ts">
import { useI18n } from "vue-i18n";

import AppInput from "@/components/shared/AppInput.vue";

const { t } = useI18n();

defineProps<{
  modelValue?: string;
}>();

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <AppInput
    floating
    id="size-roll"
    label="game.roll"
    :model-value="modelValue?.toString()"
    placeholder="game.roll"
    :rules="{ roll: true }"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template #append>
      <span class="input-group-text">{{ t("game.units.centimeters") }}</span>
    </template>
  </AppInput>
</template>
