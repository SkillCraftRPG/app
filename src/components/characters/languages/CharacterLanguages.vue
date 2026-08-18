<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-2">
      <div class="fs-5">{{ t("languages.title") }}</div>
      <TarButton v-if="!isReadOnly" icon="fas fa-plus" size="small" :text="t('actions.add')" @click="add" />
    </div>
    <TarAlert v-if="!isValid" show variant="warning">
      <strong>{{ t("characters.languages.invalid.lead") }}</strong> {{ t("characters.languages.invalid.help", { extra }) }}
    </TarAlert>
    <div v-if="languages.length" class="row">
      <div v-for="language in languages" :key="language.language.id" class="col-md-6 col-lg-4 col-xl-3 mb-3">
        <CharacterLanguageCard
          :character="character"
          class="d-flex flex-column h-100"
          :language="language"
          :readonly="isReadOnly || (language.source === 'Extra' && Boolean(language.target))"
          @click="detail(language)"
          @edit="edit(language)"
          @remove="remove(language)"
        />
      </div>
    </div>
    <p v-else>{{ t("characters.languages.empty") }}</p>
    <LanguageDetailModal v-if="language" :language="language.language" ref="detailModal" />
    <!-- edit modal -->
    <RemoveCharacterLanguageModal
      v-if="language"
      :character="character"
      :language="language"
      ref="removeModal"
      @error="$emit('error', $event)"
      @updated="$emit('updated', $event)"
    />
  </div>
</template>

<script setup lang="ts">
import { arrayUtils, parsingUtils } from "logitar-js";
import { computed, nextTick, ref } from "vue";
import { useI18n } from "vue-i18n";

import CharacterLanguageCard from "./CharacterLanguageCard.vue";
import RemoveCharacterLanguageModal from "./RemoveCharacterLanguageModal.vue";
import LanguageDetailModal from "@/components/languages/LanguageDetailModal.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import TarButton from "@/components/tar/TarButton.vue";
import type { Character, CharacterLanguage } from "@/types/characters";

const { orderBy } = arrayUtils;
const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  character: Character;
  readonly?: boolean | string;
}>();

defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Character): void;
}>();

const detailModal = ref<InstanceType<typeof LanguageDetailModal> | null>(null);
const language = ref<CharacterLanguage>();
const removeModal = ref<InstanceType<typeof RemoveCharacterLanguageModal> | null>(null);

const isReadOnly = computed<boolean>(() => parseBoolean(props.readonly) ?? false);

const extra = computed<number>(() => {
  const extra: number = props.character.lineage.languages.extra;
  return props.character.lineage.parent ? extra + props.character.lineage.parent.languages.extra : extra;
});
const isValid = computed<boolean>(() => props.character.languages.filter((language) => language.source === "Extra").length === extra.value);

type SortableCharacterLanguage = CharacterLanguage & {
  sort: string;
};
const languages = computed<SortableCharacterLanguage[]>(() => {
  const languages: CharacterLanguage[] = [...props.character.languages];
  props.character.lineage.languages.granted.forEach((language) =>
    languages.push({
      language,
      source: "Extra",
      target: props.character.lineage.id,
      createdBy: props.character.lineage.createdBy,
      createdOn: props.character.lineage.createdOn,
      updatedBy: props.character.lineage.createdBy,
      updatedOn: props.character.lineage.createdOn,
    }),
  );
  if (props.character.lineage.parent) {
    props.character.lineage.parent.languages.granted.forEach((language) =>
      languages.push({
        language,
        source: "Extra",
        target: props.character.lineage.parent!.id,
        createdBy: props.character.lineage.parent!.createdBy,
        createdOn: props.character.lineage.parent!.createdOn,
        updatedBy: props.character.lineage.parent!.createdBy,
        updatedOn: props.character.lineage.parent!.createdOn,
      }),
    );
  }
  return orderBy(
    languages.map((language) => ({ ...language, sort: language.language.name })),
    "sort",
  );
});

function add(): void {
  language.value = undefined;
  // TODO(fpion): modal
}
function detail(value: CharacterLanguage): void {
  language.value = value;
  nextTick(() => detailModal.value?.open());
}
function edit(value: CharacterLanguage): void {
  language.value = value;
  // TODO(fpion): modal
}
function remove(value: CharacterLanguage): void {
  language.value = value;
  nextTick(() => removeModal.value?.open());
}
</script>
