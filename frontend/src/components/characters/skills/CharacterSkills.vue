<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import CharacterSkill from "./CharacterSkill.vue";
import type { CharacterModel } from "@/types/characters";
import type { Skill } from "@/types/game";

const { rt, t, tm } = useI18n();

const { orderBy } = arrayUtils;

type SortedSkill = {
  text: string;
  value: Skill;
};

defineProps<{
  character: CharacterModel;
}>();

const skills = computed<SortedSkill[]>(() =>
  orderBy(
    Object.entries(tm(rt("game.skill.options"))).map(([value, text]) => ({ text, value }) as SortedSkill),
    "text",
  ),
);

defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: CharacterModel): void;
}>();
</script>

<template>
  <div>
    <p>{{ t("characters.skills.remainingPoints", { n: character.skillPoints.remaining }) }}</p>
    <div class="align-items-stretch mb-3 row">
      <div v-for="skill in skills" :key="skill.value" class="col-lg-3 mb-3">
        <CharacterSkill
          :character="character"
          class="h-100"
          :skill="skill.value"
          :text="skill.text"
          @error="$emit('error', $event)"
          @updated="$emit('updated', $event)"
        />
      </div>
    </div>
  </div>
</template>
