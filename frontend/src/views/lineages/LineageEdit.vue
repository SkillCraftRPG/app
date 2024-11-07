<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import BackButton from "@/components/shared/BackButton.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import NameInput from "@/components/shared/NameInput.vue";
import SaveButton from "@/components/shared/SaveButton.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { ApiError } from "@/types/api";
import type { CreateOrReplaceLineagePayload, LineageModel } from "@/types/lineages";
import { handleErrorKey } from "@/inject/App";
import { readLineage, replaceLineage } from "@/api/lineages";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const description = ref<string>("");
const hasLoaded = ref<boolean>(false);
const lineage = ref<LineageModel>();
const name = ref<string>("");

const hasChanges = computed<boolean>(() => !!lineage.value && (name.value !== lineage.value.name || description.value !== (lineage.value.description ?? "")));

function setModel(model: LineageModel): void {
  lineage.value = model;
  description.value = model.description ?? "";
  name.value = model.name;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  if (lineage.value) {
    try {
      const payload: CreateOrReplaceLineagePayload = {
        parentId: lineage.value.species?.id,
        name: name.value,
        description: description.value,
        attributes: lineage.value.attributes, // TODO(fpion): implement
        features: lineage.value.features, // TODO(fpion): implement
        languages: lineage.value.languages, // TODO(fpion): implement
        names: lineage.value.names, // TODO(fpion): implement
        speeds: lineage.value.speeds, // TODO(fpion): implement
        size: lineage.value.size, // TODO(fpion): implement
        weight: lineage.value.weight, // TODO(fpion): implement
        ages: lineage.value.ages, // TODO(fpion): implement
      };
      const model: LineageModel = await replaceLineage(lineage.value.id, payload, lineage.value.version);
      setModel(model);
      toasts.success("lineages.updated");
    } catch (e: unknown) {
      handleError(e);
    }
  }
});

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      const lineage: LineageModel = await readLineage(id);
      setModel(lineage);
    }
  } catch (e: unknown) {
    const { status } = e as ApiError;
    if (status === 404) {
      router.push({ path: "/not-found" });
    } else {
      handleError(e);
    }
  }
  hasLoaded.value = true;
});
</script>

<template>
  <main class="container">
    <template v-if="lineage">
      <h1>{{ lineage.name }}</h1>
      <AppBreadcrumb
        :current="lineage.name"
        :parent="{ route: { name: 'LineageList' }, text: t('lineages.list') }"
        :world="lineage.world"
        @error="handleError"
      />
      <StatusDetail :aggregate="lineage" />
      <form @submit.prevent="onSubmit">
        <NameInput required v-model="name" />
        <DescriptionTextarea v-model="description" />
        <div>
          <SaveButton class="me-1" :disabled="isSubmitting || !hasChanges" :loading="isSubmitting" />
          <BackButton class="ms-1" :has-changes="hasChanges" />
        </div>
      </form>
    </template>
  </main>
</template>
