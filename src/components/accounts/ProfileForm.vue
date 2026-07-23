<template>
  <div>
    <h2 class="h3">{{ t("account.profile.title") }}</h2>
    <form @submit.prevent="handleSubmit(submit)">
      <div class="row">
        <div class="col-md-6">
          <FirstNameInput class="mb-3" :model-value="modelValue.firstName" required />
        </div>
        <div class="col-md-6">
          <LastNameInput class="mb-3" :model-value="modelValue.lastName" required />
        </div>
      </div>
      <div class="row">
        <div class="col-md-6">
          <DateOfBirthInput class="mb-3" :model-value="modelValue.dateOfBirth ? new Date(modelValue.dateOfBirth) : undefined" />
        </div>
        <div class="col-md-6">
          <GenderRadio class="mb-3" :model-value="modelValue.gender" />
        </div>
      </div>
      <div class="row">
        <div class="col-md-6">
          <LocaleRadio class="mb-3" :model-value="modelValue.locale.code" required />
        </div>
        <div class="col-md-6">
          <TimeZoneSelect class="mb-3" :model-value="modelValue.timeZone" required />
        </div>
      </div>
      <div class="mb-3">
        <div class="mb-3">
          <div class="fw-bold">{{ t("account.profile.experience.lead") }}</div>
          <div>{{ t("account.profile.experience.help") }}</div>
        </div>
        <DefaultExperienceRadio :model-value="modelValue.defaultExperience" />
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import DateOfBirthInput from "./DateOfBirthInput.vue";
import DefaultExperienceRadio from "./DefaultExperienceRadio.vue";
import FirstNameInput from "./FirstNameInput.vue";
import GenderRadio from "./GenderRadio.vue";
import LastNameInput from "./LastNameInput.vue";
import LocaleRadio from "./LocaleRadio.vue";
import TimeZoneSelect from "./TimeZoneSelect.vue";
import type { Profile } from "@/types/account";
import { useForm } from "@/forms";

const { t } = useI18n();

defineProps<{
  modelValue: Profile;
}>();

defineEmits<{
  (e: "error", value: unknown): void;
  (e: "update:model-value", value: Profile): void;
}>();

const { handleSubmit } = useForm();
function submit(): void {
  console.log("submitting…");
}
</script>
