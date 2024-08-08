<script setup lang="ts">
import { inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AuthenticationLinkSent from "@/components/users/AuthenticationLinkSent.vue";
import SignInForm from "@/components/users/SignInForm.vue";
import type { SignInPayload, SignInResponse } from "@/types/account";
import { handleErrorKey } from "@/inject/App";
import { signIn } from "@/api/account";
import { useAccountStore } from "@/stores/account";

const account = useAccountStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { locale } = useI18n();

const hasLoaded = ref<boolean>(false);
const response = ref<SignInResponse>();

function onResponse(res: SignInResponse): void {
  if (res.currentUser) {
    account.signIn(res.currentUser);
    const redirect: string | undefined = route.query.redirect?.toString();
    router.push(redirect ?? { name: "Home" });
  } else if (res.profileCompletionToken) {
    router.push({ name: "ProfileCompletion", params: { token: res.profileCompletionToken } });
  } else {
    response.value = res;
  }
}

onMounted(async () => {
  try {
    const token: string | undefined = route.query.token?.toString();
    if (token) {
      const payload: SignInPayload = {
        locale: locale.value,
        authenticationToken: token,
      };
      const response: SignInResponse = await signIn(payload);
      onResponse(response);
    }
  } catch (e: unknown) {
    handleError(e);
    router.push({ name: "SignIn" });
  } finally {
    hasLoaded.value = true;
  }
});
</script>

<template>
  <main class="container">
    <template v-if="hasLoaded">
      <SignInForm v-if="!response || response.isPasswordRequired" :response="response" @error="handleError" @response="onResponse" />
      <AuthenticationLinkSent v-else-if="response.authenticationLinkSentTo" :sent-message="response.authenticationLinkSentTo" />
      <!-- ISSUE: https://github.com/SkillCraftRPG/app/issues/21 -->
    </template>
  </main>
</template>
