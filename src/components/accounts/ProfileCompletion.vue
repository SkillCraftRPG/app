<template>
  <div>
    <TarProgress class="mb-3" :value="progress" />
    <h2 class="h3">{{ subtitle }}</h2>
    <p>{{ help }}</p>
    <form @submit.prevent="submit">
      <ProfileStepPersonal v-if="step === Step.Personal" :email="email" v-model="personal" />
      <ProfileStepSecurity v-else-if="step === Step.Security" v-model="security" />
      <ProfileStepPreferences v-else-if="step === Step.Preferences" v-model="preferences" />
      <DefaultExperienceRadio v-else-if="step === Step.Experience" class="mb-3" v-model="experience">
        <template #after>
          <div class="form-text mt-3">{{ t("account.profile.completion.experience.note") }}</div>
        </template>
      </DefaultExperienceRadio>
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
          <TarButton
            :disabled="isLoading"
            icon="fas fa-check"
            id="complete"
            :loading="isLoading"
            :outline="step !== Step.Experience"
            :text="t('actions.complete')"
            type="submit"
          />
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
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";

import DefaultExperienceRadio from "./DefaultExperienceRadio.vue";
import ProfileAbandonModal from "./ProfileAbandonModal.vue";
import ProfileCompleteModal from "./ProfileCompleteModal.vue";
import ProfileStepPersonal from "./ProfileStepPersonal.vue";
import ProfileStepPreferences from "./ProfileStepPreferences.vue";
import ProfileStepSecurity from "./ProfileStepSecurity.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarProgress from "@/components/tar/TarProgress.vue";
import type {
  Email,
  PersonalInformation,
  PreferencesInformation,
  SecurityInformation,
  SignInAccountRequest,
  SignInAccountResponse,
  TokenPayload,
  UserExperience,
} from "@/types/account";
import { signIn } from "@/api/account";
import { useAccountStore } from "@/stores/account";
import { useForm } from "@/forms";

const account = useAccountStore();
const router = useRouter();
const { locale, t } = useI18n();
const { parseBoolean } = parsingUtils;

enum Step {
  Personal = 0,
  Security = 1,
  Preferences = 2,
  Experience = 3,
}

const props = defineProps<{
  token: string;
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
}>();

const abandonModal = ref<InstanceType<typeof ProfileAbandonModal> | null>(null);
const completeModal = ref<InstanceType<typeof ProfileCompleteModal> | null>(null);
const experience = ref<UserExperience>("Player");
const isLoading = ref<boolean>(false);
const personal = ref<PersonalInformation>({ firstName: "", lastName: "" });
const preferences = ref<PreferencesInformation>({
  locale: locale.value,
  timeZone: Intl.DateTimeFormat().resolvedOptions().timeZone,
});
const security = ref<SecurityInformation>({ mode: "PasswordLess", password: "" });
const step = ref<Step>(Step.Personal);
const submitter = ref<string>("");

const email = computed<Email | undefined>(() => {
  const payload: TokenPayload = JSON.parse(atob(props.token.split(".")[1]!));
  if (payload.email) {
    return {
      address: payload.email,
      isVerified: parseBoolean(payload.email_verified ?? undefined) ?? false,
    };
  }
});

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
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const request: SignInAccountRequest = {
        profile: {
          token: props.token,
          password: security.value.mode === "PasswordLess" ? null : security.value.password,
          multiFactorAuthenticationMode: security.value.mode === "MultiFactor" ? "Email" : "None",
          firstName: personal.value.firstName,
          lastName: personal.value.lastName,
          dateOfBirth: preferences.value.dateOfBirth,
          gender: preferences.value.gender,
          locale: preferences.value.locale,
          timeZone: preferences.value.timeZone,
          defaultExperience: experience.value,
        },
      };
      const response: SignInAccountResponse = await signIn(request);
      if (response.currentUser) {
        account.signIn(response.currentUser);
      }
      router.push({ name: "Profile" });
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
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
