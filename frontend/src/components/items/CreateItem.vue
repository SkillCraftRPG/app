<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import ItemCategorySelect from "./ItemCategorySelect.vue";
import NameInput from "@/components/shared/NameInput.vue";
import type { CreateOrReplaceItemPayload, ItemCategory, ItemModel } from "@/types/items";
import { createItem } from "@/api/items";

const { t } = useI18n();

const category = ref<ItemCategory>();
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const name = ref<string>("");

const hasChanges = computed<boolean>(() => Boolean(name.value.trim()));

function hide(): void {
  modalRef.value?.hide();
}

const emit = defineEmits<{
  (e: "created", value: ItemModel): void;
  (e: "error", value: unknown): void;
}>();

function onCancel(): void {
  category.value = undefined;
  name.value = "";
  hide();
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    const payload: CreateOrReplaceItemPayload = {
      name: name.value,
      isAttunementRequired: false,
    };
    switch (category.value) {
      case "Consumable":
        payload.consumable = { removeWhenEmpty: false };
        break;
      case "Container":
        payload.container = {};
        break;
      case "Device":
        payload.device = {};
        break;
      case "Equipment":
        payload.equipment = { defense: 0, traits: [] };
        break;
      case "Miscellaneous":
        payload.miscellaneous = {};
        break;
      case "Money":
        payload.money = {};
        break;
      case "Weapon":
        payload.weapon = { attack: 0, traits: [], damages: [], versatileDamages: [] };
        break;
      default:
        throw new Error(`The item category '${category.value}' is not supported.`);
    }
    const item: ItemModel = await createItem(payload);
    emit("created", item);
    hide();
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>

<template>
  <span>
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" data-bs-target="#create-item" />
    <TarModal :close="t('actions.close')" id="create-item" ref="modalRef" :title="t('items.create')">
      <form @submit.prevent="onSubmit">
        <ItemCategorySelect required v-model="category" />
        <NameInput required v-model="name" />
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="onCancel" />
        <TarButton
          :disabled="isSubmitting || !hasChanges"
          icon="fas fa-plus"
          :loading="isSubmitting"
          :status="t('loading')"
          :text="t('actions.create')"
          variant="success"
          @click="onSubmit"
        />
      </template>
    </TarModal>
  </span>
</template>
