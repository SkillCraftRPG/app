<script setup lang="ts">
import { computed, ref, watch } from "vue";
import { useForm } from "vee-validate";

import AgeRollInput from "@/components/characters/AgeRollInput.vue";
import BloodAlcoholContentInput from "./BloodAlcoholContentInput.vue";
import CharacterAspect from "./CharacterAspect.vue";
import CharacterCaste from "./CharacterCaste.vue";
import CharacterCustomizations from "./CharacterCustomizations.vue";
import CharacterEducation from "./CharacterEducation.vue";
import CharacterLevel from "./CharacterLevel.vue";
import CharacterLineage from "./CharacterLineage.vue";
import CharacterNature from "./CharacterNature.vue";
import CharacterTier from "./CharacterTier.vue";
import ExperienceInput from "./ExperienceInput.vue";
import HeightRollInput from "@/components/characters/HeightRollInput.vue";
import IntoxicationInput from "./IntoxicationInput.vue";
import NameInput from "@/components/shared/NameInput.vue";
import SaveButton from "@/components/shared/SaveButton.vue";
import StaminaInput from "./StaminaInput.vue";
import VitalityInput from "./VitalityInput.vue";
import WeightInput from "@/components/characters/WeightInput.vue";
import type { CharacterModel, ReplaceCharacterPayload } from "@/types/characters";
import { replaceCharacter } from "@/api/characters";
import { useToastStore } from "@/stores/toast";

const toasts = useToastStore();

const props = defineProps<{
  character: CharacterModel;
}>();

const age = ref<number>(0);
const bloodAlcoholContent = ref<number>(0);
const experience = ref<number>(0);
const height = ref<number>(0);
const intoxication = ref<number>(0);
const name = ref<string>("");
const player = ref<string>("");
const stamina = ref<number>(0);
const vitality = ref<number>(0);
const weight = ref<number>(0);

const hasChanges = computed<boolean>(
  () =>
    name.value !== props.character.name ||
    player.value !== (props.character.playerName ?? "") ||
    height.value !== props.character.height * 100.0 ||
    weight.value !== props.character.weight ||
    age.value !== props.character.age ||
    experience.value !== props.character.experience ||
    vitality.value !== props.character.vitality ||
    stamina.value !== props.character.stamina ||
    bloodAlcoholContent.value !== props.character.bloodAlcoholContent ||
    intoxication.value !== props.character.intoxication,
);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: CharacterModel): void;
}>();

function setModel(character: CharacterModel): void {
  age.value = character.age;
  bloodAlcoholContent.value = character.bloodAlcoholContent;
  experience.value = character.experience;
  height.value = character.height * 100.0;
  intoxication.value = character.intoxication;
  name.value = character.name;
  player.value = character.playerName ?? "";
  stamina.value = character.stamina;
  vitality.value = character.vitality;
  weight.value = character.weight;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    const payload: ReplaceCharacterPayload = {
      name: name.value,
      player: player.value,
      height: height.value / 100.0,
      weight: weight.value,
      age: age.value,
      experience: experience.value,
      vitality: vitality.value,
      stamina: stamina.value,
      bloodAlcoholContent: bloodAlcoholContent.value,
      intoxication: intoxication.value,
    };
    const character: CharacterModel = await replaceCharacter(props.character.id, payload, props.character.version);
    toasts.success("characters.updated");
    emit("updated", character);
  } catch (e: unknown) {
    emit("error", e);
  }
});

watch(() => props.character, setModel, { deep: true, immediate: true });
</script>

<template>
  <div>
    <form @submit.prevent="onSubmit">
      <div class="row">
        <NameInput class="col" required v-model="name" />
        <NameInput class="col" id="player" label="characters.player.label" placeholder="characters.player.label" v-model="player" />
      </div>
      <div class="row">
        <CharacterLineage class="col" :character="character" />
        <CharacterLineage class="col" :character="character" nation />
      </div>
      <div class="row">
        <CharacterCaste class="col" :character="character" />
        <CharacterEducation class="col" :character="character" />
      </div>
      <div class="row">
        <CharacterNature class="col" :character="character" />
        <CharacterAspect v-for="aspect in character.aspects" :key="aspect.id" :aspect="aspect" class="col" />
      </div>
      <div class="row">
        <HeightRollInput class="col" v-model="height" />
        <WeightInput class="col" v-model="weight" />
        <AgeRollInput class="col" v-model="age" />
      </div>
      <div class="row">
        <ExperienceInput class="col" v-model="experience" />
        <CharacterLevel class="col" :character="character" />
        <CharacterTier class="col" :character="character" />
      </div>
      <div class="row">
        <VitalityInput class="col" v-model="vitality" />
        <StaminaInput class="col" v-model="stamina" />
      </div>
      <div class="row">
        <BloodAlcoholContentInput class="col" v-model="bloodAlcoholContent" />
        <IntoxicationInput class="col" v-model="intoxication" />
      </div>
      <div class="mb-3">
        <SaveButton :disabled="isSubmitting || !hasChanges" :loading="isSubmitting" />
      </div>
    </form>
    <CharacterCustomizations :character="character" />
  </div>
</template>
