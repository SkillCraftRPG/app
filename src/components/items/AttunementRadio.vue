<template>
  <div>
    <div class="form-label">{{ t(label) }}</div>
    <div class="d-flex flex-wrap gap-3">
      <div v-for="option in options" :key="option" class="form-check">
        <input
          :checked="modelValue === option"
          class="form-check-input"
          :id="`${id}-${option}`"
          :name="id"
          type="radio"
          :value="option"
          @change="$emit('update:model-value', option)"
        />
        <label class="form-check-label" :for="`${id}-${option}`">{{ t(`items.magic.attunement.options.${option}`) }}</label>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import type { AttunementOption } from "@/types/items";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: AttunementOption | null;
  }>(),
  {
    id: "attunement",
    label: "items.magic.attunement.label",
  },
);

defineEmits<{
  (e: "update:model-value", value: AttunementOption): void;
}>();

const options: AttunementOption[] = ["none", "optional", "required"];
</script>
