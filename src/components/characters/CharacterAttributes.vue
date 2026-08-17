<template>
  <div class="row">
    <div v-for="score in scores" :key="score.key" class="col-6 col-md-4 col-lg-fifth">
      <CharacterAttribute :attribute="score.attribute" :category="score.category" class="mb-3" :value="score.value" />
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
  category: string;
  value: number;
};
const scores = computed<AttributeScore[]>(() =>
  orderBy(
    [
      {
        key: "dexterity",
        attribute: t("game.attribute.options.Dexterity"),
        category: t("game.attribute.physical"),
        value: props.attributes.dexterity.total,
      },
      {
        key: "health",
        attribute: t("game.attribute.options.Health"),
        category: t("game.attribute.universal"),
        value: props.attributes.health.total,
      },
      {
        key: "intellect",
        attribute: t("game.attribute.options.Intellect"),
        category: t("game.attribute.mental"),
        value: props.attributes.intellect.total,
      },
      {
        key: "senses",
        attribute: t("game.attribute.options.Senses"),
        category: t("game.attribute.mental"),
        value: props.attributes.senses.total,
      },
      {
        key: "vigor",
        attribute: t("game.attribute.options.Vigor"),
        category: t("game.attribute.physical"),
        value: props.attributes.vigor.total,
      },
    ],
    "attribute",
  ),
);
</script>
