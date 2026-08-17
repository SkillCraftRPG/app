<template>
  <SelectField
    floating
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder)"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import SelectField from "@/components/forms/SelectField.vue";
import type { SelectOption } from "@/types/tar/select";

const { orderBy } = arrayUtils;
const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue: string;
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    id: "kind",
    label: "characters.modifiers.kind.label",
    placeholder: "characters.modifiers.kind.placeholder",
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

const options = computed<SelectOption[]>(() =>
  orderBy(
    [
      { value: "Attribute", text: t("game.attribute.label") },
      { value: "Skill", text: t("game.skill.label") },
      { value: "Speed", text: t("game.speed.label") },
      { value: "Statistic", text: t("game.statistic.label") },
    ],
    "text",
  ),
);
</script>
