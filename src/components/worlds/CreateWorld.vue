<template>
  <div>
    <TarButton icon="fas fa-plus" size="large" :text="t('actions.create')" variant="success" />
  </div>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import TarButton from "@/components/tar/TarButton.vue";

const { t } = useI18n();
</script>
