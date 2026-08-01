<template>
  <main class="container page">
    <div v-if="lineage">
      <h1>{{ title }}</h1>
      <WorldBreadcrumb :current="title" :parent="breadcrumb" />
      <TarAlert :close="t('actions.close')" dismissible variant="success" v-model="isCreated">
        <strong>{{ t(`lineages.${isEthnicity ? "ethnicities" : "species"}.created`) }}</strong> {{ t("lineages.created", { name: title }) }}
      </TarAlert>
      <StatusDetail class="mb-3" :subject="lineage" />
      <TarTabs :key="lineage.id" class="border-top border-secondary-subtle pt-4">
        <TarTab active id="content" :title="t('content')">
          <LineageGeneral :lineage="lineage" @error="handleError" @updated="onUpdate" />
        </TarTab>
        <TarTab disabled id="features" :title="t('game.feature.title')">
          <!-- TODO(fpion): Features -->
        </TarTab>
        <TarTab id="languages" :title="t('languages.title')">
          <LineageLanguages :lineage="lineage" @error="handleError" @updated="onUpdate" />
        </TarTab>
        <TarTab disabled id="names" :title="t('lineages.names.title')">
          <!-- TODO(fpion): Names -->
        </TarTab>
        <TarTab id="physical" :title="t('lineages.physical.title')">
          <LineagePhysical :lineage="lineage" @error="handleError" @updated="onUpdate" />
        </TarTab>
        <TarTab v-if="!isEthnicity" id="ethnicities" :title="t('lineages.ethnicities.title')">
          <LineageEthnicities :lineage="lineage" @error="handleError" />
        </TarTab>
      </TarTabs>
    </div>
    <LoadingSpinner v-else />
  </main>
</template>

<script setup lang="ts">
import { computed, inject, ref, watchEffect } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import LineageEthnicities from "@/components/lineages/LineageEthnicities.vue";
import LineageGeneral from "@/components/lineages/LineageGeneral.vue";
import LineageLanguages from "@/components/lineages/LineageLanguages.vue";
import LineagePhysical from "@/components/lineages/LineagePhysical.vue";
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

const breadcrumb = computed<Breadcrumb[]>(() => {
  const breadcrumb: Breadcrumb[] = [{ text: t("lineages.species.title"), to: { name: "Lineages" } }];
  const parent: Lineage | null | undefined = lineage.value?.parent;
  if (parent) {
    breadcrumb.push({ text: parent.name, to: { name: "Lineage", params: { id: parent.id } } });
  }
  return breadcrumb;
});
const isEthnicity = computed<boolean>(() => Boolean(lineage.value?.parent));
const title = computed<string>(() => lineage.value?.name ?? "");

function onUpdate(value: Lineage): void {
  lineage.value = value;
  toasts.success("saved");
}

watchEffect(async () => {
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
