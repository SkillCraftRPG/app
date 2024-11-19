<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import MarkdownText from "@/components/shared/MarkdownText.vue";
import type { Attribute } from "@/types/game";
import type { LineageModel, TraitModel } from "@/types/lineages";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = defineProps<{
  lineage: LineageModel;
}>();

function capitalize(s: string): string {
  return s[0].toUpperCase() + s.substring(1).toLowerCase();
}

const attributes = computed<string>(() => {
  const attributes = new Map<Attribute, number>();
  let extra: number = 0;
  Object.entries(props.lineage.attributes).forEach(([key, value]) => {
    if (key === "extra") {
      extra += value;
    } else {
      attributes.set(capitalize(key) as Attribute, value);
    }
  });
  if (props.lineage.species) {
    Object.entries(props.lineage.species.attributes).forEach(([key, value]) => {
      if (key === "extra") {
        extra += value;
      } else {
        const attribute: Attribute = capitalize(key) as Attribute;
        const bonus: number = attributes.get(attribute) ?? 0;
        attributes.set(attribute, bonus + value);
      }
    });
  }
  const bonuses: string[] = [...attributes.entries()]
    .filter(([, bonus]) => bonus > 0)
    .map(([attribute, bonus]) => `+${bonus} ${t(`game.attributes.options.${attribute}`)}`);
  if (extra > 0) {
    bonuses.push(`+${extra} ${t("characters.attributes.extra.label", extra)}`);
  }
  return bonuses.join(", ");
});
const traits = computed<TraitModel[]>(() => orderBy(props.lineage.traits.concat(props.lineage.species?.traits ?? []), "name"));
</script>

<template>
  <div>
    <ul>
      <li v-if="attributes">
        <strong>{{ t("game.attributes.label") }}.</strong> {{ attributes }}
      </li>
      <li v-for="trait in traits" :key="trait.id">
        <strong>{{ trait.name }}.</strong> <MarkdownText v-if="trait.description" inline :text="trait.description" />
      </li>
    </ul>
  </div>
</template>
