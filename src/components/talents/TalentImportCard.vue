<template>
  <TalentCard :clickable="isClickable" :selected="selected" :selection="selection" :talent="talent">
    <div class="mt-2 text-end">
      <ImportStatusDisplay :status="status" />
    </div>
  </TalentCard>
</template>

<script setup lang="ts">
import { computed } from "vue";

import ImportStatusDisplay from "@/components/import/ImportStatusDisplay.vue";
import TalentCard from "@/components/talents/TalentCard.vue";
import type { ImportStatus } from "@/types/import";
import type { SelectionKind } from "@/types/shared";
import type { Talent } from "@/types/talents";

const props = defineProps<{
  selected: boolean | string;
  status: ImportStatus;
  talent: Talent;
}>();

const isClickable = computed<boolean>(() => props.status !== "UpToDate");
const selection = computed<SelectionKind | undefined>(() => (isClickable.value ? "multiple" : undefined));
</script>
