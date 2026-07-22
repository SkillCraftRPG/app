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
        <label class="form-check-label" :for="`${id}-${option}`">{{ t(`account.gender.options.${option}`) }}</label>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import type { Gender } from "@/types/account";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: Gender | null;
  }>(),
  {
    id: "gender",
    label: "account.gender.label",
  },
);

defineEmits<{
  (e: "update:model-value", value: Gender | null): void;
}>();

const options: Gender[] = ["male", "female", "other"];
</script>
