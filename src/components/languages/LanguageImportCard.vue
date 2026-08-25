<template>
  <LanguageCard :clickable="isClickable" :language="language" :selected="selected" :selection="selection">
    <div class="mt-2 text-end">
      <ImportStatusDisplay :status="status" />
    </div>
  </LanguageCard>
</template>

<script setup lang="ts">
import { computed } from "vue";

import ImportStatusDisplay from "@/components/import/ImportStatusDisplay.vue";
import LanguageCard from "@/components/languages/LanguageCard.vue";
import type { ImportStatus } from "@/types/import";
import type { Language } from "@/types/languages";
import type { SelectionKind } from "@/types/shared";

const props = defineProps<{
  language: Language;
  selected: boolean | string;
  status: ImportStatus;
}>();

const isClickable = computed<boolean>(() => props.status !== "UpToDate");
const selection = computed<SelectionKind | undefined>(() => (isClickable.value ? "multiple" : undefined));
</script>
