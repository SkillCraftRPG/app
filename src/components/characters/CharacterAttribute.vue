<template>
  <TarCard>
    <div class="card-text d-flex justify-content-between align-items-center gap-2">
      <div>
        <div class="fw-semibold">{{ attribute }}</div>
        <div class="small text-body-secondary">{{ category }}</div>
      </div>
      <div class="fs-4">{{ score }}</div>
    </div>
  </TarCard>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import TarCard from "@/components/tar/TarCard.vue";
import { formatSignedInteger } from "@/utils/format";

const { n } = useI18n();

const props = defineProps<{
  attribute: string;
  category: string;
  value: number;
}>();

const score = computed<string>(() => formatSignedInteger(props.value, (value) => n(value, "integer")));
</script>
