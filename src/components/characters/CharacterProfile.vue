<template>
  <form @submit.prevent="handleSubmit(submit)">
    <div class="row">
      <div class="col-md-6">
        <NameField class="mb-3" required v-model="name" />
      </div>
      <div class="col-md-6">
        <DominantHandRadio class="mb-3" v-model="dominantHand" />
      </div>
    </div>
    <div class="row">
      <div class="col-md-4">
        <HeightField class="mb-3" v-model="height">
          <template #append>
            <span class="input-group-text">{{ sizeCategory }}</span>
          </template>
        </HeightField>
      </div>
      <div class="col-md-4">
        <WeightField class="mb-3" v-model="weight" />
      </div>
      <div class="col-md-4">
        <AgeField class="mb-3" v-model="age" />
      </div>
    </div>
    <div class="row">
      <div class="col-md-4">
        <SkinField class="mb-3" v-model="skin" />
      </div>
      <div class="col-md-4">
        <EyesField class="mb-3" v-model="eyes" />
      </div>
      <div class="col-md-4">
        <HairField class="mb-3" v-model="hair" />
      </div>
    </div>
    <CharacterAlignment class="mb-3" v-model="alignment" />
    <div class="row">
      <div class="col-md-4">
        <PersonalityField class="mb-3" v-model="traits" />
      </div>
      <div class="col-md-4">
        <IdealsField class="mb-3" v-model="ideals" />
      </div>
      <div class="col-md-4">
        <FlawsField class="mb-3" v-model="flaws" />
      </div>
    </div>
    <BackgroundField class="mb-3" v-model="background" />
    <div class="d-flex justify-content-end mb-3">
      <TarButton
        :disabled="!hasChanges || isLoading"
        icon="fas fa-floppy-disk"
        :loading="isLoading"
        size="large"
        :status="t('loading')"
        :text="t('actions.save')"
        type="submit"
      />
    </div>
  </form>
</template>

<script setup lang="ts">
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import AgeField from "./AgeField.vue";
import BackgroundField from "./BackgroundField.vue";
import CharacterAlignment from "./CharacterAlignment.vue";
import DominantHandRadio from "./DominantHandRadio.vue";
import EyesField from "./EyesField.vue";
import FlawsField from "./FlawsField.vue";
import HairField from "./HairField.vue";
import HeightField from "./HeightField.vue";
import IdealsField from "./IdealsField.vue";
import NameField from "@/components/shared/NameField.vue";
import PersonalityField from "./PersonalityField.vue";
import SkinField from "./SkinField.vue";
import TarButton from "@/components/tar/TarButton.vue";
import WeightField from "./WeightField.vue";
import type { Alignment, Character, DominantHand, UpdateCharacterPayload } from "@/types/characters";
import { updateCharacter } from "@/api/characters";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  character: Character;
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Character): void;
}>();

const age = ref<number>(0);
const alignment = ref<Alignment | null>(null);
const background = ref<string>("");
const dominantHand = ref<DominantHand | null>(null);
const eyes = ref<string>("");
const flaws = ref<string>("");
const hair = ref<string>("");
const height = ref<number>(0);
const ideals = ref<string>("");
const isLoading = ref<boolean>(false);
const name = ref<string>("");
const skin = ref<string>("");
const traits = ref<string>("");
const weight = ref<number>(0);

const hasChanges = computed<boolean>(
  () =>
    props.character.name !== name.value ||
    (props.character.dominantHand ?? null) !== dominantHand.value ||
    (props.character.appearance.height ?? 0) !== height.value ||
    (props.character.appearance.weight ?? 0) !== weight.value ||
    (props.character.appearance.age ?? 0) !== age.value ||
    (props.character.appearance.skin ?? "") !== skin.value ||
    (props.character.appearance.eyes ?? "") !== eyes.value ||
    (props.character.appearance.hair ?? "") !== hair.value ||
    (props.character.alignment ?? null) !== alignment.value ||
    (props.character.personality.traits ?? "") !== traits.value ||
    (props.character.personality.ideals ?? "") !== ideals.value ||
    (props.character.personality.flaws ?? "") !== flaws.value ||
    (props.character.background ?? "") !== background.value,
);
const sizeCategory = computed<string>(() =>
  t(`game.size.category.options.${props.character.lineage.parent?.size.category ?? props.character.lineage.size.category}`),
);

const { handleSubmit, reinitialize } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const payload: UpdateCharacterPayload = {
        name: name.value,
        dominantHand: { value: dominantHand.value },
      };
      const character: Character = await updateCharacter(props.character.id, payload);
      emit("updated", character);
      reinitialize();
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

watch(
  () => props.character,
  (character) => {
    name.value = character.name;
    dominantHand.value = character.dominantHand ?? null;
    height.value = character.appearance.height ?? 0;
    weight.value = character.appearance.weight ?? 0;
    age.value = character.appearance.age ?? 0;
    skin.value = character.appearance.skin ?? "";
    eyes.value = character.appearance.eyes ?? "";
    hair.value = character.appearance.hair ?? "";
    alignment.value = character.alignment ?? null;
    traits.value = character.personality.traits ?? "";
    ideals.value = character.personality.ideals ?? "";
    flaws.value = character.personality.flaws ?? "";
    background.value = character.background ?? "";
  },
  { deep: true, immediate: true },
);

// TODO(fpion): height should be in meters
// TODO(fpion): weight should be in kg
</script>
