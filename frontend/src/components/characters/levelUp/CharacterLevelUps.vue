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
      <CharacterLevelUp class="me-1" :character="character" @error="$emit('error', $event)" @updated="$emit('updated', $event)" />
      <CharacterLevelUpCancel class="ms-1" :character="character" @error="$emit('error', $event)" @updated="$emit('updated', $event)" />
    </div>
    <table v-if="character.levelUps.length > 0" class="table table-striped">
      <!-- TODO(fpion): implement -->
    </table>
    <p v-else>{{ t("characters.levelUp.empty") }}</p>
  </div>
</template>
