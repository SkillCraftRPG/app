<script setup lang="ts">
import type { CharacterModel } from "@/types/characters";
import { useI18n } from "vue-i18n";

import CharacterLevelUp from "./CharacterLevelUp.vue";
import CharacterLevelUpCancel from "./CharacterLevelUpCancel.vue";

const { t } = useI18n();

defineProps<{
  character: CharacterModel;
}>();

defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: CharacterModel): void;
}>();
</script>

<template>
  <div>
    <div class="mb-3">
      <CharacterLevelUp :character="character" @error="$emit('error', $event)" @updated="$emit('updated', $event)" />
    </div>
    <table v-if="character.levelUps.length > 0" class="table table-striped">
      <thead>
        <tr>
          <th scope="col">{{ t("characters.level.label") }}</th>
          <th scope="col">{{ t("game.attribute.label") }}</th>
          <th scope="col"></th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(levelUp, index) in character.levelUps" :key="index">
          <td>{{ index + 1 }}</td>
          <td>{{ t(`game.attribute.options.${levelUp.attribute}`) }}</td>
          <td>
            <CharacterLevelUpCancel
              v-if="index === character.levelUps.length - 1"
              :attribute="levelUp.attribute"
              :character="character"
              :level="index + 1"
              @error="$emit('error', $event)"
              @updated="$emit('updated', $event)"
            />
          </td>
        </tr>
      </tbody>
    </table>
    <p v-else>{{ t("characters.levelUp.empty") }}</p>
  </div>
</template>
