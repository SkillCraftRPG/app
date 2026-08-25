<template>
  <CustomizationCard :clickable="isClickable" :customization="customization" :selected="selected" :selection="selection">
    <div class="mt-2 text-end">
      <ImportStatusDisplay :status="status" />
    </div>
  </CustomizationCard>
</template>

<script setup lang="ts">
import { computed } from "vue";

import CustomizationCard from "@/components/customizations/CustomizationCard.vue";
import ImportStatusDisplay from "@/components/import/ImportStatusDisplay.vue";
import type { Customization } from "@/types/customizations";
import type { ImportStatus } from "@/types/import";
import type { SelectionKind } from "@/types/shared";

const props = defineProps<{
  customization: Customization;
  selected: boolean | string;
  status: ImportStatus;
}>();

const isClickable = computed<boolean>(() => props.status !== "UpToDate");
const selection = computed<SelectionKind | undefined>(() => (isClickable.value ? "multiple" : undefined));
</script>
