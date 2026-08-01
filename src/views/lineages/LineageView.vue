<template>
  <main class="container page">
    <div v-if="lineage">
      <h1>{{ title }}</h1>
      <WorldBreadcrumb :current="title" :parent="breadcrumb" />
      <TarAlert :close="t('actions.close')" dismissible variant="success" v-model="isCreated">
        <strong>{{ t("lineages.species.created.lead") }}</strong> {{ t("lineages.species.created.help", { name: title }) }}
      </TarAlert>
      <StatusDetail class="mb-3" :subject="lineage" />
      <TarTabs class="border-top border-secondary-subtle pt-4">
        <TarTab active id="content" :title="t('content')">
          <LineageGeneral :lineage="lineage" @error="handleError" @updated="onUpdate" />
        </TarTab>
        <TarTab disabled id="features" :title="t('game.feature.title')">
          <p>🚧</p>
        </TarTab>
        <TarTab id="languages" :title="t('languages.title')">
          <LineageLanguages :lineage="lineage" @error="handleError" @updated="onUpdate" />
        </TarTab>
        <TarTab id="names" :title="t('lineages.names.title')">
          <p>🚧</p>
        </TarTab>
        <TarTab id="physical" :title="t('lineages.physical.title')">
          <p>🚧</p>
        </TarTab>
        <TarTab id="ethnicities" :title="t('lineages.ethnicities.title')">
          <p>🚧</p>
        </TarTab>
      </TarTabs>
    </div>
    <LoadingSpinner v-else />
  </main>
</template>

<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import LineageGeneral from "@/components/lineages/LineageGeneral.vue";
import LineageLanguages from "@/components/lineages/LineageLanguages.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import TarTab from "@/components/tar/TarTab.vue";
import TarTabs from "@/components/tar/TarTabs.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import type { Lineage } from "@/types/lineages";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { handleErrorKey } from "@/inject";
import { readLineage } from "@/api/lineages";
import { useDocument } from "@/composables/document";
import { useEventStore } from "@/stores/event";
import { useToastStore } from "@/stores/toast";

const document = useDocument();
const events = useEventStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const isCreated = ref<boolean>(events.shift() === "created");
const lineage = ref<Lineage>();

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("lineages.species.title"), to: { name: "Lineages" } }));
const title = computed<string>(() => lineage.value?.name ?? "");

function onUpdate(value: Lineage): void {
  lineage.value = value;
  toasts.success("saved");
}

onMounted(async () => {
  try {
    const id: string = (Array.isArray(route.params.id) ? route.params.id[0] : route.params.id) ?? "";
    lineage.value = await readLineage(id);
    document.setTitle(title.value);
  } catch (e: unknown) {
    const failure = e as ApiFailure;
    if (failure.status === StatusCodes.NotFound) {
      router.push("/not-found");
    } else {
      handleError(e);
    }
  }
});
</script>
