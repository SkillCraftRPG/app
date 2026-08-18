<template>
  <div>
    <TarCard class="clickable text-center" @click="open">
      <div class="fw-semibold">{{ language.name }}</div>
      <div v-if="script" class="text-body-secondary"><font-awesome-icon icon="fas fa-scroll" aria-hidden="true" />&nbsp;{{ script.name }}</div>
    </TarCard>
    <TarModal centered :close="t('actions.close')" fade ref="modal" size="large" :title="title">
      <div v-if="script" class="row text-center mb-3">
        <div class="col">
          <TarCard class="clickable" :class="{ 'border-primary bg-primary-subtle': selected === 'language' }" @click="selected = 'language'">
            <div class="fw-semibold"><font-awesome-icon icon="fas fa-language" aria-hidden="true" />&nbsp;{{ language.name }}</div>
          </TarCard>
        </div>
        <div class="col">
          <TarCard class="clickable" :class="{ 'border-primary bg-primary-subtle': selected === 'script' }" @click="selected = 'script'">
            <div class="fw-semibold"><font-awesome-icon icon="fas fa-scroll" aria-hidden="true" />&nbsp;{{ script.name }}</div>
          </TarCard>
        </div>
      </div>
      <template v-if="selected === 'language'">
        <div v-if="language.summary" class="fst-italic text-body-secondary mb-3">{{ language.summary }}</div>
        <MarkdownContent v-if="language.content" class="mb-3" :text="language.content"></MarkdownContent>
        <template v-if="language.typicalSpeakers">
          <div class="fs-5 mb-1">{{ t("languages.typicalSpeakers") }}</div>
          <MarkdownContent class="mb-3" :text="language.typicalSpeakers"></MarkdownContent>
        </template>
      </template>
      <template v-else-if="script">
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
const selected = ref<"language" | "script">("language");

const script = computed<Script | undefined>(() => props.language.script ?? undefined);
const title = computed<string>(() => {
  if (selected.value === "script" && script.value) {
    return t("scripts.format", { name: script.value.name });
  }
  return t("languages.format", { name: props.language.name });
});

function close(): void {
  modal.value?.hide();
}

function open(): void {
  modal.value?.show();
}
</script>
