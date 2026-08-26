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
          <TarCard v-else class="mb-3">
            <div class="text-body-secondary">{{ t("characters.languages.source.label") }}</div>
            <div>{{ source }}</div>
          </TarCard>
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
          <TarCard v-else class="mb-3">
            <div class="text-body-secondary">{{ source }}</div>
            <div>{{ target }}</div>
          </TarCard>
        </div>
      </div>
      <NotesField :model-value="modelValue.notes" @update:model-value="updateNotes" />
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
import TarCard from "@/components/tar/TarCard.vue";
import type { Character, CharacterLanguageAcquisition, CharacterLanguageMode, CharacterTalent } from "@/types/characters";
import type { Customization } from "@/types/customizations";
import type { Language } from "@/types/languages";
import type { SelectOption } from "@/types/tar/select";
import { formatCharacterTalent } from "@/utils/talent";

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
        props.character.talents.map((talent) => ({ text: formatCharacterTalent(talent), value: talent.id })),
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
const source = computed<string>(() => t(`characters.languages.source.options.${props.modelValue.source}`));
const target = computed<string | undefined>(() => {
  switch (props.modelValue.source) {
    case "Custom":
      return props.modelValue.target;
    case "Customization":
      const customization: Customization | undefined = props.character.customizations.find((customization) => customization.id === props.modelValue.target);
      return customization?.name;
    case "Talent":
      const talent: CharacterTalent | undefined = props.character.talents.find((talent) => talent.id === props.modelValue.target);
      return talent ? formatCharacterTalent(talent) : undefined;
  }
});

function updateNotes(notes: string): void {
  emit("update:model-value", { ...props.modelValue, notes });
}
function updateSource(source: string): void {
  const target: string = (options.value && options.value.length === 1 ? options.value[0]?.value : undefined) ?? "";
  emit("update:model-value", { ...props.modelValue, source, target });
}
function updateTarget(target: string): void {
  emit("update:model-value", { ...props.modelValue, target });
}
</script>
