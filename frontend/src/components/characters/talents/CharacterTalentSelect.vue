<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { computed } from "vue";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { TalentModel } from "@/types/talents";

const props = withDefaults(
  defineProps<{
    modelValue?: string;
    talents?: TalentModel[];
  }>(),
  {
    talents: () => [],
  },
);

const options = computed<SelectOption[]>(() => props.talents.map(({ id, name }) => ({ text: name, value: id })));

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <AppSelect
    floating
    id="talent"
    label="talents.select.label"
    :model-value="modelValue"
    :options="options"
    placeholder="talents.select.placeholder"
    required
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
