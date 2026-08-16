<template>
  <TarCard>
    <div class="card-text d-flex justify-content-between align-items-center gap-2">
      <div>
        <div class="fw-semibold">{{ attribute }}</div>
        <div class="small text-body-secondary">{{ category }}</div>
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
  category: string;
  value: number;
}>();
</script>
