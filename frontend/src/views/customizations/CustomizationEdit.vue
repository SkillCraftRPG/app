<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import BackButton from "@/components/shared/BackButton.vue";
import CustomizationTypeSelect from "@/components/customizations/CustomizationTypeSelect.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import NameInput from "@/components/shared/NameInput.vue";
import SaveButton from "@/components/shared/SaveButton.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { ApiError } from "@/types/api";
import type { CreateOrReplaceCustomizationPayload, CustomizationModel } from "@/types/customizations";
import { handleErrorKey } from "@/inject/App";
import { readCustomization, replaceCustomization } from "@/api/customizations";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const customization = ref<CustomizationModel>();
const description = ref<string>("");
const name = ref<string>("");

const hasChanges = computed<boolean>(
  () => !!customization.value && (name.value !== customization.value.name || description.value !== (customization.value.description ?? "")),
);

function setModel(model: CustomizationModel): void {
  customization.value = model;
  description.value = model.description ?? "";
  name.value = model.name;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  if (customization.value) {
    try {
      const payload: CreateOrReplaceCustomizationPayload = {
        type: customization.value.type,
        name: name.value,
        description: description.value,
      };
      const model: CustomizationModel = await replaceCustomization(customization.value.id, payload, customization.value.version);
      setModel(model);
      toasts.success("customizations.updated");
    } catch (e: unknown) {
      handleError(e);
    }
  }
});

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      const customization: CustomizationModel = await readCustomization(id);
      setModel(customization);
    }
  } catch (e: unknown) {
    const { status } = e as ApiError;
    if (status === 404) {
      router.push({ path: "/not-found" });
    } else {
      handleError(e);
    }
  }
});
</script>

<template>
  <main class="container">
    <template v-if="customization">
      <h1>{{ customization.name }}</h1>
      <AppBreadcrumb
        :current="customization.name"
        :parent="{ route: { name: 'CustomizationList' }, text: t('customizations.list') }"
        :world="customization.world"
        @error="handleError"
      />
      <StatusDetail :aggregate="customization" />
      <form @submit.prevent="onSubmit">
        <div class="row">
          <NameInput class="col-lg-6" required v-model="name" />
          <CustomizationTypeSelect class="col-lg-6" disabled :model-value="customization.type" validation="server" />
        </div>
        <DescriptionTextarea v-model="description" />
        <div>
          <SaveButton class="me-1" :disabled="isSubmitting || !hasChanges" :loading="isSubmitting" />
          <BackButton class="ms-1" :has-changes="hasChanges" />
        </div>
      </form>
    </template>
  </main>
</template>
