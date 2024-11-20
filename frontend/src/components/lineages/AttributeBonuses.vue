<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import AttributeBonusInput from "@/components/lineages/AttributeBonusInput.vue";
import ExtraAttributesInput from "@/components/lineages/ExtraAttributesInput.vue";
import type { AttributeBonusesModel } from "@/types/lineages";
import type { Attribute } from "@/types/game";
import { computed } from "vue";

const { orderBy } = arrayUtils;
const { rt, t, tm } = useI18n();

type TranslatedAttribute = {
  attribute: Attribute;
  text: string;
};

const props = defineProps<{
  modelValue: AttributeBonusesModel;
}>();

const attributes = computed<TranslatedAttribute[]>(() =>
  orderBy(
    Object.entries(tm(rt("game.attributes.options"))).map(([attribute, text]) => ({ attribute, text }) as TranslatedAttribute),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "update:model-value", value: AttributeBonusesModel): void;
}>();

function getAttribute(attribute: Attribute): number {
  switch (attribute) {
    case "Agility":
      return props.modelValue.agility;
    case "Coordination":
      return props.modelValue.coordination;
    case "Intellect":
      return props.modelValue.intellect;
    case "Presence":
      return props.modelValue.presence;
    case "Sensitivity":
      return props.modelValue.sensitivity;
    case "Spirit":
      return props.modelValue.spirit;
    case "Vigor":
      return props.modelValue.vigor;
    default:
      throw new Error(`The attribute "${attribute}" is not supported.`);
  }
}
function setAttribute(attribute: Attribute, value?: number): void {
  const attributes: AttributeBonusesModel = { ...props.modelValue };
  switch (attribute) {
    case "Agility":
      attributes.agility = value ?? 0;
      break;
    case "Coordination":
      attributes.coordination = value ?? 0;
      break;
    case "Intellect":
      attributes.intellect = value ?? 0;
      break;
    case "Presence":
      attributes.presence = value ?? 0;
      break;
    case "Sensitivity":
      attributes.sensitivity = value ?? 0;
      break;
    case "Spirit":
      attributes.spirit = value ?? 0;
      break;
    case "Vigor":
      attributes.vigor = value ?? 0;
      break;
    default:
      throw new Error(`The attribute "${attribute}" is not supported.`);
  }
  emit("update:model-value", attributes);
}
function setExtra(extra: number): void {
  const attributes: AttributeBonusesModel = { ...props.modelValue, extra };
  emit("update:model-value", attributes);
}
</script>

<template>
  <div>
    <h3>{{ t("game.attributes.label") }}</h3>
    <div class="row">
      <div v-for="translation in attributes" :key="translation.attribute" class="col">
        <AttributeBonusInput
          :attribute="translation.attribute"
          :model-value="getAttribute(translation.attribute)"
          @update:model-value="setAttribute(translation.attribute, $event)"
        />
      </div>
      <div class="col">
        <ExtraAttributesInput :attributes="modelValue" :model-value="modelValue.extra" @update:model-value="setExtra($event ?? 0)" />
      </div>
    </div>
  </div>
</template>
