<script setup lang="ts">
import { TarProgress } from "logitar-vue3-ui";
import { computed, inject, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import ProfileIdentification from "@/components/users/ProfileIdentification.vue";
import ProfilePersonal from "@/components/users/ProfilePersonal.vue";
import ProfileSecurity from "@/components/users/ProfileSecurity.vue";
import ProfileUsage from "@/components/users/ProfileUsage.vue";
import type {
  CompleteProfilePayload,
  Identification,
  JwtPayload,
  PersonalInformation,
  SecurityInformation,
  SignInPayload,
  SignInResponse,
  UserType,
} from "@/types/account";
import { decode } from "@/helpers/jwtUtils";
import { handleErrorKey } from "@/inject/App";
import { signIn } from "@/api/account";
import { useAccountStore } from "@/stores/account";

const account = useAccountStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { locale, t } = useI18n();

type Step = "Identification" | "Personal" | "Security" | "Usage";

const isSubmitting = ref<boolean>(false);
const profile = ref<CompleteProfilePayload>({
  token: "",
  multiFactorAuthenticationMode: "None",
  firstName: "",
  lastName: "",
  locale: "",
  timeZone: "",
  userType: "Player",
});
const step = ref<Step>("Identification");

const emailAddress = computed<string>(() => {
  const payload = decode(token.value) as JwtPayload;
  return payload.email ?? "";
});
const progress = computed<number>(() => {
  if (isSubmitting.value) {
    return 100.0;
  }
  switch (step.value) {
    case "Personal":
      return 50;
    case "Security":
      return 25;
    case "Usage":
      return 75;
    default:
      return 0;
  }
});
const token = computed<string>(() => route.params.token.toString());

function onIdentification(value?: Identification): void {
  if (value) {
    profile.value.firstName = value.firstName;
    profile.value.middleName = value.middleName || undefined;
    profile.value.lastName = value.lastName;
    step.value = "Security";
  } else {
    router.push({ name: "SignIn" });
  }
}
function onPersonal(value?: PersonalInformation): void {
  if (value) {
    profile.value.birthdate = value.birthdate || undefined;
    profile.value.gender = value.gender || undefined;
    profile.value.locale = value.locale;
    profile.value.timeZone = value.timeZone;
    step.value = "Usage";
  } else {
    step.value = "Security";
  }
}
function onSecurity(value?: SecurityInformation): void {
  if (value) {
    profile.value.password = value.password || undefined;
    profile.value.multiFactorAuthenticationMode = value.multiFactorAuthenticationMode;
    step.value = "Personal";
  } else {
    step.value = "Identification";
  }
}
async function onUsage(value?: UserType): Promise<void> {
  if (value) {
    profile.value.userType = value;
    try {
      isSubmitting.value = true;
      profile.value.token = token.value;
      const payload: SignInPayload = {
        locale: locale.value,
        profile: profile.value,
      };
      const response: SignInResponse = await signIn(payload);
      if (response.currentUser) {
        account.signIn(response.currentUser);
        router.push({ name: "Home" });
      } else {
        throw new Error(`The sign-in response is not valid: ${JSON.stringify(response)}`);
      }
    } catch (e: unknown) {
      handleError(e);
    } finally {
      isSubmitting.value = false;
    }
  } else {
    step.value = "Personal";
  }
}
</script>

<template>
  <main class="container">
    <h1>{{ t("users.profile.complete") }}</h1>
    <TarProgress :aria-label="t('users.profile.progress')" :value="progress" />
    <ProfileIdentification v-show="step === 'Identification'" @back="onIdentification()" @continue="onIdentification" />
    <ProfileSecurity :email-address="emailAddress" v-show="step === 'Security'" @back="onSecurity()" @continue="onSecurity" />
    <ProfilePersonal v-show="step === 'Personal'" @back="onPersonal()" @continue="onPersonal" />
    <ProfileUsage :is-loading="isSubmitting" v-show="step === 'Usage'" @back="onUsage()" @continue="onUsage" />
  </main>
</template>
