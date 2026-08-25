<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-2">
      <div class="fs-5">{{ t("characters.attributes.title") }}</div>
      <TarButton :disabled="!character.points.attributes" icon="fas fa-arrow-turn-up" size="small" :text="increaseText" />
    </div>
    <div class="row">
      <div v-for="attribute in attributes" :key="attribute.key" class="col-6 col-md-4 col-lg-fifth">
        <CharacterAttribute class="mb-3" :attribute="attribute.key" :character="character" :name="attribute.name" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import CharacterAttribute from "./CharacterAttribute.vue";
import TarButton from "@/components/tar/TarButton.vue";
import type { Attribute } from "@/types/game";
import type { Character } from "@/types/characters";
import { ATTRIBUTES } from "@/utils/game";

const { orderBy } = arrayUtils;
const { n, t } = useI18n();

const props = defineProps<{
  character: Character;
}>();

type AttributeData = {
  key: Attribute;
  name: string;
};
const attributes = computed<AttributeData[]>(() =>
  orderBy(
    ATTRIBUTES.map((key) => ({ key, name: t(`game.attribute.options.${key}`) })),
    "name",
  ),
);

const increaseText = computed<string>(() => {
  const action: string = t("actions.increase");
  const points: string = n(props.character.points.attributes, "integer");
  return props.character.points.attributes ? `${action} (+${points})` : `${action} (${points})`;
});
</script>
