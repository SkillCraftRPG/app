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
        <div class="row">
          <div class="col-md-6">
            <NameField class="mb-3" required v-model="name" />
          </div>
          <div class="col-md-6">
            <ItemRarityField class="mb-3" v-model="rarity" />
          </div>
        </div>
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
        <fieldset class="border-top border-secondary-subtle pt-3 mt-4">
          <legend>{{ t("items.charges.label") }}</legend>
          <div class="row">
            <div class="col-md-6">
              <MaximumChargesField class="mb-3" :required="areChargesRequired" v-model="maximumCharges" />
            </div>
            <div class="col-md-6">
              <DepletionBehaviorField
                class="mb-3"
                :model-value="depletionBehavior"
                :required="areChargesRequired"
                @update:model-value="updateDepletionBehavior"
              />
            </div>
          </div>
          <ItemField
            v-if="depletionBehavior === 'Replace'"
            class="mb-3"
            id="replacement"
            label="items.charges.replacement"
            :model-value="replacement?.id ?? ''"
            required
            @error="handleError"
            @selected="replacement = $event"
          />
        </fieldset>
        <fieldset class="border-top border-secondary-subtle pt-3 mt-4">
          <legend>
            <TarCheckbox id="magic" :label="t('items.magic.label')" switch v-model="isMagical" />
          </legend>
          <div v-if="isMagical" class="row">
            <div class="col-md-6">
              <AttunementRadio class="mb-3" :model-value="attunement" @update:model-value="updateAttunement" />
            </div>
            <div v-if="attunement === 'optional' || attunement === 'required'" class="col-md-6">
              <AttunementRequirementsField class="mb-3" v-model="requirements" />
            </div>
          </div>
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

import AttunementRadio from "@/components/items/AttunementRadio.vue";
import AttunementRequirementsField from "@/components/items/AttunementRequirementsField.vue";
import ContentField from "@/components/shared/ContentField.vue";
import DepletionBehaviorField from "@/components/items/DepletionBehaviorField.vue";
import ItemField from "@/components/items/ItemField.vue";
import ItemRarityField from "@/components/items/ItemRarityField.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import MaximumChargesField from "@/components/items/MaximumChargesField.vue";
import NameField from "@/components/shared/NameField.vue";
import PriceField from "@/components/items/PriceField.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import SummaryField from "@/components/shared/SummaryField.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarCheckbox from "@/components/tar/TarCheckbox.vue";
import WeightField from "@/components/items/WeightField.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { AttunementOption, CreateOrReplaceItemPayload, DepletionBehavior, Item, ItemRarity } from "@/types/items";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { fromHundredths, toHundredths } from "@/utils/number";
import { getAttunementOption } from "@/utils/item";
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

const attunement = ref<AttunementOption>();
const content = ref<string>("");
const depletionBehavior = ref<string>("");
const isCreated = ref<boolean>(false);
const isLoading = ref<boolean>(false);
const isMagical = ref<boolean>(false);
const item = ref<Item>();
const maximumCharges = ref<number>(0);
const name = ref<string>("");
const price = ref<number>();
const rarity = ref<string>("");
const replacement = ref<Item>();
const requirements = ref<string>("");
const summary = ref<string>("");
const weight = ref<number>();

const areChargesRequired = computed<boolean>(() => Boolean(maximumCharges.value || depletionBehavior.value || replacement.value));
const breadcrumb = computed<Breadcrumb>(() => ({ text: t("items.title"), to: { name: "Items" } }));
const hasChanges = computed<boolean>(() =>
  Boolean(
    (item.value &&
      (item.value.name !== name.value ||
        (item.value.rarity ?? "") !== rarity.value ||
        (item.value.summary ?? "") !== summary.value ||
        (item.value.content ?? "") !== content.value ||
        (fromHundredths(item.value.price) ?? 0) !== price.value ||
        (fromHundredths(item.value.weight) ?? 0) !== weight.value ||
        (item.value.charges?.maximum ?? 0) !== maximumCharges.value ||
        (item.value.charges?.depletionBehavior ?? "") !== depletionBehavior.value ||
        (item.value.charges?.replacement?.id ?? "") !== (replacement.value?.id ?? "") ||
        Boolean(item.value.magic) !== isMagical.value ||
        getAttunementOption(item.value) !== attunement.value)) ||
    (item.value?.magic?.attunement?.requirements ?? "") !== requirements.value,
  ),
);
const title = computed<string>(() => item.value?.name ?? "");

function updateAttunement(value: AttunementOption): void {
  attunement.value = value;
  if (value !== "optional" && value !== "required") {
    requirements.value = "";
  }
}

function updateDepletionBehavior(value: string): void {
  depletionBehavior.value = value;
  replacement.value = undefined;
}

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
        price: toHundredths(price.value) || undefined,
        weight: toHundredths(weight.value) || undefined,
        rarity: rarity.value ? (rarity.value as ItemRarity) : undefined,
        charges: maximumCharges.value
          ? {
              maximum: maximumCharges.value,
              depletionBehavior: depletionBehavior.value as DepletionBehavior,
              replacementId: replacement.value?.id,
            }
          : undefined,
        magic: isMagical.value
          ? {
              attunement:
                attunement.value === "optional" || attunement.value === "required"
                  ? {
                      isRequired: attunement.value === "required",
                      requirements: requirements.value,
                    }
                  : undefined,
            }
          : undefined,
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
    rarity.value = item?.rarity ?? "";
    summary.value = item?.summary ?? "";
    content.value = item?.content ?? "";
    price.value = fromHundredths(item?.price) ?? 0;
    weight.value = fromHundredths(item?.weight) ?? 0;
    maximumCharges.value = item?.charges?.maximum ?? 0;
    depletionBehavior.value = item?.charges?.depletionBehavior ?? "";
    replacement.value = item?.charges?.replacement ?? undefined;
    isMagical.value = Boolean(item?.magic);
    attunement.value = item ? getAttunementOption(item) : undefined;
    requirements.value = item?.magic?.attunement?.requirements ?? "";
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
