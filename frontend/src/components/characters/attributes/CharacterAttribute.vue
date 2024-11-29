<script setup lang="ts">
import { TarCard } from "logitar-vue3-ui";

import type { AttributeDetail } from "@/types/characters";

defineProps<{
  attribute: AttributeDetail;
}>();
</script>

<template>
  <TarCard class="clickable" :title="attribute.label">
    <span>
      <strong>{{ attribute.score }}</strong>
    </span>
    <span class="float-end">
      {{ attribute.modifier > 0 ? `+${attribute.modifier}` : attribute.modifier }}
    </span>
  </TarCard>
</template>

<style scoped>
.clickable:hover {
  background-color: var(--bs-secondary-bg);
  cursor: pointer;
}
</style>
