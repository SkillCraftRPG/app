<template>
  <TarSelect
    floating
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder)"
    @update:model-value="$emit('update:model-value', $event ?? '')"
  />
</template>

<script setup lang="ts">
import { arrayUtils, parsingUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import TarSelect from "@/components/tar/TarSelect.vue";
import type { SelectOption } from "@/types/tar/select";

const { orderBy } = arrayUtils;
const { parseBoolean } = parsingUtils;
const { rt, t, tm } = useI18n();

const props = withDefaults(
  defineProps<{
    extended?: boolean | string;
    id?: string;
    label?: string;
    modelValue: string;
    placeholder?: string;
  }>(),
  {
    id: "skill",
    label: "game.skill.label",
    placeholder: "all",
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

const options = computed<SelectOption[]>(() => {
  const options: SelectOption[] = orderBy(
    Object.entries(tm(rt("game.skill.options"))).map(([value, text]) => ({ text, value })),
    "text",
  );
  if (parseBoolean(props.extended)) {
    options.splice(0, 0, { value: "any", text: t("game.skill.any") }, { value: "none", text: t("game.skill.none") });
  }
  return options;
});
</script>
