<template>
  <div>
    <TarProgress class="mb-3" :value="progress" />
    <h2 class="h3">{{ subtitle }}</h2>
    <p>{{ help }}</p>
    <form @submit.prevent="submit">
      <ProfileStepPersonal v-if="step === Step.Personal" v-model="personal" />
      <ProfileStepSecurity v-else-if="step === Step.Security" v-model="security" />
      <ProfileStepPreferences v-else-if="step === Step.Preferences" v-model="preferences" />
      <ProfileStepExperience v-else-if="step === Step.Experience" v-model="experience" />
      <div class="d-flex justify-content-between">
        <div class="d-flex gap-2">
          <TarButton icon="fas fa-xmark" outline :text="t('actions.abandon')" type="button" variant="danger" @click="openAbandon" />
          <TarButton
            v-if="step !== Step.Personal"
            icon="fas fa-arrow-left"
            outline
            :text="t('actions.previous')"
            type="button"
            variant="secondary"
            @click="previous"
          />
        </div>
        <div class="d-flex gap-2">
          <TarButton icon="fas fa-check" id="complete" :outline="step !== Step.Experience" :text="t('actions.complete')" type="submit" />
          <TarButton v-if="step !== Step.Experience" icon="fas fa-arrow-right" id="next" :text="t('actions.next')" type="submit" />
        </div>
      </div>
    </form>
    <ProfileAbandonModal ref="abandonModal" @abandon="abandon" />
    <ProfileCompleteModal ref="completeModal" @complete="complete" />
  </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";

import ProfileAbandonModal from "./ProfileAbandonModal.vue";
import ProfileCompleteModal from "./ProfileCompleteModal.vue";
import ProfileStepExperience from "./ProfileStepExperience.vue";
import ProfileStepPersonal from "./ProfileStepPersonal.vue";
import ProfileStepPreferences from "./ProfileStepPreferences.vue";
import ProfileStepSecurity from "./ProfileStepSecurity.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarProgress from "@/components/tar/TarProgress.vue";
import type { PersonalInformation, PreferencesInformation, SecurityInformation, UserExperience } from "@/types/account";
import { useForm } from "@/forms";

const router = useRouter();
const { locale, t } = useI18n();

enum Step {
  Personal = 0,
  Security = 1,
  Preferences = 2,
  Experience = 3,
}

defineProps<{
  token: string;
}>();

const abandonModal = ref<InstanceType<typeof ProfileAbandonModal> | null>(null);
const completeModal = ref<InstanceType<typeof ProfileCompleteModal> | null>(null);
const experience = ref<UserExperience>("Player");
const personal = ref<PersonalInformation>({ firstName: "", lastName: "" });
const preferences = ref<PreferencesInformation>({
  locale: locale.value,
  timeZone: Intl.DateTimeFormat().resolvedOptions().timeZone,
});
const security = ref<SecurityInformation>({ mode: "PasswordLess", password: "" });
const step = ref<Step>(Step.Personal);
const submitter = ref<string>("");

function getStepKey(): string | undefined {
  switch (step.value) {
    case Step.Experience:
      return "experience";
    case Step.Personal:
      return "personal";
    case Step.Preferences:
      return "preferences";
    case Step.Security:
      return "security";
  }
}

const help = computed<string>(() => {
  const key: string | undefined = getStepKey();
  return key ? t(`account.profile.completion.${key}.help`) : "";
});
const subtitle = computed<string>(() => {
  const key: string | undefined = getStepKey();
  return key ? t(`account.profile.completion.${key}.lead`) : "";
});
const progress = computed<number>(() => Math.floor((step.value * 100) / 3));

function abandon(): void {
  router.push({ name: "SignIn" });
}

async function complete(): Promise<void> {
  alert("submitting…"); // TODO(fpion): implement
}

function openAbandon(): void {
  abandonModal.value?.open();
}

function previous(): void {
  if (step.value !== Step.Personal) {
    step.value--;
  }
}

const { handleSubmit } = useForm();
function submit(e: SubmitEvent): void {
  submitter.value = e.submitter?.id ?? "";
  handleSubmit(() => {
    switch (submitter.value) {
      case "complete":
        if (step.value === Step.Experience) {
          complete();
        } else {
          completeModal.value?.open();
        }
        break;
      case "next":
        if (step.value !== Step.Experience) {
          step.value++;
        }
        break;
    }
  });
}

watch(locale, (newValue, oldValue) => {
  if (!oldValue || preferences.value.locale === oldValue) {
    preferences.value.locale = newValue;
  }
});
</script>
