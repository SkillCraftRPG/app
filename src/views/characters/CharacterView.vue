<template>
  <main class="container page">
    <div v-if="character">
      <h1>{{ title }}</h1>
      <WorldBreadcrumb :current="title" :parent="breadcrumb" />
      <TarAlert :close="t('actions.close')" dismissible variant="success" v-model="isCreated">
        <strong>{{ t("characters.created.lead") }}</strong> {{ t("characters.created.help", { name: title }) }}
      </TarAlert>
      <StatusDetail class="mb-3" :subject="character" />
      <TarTabs>
        <TarTab active id="tab-1" title="Tab #1">
          <CharacterHeader :character="character" />
          <div class="row">
            <div class="col-md-4">
              <CharacterExperience class="mb-3" :character="character" />
            </div>
            <div class="col-md-4">
              <CharacterVitality class="mb-3" :character="character" />
            </div>
            <div class="col-md-4">
              <CharacterStamina class="mb-3" :character="character" />
            </div>
          </div>
          <!-- TODO(fpion): Hope, Blood Alcohol Content & Intoxication -->
          <CharacterConditions v-if="hasConditions" :character="character" class="mb-3" />
          <div class="fs-5 mb-1">{{ t("characters.attributes.title") }}</div>
          <CharacterAttributes :attributes="character.attributes" />
          <div class="fs-5 mb-1">{{ t("characters.statistics.title") }}</div>
          <CharacterStatistics :statistics="character.statistics" />
          <div class="fs-5 mb-1">{{ t("characters.skills.title") }}</div>
          <CharacterSkills :skills="character.skills" />
          <div class="fs-5 mb-1">{{ t("lineages.physical.speeds.lead") }}</div>
          <CharacterSpeeds :speeds="character.speeds" />
          <template v-if="character.customizations.length">
            <div class="fs-5 mb-1">{{ t("customizations.title") }}</div>
            <CharacterCustomizations :customizations="character.customizations" />
          </template>
          <template v-if="hasLanguages">
            <div class="fs-5 mb-1">{{ t("languages.title") }}</div>
            <CharacterLanguages :languages="character.languages" :lineage="character.lineage" />
          </template>
          <!-- TODO(fpion): Specializations -->
        </TarTab>
        <TarTab id="tab-2" title="Tab #2">
          <CharacterForm :character="character" @error="handleError" @updated="update" />
        </TarTab>
      </TarTabs>
      <!-- <pre>{{ JSON.stringify(character, null, 2) }}</pre> -->
    </div>
    <LoadingSpinner v-else />
  </main>
</template>

<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import CharacterAttributes from "@/components/characters/CharacterAttributes.vue";
import CharacterConditions from "@/components/characters/CharacterConditions.vue";
import CharacterCustomizations from "@/components/characters/CharacterCustomizations.vue";
import CharacterExperience from "@/components/characters/CharacterExperience.vue";
import CharacterForm from "@/components/characters/CharacterForm.vue";
import CharacterHeader from "@/components/characters/header/CharacterHeader.vue";
import CharacterLanguages from "@/components/characters/CharacterLanguages.vue";
import CharacterSkills from "@/components/characters/CharacterSkills.vue";
import CharacterStamina from "@/components/characters/CharacterStamina.vue";
import CharacterStatistics from "@/components/characters/CharacterStatistics.vue";
import CharacterVitality from "@/components/characters/CharacterVitality.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import TarTab from "@/components/tar/TarTab.vue";
import TarTabs from "@/components/tar/TarTabs.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import type { Character } from "@/types/characters";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { handleErrorKey } from "@/inject";
import { readCharacter } from "@/api/characters";
import { useDocument } from "@/composables/document";
import { useEventStore } from "@/stores/event";
import { useToastStore } from "@/stores/toast";
import CharacterSpeeds from "@/components/characters/CharacterSpeeds.vue";

const document = useDocument();
const events = useEventStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const character = ref<Character>();
const isCreated = ref<boolean>(false);

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("characters.title"), to: { name: "Characters" } }));
const hasConditions = computed<boolean>(() => false); // TODO(fpion): implement
const hasLanguages = computed<boolean>(() =>
  Boolean(
    character.value &&
    (character.value.languages.length || character.value.lineage.languages.granted.length || character.value.lineage.parent?.languages.granted.length),
  ),
);
const title = computed<string>(() => character.value?.name ?? "");

function update(value: Character): void {
  isCreated.value = false;
  character.value = value;
  toasts.success("saved");
}

onMounted(async () => {
  try {
    const id: string = (Array.isArray(route.params.id) ? route.params.id[0] : route.params.id) ?? "";
    const found: Character = await readCharacter(id);
    isCreated.value = events.shift() === "created";
    character.value = found;
    document.setTitle(title.value);
  } catch (e: unknown) {
    const failure = e as ApiFailure;
    if (failure.status === StatusCodes.NotFound) {
      router.push("/not-found");
    } else {
      handleError(e);
    }
  }
});

// TODO(fpion): Player
// TODO(fpion): Picture
// TODO(fpion): Critical
// TODO(fpion): Talents
// TODO(fpion): Spells
// TODO(fpion): Inventory
// TODO(fpion): Attacks & Defense
// TODO(fpion): Notes
</script>
