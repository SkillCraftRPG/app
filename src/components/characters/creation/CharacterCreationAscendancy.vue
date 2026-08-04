<template>
  <section>
    <h2 class="h3">{{ t("characters.creation.ascendancy.title") }}</h2>
    <section v-if="isLoading !== true">
      <h3 class="h5">{{ t("lineages.species.label") }}</h3>
      <template v-if="speciesList.length">
        <p class="text-body-secondary">{{ t("characters.creation.ascendancy.species.help") }}</p>
        <div class="row">
          <div v-for="lineage in speciesList" :key="lineage.id" class="col-md-6 col-lg-4 col-xl-3 mb-3">
            <LineageCard class="d-flex flex-column h-100" clickable :lineage="lineage" :selected="lineage.id === species?.id" @click="toggleSpecies(lineage)">
              <div class="d-flex justify-content-end mt-2">
                <font-awesome-icon :icon="lineage.id === species?.id ? 'far fa-square-check' : 'far fa-square'" />
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
              :selected="lineage.id === ethnicity?.id"
              @click="toggleEthnicity(lineage)"
            >
              <div class="d-flex justify-content-end mt-2">
                <font-awesome-icon :icon="lineage.id === ethnicity?.id ? 'far fa-square-check' : 'far fa-square'" />
              </div>
            </LineageCard>
          </div>
        </div>
      </section>
      <section v-if="showLanguages">
        <h3 class="h5">{{ t("languages.title") }}</h3>
        <p class="text-body-secondary">{{ languagesHelp }}</p>
        <div class="row">
          <div v-for="language in languageList" :key="language.id" class="col-md-6 col-lg-4 col-xl-3 mb-3">
            <LanguageCard
              class="d-flex flex-column h-100"
              :clickable="isClickable(language)"
              :language="language"
              :selected="languages.has(language.id)"
              @click="toggleLanguage(language)"
            >
              <div class="d-flex justify-content-end mt-2">
                <TarBadge v-if="grantedLanguages.has(language.id)" pill variant="secondary">
                  <font-awesome-icon aria-hidden="true" icon="fas fa-paw" />&nbsp;{{ grantedLanguages.get(language.id) }}
                </TarBadge>
                <font-awesome-icon v-else :icon="languages.has(language.id) ? 'far fa-square-check' : 'far fa-square'" />
              </div>
            </LanguageCard>
          </div>
        </div>
      </section>
    </template>
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
import type { Language } from "@/types/languages";
import type { Lineage } from "@/types/lineages";
import type { SearchResults } from "@/types/search";
import { listEthnicities, listSpecies } from "@/api/lineages";
import { listLanguages } from "@/api/languages";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
}>();

const ethnicities = ref<Lineage[]>([]);
const ethnicity = ref<Lineage>();
const isLoading = ref<boolean | "ethnicities">(true);
const languageList = ref<Language[]>([]);
const languages = ref<Set<string>>(new Set());
const species = ref<Lineage>();
const speciesList = ref<Lineage[]>([]);

const showLanguages = computed<boolean>(() => Boolean(languageList.value.length && species.value && (!ethnicities.value.length || ethnicity.value)));
const extraLanguages = computed<number>(() => (species.value?.languages.extra ?? 0) + (ethnicity.value?.languages.extra ?? 0));
const grantedLanguages = computed<Map<string, string>>(() => {
  const grantedLanguages: Map<string, string> = new Map();
  if (species.value) {
    species.value.languages.granted.forEach((language) => grantedLanguages.set(language.id, t("lineages.species.label")));
  }
  if (ethnicity.value) {
    ethnicity.value.languages.granted.forEach((language) => grantedLanguages.set(language.id, t("lineages.ethnicities.label")));
  }
  return grantedLanguages;
});
const remainingLanguages = computed<number>(() => (extraLanguages.value <= languages.value.size ? 0 : extraLanguages.value - languages.value.size));
const languagesHelp = computed<string>(() => {
  if (extraLanguages.value) {
    return t("characters.creation.ascendancy.languages.extra", remainingLanguages.value);
  }
  return t("characters.creation.ascendancy.languages.none");
});

function isClickable(language: Language): boolean {
  return Boolean(!grantedLanguages.value.has(language.id) && (languages.value.has(language.id) || remainingLanguages.value));
}

function toggleEthnicity(value: Lineage): void {
  if (ethnicity.value?.id === value.id) {
    ethnicity.value = undefined;
  } else {
    ethnicity.value = value;
  }
  languages.value.clear();
}

function toggleLanguage(value: Language): void {
  if (languages.value.has(value.id)) {
    languages.value.delete(value.id);
  } else if (isClickable(value)) {
    languages.value.add(value.id);
  }
}

async function toggleSpecies(value: Lineage): Promise<void> {
  if (species.value?.id === value.id) {
    species.value = undefined;
    ethnicities.value = [];
  } else {
    species.value = value;

    isLoading.value = "ethnicities";
    try {
      const results: SearchResults<Lineage> = await listEthnicities(species.value.id);
      ethnicities.value = orderBy(results.items, "name");
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
  ethnicity.value = undefined;
  languages.value.clear();
}

onMounted(async () => {
  try {
    const species: SearchResults<Lineage> = await listSpecies();
    speciesList.value = orderBy(species.items, "name");

    const languages: SearchResults<Language> = await listLanguages();
    languageList.value = orderBy(languages.items, "name");
  } catch (e: unknown) {
    emit("error", e);
  } finally {
    isLoading.value = false;
  }
});
</script>
