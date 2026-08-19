<template>
  <div>
    <section v-if="languages.length" class="row">
      <div class="col-md-6">
        <SearchInput class="mb-3" v-model="search" />
      </div>
      <div class="col-md-6">
        <ScriptSelect class="mb-3" :model-value="script?.id ?? ''" @selected="script = $event" />
      </div>
    </section>
    <section v-if="options.length" class="border-top border-secondary-subtle pt-3">
      <div class="row">
        <div v-for="language in options" :key="language.id" class="col-lg-6 col-xl-4 mb-3">
          <LanguageCard
            class="d-flex flex-column h-100"
            clickable
            :language="language"
            :selected="language.id === selected?.id"
            selection="single"
            @click="$emit('toggle', language)"
          />
        </div>
      </div>
      <a v-if="showLimit" href="#" @click="toggleLimit">{{ t(limit ? "actions.showMore" : "actions.showLess") }}</a>
    </section>
    <section v-else class="d-flex flex-column align-items-center justify-content-center text-center flex-grow-1 py-5">
      <font-awesome-icon icon="fas fa-magnifying-glass" class="display-4 text-body-secondary mb-3" aria-hidden="true" />
      <h2 class="h4 mb-2">{{ t("empty.lead") }}</h2>
      <p class="text-body-secondary mb-0">{{ t("empty.help") }}</p>
      <TarButton v-if="hasFilters" class="mt-3" icon="fas fa-arrow-rotate-left" outline :text="t('filters.clear')" variant="secondary" @click="clearFilters" />
    </section>
  </div>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import LanguageCard from "@/components/languages/LanguageCard.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import ScriptSelect from "@/components/scripts/ScriptSelect.vue";
import TarButton from "@/components/tar/TarButton.vue";
import type { Character } from "@/types/characters";
import type { Language } from "@/types/languages";
import type { Script } from "@/types/scripts";

const LIMIT: number = 12;
const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = defineProps<{
  character: Character;
  languages: Language[];
  selected?: Language;
}>();

defineEmits<{
  (e: "toggle", value: Language): void;
}>();

const limit = ref<number>(LIMIT);
const script = ref<Script>();
const search = ref<string>("");

const exclude = computed<Set<string>>(() => {
  const exclude: Set<string> = new Set(props.character.languages.map((language) => language.language.id));
  props.character.lineage.languages.granted.forEach((language) => exclude.add(language.id));
  if (props.character.lineage.parent) {
    props.character.lineage.parent.languages.granted.forEach((language) => exclude.add(language.id));
  }
  return exclude;
});
const filtered = computed<Language[]>(() =>
  props.languages.filter((language) => {
    if (exclude.value.has(language.id)) {
      return false;
    }
    const searchText: string = search.value.trim().toLocaleLowerCase();
    if (searchText && !language.name.toLocaleLowerCase().includes(searchText) && language.summary?.toLocaleLowerCase().includes(searchText) !== true) {
      return false;
    }
    if (script.value && language.script?.id !== script.value.id) {
      return false;
    }
    return true;
  }),
);
const hasFilters = computed<boolean>(() => Boolean(search.value || script.value));
const options = computed<Language[]>(() => {
  const ordered: Language[] = orderBy(filtered.value, "name");
  return limit.value ? ordered.slice(0, limit.value) : ordered;
});
const showLimit = computed<boolean>(() => options.value.length < filtered.value.length || options.value.length > LIMIT);

function toggleLimit(): void {
  limit.value = limit.value ? 0 : LIMIT;
}

function clearFilters(): void {
  limit.value = LIMIT;
  script.value = undefined;
  search.value = "";
}
defineExpose({ clearFilters });
</script>
