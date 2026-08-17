<template>
  <TarCard class="text-center">
    <div class="d-flex justify-content-between align-items-center gap-2">
      <div class="fw-semibold">{{ t("characters.level") }}&nbsp;{{ n(character.level, "integer") }}</div>
      <div class="fw-semibold">{{ t("characters.tier") }}&nbsp;{{ n(character.tier, "integer") }}</div>
    </div>
    <div class="d-flex justify-content-between align-items-center gap-2">
      <div class="fw-semibold">{{ label }}</div>
      <div class="text-warning">{{ n(character.experience, "integer") }} / {{ n(total, "integer") }}</div>
    </div>
    <TarProgress :aria-label="label" class="mt-1" :value="value" variant="warning" />
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

const label = computed<string>(() => t("characters.experience"));
const total = computed<number>(() => 100); // TODO(fpion): implement
const value = computed<number>(() => Math.floor((props.character.experience * 100) / total.value));
</script>
