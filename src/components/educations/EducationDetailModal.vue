<template>
  <TarModal centered :close="t('actions.close')" fade ref="modal" size="large" :title="title">
    <div v-if="education.skill || education.wealthMultiplier" class="row text-center mb-3">
      <div class="col">
        <template v-if="education.skill">
          <div class="small text-body-secondary">{{ t("game.skill.label") }}</div>
          <div><font-awesome-icon icon="fas fa-kitchen-set" aria-hidden="true" />&nbsp;{{ t(`game.skill.options.${education.skill}`) }}</div>
        </template>
      </div>
      <div v-if="education.wealthMultiplier" class="col">
        <div class="small text-body-secondary">{{ t("educations.wealthMultiplier") }}</div>
        <div><font-awesome-icon icon="fas fa-coins" aria-hidden="true" />&nbsp;×{{ n(education.wealthMultiplier, "integer") }}</div>
      </div>
    </div>
    <div v-if="education.summary" class="fst-italic text-body-secondary mb-3">{{ education.summary }}</div>
    <MarkdownContent v-if="education.content" class="mb-3" :text="education.content"></MarkdownContent>
    <template v-if="education.feature">
      <div class="fs-5 mb-1">{{ t("game.feature.format", { name: education.feature.name }) }}</div>
      <MarkdownContent v-if="education.feature.content" class="mb-3" :text="education.feature.content"></MarkdownContent>
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
import type { Education } from "@/types/educations";

const { n, t } = useI18n();

const props = defineProps<{
  education: Education;
}>();

const modal = ref<InstanceType<typeof TarModal> | null>(null);

const title = computed<string>(() => t("educations.format", { name: props.education.name }));

function close(): void {
  modal.value?.hide();
}

function open(): void {
  modal.value?.show();
}
defineExpose({ open });
</script>
