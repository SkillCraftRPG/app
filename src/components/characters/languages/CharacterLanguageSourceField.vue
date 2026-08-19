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
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import SelectField from "@/components/forms/SelectField.vue";
import type { Character } from "@/types/characters";
import type { SelectOption } from "@/types/tar/select";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    character: Character;
    id?: string;
    label?: string;
    modelValue: string;
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    id: "source",
    label: "characters.languages.source.label",
    placeholder: "characters.languages.source.placeholder",
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

const options = computed<SelectOption[]>(() => {
  const options: string[] = [];
  if (true) {
    let extra: number = props.character.lineage.languages.extra;
    if (props.character.lineage.parent) {
      extra += props.character.lineage.parent.languages.extra;
    }
    const count: number = props.character.languages.filter((language) => language.source === "Extra").length;
    if (count < extra) {
      options.push("Extra");
    }
  }
  if (props.character.customizations.length) {
    options.push("Customization");
  }
  if (props.character.talents.length) {
    options.push("Talent");
  }
  options.push("Custom");
  return options.map((value) => ({ text: t(`characters.languages.source.options.${value}`), value }));
});
</script>
