<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { DamageType } from "@/types/game";

const { orderBy } = arrayUtils;
const { rt, tm } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    modelValue?: DamageType;
    required?: boolean | string;
  }>(),
  {
    id: "type",
  },
);

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("game.damage.type.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

defineEmits<{
  (e: "update:model-value", value?: DamageType): void;
}>();
</script>

<template>
  <AppSelect
    floating
    :id="id"
    label="game.damage.type.label"
    :model-value="modelValue"
    :options="options"
    placeholder="game.damage.type.placeholder"
    :required="required"
    @update:model-value="$emit('update:model-value', $event === '' ? undefined : ($event as DamageType))"
  />
</template>
