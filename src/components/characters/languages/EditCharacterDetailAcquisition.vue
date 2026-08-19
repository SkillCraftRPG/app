<template>
  <div>
    <LanguageCard class="mb-3" :language="language" />
    <section>
      <div class="row">
        <div class="col-md-6">
          <CharacterLanguageSourceField
            v-if="mode === 'add'"
            :character="character"
            class="mb-3"
            :model-value="modelValue.source"
            required
            @update:model-value="updateSource"
          />
          <div v-else>
            <div class="fw-semibold">{{ t("characters.languages.source.label") }}</div>
            <div>{{ t(`characters.languages.source.options.${modelValue.source}`) }}</div>
          </div>
        </div>
        <div v-if="showTarget" class="col-md-6">
          <CharacterLanguageTargetField
            v-if="mode === 'add'"
            class="mb-3"
            :model-value="modelValue.target"
            :options="options"
            :placeholder="placeholder"
            required
            @update:model-value="updateTarget"
          />
          <div v-else>
            <div class="fw-semibold">{{ t("characters.languages.target") }}</div>
            <div>{{ target }}</div>
          </div>
        </div>
      </div>
      <NotesField />
    </section>
  </div>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import CharacterLanguageSourceField from "./CharacterLanguageSourceField.vue";
import CharacterLanguageTargetField from "./CharacterLanguageTargetField.vue";
import LanguageCard from "@/components/languages/LanguageCard.vue";
import NotesField from "@/components/shared/NotesField.vue";
import type { Character, CharacterLanguageAcquisition, CharacterLanguageMode } from "@/types/characters";
import type { Language } from "@/types/languages";
import type { SelectOption } from "@/types/tar/select";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = defineProps<{
  character: Character;
  language: Language;
  mode: CharacterLanguageMode;
  modelValue: CharacterLanguageAcquisition;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: CharacterLanguageAcquisition): void;
}>();

const options = computed<SelectOption[] | undefined>(() => {
  switch (props.modelValue.source) {
    case "Customization":
      return orderBy(
        props.character.customizations.map(({ id, name }) => ({ text: name, value: id })),
        "text",
      );
    case "Talent":
      return orderBy(
        props.character.talents.map((talent) => ({ text: talent.talent.name, value: talent.talent.id })), // TODO(fpion): qualifier
        "text",
      );
  }
});
const placeholder = computed<string | undefined>(() => {
  switch (props.modelValue.source) {
    case "Customization":
      return "customizations.placeholder";
    case "Talent":
      return "talents.placeholder";
  }
});
const showTarget = computed<boolean>(() => Boolean(props.modelValue.source && props.modelValue.source !== "Extra"));
const target = computed<string>(() => ""); // TODO(fpion): implement

function updateSource(source: string): void {
  emit("update:model-value", { ...props.modelValue, source, target: "" });
}
function updateTarget(target: string): void {
  emit("update:model-value", { ...props.modelValue, target });
}
</script>
