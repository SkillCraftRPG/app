<script setup lang="ts">
import { useI18n } from "vue-i18n";

const { t } = useI18n();
</script>

<template>
  <div>
    <h3>{{ t("characters.steps.attributes") }}</h3>
  </div>
</template>
