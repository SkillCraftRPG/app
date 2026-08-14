<template>
  <div>
    <TarCard class="clickable text-center" @click="open">
      <div class="small text-body-secondary">{{ t("castes.label") }}</div>
      <div class="fw-semibold">{{ caste.name }}</div>
      <div v-if="caste.feature" class="text-body-secondary">{{ caste.feature.name }}</div>
    </TarCard>
    <TarModal centered :close="t('actions.close')" fade ref="modal" size="large" :title="t('castes.format', { name: caste.name })">
      <div v-if="caste.skill || caste.wealthRoll" class="row text-center mb-3">
        <div class="col">
          <template v-if="caste.skill">
            <div class="small text-body-secondary">{{ t("game.skill.label") }}</div>
            <div><font-awesome-icon icon="fas fa-kitchen-set" aria-hidden="true" />&nbsp;{{ t(`game.skill.options.${caste.skill}`) }}</div>
          </template>
        </div>
        <div v-if="caste.wealthRoll" class="col">
          <div class="small text-body-secondary">{{ t("castes.wealthRoll") }}</div>
          <div><font-awesome-icon icon="fas fa-coins" aria-hidden="true" />&nbsp;{{ caste.wealthRoll }}</div>
        </div>
      </div>
      <div v-if="caste.summary" class="fst-italic text-body-secondary mb-3">{{ caste.summary }}</div>
      <MarkdownContent v-if="caste.content" class="mb-3" :text="caste.content"></MarkdownContent>
      <template v-if="caste.feature">
        <p class="fw-bold">{{ t("game.feature.format", { name: caste.feature.name }) }}</p>
        <MarkdownContent v-if="caste.feature.content" class="mb-3" :text="caste.feature.content"></MarkdownContent>
      </template>
      <template #footer>
        <TarButton icon="fas fa-xmark" :text="t('actions.close')" variant="secondary" @click="close" />
      </template>
    </TarModal>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import MarkdownContent from "@/components/shared/MarkdownContent.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarCard from "@/components/tar/TarCard.vue";
import TarModal from "@/components/tar/TarModal.vue";
import type { Caste } from "@/types/castes";

const { t } = useI18n();

defineProps<{
  caste: Caste;
}>();

const modal = ref<InstanceType<typeof TarModal> | null>(null);

function close(): void {
  modal.value?.hide();
}

function open(): void {
  modal.value?.show();
}
</script>
