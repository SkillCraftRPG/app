<template>
  <main class="container">
    <h1>{{ title }}</h1>
    <ProfileCompletion v-if="token" :token="token" @error="handleError" />
    <ProfileContainer v-if="profile" v-model="profile" @error="handleError" />
  </main>
</template>

<script setup lang="ts">
import { computed, inject, onMounted, ref, watchEffect } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute } from "vue-router";

import ProfileCompletion from "@/components/accounts/ProfileCompletion.vue";
import ProfileContainer from "@/components/accounts/ProfileContainer.vue";
import type { Profile } from "@/types/account";
import { getProfile } from "@/api/account";
import { handleErrorKey } from "@/inject";
import { useDocument } from "@/composables/document";

const document = useDocument();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const { t } = useI18n();

const profile = ref<Profile>();
const token = ref<string>("");

const title = computed<string>(() => {
  // TODO(fpion): unify both titles
  if (token.value) {
    return "account.profile.completion.title";
  } else if (profile.value) {
    return profile.value.fullName;
  }
  return t("account.profile.title");
});

watchEffect(() => document.setTitle(title.value));

onMounted(async () => {
  const completionToken: string = (Array.isArray(route.query.token) ? route.query.token[0] : route.query.token) ?? "";
  if (completionToken) {
    token.value = completionToken;
    return;
  }

  try {
    profile.value = await getProfile();
  } catch (e: unknown) {
    handleError(e);
  }
});
</script>
