<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import CharacterAttribute from "./CharacterAttribute.vue";
import type { Attribute } from "@/types/game";
import type { CharacterModel } from "@/types/characters";

type SortedAttribute = {
  text: string;
  value: Attribute;
};

const { orderBy } = arrayUtils;
const { rt, t, tm } = useI18n();

defineProps<{
  character: CharacterModel;
}>();

const attributes = computed<SortedAttribute[]>(() =>
  orderBy(
    Object.entries(tm(rt("game.attribute.options"))).map(([value, text]) => ({ text, value }) as SortedAttribute),
    "text",
  ),
);
</script>

<template>
  <div>
    <h5>{{ t("game.attributes") }}</h5>
    <div class="mb-3 row">
      <div v-for="attribute in attributes" :key="attribute.value" class="col">
        <CharacterAttribute :attribute="attribute.value" :character="character" :text="attribute.text" />
      </div>
    </div>
  </div>
</template>
