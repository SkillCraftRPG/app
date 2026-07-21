<template>
  <main class="container">
    <div class="row vh-100 text-center align-items-center">
      <div class="col jumbotron">
        <h1 class="display-4">404</h1>
        <p class="lead">{{ title }}</p>
        <hr class="my-4" />
        <p>{{ t("notFound.help") }}</p>
        <p class="lead">
          <RouterLink :to="{ name: 'Home' }" class="btn btn-primary btn-lg" role="button">
            <font-awesome-icon icon="fas fa-home" /> {{ t("notFound.link") }}
          </RouterLink>
        </p>
      </div>
    </div>
  </main>
</template>

<script setup lang="ts">
import { computed, onMounted } from "vue";
import { useI18n } from "vue-i18n";

import { useDocument } from "@/composables/document";

const document = useDocument();
const { t } = useI18n();

const title = computed<string>(() => t("notFound.lead"));

onMounted(() => document.setTitle(title.value));
</script>

<style scoped>
@media (min-width: 576px) {
  .jumbotron {
    padding: 4rem 2rem;
  }
}

.jumbotron {
  padding: 2rem 1rem;
  margin-bottom: 2rem;
  background-color: #e9ecef;
  border-radius: 0.3rem;
  text-align: center;
}
</style>
