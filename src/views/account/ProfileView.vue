<template>
  <main class="container">
    <h1>{{ title }}</h1>
  </main>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watchEffect } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import { useAccountStore } from "@/stores/account";
import { useDocument } from "@/composables/document";

const account = useAccountStore();
const document = useDocument();
const route = useRoute();
const router = useRouter();
const { t } = useI18n();

const profileCompletionToken = ref<string>("");

const title = computed<string>(() => "ProfileView"); // TODO(fpion): implement

watchEffect(() => document.setTitle(title.value));

onMounted(async () => {
  const token: string = (Array.isArray(route.query.token) ? route.query.token[0] : route.query.token) ?? "";
  if (token) {
    profileCompletionToken.value = token;
  } else if (account.currentUser) {
    // TODO(fpion): retrieve profile from back-end
  } else {
    router.push({ name: "SignIn", query: { redirect: route.fullPath } });
  }
});
</script>
