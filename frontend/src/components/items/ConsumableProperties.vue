<script setup lang="ts">
import { TarAlert, TarCheckbox } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import ChargesInput from "./ChargesInput.vue";
import ItemSelect from "./ItemSelect.vue";
import type { ConsumablePropertiesModel, ItemModel } from "@/types/items";

const { t } = useI18n();

const props = defineProps<{
  modelValue: ConsumablePropertiesModel;
}>();

const isValid = ref<boolean>(true);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "update:model-value", value: ConsumablePropertiesModel): void;
  (e: "validated", value: boolean): void;
}>();

function validate(properties: ConsumablePropertiesModel): void {
  isValid.value = !properties.removeWhenEmpty || !properties.replaceWithItemWhenEmptyId;
  emit("validated", isValid.value);
}

function setCharges(charges?: number): void {
  const properties: ConsumablePropertiesModel = { ...props.modelValue, charges };
  emit("update:model-value", properties);
}
function setRemoveWhenEmpty(removeWhenEmpty: boolean): void {
  const properties: ConsumablePropertiesModel = { ...props.modelValue, removeWhenEmpty };
  emit("update:model-value", properties);
  validate(properties);
}
function setReplaceWithItemWhenEmptyId(replaceWithItemWhenEmpty?: ItemModel): void {
  const properties: ConsumablePropertiesModel = { ...props.modelValue, replaceWithItemWhenEmptyId: replaceWithItemWhenEmpty?.id };
  emit("update:model-value", properties);
  validate(properties);
}
</script>

<template>
  <div>
    <TarAlert :show="!isValid" variant="danger"> <font-awesome-icon icon="fas fa-triangle-exclamation" /> {{ t("items.consumable.invalid") }} </TarAlert>
    <div class="row">
      <ChargesInput class="col-lg-6" :model-value="modelValue.charges" @update:model-value="setCharges" />
      <ItemSelect
        class="col-lg-6"
        id="replace-with-item-when-empty"
        label="items.consumable.replaceWithItemWhenEmpty"
        :model-value="modelValue.replaceWithItemWhenEmptyId"
        @error="$emit('error', $event)"
        @selected="setReplaceWithItemWhenEmptyId"
      />
    </div>
    <TarCheckbox
      class="mb-3"
      id="remove-when-empty"
      :label="t('items.consumable.removeWhenEmpty')"
      :model-value="modelValue.removeWhenEmpty ?? false"
      @update:model-value="setRemoveWhenEmpty"
    />
  </div>
</template>
