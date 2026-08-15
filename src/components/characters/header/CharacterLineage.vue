<template>
  <div>
    <TarCard class="clickable text-center" @click="open">
      <div class="small text-body-secondary">{{ t("lineages.label") }}</div>
      <div class="fw-semibold">{{ species.name }}</div>
      <div v-if="ethnicity" class="text-body-secondary">{{ ethnicity.name }}</div>
    </TarCard>
    <TarModal centered :close="t('actions.close')" fade ref="modal" size="large" :title="t('lineages.label')">
      <div v-if="ethnicity" class="row text-center mb-3">
        <div class="col">
          <TarCard class="clickable" :class="{ 'border-primary bg-primary-subtle': selected === 'species' }" @click="selected = 'species'">
            <div class="fw-semibold">{{ species.name }}</div>
          </TarCard>
        </div>
        <div class="col">
          <TarCard class="clickable" :class="{ 'border-primary bg-primary-subtle': selected === 'ethnicity' }" @click="selected = 'ethnicity'">
            <div class="fw-semibold">{{ ethnicity.name }}</div>
          </TarCard>
        </div>
      </div>
      <div v-if="target.summary" class="fst-italic text-body-secondary mb-3">{{ target.summary }}</div>
      <MarkdownContent v-if="target.content" class="mb-3" :text="target.content"></MarkdownContent>
      <!-- TODO(fpion): Features -->
      <!-- TODO(fpion): Speeds -->
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
import type { Lineage } from "@/types/lineages";

const { t } = useI18n();

const props = defineProps<{
  lineage: Lineage;
}>();

const modal = ref<InstanceType<typeof TarModal> | null>(null);
const selected = ref<"ethnicity" | "species">("species");

const ethnicity = computed<Lineage | undefined>(() => (props.lineage.parent ? props.lineage : undefined));
const species = computed<Lineage>(() => props.lineage.parent ?? props.lineage);
const target = computed<Lineage>(() => (selected.value === "ethnicity" && ethnicity.value ? ethnicity.value : species.value));

function close(): void {
  modal.value?.hide();
}

function open(): void {
  modal.value?.show();
}
</script>
