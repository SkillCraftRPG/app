<template>
  <div class="card">
    <button
      type="button"
      class="clickable card-body btn text-body text-center d-flex flex-column align-items-center justify-content-center pe-5"
      @click="$emit('click')"
    >
      <div>
        <span class="fw-semibold">{{ language.language.name }}</span>
        <span v-if="language.language.script" class="ms-1 small text-body-secondary">({{ language.language.script.name }})</span>
      </div>
      <div class="text-body-secondary"><font-awesome-icon :icon="icon" aria-hidden="true" />&nbsp;{{ source }}</div>
    </button>
    <div v-if="!isReadOnly" class="position-absolute top-0 end-0 m-2" @click.stop>
      <button type="button" class="btn btn-sm" @click="$emit('remove')">
        <font-awesome-icon icon="fas fa-xmark" aria-hidden="true" />
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import type { Character, CharacterLanguage, CharacterTalent } from "@/types/characters";
import type { Customization } from "@/types/customizations";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  character: Character;
  language: CharacterLanguage;
  readonly?: boolean | string;
}>();

defineEmits<{
  (e: "click"): void;
  (e: "edit"): void;
  (e: "remove"): void;
}>();

const icon = computed<string>(() => {
  switch (props.language.source) {
    case "Customization":
      return "fas fa-wheelchair";
    case "Extra":
      return "fas fa-paw";
    case "Talent":
      return "fas fa-code-branch";
    default:
      return "fas fa-edit";
  }
});
const isReadOnly = computed<boolean>(() => parseBoolean(props.readonly) ?? false);
const source = computed<string>(() => {
  switch (props.language.source) {
    case "Customization":
      const customization: Customization | undefined = props.character.customizations.find((customization) => customization.id === props.language.target);
      return customization?.name ?? "";
    case "Extra":
      if (!props.language.target) {
        return t("characters.languages.extra");
      } else if (props.language.target === props.character.lineage.id) {
        return props.character.lineage.name;
      } else if (props.language.target === props.character.lineage.parent?.id) {
        return props.character.lineage.parent.name;
      }
      return "";
    case "Talent":
      const talent: CharacterTalent | undefined = props.character.talents.find((talent) => talent.id === props.language.target);
      return talent?.talent.name ?? ""; // TODO(fpion): qualifier
    default:
      return props.language.target ?? "";
  }
});
</script>
