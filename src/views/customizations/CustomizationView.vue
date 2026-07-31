<template>
  <main class="container page">
    <div v-if="customization">
      <div class="d-flex flex-wrap align-items-center gap-2">
        <h1>{{ title }}</h1>
        <TarBadge class="fs-6" variant="secondary"><CustomizationKindDisplay :kind="customization.kind" /></TarBadge>
      </div>
      <WorldBreadcrumb :current="title" :parent="breadcrumb" />
      <TarAlert :close="t('actions.close')" dismissible variant="success" v-model="isCreated">
        <strong>{{ t("customizations.created.lead") }}</strong> {{ t("customizations.created.help", { name: title }) }}
      </TarAlert>
      <StatusDetail class="mb-3" :subject="customization" />
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
import CustomizationKindDisplay from "@/components/customizations/CustomizationKindDisplay.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import NameField from "@/components/shared/NameField.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import SummaryField from "@/components/shared/SummaryField.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import TarBadge from "@/components/tar/TarBadge.vue";
import TarButton from "@/components/tar/TarButton.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import type { CreateOrReplaceCustomizationPayload, Customization } from "@/types/customizations";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { handleErrorKey } from "@/inject";
import { readCustomization, replaceCustomization } from "@/api/customizations";
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
const customization = ref<Customization>();
const isCreated = ref<boolean>(events.shift() === "created");
const isLoading = ref<boolean>(false);
const name = ref<string>("");
const summary = ref<string>("");

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("customizations.title"), to: { name: "Customizations" } }));
const hasChanges = computed<boolean>(() =>
  Boolean(
    customization.value &&
    (customization.value.name !== name.value || (customization.value.summary ?? "") !== summary.value || (customization.value.content ?? "") !== content.value),
  ),
);
const title = computed<string>(() => customization.value?.name ?? "");

const { handleSubmit, reinitialize } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value && customization.value) {
    isLoading.value = true;
    try {
      const payload: CreateOrReplaceCustomizationPayload = {
        kind: customization.value.kind,
        name: name.value,
        summary: summary.value,
        content: content.value,
      };
      customization.value = await replaceCustomization(customization.value.id, payload);
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
  customization,
  (customization) => {
    name.value = customization?.name ?? "";
    summary.value = customization?.summary ?? "";
    content.value = customization?.content ?? "";
  },
  { deep: true },
);

onMounted(async () => {
  try {
    const id: string = (Array.isArray(route.params.id) ? route.params.id[0] : route.params.id) ?? "";
    customization.value = await readCustomization(id);
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
