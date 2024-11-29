<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import CharacterAttribute from "./CharacterAttribute.vue";
import type { AttributeDetail, CharacterModel } from "@/types/characters";
import type { Attribute } from "@/types/game";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = defineProps<{
  character: CharacterModel;
}>();

const attributes = computed<AttributeDetail[]>(() => {
  const attributes = new Map<Attribute, AttributeDetail>();
  attributes.set("Agility", {
    attribute: "Agility",
    label: t("game.attribute.options.Agility"),
    base: props.character.baseAttributes.agility,
    nature: props.character.nature.attribute === "Agility" ? props.character.nature : undefined,
    score: 0,
    modifier: 0,
  });
  attributes.set("Coordination", {
    attribute: "Coordination",
    label: t("game.attribute.options.Coordination"),
    base: props.character.baseAttributes.coordination,
    nature: props.character.nature.attribute === "Coordination" ? props.character.nature : undefined,
    score: 0,
    modifier: 0,
  });
  attributes.set("Intellect", {
    attribute: "Intellect",
    label: t("game.attribute.options.Intellect"),
    base: props.character.baseAttributes.intellect,
    nature: props.character.nature.attribute === "Intellect" ? props.character.nature : undefined,
    score: 0,
    modifier: 0,
  });
  attributes.set("Presence", {
    attribute: "Presence",
    label: t("game.attribute.options.Presence"),
    base: props.character.baseAttributes.presence,
    nature: props.character.nature.attribute === "Presence" ? props.character.nature : undefined,
    score: 0,
    modifier: 0,
  });
  attributes.set("Sensitivity", {
    attribute: "Sensitivity",
    label: t("game.attribute.options.Sensitivity"),
    base: props.character.baseAttributes.sensitivity,
    nature: props.character.nature.attribute === "Sensitivity" ? props.character.nature : undefined,
    score: 0,
    modifier: 0,
  });
  attributes.set("Spirit", {
    attribute: "Spirit",
    label: t("game.attribute.options.Spirit"),
    base: props.character.baseAttributes.spirit,
    nature: props.character.nature.attribute === "Spirit" ? props.character.nature : undefined,
    score: 0,
    modifier: 0,
  });
  attributes.set("Vigor", {
    attribute: "Vigor",
    label: t("game.attribute.options.Vigor"),
    base: props.character.baseAttributes.vigor,
    nature: props.character.nature.attribute === "Vigor" ? props.character.nature : undefined,
    score: 0,
    modifier: 0,
  });
  // TODO(fpion): lineage, extra
  // TODO(fpion): best, worst, mandatory, optional
  // TODO(fpion): level-ups
  // TODO(fpion): bonuses
  return orderBy([...attributes.values()], "label");
});
</script>

<template>
  <div class="row">
    <div v-for="attribute in attributes" :key="attribute.attribute" class="col">
      <CharacterAttribute :attribute="attribute" />
    </div>
  </div>
</template>
