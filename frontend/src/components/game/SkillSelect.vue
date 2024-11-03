<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { Skill } from "@/types/game";
import type { ValidationType } from "@/types/validation";

const { orderBy } = arrayUtils;
const { rt, tm } = useI18n();

defineProps<{
  modelValue?: Skill;
  validation?: ValidationType;
}>();

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("game.skills"))).map(([value, text]) => ({ text, value }) as SelectOption),
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
    id="skill"
    label="game.skill"
    :model-value="modelValue"
    :options="options"
    placeholder="game.skill"
    :validation="validation"
    @update:model-value="$emit('update:model-value', $event as Skill)"
  />
</template>
