<template>
  <div class="row text-center">
    <div v-for="score in scores" :key="score.key" class="col-4 col-md-fifth">
      <CharacterAttribute :attribute="score.attribute" :value="score.value" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import CharacterAttribute from "./CharacterAttribute.vue";
import type { CharacterAttributes } from "@/types/characters";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = defineProps<{
  attributes: CharacterAttributes;
}>();

type AttributeScore = {
  key: string;
  attribute: string;
  value: number;
};
const scores = computed<AttributeScore[]>(() =>
  orderBy(
    [
      {
        key: "dexterity",
        attribute: t("game.attribute.options.Dexterity"),
        value: props.attributes.dexterity.total,
      },
      {
        key: "health",
        attribute: t("game.attribute.options.Health"),
        value: props.attributes.health.total,
      },
      {
        key: "intellect",
        attribute: t("game.attribute.options.Intellect"),
        value: props.attributes.intellect.total,
      },
      {
        key: "senses",
        attribute: t("game.attribute.options.Senses"),
        value: props.attributes.senses.total,
      },
      {
        key: "vigor",
        attribute: t("game.attribute.options.Vigor"),
        value: props.attributes.vigor.total,
      },
    ],
    "attribute",
  ),
);
</script>
