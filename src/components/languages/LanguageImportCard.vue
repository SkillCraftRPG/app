<template>
  <LanguageCard :clickable="isClickable" :language="language" :selected="selected">
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
import LanguageCard from "@/components/languages/LanguageCard.vue";
import type { ImportStatus } from "@/types/import";
import type { Language } from "@/types/languages";

const { parseBoolean } = parsingUtils;

const props = defineProps<{
  language: Language;
  selected: boolean | string;
  status: ImportStatus;
}>();

const isClickable = computed<boolean>(() => props.status !== "UpToDate");
const icon = computed<string | undefined>(() => {
  if (isClickable.value) {
    return parseBoolean(props.selected) ? "far fa-square-check" : "far fa-square";
  }
});
</script>
