<template>
  <TarCard class="mb-3">
    <div class="fw-semibold">{{ attribute }}</div>
    <div class="fs-5">{{ value }}</div>
  </TarCard>
</template>

<script setup lang="ts">
import TarCard from "@/components/tar/TarCard.vue";

defineProps<{
  attribute: string;
  value: number;
}>();
</script>
