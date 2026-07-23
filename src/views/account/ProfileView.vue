<template>
  <main class="container">
    <h1>{{ title }}</h1>
    <ProfileCompletion v-if="token" :token="token" @error="handleError" />
    <ProfileContainer v-if="profile" v-model="profile" @error="handleError" />
  </main>
</template>

<script setup lang="ts">
import { inject, onMounted, ref } from "vue";
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
const title = ref<string>("");
const token = ref<string>("");

onMounted(async () => {
  const completionToken: string = (Array.isArray(route.query.token) ? route.query.token[0] : route.query.token) ?? "";
  if (completionToken) {
    token.value = completionToken;
    title.value = t("account.profile.setup.title");
    document.setTitle(title.value);
    return;
  }

  try {
    profile.value = await getProfile();
    title.value = profile.value.fullName;
    document.setTitle(title.value);
  } catch (e: unknown) {
    handleError(e);
  }
});
</script>
