<template>
  <main class="container">
    <h1>Characters</h1>
    <RouterLink :to="{ name: 'Worlds' }"><font-awesome-icon icon="fas fa-dungeon" />&nbsp;{{ t("worlds.go") }}</RouterLink>
  </main>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

const { t } = useI18n();
</script>
