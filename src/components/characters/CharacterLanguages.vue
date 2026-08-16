<template>
  <div class="row">
    <div v-for="language in allLanguages" :key="language.id" class="col-md-6 col-lg-4 col-xl-3">
      <TarCard class="clickable text-center mb-3">
        <div class="fw-semibold">{{ language.name }}</div>
        <div class="text-body-secondary">{{ language.script?.name ?? "—" }}</div>
      </TarCard>
    </div>
  </div>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";

import TarCard from "@/components/tar/TarCard.vue";
import type { CharacterLanguage } from "@/types/characters";
import type { Language } from "@/types/languages";
import type { Lineage } from "@/types/lineages";

const { orderBy } = arrayUtils;

const props = defineProps<{
  languages: CharacterLanguage[];
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
