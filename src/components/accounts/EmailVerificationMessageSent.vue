<template>
  <div class="row flex-grow-1 text-center align-items-center mx-0">
    <div class="col jumbotron mb-0">
      <h1 class="display-4">{{ t("account.email.verificationMessageSent.title") }}</h1>
      <p class="lead">{{ t("account.email.verificationMessageSent.help") }}</p>
      <hr class="my-4" />
      <p>
        {{ t("account.email.verificationMessageSent.reference") }}&nbsp;<strong>{{ id }}</strong>
      </p>
      <p class="lead">
        <TarButton v-if="!goToHome" icon="fas fa-xmark" size="large" :text="t('actions.close')" @click="close" />
        <RouterLink v-else :to="{ name: 'Home' }" class="btn btn-primary btn-lg" role="button">
          <font-awesome-icon icon="fas fa-home" /> {{ t("home.go") }}
        </RouterLink>
      </p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import TarButton from "@/components/tar/TarButton.vue";

const { t } = useI18n();

defineProps<{
  id: string;
}>();

const goToHome = ref<boolean>(false);

function close(): void {
  window.close();
  window.setTimeout(() => (goToHome.value = true), 1000); // If the browser blocked close(), the page is still here; fall back to Home.
}
</script>
