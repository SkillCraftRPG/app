<template>
  <TarBadge pill variant="secondary">
    <font-awesome-icon icon="fas fa-graduation-cap" aria-hidden="true" />&nbsp;{{ t("characters.skills.trained") }}
  </TarBadge>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import TarBadge from "@/components/tar/TarBadge.vue";

const { t } = useI18n();
</script>
