<template>
  <div class="row">
    <div v-for="attribute in attributes" :key="attribute.key" class="col-6 col-md-4 col-lg-fifth">
      <CharacterAttribute class="mb-3" :attribute="attribute.key" :character="character" :name="attribute.name" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import CharacterAttribute from "./CharacterAttribute.vue";
import type { Character } from "@/types/characters";
import type { Attribute } from "@/types/game";

const { orderBy } = arrayUtils;
const { t } = useI18n();

defineProps<{
  character: Character;
}>();

type AttributeData = {
  key: Attribute;
  name: string;
};
const attributes = computed<AttributeData[]>(() =>
  orderBy(
    [
      {
        key: "Dexterity",
        name: t("game.attribute.options.Dexterity"),
      },
      {
        key: "Health",
        name: t("game.attribute.options.Health"),
      },
      {
        key: "Intellect",
        name: t("game.attribute.options.Intellect"),
      },
      {
        key: "Senses",
        name: t("game.attribute.options.Senses"),
      },
      {
        key: "Vigor",
        name: t("game.attribute.options.Vigor"),
      },
    ],
    "name",
  ),
);
</script>
