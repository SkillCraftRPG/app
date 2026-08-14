<template>
  <div>
    <div class="row">
      <div class="col-md-4 mb-3">
        <CharacterLineage :lineage="character.lineage" />
      </div>
      <div class="col-md-4 mb-3">
        <CharacterCaste :caste="character.caste" />
      </div>
      <div class="col-md-4 mb-3">
        <CharacterEducation :education="character.education" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import CharacterCaste from "./CharacterCaste.vue";
import CharacterEducation from "./CharacterEducation.vue";
import CharacterLineage from "./CharacterLineage.vue";
import type { Character } from "@/types/characters";

defineProps<{
  character: Character;
}>();

// TODO(fpion): cards should have the same height
// TODO(fpion): should the mb-3 exist on the card, or the wrapping col?
</script>
