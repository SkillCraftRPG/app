<template>
  <CustomizationCard :clickable="isClickable" :customization="customization" :selected="selected">
    <div class="d-flex justify-content-between mt-2">
      <div>
        <font-awesome-icon v-if="icon" :icon="icon" />
      </div>
      <div>
        <ImportStatusDisplay :status="status" />
      </div>
    </div>
  </CustomizationCard>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";

import CustomizationCard from "@/components/customizations/CustomizationCard.vue";
import ImportStatusDisplay from "@/components/import/ImportStatusDisplay.vue";
import type { Customization } from "@/types/customizations";
import type { ImportStatus } from "@/types/import";

const { parseBoolean } = parsingUtils;

const props = defineProps<{
  customization: Customization;
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
