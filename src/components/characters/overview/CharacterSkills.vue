<template>
  <div>
    <div class="fs-5 mb-1">{{ t("characters.skills.title") }}</div>
    <div class="row">
      <div v-for="skill in skills" :key="skill.key" class="col-6 col-md-4 col-lg-fifth">
        <CharacterSkill class="mb-3" :character="character" :name="skill.name" :skill="skill.key" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import CharacterSkill from "./CharacterSkill.vue";
import type { Character } from "@/types/characters";
import type { Skill } from "@/types/game";
import { SKILLS } from "@/utils/game";

const { orderBy } = arrayUtils;
const { t } = useI18n();

defineProps<{
  character: Character;
}>();

type SkillData = {
  key: Skill;
  name: string;
};
const skills = computed<SkillData[]>(() =>
  orderBy(
    SKILLS.map((key) => ({ key, name: t(`game.skill.options.${key}`) })),
    "name",
  ),
);
</script>
