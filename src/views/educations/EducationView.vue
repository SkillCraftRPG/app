<template>
  <main class="container page">
    <div v-if="education">
      <h1>{{ title }}</h1>
      <WorldBreadcrumb :current="title" :parent="breadcrumb" />
      <TarAlert :close="t('actions.close')" dismissible variant="success" v-model="isCreated">
        <strong>{{ t("educations.created.lead") }}</strong> {{ t("educations.created.help", { name: title }) }}
      </TarAlert>
      <StatusDetail class="mb-3" :subject="education" />
      <form class="border-top border-secondary-subtle pt-4" @submit.prevent="handleSubmit(submit)">
        <NameField class="mb-3" required v-model="name" />
        <div class="row">
          <div class="col-md-6">
            <SkillField class="mb-3" v-model="skill" />
          </div>
          <div class="col-md-6">
            <WealthMultiplierField class="mb-3" v-model="wealthMultiplier" />
          </div>
        </div>
        <SummaryField class="mb-3" v-model="summary" />
        <ContentField class="mb-3" v-model="content" />
        <fieldset class="border-top border-secondary-subtle pt-3 mt-4">
          <legend>{{ t("game.feature.label") }}</legend>
          <NameField class="mb-3" id="feature-name" v-model="feature.name" />
          <ContentField class="mb-3" id="feature-content" :model-value="feature.content ?? ''" @update:model-value="feature.content = $event" />
        </fieldset>
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
import SkillField from "@/components/skills/SkillField.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import SummaryField from "@/components/shared/SummaryField.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import TarButton from "@/components/tar/TarButton.vue";
import WealthMultiplierField from "@/components/educations/WealthMultiplierField.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import type { CreateOrReplaceEducationPayload, Education } from "@/types/educations";
import type { Feature } from "@/types/features";
import type { Skill } from "@/types/game";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { handleErrorKey } from "@/inject";
import { readEducation, replaceEducation } from "@/api/educations";
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
const education = ref<Education>();
const feature = ref<Feature>({ name: "" });
const isCreated = ref<boolean>(events.shift() === "created");
const isLoading = ref<boolean>(false);
const name = ref<string>("");
const skill = ref<string>("");
const summary = ref<string>("");
const wealthMultiplier = ref<number>(0);

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("educations.title"), to: { name: "Educations" } }));
const hasChanges = computed<boolean>(() =>
  Boolean(
    education.value &&
    (education.value.name !== name.value ||
      (education.value.skill ?? "") !== skill.value ||
      (education.value.wealthMultiplier ?? 0) !== wealthMultiplier.value ||
      (education.value.summary ?? "") !== summary.value ||
      (education.value.content ?? "") !== content.value ||
      (education.value.feature?.name ?? "") !== feature.value.name ||
      (education.value.feature?.content ?? "") !== (feature.value.content ?? "")),
  ),
);
const title = computed<string>(() => education.value?.name ?? "");

const { handleSubmit, reinitialize } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value && education.value) {
    isLoading.value = true;
    try {
      const payload: CreateOrReplaceEducationPayload = {
        name: name.value,
        summary: summary.value,
        content: content.value,
        skill: skill.value ? (skill.value as Skill) : undefined,
        wealthMultiplier: wealthMultiplier.value || undefined,
        feature: feature.value.name ? feature.value : undefined,
      };
      education.value = await replaceEducation(education.value.id, payload);
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
  education,
  (education) => {
    name.value = education?.name ?? "";
    skill.value = education?.skill ?? "";
    wealthMultiplier.value = education?.wealthMultiplier ?? 0;
    summary.value = education?.summary ?? "";
    content.value = education?.content ?? "";
    feature.value.name = education?.feature?.name ?? "";
    feature.value.content = education?.feature?.content;
  },
  { deep: true },
);

onMounted(async () => {
  try {
    const id: string = (Array.isArray(route.params.id) ? route.params.id[0] : route.params.id) ?? "";
    education.value = await readEducation(id);
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
