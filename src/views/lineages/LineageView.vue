<template>
  <main class="container page">
    <div v-if="lineage">
      <h1>{{ title }}</h1>
      <WorldBreadcrumb :current="title" :parent="breadcrumb" />
      <TarAlert :close="t('actions.close')" dismissible variant="success" v-model="isCreated">
        <strong>{{ t("lineages.created.lead") }}</strong> {{ t("lineages.created.help", { name: title }) }}
      </TarAlert>
      <StatusDetail class="mb-3" :subject="lineage" />
      <form class="border-top border-secondary-subtle pt-4" @submit.prevent="handleSubmit(submit)">
        <NameField class="mb-3" required v-model="name" />
        <SummaryField class="mb-3" v-model="summary" />
        <ContentField class="mb-3" v-model="content" />
        <div class="d-flex justify-content-end mb-3">
          <TarButton
            :disabled="!hasChanges || isLoading"
            icon="fas fa-floppy-disk"
            :loading="isLoading"
            size="large"
            :status="t('loading')"
            :text="t('actions.save')"
            type="submit"
          />
        </div>
      </form>
    </div>
    <LoadingSpinner v-else />
  </main>
</template>

<script setup lang="ts">
import { computed, inject, onMounted, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import ContentField from "@/components/shared/ContentField.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import NameField from "@/components/shared/NameField.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import SummaryField from "@/components/shared/SummaryField.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import TarButton from "@/components/tar/TarButton.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import type { Lineage, UpdateLineagePayload } from "@/types/lineages";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { handleErrorKey } from "@/inject";
import { readLineage, updateLineage } from "@/api/lineages";
import { useDocument } from "@/composables/document";
import { useEventStore } from "@/stores/event";
import { useForm } from "@/forms";
import { useToastStore } from "@/stores/toast";

const document = useDocument();
const events = useEventStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const content = ref<string>("");
const isCreated = ref<boolean>(events.shift() === "created");
const isLoading = ref<boolean>(false);
const lineage = ref<Lineage>();
const name = ref<string>("");
const summary = ref<string>("");

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("lineages.title"), to: { name: "Lineages" } }));
const hasChanges = computed<boolean>(() =>
  Boolean(
    lineage.value && (lineage.value.name !== name.value || (lineage.value.summary ?? "") !== summary.value || (lineage.value.content ?? "") !== content.value),
  ),
);
const title = computed<string>(() => lineage.value?.name ?? "");

const { handleSubmit, reinitialize } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value && lineage.value) {
    isLoading.value = true;
    try {
      const payload: UpdateLineagePayload = {
        name: name.value,
        summary: { value: summary.value || null },
        content: { value: content.value || null },
      };
      lineage.value = await updateLineage(lineage.value.id, payload);
      reinitialize();
      toasts.success("saved");
    } catch (e: unknown) {
      handleError(e);
    } finally {
      isLoading.value = false;
    }
  }
}

watch(
  lineage,
  (lineage) => {
    name.value = lineage?.name ?? "";
    summary.value = lineage?.summary ?? "";
    content.value = lineage?.content ?? "";
  },
  { deep: true },
);

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
