<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { Attribute } from "@/types/game";
import type { ValidationType } from "@/types/validation";

const { orderBy } = arrayUtils;
const { rt, tm } = useI18n();

defineProps<{
  modelValue?: Attribute;
  validation?: ValidationType;
}>();

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("game.attributes"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <AppSelect
    floating
    id="attribute"
    label="game.attribute"
    :model-value="modelValue"
    :options="options"
    placeholder="game.attribute"
    :validation="validation"
    @update:model-value="$emit('update:model-value', $event as Attribute)"
  />
</template>
