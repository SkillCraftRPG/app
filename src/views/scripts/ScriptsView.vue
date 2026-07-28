<template>
  <main class="container">
    <h1>{{ title }}</h1>
  </main>
</template>

<script setup lang="ts">
import { computed, watchEffect } from "vue";
import { useI18n } from "vue-i18n";

import { useDocument } from "@/composables/document";

const document = useDocument();
const { t } = useI18n();

const title = computed<string>(() => t("scripts.title"));

watchEffect(() => document.setTitle(title.value));
</script>
