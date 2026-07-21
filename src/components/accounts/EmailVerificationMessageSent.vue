<template>
  <div class="row vh-100 text-center align-items-center">
    <div class="col jumbotron">
      <h1 class="display-4">{{ t("account.signIn.emailVerificationMessageSent.title") }}</h1>
      <p class="lead">{{ t("account.signIn.emailVerificationMessageSent.help") }}</p>
      <hr class="my-4" />
      <p>{{ t("account.signIn.emailVerificationMessageSent.reference") }}</p>
      <p class="lead">
        <RouterLink :to="{ name: 'Home' }" class="btn btn-primary btn-lg" role="button">
          <font-awesome-icon icon="fas fa-home" /> {{ t("notFound.link") }}
        </RouterLink>
      </p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

const { t } = useI18n();

defineProps<{
  id: string;
}>();
</script>
