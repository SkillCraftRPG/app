<template>
  <fieldset>
    <legend class="visually-hidden">{{ t("account.profile.experience.lead") }}</legend>
    <div class="row g-3">
      <div v-for="option in options" :key="option.value" class="col-12 col-md-6">
        <input
          :id="`experience-${option.value}`"
          :checked="modelValue === option.value"
          autocomplete="off"
          class="btn-check"
          name="defaultExperience"
          required
          type="radio"
          :value="option.value"
          @change="$emit('update:model-value', option.value)"
        />
        <label
          class="experience-card card h-100 text-center"
          :class="{ 'border-primary bg-primary-subtle': modelValue === option.value }"
          :for="`experience-${option.value}`"
        >
          <div class="card-body p-4">
            <font-awesome-icon :icon="option.icon" class="fs-1 mb-3" aria-hidden="true" />
            <h3 class="h5">{{ t(option.label) }}</h3>
            <p class="text-body-secondary mb-0">{{ t(option.help) }}</p>
          </div>
        </label>
      </div>
    </div>
    <slot name="after"></slot>
  </fieldset>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import type { UserExperience } from "@/types/account";

const { t } = useI18n();

defineProps<{
  modelValue: UserExperience;
}>();

defineEmits<{
  (e: "update:model-value", value: UserExperience): void;
}>();

type Option = {
  value: UserExperience;
  label: string;
  help: string;
  icon: string;
};
const options: Option[] = [
  {
    value: "Player",
    label: "account.profile.experience.options.Player.label",
    help: "account.profile.experience.options.Player.help",
    icon: "fas fa-dice",
  },
  {
    value: "Gamemaster",
    label: "account.profile.experience.options.Gamemaster.label",
    help: "account.profile.experience.options.Gamemaster.help",
    icon: "fas fa-hat-wizard",
  },
];
</script>

<style scoped>
.experience-card {
  cursor: pointer;
  transition:
    border-color 150ms ease,
    background-color 150ms ease,
    box-shadow 150ms ease;
}

.experience-card:hover {
  border-color: var(--bs-primary);
}

.btn-check:focus-visible + .experience-card {
  box-shadow: 0 0 0 0.25rem rgba(var(--bs-primary-rgb), 0.25);
}
</style>
