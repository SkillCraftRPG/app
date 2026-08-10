<template>
  <main class="container page">
    <h1>{{ title }}</h1>
    <WorldBreadcrumb :current="t('characters.creation.label')" :parent="breadcrumb" />
    <LoadingSpinner v-if="isLoading" />
    <template v-else>
      <TarProgress class="mb-3" :value="progress" />
      <CharacterCreationAscendancy v-if="character.step === CharacterCreationStep.Ascendancy" @abandon="abandon" @error="handleError" />
      <CharacterCreationCustomization v-else-if="character.step === CharacterCreationStep.Customization" @abandon="abandon" @error="handleError" />
      <CharacterCreationContext v-else-if="character.step === CharacterCreationStep.Context" @abandon="abandon" @error="handleError" />
      <CharacterCreationTalents v-else-if="character.step === CharacterCreationStep.Talents" @abandon="abandon" @error="handleError" />
      <CharacterCreationAttributes v-else-if="character.step === CharacterCreationStep.Attributes" @abandon="abandon" />
      <CharacterCreationSkills v-else-if="character.step === CharacterCreationStep.Skills" @abandon="abandon" />
      <CharacterCreationAppearance v-else-if="character.step === CharacterCreationStep.Appearance" @abandon="abandon" />
      <CharacterCreationPersonality v-else-if="character.step === CharacterCreationStep.Personality" @abandon="abandon" />
      <CharacterCreationBackground v-else-if="character.step === CharacterCreationStep.Background" @abandon="abandon" />
      <CharacterCreationEquipment v-else-if="character.step === CharacterCreationStep.Equipment" @abandon="abandon" @error="handleError" @complete="complete" />
    </template>
  </main>
</template>

<script setup lang="ts">
import { computed, inject, ref, watchEffect } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";

import CharacterCreationAppearance from "@/components/characters/creation/CharacterCreationAppearance.vue";
import CharacterCreationAscendancy from "@/components/characters/creation/CharacterCreationAscendancy.vue";
import CharacterCreationAttributes from "@/components/characters/creation/CharacterCreationAttributes.vue";
import CharacterCreationBackground from "@/components/characters/creation/CharacterCreationBackground.vue";
import CharacterCreationContext from "@/components/characters/creation/CharacterCreationContext.vue";
import CharacterCreationCustomization from "@/components/characters/creation/CharacterCreationCustomization.vue";
import CharacterCreationEquipment from "@/components/characters/creation/CharacterCreationEquipment.vue";
import CharacterCreationPersonality from "@/components/characters/creation/CharacterCreationPersonality.vue";
import CharacterCreationSkills from "@/components/characters/creation/CharacterCreationSkills.vue";
import CharacterCreationTalents from "@/components/characters/creation/CharacterCreationTalents.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import TarProgress from "@/components/tar/TarProgress.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import type { Character, CreateCharacterPayload } from "@/types/characters";
import { CharacterCreationStep } from "@/types/characters";
import { createCharacter } from "@/api/characters";
import { handleErrorKey } from "@/inject";
import { useCharacterStore } from "@/stores/character";
import { useDocument } from "@/composables/document";

const character = useCharacterStore();
const document = useDocument();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const router = useRouter();
const { t } = useI18n();

const isLoading = ref<boolean>(false);

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("characters.title"), to: { name: "Characters" } }));
const progress = computed<number>(() => Math.floor(character.step * 100) / 9);
const title = computed<string>(() => t("characters.creation.title"));

function abandon(): void {
  character.abandon();
  router.push({ name: "Characters" });
}

async function complete(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const payload: CreateCharacterPayload = {
        lineageId: character.creation.ethnicity?.id ?? character.creation.species?.id ?? "",
        languageIds: character.creation.languages.map((language) => language.id),
        name: character.creation.name,
        dominantHand: character.creation.dominantHand,
        customizationIds: character.creation.customizations.map((customization) => customization.id),
        casteId: character.creation.caste?.id ?? "",
        educationId: character.creation.education?.id ?? "",
        talents: character.creation.talents.map((acquisition) => ({
          talentId: acquisition.talent.id,
          qualifier: acquisition.qualifier,
          notes: acquisition.notes,
          discounts: acquisition.discounts,
        })),
        attributes: character.creation.attributes,
        skills: character.creation.skills,
        appearance: character.creation.appearance,
        alignment: character.creation.alignment,
        personality: character.creation.personality,
        background: character.creation.background,
        startingWealth:
          character.creation.currency && character.creation.quantity
            ? { currencyId: character.creation.currency.id, quantity: character.creation.quantity }
            : undefined,
      };
      // TODO(fpion): const result: Character = await createCharacter(payload);
      // TODO(fpion): console.log(result);
    } catch (e: unknown) {
      handleError(e);
    } finally {
      // TODO(fpion): isLoading.value = false;
    }
  }
}

watchEffect(() => document.setTitle(title.value));
</script>
