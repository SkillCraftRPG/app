<template>
  <EducationCard :clickable="isClickable" :education="education" :selected="selected" :selection="selection">
    <div class="mt-2 text-end">
      <ImportStatusDisplay :status="status" />
    </div>
  </EducationCard>
</template>

<script setup lang="ts">
import { computed } from "vue";

import EducationCard from "@/components/educations/EducationCard.vue";
import ImportStatusDisplay from "@/components/import/ImportStatusDisplay.vue";
import type { Education } from "@/types/educations";
import type { ImportStatus } from "@/types/import";
import type { SelectionKind } from "@/types/shared";

const props = defineProps<{
  education: Education;
  selected: boolean | string;
  status: ImportStatus;
}>();

const isClickable = computed<boolean>(() => props.status !== "UpToDate");
const selection = computed<SelectionKind | undefined>(() => (isClickable.value ? "multiple" : undefined));
</script>
