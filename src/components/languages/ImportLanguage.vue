<template>
  <LanguageCard :clickable="isClickable" :language="language" :selected="selected" @click="onClick">
    <div class="d-flex justify-content-between mt-2">
      <div>
        <font-awesome-icon v-if="icon" :icon="icon" />
      </div>
      <div>
        <ImportStatusDisplay :status="status" />
      </div>
    </div>
  </LanguageCard>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";

import ImportStatusDisplay from "@/components/import/ImportStatusDisplay.vue";
import LanguageCard from "./LanguageCard.vue";
import type { ImportStatus } from "@/types/import";
import type { Language } from "@/types/languages";

const { parseBoolean } = parsingUtils;

const props = defineProps<{
  existing?: Language | null;
  language: Language;
  selected?: boolean | string;
}>();

const emit = defineEmits<{
  (e: "toggle"): void;
}>();

function compare(left: Language, right: Language): boolean {
  return (
    left.name === right.name &&
    (left.summary ?? "") === (right.summary ?? "") &&
    (left.content ?? "") === (right.content ?? "") &&
    (left.script?.id ?? "") === (right.script?.id ?? "") &&
    (left.typicalSpeakers ?? "") === (right.typicalSpeakers ?? "")
  );
}

const status = computed<ImportStatus>(() => {
  if (!props.existing) {
    return "NotImported";
  }
  return compare(props.existing, props.language) ? "UpToDate" : "Outdated";
});
const isClickable = computed<boolean>(() => status.value !== "UpToDate");
const icon = computed<string | undefined>(() => {
  if (isClickable.value) {
    return parseBoolean(props.selected) ? "far fa-square-check" : "far fa-square";
  }
});

function onClick(): void {
  if (isClickable.value) {
    emit("toggle");
  }
}
</script>
