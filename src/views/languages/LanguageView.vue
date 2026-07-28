<template>
  <main class="container page">
    <div v-if="language">
      <h1>{{ title }}</h1>
      <WorldBreadcrumb :current="title" :parent="breadcrumb" />
      <TarAlert :close="t('actions.close')" dismissible variant="success" v-model="isCreated">
        <strong>{{ t("languages.created.lead") }}</strong> {{ t("languages.created.help", { name: title }) }}
      </TarAlert>
      <StatusDetail class="mb-3" :subject="language" />
      <form class="border-top border-secondary-subtle pt-4" @submit.prevent="handleSubmit(submit)">
        <div class="row">
          <div class="col-md-6">
            <NameField class="mb-3" required v-model="name" />
          </div>
          <div class="col-md-6">
            <ScriptField class="mb-3" :model-value="script?.id ?? ''" @selected="script = $event" />
          </div>
        </div>
        <SummaryField class="mb-3" v-model="summary" />
        <TypicalSpeakersField class="mb-3" v-model="typicalSpeakers" />
        <ContentField class="mb-3" v-model="htmlContent" />
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
import ScriptField from "@/components/scripts/ScriptField.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import SummaryField from "@/components/shared/SummaryField.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TypicalSpeakersField from "@/components/languages/TypicalSpeakersField.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import type { CreateOrReplaceLanguagePayload, Language } from "@/types/languages";
import type { Script } from "@/types/scripts";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { handleErrorKey } from "@/inject";
import { readLanguage, replaceLanguage } from "@/api/languages";
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
const language = ref<Language>();
const script = ref<Script>();
const summary = ref<string>("");
const typicalSpeakers = ref<string>("");

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("languages.title"), to: { name: "Languages" } }));
const hasChanges = computed<boolean>(() =>
  Boolean(
    language.value &&
    (language.value.name !== name.value ||
      (language.value.script?.id ?? "") !== (script.value?.id ?? "") ||
      (language.value.summary ?? "") !== summary.value ||
      (language.value.typicalSpeakers ?? "") !== typicalSpeakers.value ||
      (language.value.htmlContent ?? "") !== htmlContent.value),
  ),
);
const title = computed<string>(() => language.value?.name ?? "");

const { handleSubmit, reinitialize } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value && language.value) {
    isLoading.value = true;
    try {
      const payload: CreateOrReplaceLanguagePayload = {
        name: name.value,
        summary: summary.value,
        htmlContent: htmlContent.value,
        scriptId: script.value?.id,
        typicalSpeakers: typicalSpeakers.value,
      };
      language.value = await replaceLanguage(language.value.id, payload);
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
  language,
  (language) => {
    name.value = language?.name ?? "";
    script.value = language?.script ? { ...language.script } : undefined;
    summary.value = language?.summary ?? "";
    typicalSpeakers.value = language?.typicalSpeakers ?? "";
    htmlContent.value = language?.htmlContent ?? "";
  },
  { deep: true },
);

onMounted(async () => {
  try {
    const id: string = (Array.isArray(route.params.id) ? route.params.id[0] : route.params.id) ?? "";
    language.value = await readLanguage(id);
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
