<template>
  <div>
    <TarCard class="clickable text-center" @click="open">
      <div class="fw-semibold">{{ language.name }}</div>
      <div v-if="script" class="text-body-secondary"><font-awesome-icon icon="fas fa-scroll" aria-hidden="true" />&nbsp;{{ script.name }}</div>
    </TarCard>
    <TarModal centered :close="t('actions.close')" fade ref="modal" size="large" :title="t('languages.format', { name: language.name })">
      <div v-if="language.summary" class="fst-italic text-body-secondary mb-3">{{ language.summary }}</div>
      <MarkdownContent v-if="language.content" class="mb-3" :text="language.content"></MarkdownContent>
      <template v-if="language.typicalSpeakers">
        <div class="fs-5 mb-1">{{ t("languages.typicalSpeakers") }}</div>
        <MarkdownContent class="mb-3" :text="language.typicalSpeakers"></MarkdownContent>
      </template>
      <template v-if="script">
        <div class="fs-5 mb-1">{{ t("scripts.format", { name: script.name }) }}</div>
        <div v-if="script.summary" class="fst-italic text-body-secondary mb-3">{{ script.summary }}</div>
        <MarkdownContent v-if="script.content" class="mb-3" :text="script.content"></MarkdownContent>
      </template>
      <template #footer>
        <TarButton icon="fas fa-xmark" :text="t('actions.close')" variant="secondary" @click="close" />
      </template>
    </TarModal>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import MarkdownContent from "@/components/shared/MarkdownContent.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarCard from "@/components/tar/TarCard.vue";
import TarModal from "@/components/tar/TarModal.vue";
import type { Language } from "@/types/languages";
import type { Script } from "@/types/scripts";

const { t } = useI18n();

const props = defineProps<{
  language: Language;
}>();

const modal = ref<InstanceType<typeof TarModal> | null>(null);

function close(): void {
  modal.value?.hide();
}

function open(): void {
  modal.value?.show();
}

const script = computed<Script | undefined>(() => props.language.script ?? undefined);
</script>
