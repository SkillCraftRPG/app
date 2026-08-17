<template>
  <TarCard>
    <div class="card-text d-flex justify-content-between align-items-center gap-2">
      <div>
        <div class="fw-semibold">{{ statistic }}</div>
        <div class="small text-body-secondary">{{ attribute }}</div>
      </div>
      <div class="fs-4">{{ n(value, "integer") }}</div>
    </div>
  </TarCard>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import TarCard from "@/components/tar/TarCard.vue";

const { n } = useI18n();

defineProps<{
  attribute: string;
  statistic: string;
  value: number;
}>();
</script>
