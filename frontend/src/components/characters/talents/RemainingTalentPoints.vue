<script setup lang="ts">
import { computed } from "vue";

import AppInput from "@/components/shared/AppInput.vue";
import type { CharacterModel } from "@/types/characters";

const props = defineProps<{
  character: CharacterModel;
}>();

const availablePoints = computed<number>(() => 8 + props.character.level * 4);
const spentPoints = computed<number>(() => props.character.talents.reduce((acc, { cost }) => acc + cost, 0));
const remainingPoints = computed<number>(() => availablePoints.value - spentPoints.value);
</script>

<template>
  <AppInput disabled floating id="remaining-talent-points" label="characters.talents.remainingPoints" :model-value="remainingPoints.toString()" />
</template>
