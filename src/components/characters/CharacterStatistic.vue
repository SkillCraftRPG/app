<template>
  <TarCard class="mb-3">
    <div class="small text-body-secondary">{{ attribute }}</div>
    <div class="fw-semibold">{{ statistic }}</div>
    <div class="fs-5">{{ value }}</div>
  </TarCard>
</template>

<script setup lang="ts">
import TarCard from "@/components/tar/TarCard.vue";

defineProps<{
  attribute: string;
  statistic: string;
  value: number;
}>();
</script>
