<template>
  <div>
    <div class="mb-3">
      <div>Full Name: {{ modelValue.fullName }}</div>
      <div>Created on: {{ d(modelValue.createdOn, "medium") }}</div>
      <div>Updated on: {{ d(modelValue.updatedOn, "medium") }}</div>
    </div>
    <EmailDisplay class="mb-3" :email="email" />
    <div class="mb-3">
      <div v-if="modelValue.passwordChangedOn">{{ d(modelValue.passwordChangedOn, "medium") }}</div>
      <div>MfaMode: {{ modelValue.multiFactorAuthenticationMode }}</div>
    </div>
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
    </form>
    <div class="mb-3">{{ modelValue.defaultExperience }}</div>
    <div class="mb-3">
      <div v-if="modelValue.authenticatedOn">{{ d(modelValue.authenticatedOn, "medium") }}</div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import DateOfBirthInput from "./DateOfBirthInput.vue";
import EmailDisplay from "./EmailDisplay.vue";
import FirstNameInput from "./FirstNameInput.vue";
import GenderRadio from "./GenderRadio.vue";
import LastNameInput from "./LastNameInput.vue";
import LocaleRadio from "./LocaleRadio.vue";
import TimeZoneSelect from "./TimeZoneSelect.vue";
import type { Email, Profile } from "@/types/account";
import { computed } from "vue";
import { useForm } from "@/forms";

const { d } = useI18n();

const props = defineProps<{
  modelValue: Profile;
}>();

defineEmits<{
  (e: "error", value: unknown): void;
  (e: "update:model-value", value: Profile): void;
}>();

const email = computed<Email>(() => ({ address: props.modelValue.emailAddress, isVerified: true }));

const { handleSubmit } = useForm();
function submit(): void {
  console.log("submitting…");
}
</script>
