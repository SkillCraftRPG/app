<template>
  <div class="row flex-grow-1 align-items-center mx-0">
    <div class="col jumbotron mb-0">
      <div class="text-center">
        <h1 class="display-4">{{ title }}</h1>
        <p class="lead">{{ t("account.signIn.help") }}</p>
      </div>
      <hr class="my-4" />
      <div class="row justify-content-center">
        <form class="col-md-6" @submit.prevent="handleSubmit(submit)">
          <SessionClosed v-model="sessionClosed" />
          <SessionExpired v-model="sessionExpired" />
          <InvalidCredentials v-model="invalidCredentials" />
          <EmailAddressInput class="mb-3" required v-model="emailAddress" />
          <PasswordInput v-if="isPasswordFlowAllowed" class="mb-3" ref="passwordInput" required v-model="password" />
          <TarButton :disabled="isLoading" icon="fas fa-user" :loading="isLoading" size="large" :text="t('account.signIn.submit')" type="submit" />
        </form>
      </div>
      <div v-if="isPasswordLessFlowAllowed" class="row justify-content-center">
        <div class="col-md-6">
          <hr />
          <p>{{ t("account.signIn.passwordLess.help") }}</p>
          <TarButton
            v-if="isPasswordLessFlowAllowed"
            class="w-100"
            :disabled="isLoading"
            icon="fas fa-envelope"
            :loading="isLoading"
            outline
            size="large"
            :text="t('account.signIn.passwordLess.lead')"
            @click="usePasswordLessFlow"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import EmailAddressInput from "./EmailAddressInput.vue";
import InvalidCredentials from "./InvalidCredentials.vue";
import PasswordInput from "./PasswordInput.vue";
import SessionClosed from "./SessionClosed.vue";
import SessionExpired from "./SessionExpired.vue";
import TarButton from "@/components/tar/TarButton.vue";
import type { AuthenticationFlow, SignInAccountRequest, SignInAccountResponse, SignOutEvent } from "@/types/account";
import { ErrorCodes, StatusCodes, type ApiFailure, type ProblemDetails } from "@/types/api";
import { signIn } from "@/api/account";
import { useAccountStore } from "@/stores/account";
import { useForm } from "@/forms";

const account = useAccountStore();
const { locale, t } = useI18n();

const props = defineProps<{
  flows: AuthenticationFlow[];
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "submitted", value: SignInAccountResponse): void;
}>();

const emailAddress = ref<string>("");
const invalidCredentials = ref<boolean>(false);
const isLoading = ref<boolean>(false);
const password = ref<string>("");
const passwordInput = ref<InstanceType<typeof PasswordInput> | null>(null);

const signOutEvent: SignOutEvent | undefined = account.consumeSignOutEvent();
const sessionClosed = ref<boolean>(signOutEvent === "closed");
const sessionExpired = ref<boolean>(signOutEvent === "expired");

const isPasswordFlowAllowed = computed<boolean>(() => props.flows.includes("Password"));
const isPasswordLessFlowAllowed = computed<boolean>(() => props.flows.includes("Passwordless"));
const title = computed<string>(() => t("account.signIn.welcome"));

const { handleSubmit } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    invalidCredentials.value = false;
    sessionClosed.value = false;
    sessionExpired.value = false;
    try {
      const request: SignInAccountRequest = {
        credentials: {
          locale: locale.value,
          emailAddress: emailAddress.value,
          password: password.value || null,
          usePasswordless: false,
        },
      };
      const response: SignInAccountResponse = await signIn(request);
      emit("submitted", response);
    } catch (e: unknown) {
      const failure = e as ApiFailure;
      if (failure.status === StatusCodes.BadRequest) {
        const problemDetails = failure.data as ProblemDetails;
        if (problemDetails.error && problemDetails.error.code === ErrorCodes.InvalidCredentials) {
          invalidCredentials.value = true;
          password.value = "";
          passwordInput.value?.focus();
          return;
        }
      }
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

async function usePasswordLessFlow(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const request: SignInAccountRequest = {
        credentials: {
          locale: locale.value,
          emailAddress: emailAddress.value,
          usePasswordless: true,
        },
      };
      const response: SignInAccountResponse = await signIn(request);
      emit("submitted", response);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}
</script>
