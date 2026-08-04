<template>
  <section>
    <h2 class="h3">{{ t("characters.creation.ascendancy.title") }}</h2>
    <section v-if="isLoading !== true">
      <h3 class="h5">{{ t("lineages.species.label") }}</h3>
      <template v-if="species.length">
        <p class="text-body-secondary">{{ t("characters.creation.ascendancy.species.help") }}</p>
        <div class="row">
          <div v-for="lineage in species" :key="lineage.id" class="col-md-6 col-lg-4 col-xl-3 mb-3">
            <LineageCard
              class="d-flex flex-column h-100"
              clickable
              :lineage="lineage"
              :selected="lineage.id === selectedSpecies?.id"
              @click="toggleSpecies(lineage)"
            >
              <div class="d-flex justify-content-end mt-2">
                <font-awesome-icon :icon="lineage.id === selectedSpecies?.id ? 'far fa-square-check' : 'far fa-square'" />
              </div>
            </LineageCard>
          </div>
        </div>
      </template>
      <TarAlert v-else class="d-flex justify-content-between" show variant="warning">
        <div>
          <strong>{{ t("characters.creation.ascendancy.species.empty.lead") }}</strong> {{ t("characters.creation.ascendancy.species.empty.help") }}
        </div>
        <RouterLink :to="{ name: 'Lineages' }" class="btn btn-primary">
          <font-awesome-icon aria-hidden="true" icon="fas fa-paw" />&nbsp;{{ t("lineages.species.title") }}
        </RouterLink>
      </TarAlert>
    </section>
    <LoadingSpinner v-if="isLoading" />
    <template v-else>
      <section v-if="ethnicities.length">
        <h3 class="h5">{{ t("lineages.ethnicities.label") }}</h3>
        <p class="text-body-secondary">{{ t("characters.creation.ascendancy.ethnicity") }}</p>
        <div class="row">
          <div v-for="lineage in ethnicities" :key="lineage.id" class="col-md-6 col-lg-4 col-xl-3 mb-3">
            <LineageCard
              class="d-flex flex-column h-100"
              clickable
              :lineage="lineage"
              :selected="lineage.id === selectedEthnicity?.id"
              @click="toggleEthnicity(lineage)"
            >
              <div class="d-flex justify-content-end mt-2">
                <font-awesome-icon :icon="lineage.id === selectedEthnicity?.id ? 'far fa-square-check' : 'far fa-square'" />
              </div>
            </LineageCard>
          </div>
        </div>
      </section>
      <section v-if="showLanguages">
        <h3 class="h5">{{ t("languages.title") }}</h3>
        <p class="text-body-secondary">{{ languagesHelp }}</p>
        <div class="row">
          <div v-for="language in languages" :key="language.id" class="col-md-6 col-lg-4 col-xl-3 mb-3">
            <LanguageCard
              class="d-flex flex-column h-100"
              :clickable="isClickable(language)"
              :language="language"
              :selected="selectedLanguages.has(language.id)"
              @click="toggleLanguage(language)"
            >
              <div class="d-flex justify-content-end mt-2">
                <TarBadge v-if="grantedLanguages.has(language.id)" pill variant="secondary">
                  <font-awesome-icon aria-hidden="true" icon="fas fa-paw" />&nbsp;{{ grantedLanguages.get(language.id) }}
                </TarBadge>
                <font-awesome-icon v-else :icon="selectedLanguages.has(language.id) ? 'far fa-square-check' : 'far fa-square'" />
              </div>
            </LanguageCard>
          </div>
        </div>
      </section>
    </template>
    <div class="d-flex justify-content-between">
      <TarButton icon="fas fa-xmark" outline :text="t('actions.abandon')" variant="danger" @click="$emit('abandon')" />
      <TarButton :disabled="!isValid" icon="fas fa-arrow-right" :text="t('actions.next')" @click="submit" />
    </div>
  </section>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import LanguageCard from "@/components/languages/LanguageCard.vue";
import LineageCard from "@/components/lineages/LineageCard.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import TarBadge from "@/components/tar/TarBadge.vue";
import TarButton from "@/components/tar/TarButton.vue";
import type { Language } from "@/types/languages";
import type { Lineage } from "@/types/lineages";
import type { SearchResults } from "@/types/search";
import { listEthnicities, listSpecies } from "@/api/lineages";
import { listLanguages } from "@/api/languages";
import { useCharacterStore } from "@/stores/character";

const character = useCharacterStore();
const { orderBy } = arrayUtils;
const { t } = useI18n();

