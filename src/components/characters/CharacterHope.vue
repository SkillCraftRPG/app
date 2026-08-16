<template>
  <TarCard class="text-center">
    <div class="card-text d-flex justify-content-between align-items-center gap-2">
      <div class="fw-semibold">{{ label }}</div>
      <div class="text-success">{{ n(character.hope, "integer") }} / {{ n(total, "integer") }}</div>
    </div>
    <TarProgress :aria-label="label" class="mt-1" :value="value" variant="success" />
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

const label = computed<string>(() => t("game.hope"));
const total = computed<number>(() => 3); // TODO(fpion): implement
const value = computed<number>(() => Math.floor((props.character.hope * 100) / total.value));
</script>
