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
      <template v-if="hasSpeeds">
        <div class="fs-5 mb-1">{{ t("lineages.physical.speeds.lead") }}</div>
        <div class="row text-center">
          <template v-for="kind in SPEED_KINDS" :key="kind">
            <div v-if="target.speeds[camelCase(kind)]" class="col-4 col-md-fifth mb-3">
              <div class="fw-bold">{{ t(kind === "Fly" && target.speeds.hover ? "game.speed.hover" : `game.speed.kind.options.${kind}`) }}</div>
              <div>{{ n(target.speeds[camelCase(kind)], "integer") }}</div>
            </div>
          </template>
        </div>
      </template>
      <template v-if="features.length">
        <template v-for="(feature, index) in features" :key="index">
          <div class="fs-5 mb-1">{{ t("game.feature.format", { name: feature.name }) }}</div>
          <MarkdownContent v-if="feature.content" class="mb-3" :text="feature.content"></MarkdownContent>
        </template>
      </template>
      <template #footer>
        <TarButton icon="fas fa-xmark" :text="t('actions.close')" variant="secondary" @click="close" />
      </template>
    </TarModal>
  </div>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import MarkdownContent from "@/components/shared/MarkdownContent.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarCard from "@/components/tar/TarCard.vue";
import TarModal from "@/components/tar/TarModal.vue";
import type { Feature } from "@/types/features";
import type { Lineage } from "@/types/lineages";
import { SPEED_KINDS, camelCase } from "@/utils/game";

const { orderBy } = arrayUtils;
const { n, t } = useI18n();

const props = defineProps<{
  lineage: Lineage;
}>();

const modal = ref<InstanceType<typeof TarModal> | null>(null);
const selected = ref<"ethnicity" | "species">("species");

const ethnicity = computed<Lineage | undefined>(() => (props.lineage.parent ? props.lineage : undefined));
const species = computed<Lineage>(() => props.lineage.parent ?? props.lineage);
const target = computed<Lineage>(() => (selected.value === "ethnicity" && ethnicity.value ? ethnicity.value : species.value));
const features = computed<Feature[]>(() => orderBy(target.value.features, "name"));
const hasSpeeds = computed<boolean>(() => SPEED_KINDS.some((kind) => Boolean(target.value.speeds[camelCase(kind)])));

function close(): void {
  modal.value?.hide();
}

function open(): void {
  modal.value?.show();
}
</script>
