<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import CharacterAttribute from "./CharacterAttribute.vue";
import type { CharacterAttributes, CharacterModel } from "@/types/characters";
import type { Attribute } from "@/types/game";

type SortedAttribute = {
  text: string;
  value: Attribute;
  score: number;
  modifier: number;
};

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = defineProps<{
  attributes: CharacterAttributes;
  character: CharacterModel;
}>();

const sortedAttributes = computed<SortedAttribute[]>(() =>
  orderBy(
    [
      { text: t("game.attribute.options.Agility"), value: "Agility", score: props.attributes.agility.score, modifier: props.attributes.agility.modifier },
      {
        text: t("game.attribute.options.Coordination"),
        value: "Coordination",
        score: props.attributes.coordination.score,
        modifier: props.attributes.coordination.modifier,
      },
      {
        text: t("game.attribute.options.Intellect"),
        value: "Intellect",
        score: props.attributes.intellect.score,
        modifier: props.attributes.intellect.modifier,
      },
      { text: t("game.attribute.options.Presence"), value: "Presence", score: props.attributes.presence.score, modifier: props.attributes.presence.modifier },
      {
        text: t("game.attribute.options.Sensitivity"),
        value: "Sensitivity",
        score: props.attributes.sensitivity.score,
        modifier: props.attributes.sensitivity.modifier,
      },
      { text: t("game.attribute.options.Spirit"), value: "Spirit", score: props.attributes.spirit.score, modifier: props.attributes.spirit.modifier },
      { text: t("game.attribute.options.Vigor"), value: "Vigor", score: props.attributes.vigor.score, modifier: props.attributes.vigor.modifier },
    ],
    "text",
  ),
);
</script>

<template>
  <div>
    <h5>{{ t("game.attributes") }}</h5>
    <div class="mb-3 row">
      <div v-for="attribute in sortedAttributes" :key="attribute.value" class="col">
        <CharacterAttribute
          :attribute="attribute.value"
          :character="character"
          :modifier="attribute.modifier"
          :score="attribute.score"
          :text="attribute.text"
        />
      </div>
    </div>
  </div>
</template>
