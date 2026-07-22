<template>
  <div>
    <TarProgress class="mb-3" :value="progress" />
    <h2 class="h3">{{ subtitle }}</h2>
    <p>{{ help }}</p>
    <form @submit.prevent="submit">
      <ProfileStepPersonal v-if="step === Step.Personal" v-model="personal" />
      <ProfileStepSecurity v-else-if="step === Step.Security" v-model="security" />
      <ProfileStepPreferences v-else-if="step === Step.Preferences" v-model="preferences" />
      <div class="d-flex justify-content-between">
        <div class="d-flex gap-2">
          <TarButton icon="fas fa-xmark" outline :text="t('actions.abort')" type="button" variant="danger" />
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
          <TarButton icon="fas fa-check" :outline="step !== Step.Experience" :text="t('actions.complete')" type="submit" />
          <TarButton v-if="step !== Step.Experience" icon="fas fa-arrow-right" :text="t('actions.next')" type="submit" />
        </div>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import ProfileStepPersonal from "./ProfileStepPersonal.vue";
import ProfileStepPreferences from "./ProfileStepPreferences.vue";
import ProfileStepSecurity from "./ProfileStepSecurity.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarProgress from "@/components/tar/TarProgress.vue";
import type { PersonalInformation, PreferencesInformation, SecurityInformation } from "@/types/account";

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

const personal = ref<PersonalInformation>({ firstName: "", lastName: "" });
const preferences = ref<PreferencesInformation>({
  locale: locale.value,
  timeZone: Intl.DateTimeFormat().resolvedOptions().timeZone,
});
const security = ref<SecurityInformation>({ mode: "PasswordLess", password: "" });
const step = ref<Step>(Step.Personal);

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

function previous(): void {
  if (step.value !== Step.Personal) {
    step.value--;
  }
}

async function submit(): Promise<void> {
  // TODO(fpion): we cannot submit when password rules do not succeed!
  if (step.value !== Step.Experience) {
    step.value++;
  }
  // TODO(fpion): implement
}

watch(locale, (newValue, oldValue) => {
  if (!oldValue || preferences.value.locale === oldValue) {
    preferences.value.locale = newValue;
  }
});
</script>
