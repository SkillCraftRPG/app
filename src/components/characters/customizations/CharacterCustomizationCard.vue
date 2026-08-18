<template>
  <div class="card">
    <button
      type="button"
      class="clickable card-body btn text-body text-center d-flex flex-column align-items-center justify-content-center pe-5"
      @click="$emit('click')"
    >
      <div class="fw-semibold">{{ customization.name }}</div>
      <div class="text-body-secondary">
        <CustomizationKindDisplay :kind="customization.kind" />
      </div>
    </button>
    <div v-if="!isReadOnly" class="position-absolute top-0 end-0 m-2" @click.stop>
      <button type="button" class="btn btn-sm" @click="$emit('remove')">
        <font-awesome-icon icon="fas fa-xmark" aria-hidden="true" />
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";

import CustomizationKindDisplay from "@/components/customizations/CustomizationKindDisplay.vue";
import type { Customization } from "@/types/customizations";

const { parseBoolean } = parsingUtils;

const props = defineProps<{
  customization: Customization;
  readonly?: boolean | string;
}>();

defineEmits<{
  (e: "click"): void;
  (e: "remove"): void;
}>();

const isReadOnly = computed<boolean>(() => parseBoolean(props.readonly) ?? false);
</script>
