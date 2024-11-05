<script setup lang="ts">
import { TarCheckbox } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import DefenseInput from "./DefenseInput.vue";
import ResistanceInput from "./ResistanceInput.vue";
import type { EquipmentPropertiesModel, EquipmentTrait } from "@/types/items";

const { rt, tm } = useI18n();

type TranslatedTrait = {
  value: EquipmentTrait;
  text: string;
};

const props = defineProps<{
  modelValue: EquipmentPropertiesModel;
}>();

const traits = ref<TranslatedTrait[]>(
  arrayUtils.orderBy(
    Object.entries(tm(rt("items.equipment.traits"))).map(([value, text]) => ({ value, text }) as TranslatedTrait),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "update:model-value", value: EquipmentPropertiesModel): void;
}>();

function setDefense(defense?: number): void {
  const properties: EquipmentPropertiesModel = { ...props.modelValue, defense: defense ?? 0 };
  emit("update:model-value", properties);
}
function setResistance(resistance?: number): void {
  const properties: EquipmentPropertiesModel = { ...props.modelValue, resistance };
  emit("update:model-value", properties);
}

function hasTrait(trait: EquipmentTrait): boolean {
  return props.modelValue.traits.includes(trait);
}
function toggleTrait(trait: EquipmentTrait, add: boolean): void {
  const properties: EquipmentPropertiesModel = { ...props.modelValue };
  properties.traits = properties.traits.filter((t) => t !== trait);
  if (add) {
    properties.traits.push(trait);
  }
  emit("update:model-value", properties);
}
</script>

<template>
  <div>
    <div class="row">
      <DefenseInput class="col-lg-6" :model-value="modelValue.defense" required @update:model-value="setDefense" />
      <ResistanceInput
        class="col-lg-6"
        label="items.equipment.resistance"
        :model-value="modelValue.resistance"
        placeholder="items.equipment.resistance"
        @update:model-value="setResistance"
      />
    </div>
    <div class="mb-3">
      <TarCheckbox
        v-for="trait in traits"
        :key="trait.value"
        :id="trait.value?.toLowerCase()"
        :label="trait.text"
        :model-value="hasTrait(trait.value)"
        @update:model-value="toggleTrait(trait.value, $event)"
      />
    </div>
  </div>
</template>
