<template>
  <main class="container page">
    <div v-if="script">
      <h1>{{ title }}</h1>
      <WorldBreadcrumb :current="title" :parent="breadcrumb" />
      <TarAlert :close="t('actions.close')" dismissible variant="success" v-model="isCreated">
        <strong>{{ t("scripts.created.lead") }}</strong> {{ t("scripts.created.help", { name: title }) }}
      </TarAlert>
      <StatusDetail class="mb-4" :subject="script" />
      <form @submit.prevent="handleSubmit(submit)">
        <NameInput class="mb-3" required v-model="name" />
        <SummaryField class="mb-3" v-model="summary" />
        <ContentTextarea class="mb-3" v-model="htmlContent" />
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

import ContentTextarea from "@/components/shared/ContentTextarea.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import NameInput from "@/components/shared/NameInput.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import SummaryField from "@/components/shared/SummaryField.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import TarButton from "@/components/tar/TarButton.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import type { CreateOrReplaceScriptPayload, Script } from "@/types/scripts";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { handleErrorKey } from "@/inject";
import { readScript, replaceScript } from "@/api/scripts";
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

const htmlContent = ref<string>("");
const isCreated = ref<boolean>(events.shift() === "created");
const isLoading = ref<boolean>(false);
const name = ref<string>("");
const script = ref<Script>();
const summary = ref<string>("");

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("scripts.title"), to: { name: "Scripts" } }));
const hasChanges = computed<boolean>(() =>
  Boolean(
    script.value &&
    (script.value.name !== name.value || (script.value.summary ?? "") !== summary.value || (script.value.htmlContent ?? "") !== htmlContent.value),
  ),
);
const title = computed<string>(() => script.value?.name ?? "");

const { handleSubmit, reinitialize } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value && script.value) {
    isLoading.value = true;
    try {
      const payload: CreateOrReplaceScriptPayload = {
        name: name.value,
        summary: summary.value,
        htmlContent: htmlContent.value,
      };
      script.value = await replaceScript(script.value.id, payload);
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
  script,
  (script) => {
    name.value = script?.name ?? "";
    summary.value = script?.summary ?? "";
    htmlContent.value = script?.htmlContent ?? "";
  },
  { deep: true },
);

onMounted(async () => {
  try {
    const id: string = (Array.isArray(route.params.id) ? route.params.id[0] : route.params.id) ?? "";
    script.value = await readScript(id);
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
