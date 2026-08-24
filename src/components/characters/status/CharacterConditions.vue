<template>
  <TarCard>
    <div class="card-text fw-semibold">{{ t("characters.conditions") }}</div>
  </TarCard>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import TarCard from "@/components/tar/TarCard.vue";
import type { Character } from "@/types/characters";

const { t } = useI18n();

defineProps<{
  character: Character;
}>();
</script>
