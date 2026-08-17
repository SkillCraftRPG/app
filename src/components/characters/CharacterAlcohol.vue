<template>
  <TarCard class="text-center">
    <div class="card-text d-flex justify-content-between align-items-center gap-2">
      <div class="fw-semibold">{{ label }}</div>
      <div class="text-alcohol">{{ n(character.bloodAlcoholContent, "integer") }} / {{ n(total, "integer") }}</div>
    </div>
    <TarProgress :aria-label="label" class="progress-alcohol mt-1" :value="value" />
  </TarCard>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import TarCard from "@/components/tar/TarCard.vue";
import TarProgress from "@/components/tar/TarProgress.vue";
import type { Character } from "@/types/characters";

const { n, t } = useI18n();

const props = defineProps<{
  character: Character;
}>();

const label = computed<string>(() => t("characters.bloodAlcoholContent"));
const total = computed<number>(() => props.character.attributes.health.total + 5);
const value = computed<number>(() => Math.floor((props.character.bloodAlcoholContent * 100) / total.value));
</script>

<style scoped>
.progress-alcohol {
  --bs-progress-bar-bg: var(--bs-orange);
}

.text-alcohol {
  color: var(--bs-orange);
}
</style>
