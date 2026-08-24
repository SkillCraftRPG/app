<template>
  <TarModal centered :close="t('actions.close')" fade scrollable ref="modal" size="large" :title="title">
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
      <div class="fs-5 mb-1">{{ t("game.feature.format", { name: caste.feature.name }) }}</div>
      <MarkdownContent v-if="caste.feature.content" class="mb-3" :text="caste.feature.content"></MarkdownContent>
    </template>
    <template #footer>
      <TarButton icon="fas fa-xmark" :text="t('actions.close')" variant="secondary" @click="close" />
    </template>
  </TarModal>
</template>

<script setup lang="ts">
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import MarkdownContent from "@/components/shared/MarkdownContent.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarModal from "@/components/tar/TarModal.vue";
import type { Caste } from "@/types/castes";

const { t } = useI18n();

const props = defineProps<{
  caste: Caste;
}>();

const modal = ref<InstanceType<typeof TarModal> | null>(null);

const title = computed<string>(() => t("castes.format", { name: props.caste.name }));

function close(): void {
  modal.value?.hide();
}

function open(): void {
  modal.value?.show();
}
defineExpose({ open });
</script>
