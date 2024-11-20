<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import AttributeSelect from "@/components/game/AttributeSelect.vue";
import BackButton from "@/components/shared/BackButton.vue";
import CustomizationSelect from "@/components/customizations/CustomizationSelect.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import NameInput from "@/components/shared/NameInput.vue";
import SaveButton from "@/components/shared/SaveButton.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { ApiError } from "@/types/api";
import type { Attribute } from "@/types/game";
import type { CreateOrReplaceNaturePayload, NatureModel } from "@/types/natures";
import type { CustomizationModel } from "@/types/customizations";
import { handleErrorKey } from "@/inject/App";
import { readNature, replaceNature } from "@/api/natures";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const attribute = ref<Attribute>();
const description = ref<string>("");
const gift = ref<CustomizationModel>();
const name = ref<string>("");
const nature = ref<NatureModel>();

const hasChanges = computed<boolean>(
  () =>
    !!nature.value &&
    (name.value !== nature.value.name ||
      attribute.value !== (nature.value.attribute ?? undefined) ||
      gift.value?.id !== nature.value.gift?.id ||
      description.value !== (nature.value.description ?? "")),
);

function setModel(model: NatureModel): void {
  nature.value = model;
  attribute.value = model.attribute ?? undefined;
  description.value = model.description ?? "";
  gift.value = model.gift ?? undefined;
  name.value = model.name;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  if (nature.value) {
    try {
      const payload: CreateOrReplaceNaturePayload = {
        name: name.value,
        description: description.value,
        attribute: attribute.value,
        giftId: gift.value?.id,
      };
      const model: NatureModel = await replaceNature(nature.value.id, payload, nature.value.version);
      setModel(model);
      toasts.success("natures.updated");
    } catch (e: unknown) {
      handleError(e);
    }
  }
});

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      const nature: NatureModel = await readNature(id);
      setModel(nature);
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
    <template v-if="nature">
      <h1>{{ nature.name }}</h1>
      <AppBreadcrumb :current="nature.name" :parent="{ route: { name: 'NatureList' }, text: t('natures.list') }" :world="nature.world" @error="handleError" />
      <StatusDetail :aggregate="nature" />
      <form @submit.prevent="onSubmit">
        <div class="row">
          <NameInput class="col-lg-4" required v-model="name" />
          <AttributeSelect class="col-lg-4" v-model="attribute" />
          <CustomizationSelect
            class="col-lg-4"
            label="customizations.type.options.Gift"
            :model-value="gift?.id"
            placeholder="customizations.select.gift"
            type="Gift"
            @error="handleError"
            @selected="gift = $event"
          />
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
