<template>
  <div>
    <h2 class="h3">{{ t("account.profile.title") }}</h2>
    <form @submit.prevent="handleSubmit(submit)">
      <div class="row">
        <div class="col-md-6">
          <FirstNameInput class="mb-3" required v-model="firstName" />
        </div>
        <div class="col-md-6">
          <LastNameInput class="mb-3" required v-model="lastName" />
        </div>
      </div>
      <div class="row">
        <div class="col-md-6">
          <DateOfBirthInput class="mb-3" v-model="dateOfBirth" />
        </div>
        <div class="col-md-6">
          <GenderRadio class="mb-3" v-model="gender" />
        </div>
      </div>
      <div class="row">
        <div class="col-md-6">
          <LocaleRadio class="mb-3" required v-model="locale" />
        </div>
        <div class="col-md-6">
          <TimeZoneSelect class="mb-3" required v-model="timeZone" />
        </div>
      </div>
      <div class="mb-3">
        <div class="mb-3">
          <div class="fw-bold">{{ t("account.profile.experience.label") }}</div>
          <div>{{ t("account.profile.experience.help") }}</div>
        </div>
        <DefaultExperienceRadio v-model="defaultExperience" />
      </div>
      <div class="mb-3">
        <TarButton :disabled="!hasChanges || isLoading" icon="fas fa-floppy-disk" :loading="isLoading" :text="t('actions.save')" type="submit" />
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import DateOfBirthInput from "./DateOfBirthInput.vue";
import DefaultExperienceRadio from "./DefaultExperienceRadio.vue";
import FirstNameInput from "./FirstNameInput.vue";
import GenderRadio from "./GenderRadio.vue";
import LastNameInput from "./LastNameInput.vue";
import LocaleRadio from "./LocaleRadio.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TimeZoneSelect from "./TimeZoneSelect.vue";
import type { Gender, Profile, UpdateProfilePayload, UserExperience } from "@/types/account";
import { saveProfile } from "@/api/account";
import { useAccountStore } from "@/stores/account";
import { useForm } from "@/forms";
import { useToastStore } from "@/stores/toast";

const account = useAccountStore();
const toasts = useToastStore();
const { t } = useI18n();

const props = defineProps<{
  modelValue: Profile;
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "update:model-value", value: Profile): void;
}>();

const dateOfBirth = ref<Date>();
const defaultExperience = ref<UserExperience>("Player");
const firstName = ref<string>("");
const gender = ref<Gender>();
const isLoading = ref<boolean>(false);
const lastName = ref<string>("");
const locale = ref<string>("");
const timeZone = ref<string>("");

const hasDateOfBirthChanged = computed<boolean>(() => {
  const profile: string | null = props.modelValue.dateOfBirth ? props.modelValue.dateOfBirth.split("T")[0]! : null;
  const input: string | null = dateOfBirth.value?.toISOString().split("T")[0]! ?? null;
  return profile !== input;
});
const hasChanges = computed<boolean>(() => {
  const profile: Profile = props.modelValue;
  return (
    profile.firstName !== firstName.value ||
    profile.lastName !== lastName.value ||
    hasDateOfBirthChanged.value ||
    profile.gender !== gender.value ||
    profile.locale.code !== locale.value ||
    profile.timeZone !== timeZone.value ||
    profile.defaultExperience !== defaultExperience.value
  );
});

const { handleSubmit } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const payload: UpdateProfilePayload = {
        firstName: firstName.value,
        lastName: lastName.value,
        dateOfBirth: { value: dateOfBirth.value },
        gender: { value: gender.value },
        locale: locale.value,
        timeZone: timeZone.value,
        defaultExperience: defaultExperience.value,
      };
      const profile: Profile = await saveProfile(payload);
      account.saveProfile(profile);
      toasts.success("account.profile.updated");
      emit("update:model-value", profile);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

watch(
  () => props.modelValue,
  (profile) => {
    dateOfBirth.value = profile.dateOfBirth ? new Date(profile.dateOfBirth) : undefined;
    defaultExperience.value = profile.defaultExperience;
    firstName.value = profile.firstName;
    gender.value = profile.gender;
    lastName.value = profile.lastName;
    locale.value = profile.locale.code;
    timeZone.value = profile.timeZone;
  },
  {
    deep: true,
    immediate: true,
  },
);
</script>
