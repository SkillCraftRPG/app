<template>
  <div class="h-100">
    <TarCard class="clickable h-100" @click="open">
      <div class="card-text d-flex justify-content-between align-items-center gap-2">
        <div class="fw-semibold">{{ label }}</div>
        <div class="text-danger">{{ n(character.vitality, "integer") }} / {{ n(total, "integer") }}</div>
      </div>
      <div class="card-text d-flex justify-content-between align-items-center gap-2">
        <div>{{ t("game.rest.long") }}</div>
        <div>+{{ n(regeneration, "integer") }}</div>
      </div>
      <TarProgress :aria-label="label" class="mt-1" :value="value" variant="danger" />
    </TarCard>
    <TarModal centered :close="t('actions.close')" fade ref="modal" size="large" :title="t('game.statistic.options.Vitality')">
      <template #footer>
        <TarButton icon="fas fa-xmark" :text="t('actions.close')" variant="secondary" @click="close" />
      </template>
    </TarModal>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import TarButton from "@/components/tar/TarButton.vue";
import TarCard from "@/components/tar/TarCard.vue";
import TarModal from "@/components/tar/TarModal.vue";
import TarProgress from "@/components/tar/TarProgress.vue";
import type { Character } from "@/types/characters";

const { n, t } = useI18n();

const props = defineProps<{
  character: Character;
}>();

const modal = ref<InstanceType<typeof TarModal> | null>(null);

const label = computed<string>(() => t("game.statistic.options.Vitality"));
const total = computed<number>(() => props.character.statistics.vitality.total);
const regeneration = computed<number>(() => Math.round(total.value / 7));
const value = computed<number>(() => Math.floor((props.character.vitality * 100) / total.value));

function close(): void {
  modal.value?.hide();
}

function open(): void {
  modal.value?.show();
}
</script>
