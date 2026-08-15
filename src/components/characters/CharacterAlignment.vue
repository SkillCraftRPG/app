<template>
  <div class="card">
    <div
      class="clickable"
      :class="{ 'card-body': !isOpened, 'card-header': isOpened }"
      data-bs-toggle="collapse"
      data-bs-target="#alignment-collapse"
      :aria-expanded="isOpened"
      aria-controls="alignment-collapse"
      @click="isOpened = !isOpened"
    >
      <div class="d-flex justify-content-between align-items-center gap-2">
        <div>
          <div class="small text-body-secondary">{{ t("characters.alignment.label") }}</div>
          <div>{{ label }}</div>
        </div>
        <font-awesome-icon class="fs-4" :icon="icon" />
      </div>
    </div>
    <div id="alignment-collapse" class="card-body collapse">
      <AlignmentRadio class="mb-3" :model-value="modelValue" @update:model-value="$emit('update:model-value', $event)" />
      <div>{{ tendency }}</div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import AlignmentRadio from "./AlignmentRadio.vue";
import type { Alignment } from "@/types/characters";

const { t } = useI18n();

const props = defineProps<{
  modelValue: Alignment | null;
}>();

defineEmits<{
  (e: "update:model-value", value: Alignment | null): void;
}>();

const isOpened = ref<boolean>(false);

const icon = computed<string>(() => `fas fa-chevron-${isOpened.value ? "up" : "down"}`);
const label = computed<string>(() => t(`characters.alignment.options.${props.modelValue ?? "Unaligned"}`));
const tendency = computed<string>(() => t(`characters.alignment.tendencies.${props.modelValue ?? "Unaligned"}`));

// TODO(fpion): weird transition on close
</script>
