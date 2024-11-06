<script setup lang="ts">
import { TarCheckbox } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import AttackInput from "./AttackInput.vue";
import ResistanceInput from "./ResistanceInput.vue";
import type { WeaponPropertiesModel, WeaponTrait } from "@/types/items";

const { rt, tm } = useI18n();

type TranslatedTrait = {
  value: WeaponTrait;
  text: string;
};

const props = defineProps<{
  modelValue: WeaponPropertiesModel;
}>();

const traits = ref<TranslatedTrait[]>(
  arrayUtils.orderBy(
    Object.entries(tm(rt("items.weapon.traits"))).map(([value, text]) => ({ value, text }) as TranslatedTrait),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "update:model-value", value: WeaponPropertiesModel): void;
}>();

function setAttack(attack?: number): void {
  const properties: WeaponPropertiesModel = { ...props.modelValue, attack: attack ?? 0 };
  emit("update:model-value", properties);
}
function setReloadCount(reloadCount?: number): void {
  const properties: WeaponPropertiesModel = { ...props.modelValue, reloadCount };
  emit("update:model-value", properties);
}
function setResistance(resistance?: number): void {
  const properties: WeaponPropertiesModel = { ...props.modelValue, resistance };
  emit("update:model-value", properties);
}

function hasTrait(trait: WeaponTrait): boolean {
  return props.modelValue.traits.includes(trait);
}
function toggleTrait(trait: WeaponTrait, add: boolean): void {
  const properties: WeaponPropertiesModel = { ...props.modelValue };
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
      <AttackInput class="col-lg-4" :model-value="modelValue.attack" required @update:model-value="setAttack" />
      <ResistanceInput
        class="col-lg-4"
        label="items.weapon.resistance"
        :model-value="modelValue.resistance"
        placeholder="items.weapon.resistance"
        @update:model-value="setResistance"
      />
      <ReloadCountInput class="col-lg-4" :model-value="modelValue.reloadCount" @update:model-value="setReloadCount" />
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
