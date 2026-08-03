<template>
  <div>
    <TarButton icon="fas fa-download" outline size="large" :text="t('actions.import')" @click="open" />
    <TarModal centered :close="t('actions.close')" fade fullscreen ref="modal" :title="t('languages.import.lead')">
      <LoadingSpinner v-if="isLoading" />
      <template v-else>
        <template v-if="compendium.length">
          <p class="text-body-secondary">{{ t("languages.import.help") }}</p>
          <div class="row">
            <div v-for="language in compendium" :key="language.id" class="col-md-6 col-lg-4 col-xl-3 mb-3">
              <LanguageCard class="d-flex flex-column h-100" clickable :language="language" :selected="selected.has(language.id)" @click="toggle(language)">
                <div class="d-flex justify-content-between mt-2">
                  <div>
                    <font-awesome-icon :icon="selected.has(language.id) ? 'fas fa-square-xmark' : 'far fa-square'" />
                  </div>
                  <div>
                    <ImportStatusDisplay :status="getStatus(language)" />
                  </div>
                </div>
              </LanguageCard>
            </div>
          </div>
        </template>
        <p v-else>{{ t("languages.import.empty") }}</p>
      </template>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="onCancel" />
        <TarButton
          :disabled="!selected.size || isLoading"
          icon="fas fa-download"
          :loading="isLoading"
          :status="t('loading')"
          :text="t('actions.import')"
          @click="onImport"
        />
      </template>
    </TarModal>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import ImportStatusDisplay from "@/components/shared/ImportStatusDisplay.vue";
import LanguageCard from "./LanguageCard.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarModal from "@/components/tar/TarModal.vue";
import type { CreateOrReplaceLanguagePayload, Language, SearchLanguagesPayload } from "@/types/languages";
import type { CreateOrReplaceScriptPayload, Script } from "@/types/scripts";
import type { ImportStatus } from "@/types/import";
import type { SearchResults } from "@/types/search";
import { getCompendiumLanguages } from "@/api/compendium";
import { replaceLanguage, searchLanguages } from "@/api/languages";
import { replaceScript } from "@/api/scripts";

const { t } = useI18n();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "imported"): void;
}>();

const compendium = ref<Language[]>([]);
const isLoading = ref<boolean>(false);
const languages = ref<Map<string, Language>>(new Map());
const modal = ref<InstanceType<typeof TarModal> | null>(null);
const selected = ref<Map<string, Language>>(new Map());

function getStatus(language: Language): ImportStatus {
  const existing: Language | undefined = languages.value.get(language.id);
  if (!existing) {
    return "NotImported";
  }
  return compare(language, existing) ? "UpToDate" : "Outdated";
}
function compare(left: Language, right: Language): boolean {
  return (
    left.name === right.name &&
    (left.summary ?? "") === (right.summary ?? "") &&
    (left.content ?? "") === (right.content ?? "") &&
    (left.script?.id ?? "") === (right.script?.id ?? "") &&
    (left.typicalSpeakers ?? "") === (right.typicalSpeakers ?? "")
  );
}

function toggle(language: Language): void {
  if (selected.value.has(language.id)) {
    selected.value.delete(language.id);
  } else {
    selected.value.set(language.id, language);
  }
}

async function refresh(): Promise<void> {
  compendium.value = [];
  languages.value.clear();
  selected.value.clear();

  let results: SearchResults<Language> = await getCompendiumLanguages();
  compendium.value = [...results.items];

  const payload: SearchLanguagesPayload = {
    ids: [],
    search: { terms: [], operator: "And" },
    sort: [],
    skip: 0,
    limit: 0,
  };
  results = await searchLanguages(payload);
  results.items.forEach((language) => languages.value.set(language.id, language));
}

async function open(): Promise<void> {
  modal.value?.show();
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      await refresh();
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

function onCancel(): void {
  modal.value?.hide();
}

async function importScript(script: Script): Promise<void> {
  const payload: CreateOrReplaceScriptPayload = {
    name: script.name,
    summary: script.summary,
    content: script.content,
  };
  await replaceScript(script.id, payload);
}

async function importLanguage(language: Language): Promise<void> {
  if (language.script) {
    // TODO(fpion): only import if not exist, even if outdated
    await importScript(language.script);
  }

  const payload: CreateOrReplaceLanguagePayload = {
    name: language.name,
    summary: language.summary,
    content: language.content,
    scriptId: language.script?.id,
    typicalSpeakers: language.typicalSpeakers,
  };
  await replaceLanguage(language.id, payload);
}

async function onImport(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      for (const [id, language] of [...selected.value]) {
        await importLanguage(language);
        selected.value.delete(id);
      }
      emit("imported");
      await refresh();
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

// TODO(fpion): could we only select Languages that are not up-to-date?
// TODO(fpion): success toast
// TODO(fpion): progress bar instead of LoadingSpinner when importing
// TODO(fpion): (un)select all buttons
</script>
