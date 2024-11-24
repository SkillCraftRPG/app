<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import type { AspectModel } from "@/types/aspects";
import type { Skill } from "@/types/game";

const { orderBy } = arrayUtils;
const { t } = useI18n();

type TranslatedSkill = {
  skill: Skill;
  text: string;
};

const props = defineProps<{
  aspect: AspectModel;
}>();

const skills = computed<TranslatedSkill[]>(() => {
  const skills: TranslatedSkill[] = [];
  if (props.aspect.skills.discounted1) {
    skills.push({ skill: props.aspect.skills.discounted1, text: t(`game.skill.options.${props.aspect.skills.discounted1}`) });
  }
  if (props.aspect.skills.discounted2) {
    skills.push({ skill: props.aspect.skills.discounted2, text: t(`game.skill.options.${props.aspect.skills.discounted2}`) });
  }
  return orderBy(skills, "text");
});
</script>

<template>
  <div>
    <template v-if="skills.length === 2">
      {{ skills[0].text }}
      <br />
      {{ skills[1].text }}
    </template>
    <template v-else-if="skills.length === 1">{{ skills[0].text }}</template>
    <template v-else>{{ "—" }}</template>
  </div>
</template>
