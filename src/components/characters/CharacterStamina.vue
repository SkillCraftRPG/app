<template>
  <TarCard class="text-center">
    <div class="card-text d-flex justify-content-between align-items-center gap-2">
      <div class="fw-semibold">{{ label }}</div>
      <div class="text-primary">{{ n(character.stamina, "integer") }} / {{ n(total, "integer") }}</div>
    </div>
    <div class="card-text d-flex justify-content-between align-items-center gap-2">
      <div class="fw-semibold">{{ t("game.rest.short") }}</div>
      <div>+{{ n(regeneration, "integer") }}</div>
    </div>
    <TarProgress :aria-label="label" class="mt-1" :value="value" variant="primary" />
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

const label = computed<string>(() => t("game.statistic.options.Stamina"));
const total = computed<number>(() => props.character.statistics.stamina.total);
const regeneration = computed<number>(() => Math.round(total.value / 10));
const value = computed<number>(() => Math.floor((props.character.stamina * 100) / total.value));
</script>
