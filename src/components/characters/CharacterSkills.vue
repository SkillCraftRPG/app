<template>
  <div class="row">
    <div v-for="score in scores" :key="score.key" class="col-6 col-md-4 col-lg-fifth">
      <CharacterSkill :attribute="score.attribute" class="mb-3" :skill="score.skill" :value="score.value" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import CharacterSkill from "./CharacterSkill.vue";
import type { CharacterSkills } from "@/types/characters";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = defineProps<{
  skills: CharacterSkills;
}>();

type SkillScore = {
  key: string;
  attribute: string;
  skill: string;
  value: number;
};
const scores = computed<SkillScore[]>(() =>
  orderBy(
    [
      {
        key: "acrobatics",
        attribute: t("game.attribute.options.Dexterity"),
        skill: t("game.skill.options.Acrobatics"),
        value: props.skills.acrobatics.total,
      },
      {
        key: "athletics",
        attribute: t("game.attribute.options.Vigor"),
        skill: t("game.skill.options.Athletics"),
        value: props.skills.athletics.total,
      },
      {
        key: "crafting",
        attribute: t("game.attribute.options.Dexterity"),
        skill: t("game.skill.options.Crafting"),
        value: props.skills.crafting.total,
      },
      {
        key: "deception",
        attribute: t("characters.social"),
        skill: t("game.skill.options.Deception"),
        value: props.skills.deception.total,
      },
      {
        key: "diplomacy",
        attribute: t("characters.social"),
        skill: t("game.skill.options.Diplomacy"),
        value: props.skills.diplomacy.total,
      },
      {
        key: "discipline",
        attribute: t("game.attribute.options.Health"),
        skill: t("game.skill.options.Discipline"),
        value: props.skills.discipline.total,
      },
      {
        key: "insight",
        attribute: t("game.attribute.options.Senses"),
        skill: t("game.skill.options.Insight"),
        value: props.skills.insight.total,
      },
      {
        key: "investigation",
        attribute: t("game.attribute.options.Intellect"),
        skill: t("game.skill.options.Investigation"),
        value: props.skills.investigation.total,
      },
      {
        key: "knowledge",
        attribute: t("game.attribute.options.Intellect"),
        skill: t("game.skill.options.Knowledge"),
        value: props.skills.knowledge.total,
      },
      {
        key: "linguistics",
        attribute: t("game.attribute.options.Intellect"),
        skill: t("game.skill.options.Linguistics"),
        value: props.skills.linguistics.total,
      },
      {
        key: "medicine",
        attribute: t("game.attribute.options.Intellect"),
        skill: t("game.skill.options.Medicine"),
        value: props.skills.medicine.total,
      },
      {
        key: "melee",
        attribute: t("game.attribute.options.Vigor"),
        skill: t("game.skill.options.Melee"),
        value: props.skills.melee.total,
      },
      {
        key: "occultism",
        attribute: t("game.attribute.options.Senses"),
        skill: t("game.skill.options.Occultism"),
        value: props.skills.occultism.total,
      },
      {
        key: "orientation",
        attribute: t("game.attribute.options.Dexterity"),
        skill: t("game.skill.options.Orientation"),
        value: props.skills.orientation.total,
      },
      {
        key: "perception",
        attribute: t("game.attribute.options.Senses"),
        skill: t("game.skill.options.Perception"),
        value: props.skills.perception.total,
      },
      {
        key: "performance",
        attribute: t("characters.social"),
        skill: t("game.skill.options.Performance"),
        value: props.skills.performance.total,
      },
      {
        key: "resistance",
        attribute: t("game.attribute.options.Health"),
        skill: t("game.skill.options.Resistance"),
        value: props.skills.resistance.total,
      },
      {
        key: "stealth",
        attribute: t("game.attribute.options.Dexterity"),
        skill: t("game.skill.options.Stealth"),
        value: props.skills.stealth.total,
      },
      {
        key: "thievery",
        attribute: t("game.attribute.options.Dexterity"),
        skill: t("game.skill.options.Thievery"),
        value: props.skills.thievery.total,
      },
      {
        key: "survival",
        attribute: t("game.attribute.options.Senses"),
        skill: t("game.skill.options.Survival"),
        value: props.skills.survival.total,
      },
    ],
    "skill",
  ),
);
</script>
