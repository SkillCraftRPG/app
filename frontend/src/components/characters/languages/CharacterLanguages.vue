<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import CharacterLanguageEdit from "./CharacterLanguageEdit.vue";
import CharacterLanguageRemove from "./CharacterLanguageRemove.vue";
import LanguageCard from "@/components/languages/LanguageCard.vue";
import LanguageIcon from "@/components/languages/LanguageIcon.vue";
import ScriptIcon from "@/components/languages/ScriptIcon.vue";
import type { CharacterModel } from "@/types/characters";
import type { LanguageModel } from "@/types/languages";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = defineProps<{
  character: CharacterModel;
}>();

const lineageLanguages = computed<LanguageModel[]>(() => {
  const lineageLanguages: LanguageModel[] = props.character.lineage.languages.items;
  if (props.character.lineage.species) {
    lineageLanguages.push(...props.character.lineage.species.languages.items);
  }
  return orderBy(lineageLanguages, "name");
});
const excludedLanguages = computed<LanguageModel[]>(() => lineageLanguages.value.concat(props.character.languages.map(({ language }) => language)));

defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: CharacterModel): void;
}>();
</script>

<template>
  <div>
    <h5>{{ t("characters.lineage") }}</h5>
    <div class="mb-3 row">
      <div v-for="language in lineageLanguages" :key="language.id" class="col-lg-3">
        <LanguageCard :language="language" view />
      </div>
    </div>
    <h5>{{ t("characters.languages.extra") }}</h5>
    <div class="mb-3">
      <CharacterLanguageEdit :character="character" :exclude="excludedLanguages" @error="$emit('error', $event)" />
    </div>
    <table v-if="character.languages.length > 0" class="table table-striped">
      <thead>
        <tr>
          <th scope="col">{{ t("languages.select.label") }}</th>
          <th scope="col">{{ t("characters.languages.notes") }}</th>
          <th scope="col"></th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="item in character.languages" :key="item.language.id">
          <td>
            <RouterLink :to="{ name: 'LanguageEdit', params: { id: item.language.id } }" target="_blank"><LanguageIcon />{{ item.language.name }}</RouterLink>
            <br />
            <ScriptIcon /> {{ item.language.script ?? "—" }}
          </td>
          <td>{{ item.notes ?? "—" }}</td>
          <td>
            <CharacterLanguageEdit class="me-1" :character="character" :language="item" @error="$emit('error', $event)" @updated="$emit('updated', $event)" />
            <CharacterLanguageRemove class="ms-1" :character="character" :language="item" @error="$emit('error', $event)" @updated="$emit('updated', $event)" />
          </td>
        </tr>
      </tbody>
    </table>
    <p v-else>{{ t("characters.languages.empty") }}</p>
  </div>
</template>
