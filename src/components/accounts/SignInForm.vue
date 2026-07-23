<template>
  <div class="row flex-grow-1 align-items-center mx-0">
    <div class="col jumbotron mb-0">
      <div class="text-center">
        <h1 class="display-4">{{ title }}</h1>
        <p class="lead">{{ t("account.signIn.help") }}</p>
      </div>
      <hr class="my-4" />
      <div class="row justify-content-center">
        <form class="col-md-6" @submit.prevent="submit">
          <EmailAddressInput class="mb-3" required v-model="emailAddress" />
          <div v-if="isPasswordFlowAllowed" class="mb-3">🚧 Password Input…</div>
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
import TarButton from "@/components/tar/TarButton.vue";
import type { AuthenticationFlow, SignInAccountRequest, SignInAccountResponse } from "@/types/account.ts";
import { ErrorCodes, StatusCodes, type ApiFailure, type ProblemDetails } from "@/types/api.ts";
import { signIn } from "@/api/account.ts";

const { locale, t } = useI18n();

const props = defineProps<{
  flows: AuthenticationFlow[];
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "submitted", value: SignInAccountResponse): void;
}>();

const emailAddress = ref<string>("");
const isLoading = ref<boolean>(false);
const password = ref<string>("");

const isPasswordFlowAllowed = computed<boolean>(() => props.flows.includes("Password"));
const isPasswordLessFlowAllowed = computed<boolean>(() => props.flows.includes("Passwordless"));
const title = computed<string>(() => t("account.signIn.welcome"));

async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
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
          // TODO(fpion): invalidCredentials.value = true;
          // TODO(fpion): password.value = "";
          // TODO(fpion): passwordRef.value?.focus();
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
