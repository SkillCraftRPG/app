<script setup lang="ts">
import type { NatureModel } from "@/types/natures";

import NatureIcon from "./NatureIcon.vue";

defineProps<{
  nature: NatureModel;
}>();
</script>

<template>
  <span>
    <RouterLink :to="{ name: 'NatureEdit', params: { id: nature.id } }" target="_blank"><NatureIcon /></RouterLink>
    {{ " " }}
    <RouterLink :to="{ name: 'NatureEdit', params: { id: nature.id } }" target="_blank">{{ nature.name }}</RouterLink>
  </span>
</template>
