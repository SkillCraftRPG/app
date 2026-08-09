<template>
  <fieldset>
    <legend class="visually-hidden">{{ t("characters.alignment.label") }}</legend>
    <div class="row g-3">
      <div v-for="option in options" :key="option.value" class="col-4">
        <input
          :id="`alignment-${option.value}`"
          :checked="modelValue === option.value"
          autocomplete="off"
          class="btn-check"
          name="alignment"
          required
          type="radio"
          :value="option.value"
          @change="$emit('update:model-value', option.value)"
        />
        <label
          class="alignment-card card h-100 text-center"
          :class="{ 'border-primary bg-primary-subtle': modelValue === option.value }"
          :for="`alignment-${option.value}`"
        >
          <div class="card-body d-flex flex-column justify-content-center align-items-center gap-2 p-4">
            <font-awesome-icon :icon="option.icon" class="fs-1" aria-hidden="true" />
            <div class="fw-semibold alignment-label">{{ t(`characters.alignment.options.${option.value}`) }}</div>
          </div>
        </label>
      </div>
      <div class="col">
        <input
          id="unaligned"
          :checked="modelValue === null"
          autocomplete="off"
          class="btn-check"
          name="alignment"
          required
          type="radio"
          value="unaligned"
          @change="$emit('update:model-value', null)"
        />
        <label class="alignment-card card h-100 text-center" :class="{ 'border-primary bg-primary-subtle': modelValue === null }" for="unaligned">
          <div class="card-body d-flex flex-column justify-content-center align-items-center gap-2 p-3">
            <font-awesome-icon icon="fas fa-ban" class="fs-1" aria-hidden="true" />
            <div class="fw-semibold alignment-label">{{ t("characters.alignment.options.Unaligned") }}</div>
          </div>
        </label>
      </div>
    </div>
  </fieldset>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import type { Alignment } from "@/types/characters";

const { t } = useI18n();

defineProps<{
  modelValue?: Alignment | null;
}>();

defineEmits<{
  (e: "update:model-value", value: Alignment | null): void;
}>();

type Option = {
  value: Alignment;
  icon: string;
};
const options: Option[] = [
  {
    value: "LawfulGood",
    icon: "fas fa-shield-heart",
  },
  {
    value: "NeutralGood",
    icon: "fas fa-heart",
  },
  {
    value: "ChaoticGood",
    icon: "fas fa-dove",
  },
  {
    value: "LawfulNeutral",
    icon: "fas fa-scale-balanced",
  },
  {
    value: "TrueNeutral",
    icon: "fas fa-yin-yang",
  },
  {
    value: "ChaoticNeutral",
    icon: "fas fa-shuffle",
  },
  {
    value: "LawfulEvil",
    icon: "fas fa-crown",
  },
  {
    value: "NeutralEvil",
    icon: "fas fa-skull",
  },
  {
    value: "ChaoticEvil",
    icon: "fas fa-fire",
  },
];
</script>

<style scoped>
.alignment-card {
  cursor: pointer;
  transition:
    border-color 150ms ease,
    background-color 150ms ease,
    box-shadow 150ms ease;
}

.alignment-card:hover {
  border-color: var(--bs-primary);
}

.alignment-label {
  white-space: normal;
  word-break: normal;
  overflow-wrap: normal;
  hyphens: none;
}

.btn-check:focus-visible + .alignment-card {
  box-shadow: 0 0 0 0.25rem rgba(var(--bs-primary-rgb), 0.25);
}
</style>
