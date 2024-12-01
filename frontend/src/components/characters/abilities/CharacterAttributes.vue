<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import CharacterAttribute from "./CharacterAttribute.vue";
import type { CharacterAttribute as CharacterAttributeT, CharacterAttributes, CharacterModel } from "@/types/characters";
import type { Attribute } from "@/types/game";

type SortedAttribute = {
  text: string;
  value: Attribute;
}; // TODO(fpion): refactor to include CharacterAttributeT

const { orderBy } = arrayUtils;
const { rt, t, tm } = useI18n();

const props = defineProps<{
  attributes: CharacterAttributes;
  character: CharacterModel;
}>();

const sortedAttributes = computed<SortedAttribute[]>(() =>
  orderBy(
    Object.entries(tm(rt("game.attribute.options"))).map(([value, text]) => ({ text, value }) as SortedAttribute),
    "text",
  ),
);

function getAttribute(attribute: Attribute): CharacterAttributeT {
  switch (attribute) {
    case "Agility":
      return props.attributes.agility;
    case "Coordination":
      return props.attributes.coordination;
    case "Intellect":
      return props.attributes.intellect;
    case "Presence":
      return props.attributes.presence;
    case "Sensitivity":
      return props.attributes.sensitivity;
    case "Spirit":
      return props.attributes.spirit;
    case "Vigor":
      return props.attributes.vigor;
    default:
      throw new Error(`The attribute '${attribute}' is not supported.`);
  }
}
function getModifier(attribute: Attribute): number {
  return getAttribute(attribute).modifier;
}
function getScore(attribute: Attribute): number {
  return getAttribute(attribute).score;
}
</script>

<template>
  <div>
    <h5>{{ t("game.attributes") }}</h5>
    <div class="mb-3 row">
      <div v-for="attribute in sortedAttributes" :key="attribute.value" class="col">
        <CharacterAttribute
          :attribute="attribute.value"
          :character="character"
          :modifier="getModifier(attribute.value)"
          :score="getScore(attribute.value)"
          :text="attribute.text"
        />
      </div>
    </div>
  </div>
</template>
