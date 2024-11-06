<script setup lang="ts">
import { TarCheckbox } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import BackButton from "@/components/shared/BackButton.vue";
import ConsumableProperties from "@/components/items/ConsumableProperties.vue";
import ContainerProperties from "@/components/items/ContainerProperties.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import EquipmentProperties from "@/components/items/EquipmentProperties.vue";
import ItemCategorySelect from "@/components/items/ItemCategorySelect.vue";
import NameInput from "@/components/shared/NameInput.vue";
import SaveButton from "@/components/shared/SaveButton.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import ValueInput from "@/components/items/ValueInput.vue";
import WeaponProperties from "@/components/items/WeaponProperties.vue";
import WeightInput from "@/components/items/WeightInput.vue";
import type { ApiError } from "@/types/api";
import type {
  ConsumablePropertiesModel,
  ContainerPropertiesModel,
  CreateOrReplaceItemPayload,
  EquipmentPropertiesModel,
  ItemModel,
  WeaponPropertiesModel,
} from "@/types/items";
import { handleErrorKey } from "@/inject/App";
import { readItem, replaceItem } from "@/api/items";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const consumable = ref<ConsumablePropertiesModel>();
const container = ref<ContainerPropertiesModel>();
const description = ref<string>("");
const equipment = ref<EquipmentPropertiesModel>();
const hasLoaded = ref<boolean>(false);
const isAttunementRequired = ref<boolean>(false);
const isConsumableValid = ref<boolean>(true);
const item = ref<ItemModel>();
const name = ref<string>("");
const value = ref<number>();
const weapon = ref<WeaponPropertiesModel>();
const weight = ref<number>();

const hasChanges = computed<boolean>(
  () =>
    !!item.value &&
    (name.value !== item.value.name ||
      value.value !== (item.value.value ?? undefined) ||
      weight.value !== (item.value.weight ?? undefined) ||
      isAttunementRequired.value !== item.value.isAttunementRequired ||
      description.value !== (item.value.description ?? "") ||
      JSON.stringify(consumable.value) !== JSON.stringify(item.value.consumable ?? undefined) ||
      JSON.stringify(container.value) !== JSON.stringify(item.value.container ?? undefined) ||
      JSON.stringify(equipment.value) !== JSON.stringify(item.value.equipment ?? undefined) ||
      JSON.stringify(weapon.value) !== JSON.stringify(item.value.weapon ?? undefined)),
);
const hasProperties = computed<boolean>(
  () =>
    item.value?.category === "Consumable" || item.value?.category === "Container" || item.value?.category === "Equipment" || item.value?.category === "Weapon",
);

function setModel(model: ItemModel): void {
  item.value = model;
  consumable.value = model.consumable ? { ...model.consumable } : undefined;
  container.value = model.container ? { ...model.container } : undefined;
  description.value = model.description ?? "";
  equipment.value = model.equipment ? { ...model.equipment } : undefined;
  isAttunementRequired.value = model.isAttunementRequired;
  name.value = model.name;
  value.value = model.value ?? undefined;
  weapon.value = model.weapon ? { ...model.weapon } : undefined;
  weight.value = model.weight ?? undefined;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  if (item.value) {
    try {
      const payload: CreateOrReplaceItemPayload = {
        name: name.value,
        description: description.value,
        value: value.value,
        weight: weight.value,
        isAttunementRequired: isAttunementRequired.value,
        consumable: consumable.value,
        container: container.value,
        device: item.value.device,
        equipment: equipment.value,
        miscellaneous: item.value.miscellaneous,
        money: item.value.money,
        weapon: weapon.value,
      };
      const model: ItemModel = await replaceItem(item.value.id, payload, item.value.version);
      setModel(model);
      toasts.success("items.updated");
    } catch (e: unknown) {
      handleError(e);
    }
  }
});

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      const item: ItemModel = await readItem(id);
      setModel(item);
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
    <template v-if="item">
      <h1>{{ item.name }}</h1>
      <AppBreadcrumb :current="item.name" :parent="{ route: { name: 'ItemList' }, text: t('items.list') }" :world="item.world" @error="handleError" />
      <StatusDetail :aggregate="item" />
      <form @submit.prevent="onSubmit">
        <div class="row">
          <NameInput class="col-lg-6" required v-model="name" />
          <ItemCategorySelect class="col-lg-6" disabled :model-value="item.category" validation="server" />
        </div>
        <div class="row">
          <ValueInput class="col-lg-6" v-model="value" />
          <WeightInput class="col-lg-6" v-model="weight" />
        </div>
        <TarCheckbox class="mb-3" id="is-attunement-required" :label="t('items.isAttunementRequired')" v-model="isAttunementRequired" />
        <DescriptionTextarea v-model="description" />
        <h3 v-if="hasProperties">{{ t("items.properties") }}</h3>
        <ConsumableProperties v-if="consumable" v-model="consumable" @error="handleError" @validated="isConsumableValid = $event" />
        <ContainerProperties v-if="container" v-model="container" />
        <EquipmentProperties v-if="equipment" v-model="equipment" />
        <WeaponProperties v-if="weapon" v-model="weapon" />
        <div>
          <SaveButton class="me-1" :disabled="!isConsumableValid || isSubmitting || !hasChanges" :loading="isSubmitting" />
          <BackButton class="ms-1" :has-changes="hasChanges" />
        </div>
      </form>
    </template>
  </main>
</template>
