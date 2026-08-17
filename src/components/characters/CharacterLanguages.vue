<template>
  <div class="row">
    <div v-for="language in allLanguages" :key="language.id" class="col-md-6 col-lg-4 col-xl-3">
      <CharacterLanguage class="mb-3" :language="language" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";

import CharacterLanguage from "./CharacterLanguage.vue";
import type { CharacterLanguage as CharacterLanguageT } from "@/types/characters";
import type { Language } from "@/types/languages";
import type { Lineage } from "@/types/lineages";

const { orderBy } = arrayUtils;

const props = defineProps<{
  languages: CharacterLanguageT[];
  lineage: Lineage;
}>();

const allLanguages = computed<Language[]>(() => {
  const languages: Language[] = [...props.languages.map((language) => language.language)];
  props.lineage.languages.granted.forEach((language) => languages.push(language));
  if (props.lineage.parent) {
    props.lineage.parent.languages.granted.forEach((language) => languages.push(language));
  }
  return orderBy(languages, "name");
});
</script>
