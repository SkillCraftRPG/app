<script setup lang="ts">
import AppInput from "@/components/shared/AppInput.vue";
import type { CharacterModel } from "@/types/characters";

defineProps<{
  character: CharacterModel;
}>();
</script>

<template>
  <AppInput disabled floating id="tier" label="characters.tier" :model-value="character.tier.toString()" type="number" validation="server" />
</template>
