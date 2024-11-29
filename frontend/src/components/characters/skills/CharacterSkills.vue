<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import type { Skill } from "@/types/game";
import type { UpdateCharacterPayload } from "@/types/characters";
import { increaseCharacterSkillRank, updateCharacter } from "@/api/characters";

const { rt, t, tm } = useI18n();

const { orderBy } = arrayUtils;

type SortedSkill = {
  text: string;
  value: Skill;
};

const skills = computed<SortedSkill[]>(() =>
  orderBy(
    Object.entries(tm(rt("game.skill.options"))).map(([value, text]) => ({ text, value }) as SortedSkill),
    "text",
  ),
);
</script>

<template>
  <div>
    <p>TODO: points restants</p>
    <table class="table table-striped">
      <thead>
        <tr>
          <th scope="col">{{ t("game.skill.label") }}</th>
          <th scope="col">TODO: formé ou non</th>
          <th scope="col">TODO: points dépensés</th>
          <th scope="col">TODO: rang effectif</th>
          <th scope="col">TODO: bonus d'attribut</th>
          <th scope="col">TODO: bonuses</th>
          <th scope="col">TODO: total</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="skill in skills" :key="skill.value">
          <td>{{ skill.text }}</td>
          <td></td>
          <td></td>
          <td></td>
          <td></td>
          <td></td>
          <td></td>
          <td></td>
        </tr>
      </tbody>
    </table>
    <!-- TODO(fpion): save button -->
  </div>
</template>
