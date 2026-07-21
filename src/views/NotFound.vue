<template>
  <main class="container flex-grow-1 d-flex flex-column">
    <div class="row flex-grow-1 text-center align-items-center mx-0">
      <div class="col jumbotron mb-0">
        <h1 class="display-4">404</h1>
        <p class="lead">{{ title }}</p>
        <hr class="my-4" />
        <p>{{ t("notFound.help") }}</p>
        <p class="lead">
          <RouterLink :to="{ name: 'Home' }" class="btn btn-primary btn-lg" role="button">
            <font-awesome-icon icon="fas fa-home" /> {{ t("home.go") }}
          </RouterLink>
        </p>
      </div>
    </div>
  </main>
</template>

<script setup lang="ts">
import { computed, watchEffect } from "vue";
import { useI18n } from "vue-i18n";

import { useDocument } from "@/composables/document";

const document = useDocument();
const { t } = useI18n();

const title = computed<string>(() => t("notFound.lead"));

watchEffect(() => document.setTitle(title.value));
</script>
