<template>
  <TarAlert show variant="danger">
    <strong>{{ t("account.multiFactorAuthentication.invalidCode.lead") }}</strong> {{ t("account.multiFactorAuthentication.invalidCode.help") }}
  </TarAlert>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import TarAlert from "@/components/tar/TarAlert.vue";

const { t } = useI18n();
</script>
