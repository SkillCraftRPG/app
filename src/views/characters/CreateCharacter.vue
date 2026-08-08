<template>
  <main class="container page">
    <h1>{{ title }}</h1>
    <WorldBreadcrumb :current="t('characters.creation.label')" :parent="breadcrumb" />
    <TarProgress class="mb-3" :value="progress" />
    <CharacterCreationAscendancy v-if="character.step === CharacterCreationStep.Ascendancy" @abandon="abandon" @error="handleError" />
    <CharacterCreationCustomization v-else-if="character.step === CharacterCreationStep.Customization" @abandon="abandon" @error="handleError" />
    <CharacterCreationContext v-else-if="character.step === CharacterCreationStep.Context" @abandon="abandon" @error="handleError" />
    <CharacterCreationTalents v-else-if="character.step === CharacterCreationStep.Talents" @abandon="abandon" @error="handleError" />
    <CharacterCreationAttributes v-else-if="character.step === CharacterCreationStep.Attributes" @abandon="abandon" />
    <CharacterCreationSkills v-else-if="character.step === CharacterCreationStep.Skills" @abandon="abandon" />
    <CharacterCreationAppearance v-else-if="character.step === CharacterCreationStep.Appearance" @abandon="abandon" />
    <CharacterCreationPersonality v-else-if="character.step === CharacterCreationStep.Personality" @abandon="abandon" />
  </main>
</template>

<script setup lang="ts">
import { computed, inject, watchEffect } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";

import CharacterCreationAppearance from "@/components/characters/creation/CharacterCreationAppearance.vue";
import CharacterCreationAscendancy from "@/components/characters/creation/CharacterCreationAscendancy.vue";
import CharacterCreationAttributes from "@/components/characters/creation/CharacterCreationAttributes.vue";
import CharacterCreationContext from "@/components/characters/creation/CharacterCreationContext.vue";
import CharacterCreationCustomization from "@/components/characters/creation/CharacterCreationCustomization.vue";
import CharacterCreationPersonality from "@/components/characters/creation/CharacterCreationPersonality.vue";
import CharacterCreationSkills from "@/components/characters/creation/CharacterCreationSkills.vue";
import CharacterCreationTalents from "@/components/characters/creation/CharacterCreationTalents.vue";
import TarProgress from "@/components/tar/TarProgress.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import { CharacterCreationStep } from "@/types/characters";
import { handleErrorKey } from "@/inject";
import { useCharacterStore } from "@/stores/character";
import { useDocument } from "@/composables/document";

const character = useCharacterStore();
const document = useDocument();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const router = useRouter();
const { t } = useI18n();

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("characters.title"), to: { name: "Characters" } }));
const progress = computed<number>(() => Math.floor(character.step * 100) / 9);
const title = computed<string>(() => t("characters.creation.title"));

function abandon(): void {
  character.abandon();
  router.push({ name: "Characters" });
}

watchEffect(() => document.setTitle(title.value));
</script>