const emit = defineEmits<{
  (e: "abandon"): void;
  (e: "error", value: unknown): void;
}>();

const ethnicities = ref<Lineage[]>([]);
const isLoading = ref<boolean | "ethnicities">(true);
const languages = ref<Language[]>([]);
const selectedEthnicity = ref<Lineage>();
const selectedLanguages = ref<Set<string>>(new Set());
const selectedSpecies = ref<Lineage>();
const species = ref<Lineage[]>([]);

const showLanguages = computed<boolean>(() =>
  Boolean(languages.value.length && selectedSpecies.value && (!ethnicities.value.length || selectedEthnicity.value)),
);
const extraLanguages = computed<number>(() => (selectedSpecies.value?.languages.extra ?? 0) + (selectedEthnicity.value?.languages.extra ?? 0));
const grantedLanguages = computed<Map<string, string>>(() => {
  const grantedLanguages: Map<string, string> = new Map();
  if (selectedSpecies.value) {
    selectedSpecies.value.languages.granted.forEach((language) => grantedLanguages.set(language.id, t("lineages.species.label")));
  }
  if (selectedEthnicity.value) {
    selectedEthnicity.value.languages.granted.forEach((language) => grantedLanguages.set(language.id, t("lineages.ethnicities.label")));
  }
  return grantedLanguages;
});
const remainingLanguages = computed<number>(() =>
  extraLanguages.value <= selectedLanguages.value.size ? 0 : extraLanguages.value - selectedLanguages.value.size,
);
const languagesHelp = computed<string>(() => {
  if (extraLanguages.value) {
    return t("characters.creation.ascendancy.languages.extra", remainingLanguages.value);
  }
  return t("characters.creation.ascendancy.languages.none");
});

const isValid = computed<boolean>(() => Boolean(selectedSpecies.value && (!ethnicities.value.length || selectedEthnicity.value) && !remainingLanguages.value));

function isClickable(language: Language): boolean {
  return Boolean(!grantedLanguages.value.has(language.id) && (selectedLanguages.value.has(language.id) || remainingLanguages.value));
}

function toggleEthnicity(value: Lineage): void {
  if (selectedEthnicity.value?.id === value.id) {
    selectedEthnicity.value = undefined;
  } else {
    selectedEthnicity.value = value;
  }
  selectedLanguages.value.clear();
}

function toggleLanguage(value: Language): void {
  if (selectedLanguages.value.has(value.id)) {
    selectedLanguages.value.delete(value.id);
  } else if (isClickable(value)) {
    selectedLanguages.value.add(value.id);
  }
}

async function loadEthnicities(species: Lineage): Promise<void> {
  isLoading.value = "ethnicities";
  try {
    const results: SearchResults<Lineage> = await listEthnicities(species.id);
    ethnicities.value = orderBy(results.items, "name");
  } catch (e: unknown) {
    emit("error", e);
  } finally {
    isLoading.value = false;
  }
}

async function toggleSpecies(value: Lineage): Promise<void> {
  if (selectedSpecies.value?.id === value.id) {
    selectedSpecies.value = undefined;
    ethnicities.value = [];
  } else {
    selectedSpecies.value = value;
    await loadEthnicities(selectedSpecies.value);
  }
  selectedEthnicity.value = undefined;
  selectedLanguages.value.clear();
}

function submit(): void {
  if (isValid.value && selectedSpecies.value) {
    character.saveAscendancy(
      selectedSpecies.value,
      languages.value.filter((language) => selectedLanguages.value.has(language.id)),
      selectedEthnicity.value,
    );
  }
}

onMounted(async () => {
  try {
    const speciesResults: SearchResults<Lineage> = await listSpecies();
    species.value = orderBy(speciesResults.items, "name");
    selectedSpecies.value = species.value.find((species) => species.id === character.creation.species?.id);

    if (selectedSpecies.value) {
      await loadEthnicities(selectedSpecies.value);
      selectedEthnicity.value = ethnicities.value.find((ethnicity) => ethnicity.id === character.creation.ethnicity?.id);
    }

    const languageResults: SearchResults<Language> = await listLanguages();
    languages.value = orderBy(languageResults.items, "name");

    const languageIds: Set<string> = new Set(character.creation.languages.map((language) => language.id));
    languages.value.forEach((language) => {
      if (languageIds.has(language.id)) {
        selectedLanguages.value.add(language.id);
      }
    });
  } catch (e: unknown) {
    emit("error", e);
  } finally {
    isLoading.value = false;
  }
});
</script>
