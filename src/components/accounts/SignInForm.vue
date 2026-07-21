<template>
  <div>
    <h1>{{ title }}</h1>
    <form @submit.prevent="submit">
      <EmailAddressInput required v-model="emailAddress" />
      <TarButton :disabled="isLoading" icon="fas fa-user" :loading="isLoading" :text="t('account.signIn.submit')" type="submit" />
    </form>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";

import EmailAddressInput from "./EmailAddressInput.vue";
import TarButton from "@/components/tar/TarButton.vue";
import type { SignInAccountRequest, SignInAccountResponse } from "@/types/account.ts";
import { signIn } from "@/api/account.ts";
import { useAccountStore } from "@/stores/account.ts";

const account = useAccountStore();
const router = useRouter();
const { locale, t } = useI18n();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "submitted", value: SignInAccountResponse): void;
}>();

const emailAddress = ref<string>("");
const isLoading = ref<boolean>(false);
const password = ref<string>("");
const showPassword = ref<boolean>(false);
const showPasswordless = ref<boolean>(false);

const title = computed<string>(() => t("account.signIn.title"));

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
      if (response.currentUser) {
        account.signIn(response.currentUser);
        router.push({ name: "Home" }); // TODO(fpion): redirect
      } else if (response.profileCompletionToken) {
        console.log(response.profileCompletionToken); // TODO(fpion): redirect
      } else {
        response.allowedFlows.forEach((flow) => {
          switch (flow) {
            case "Password":
              showPassword.value = true;
              break;
            case "Passwordless":
              showPasswordless.value = true;
              break;
          }
        });
        emit("submitted", response);
      }
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}
</script>
