<script setup lang="ts">
import AppInput from "@/components/shared/AppInput.vue";
import type { CharacterModel } from "@/types/characters";

defineProps<{
  character: CharacterModel;
}>();
</script>

<template>
  <AppInput disabled floating id="level" label="characters.level" :model-value="character.level.toString()" type="number" validation="server">
    <template #append>
      <slot name="append"></slot>
    </template>
  </AppInput>
</template>
