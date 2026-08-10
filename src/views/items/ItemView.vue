<template>
  <main class="container page">
    <div v-if="item">
      <h1>{{ title }}</h1>
      <WorldBreadcrumb :current="title" :parent="breadcrumb" />
      <TarAlert :close="t('actions.close')" dismissible variant="success" v-model="isCreated">
        <strong>{{ t("items.created.lead") }}</strong> {{ t("items.created.help", { name: title }) }}
      </TarAlert>
      <StatusDetail class="mb-3" :subject="item" />
      <form class="border-top border-secondary-subtle pt-4" @submit.prevent="handleSubmit(submit)">
        <NameField class="mb-3" required v-model="name" />
        <div class="row">
          <div class="col-md-6">
            <PriceField class="mb-3" v-model="price" />
          </div>
          <div class="col-md-6">
            <WeightField class="mb-3" v-model="weight" />
          </div>
        </div>
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
import PriceField from "@/components/items/PriceField.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import SummaryField from "@/components/shared/SummaryField.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import TarButton from "@/components/tar/TarButton.vue";
import WeightField from "@/components/items/WeightField.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import type { CreateOrReplaceItemPayload, Item } from "@/types/items";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { fromHundredths, toHundredths } from "@/utils/number";
import { handleErrorKey } from "@/inject";
import { readItem, replaceItem } from "@/api/items";
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
const isCreated = ref<boolean>(false);
const isLoading = ref<boolean>(false);
const item = ref<Item>();
const name = ref<string>("");
const price = ref<number>();
const summary = ref<string>("");
const weight = ref<number>();

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("items.title"), to: { name: "Items" } }));
const hasChanges = computed<boolean>(() =>
  Boolean(
    item.value &&
    (item.value.name !== name.value ||
      (item.value.summary ?? "") !== summary.value ||
      (item.value.content ?? "") !== content.value ||
      fromHundredths(item.value.price) !== price.value ||
      fromHundredths(item.value.weight) !== weight.value),
  ),
);
const title = computed<string>(() => item.value?.name ?? "");

const { handleSubmit, reinitialize } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value && item.value) {
    isLoading.value = true;
    try {
      const payload: CreateOrReplaceItemPayload = {
        category: item.value.category,
        name: name.value,
        summary: summary.value,
        content: content.value,
        price: toHundredths(price.value),
        weight: toHundredths(weight.value),
      };
      item.value = await replaceItem(item.value.id, payload);
      isCreated.value = false;
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
  item,
  (item) => {
    name.value = item?.name ?? "";
    summary.value = item?.summary ?? "";
    content.value = item?.content ?? "";
    price.value = fromHundredths(item?.price);
    weight.value = fromHundredths(item?.weight);
  },
  { deep: true },
);

onMounted(async () => {
  try {
    const id: string = (Array.isArray(route.params.id) ? route.params.id[0] : route.params.id) ?? "";
    item.value = await readItem(id);
    isCreated.value = events.shift() === "created";
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
