<template>
  <main class="container">
    <EmailVerificationMessageSent v-if="response.emailVerificationMessageId" :id="response.emailVerificationMessageId" />
    <SignInForm v-else @error="handleError" @submitted="response = $event" />
  </main>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watchEffect } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import EmailVerificationMessageSent from "@/components/accounts/EmailVerificationMessageSent.vue";
import SignInForm from "@/components/accounts/SignInForm.vue";
import type { SignInAccountRequest, SignInAccountResponse } from "@/types/account";
import { signIn } from "@/api/account";
import { useAccountStore } from "@/stores/account";
import { useDocument } from "@/composables/document";

const account = useAccountStore();
const document = useDocument();
const route = useRoute();
const router = useRouter();
const { t } = useI18n();

const response = ref<SignInAccountResponse>({ allowedFlows: [] });

const title = computed<string>(() => t("account.signIn.title"));

function handleError(e: unknown): void {
  console.error(e); // TODO(fpion): implement
}

function handleResponse(res: SignInAccountResponse) {
  if (res.currentUser) {
    account.signIn(res.currentUser);
    const redirect: string = (Array.isArray(route.query.redirect) ? route.query.redirect[0] : route.query.redirect) ?? "";
    router.push(redirect || { name: "Home" });
  } else if (res.profileCompletionToken) {
    router.push({ name: "Profile", query: { token: res.profileCompletionToken } });
  } else {
    response.value = res;
  }
}

watchEffect(() => document.setTitle(title.value));

onMounted(async () => {
  const token: string = (Array.isArray(route.query.token) ? route.query.token[0] : route.query.token) ?? "";
  if (token) {
    try {
      const request: SignInAccountRequest = {
        authenticationToken: token,
      };
      const response: SignInAccountResponse = await signIn(request);
      handleResponse(response);
    } catch (e: unknown) {
      handleError(e);
    }
  }
});
</script>
