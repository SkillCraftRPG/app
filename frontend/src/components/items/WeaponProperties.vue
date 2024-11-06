<script setup lang="ts">
import { TarCheckbox } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import AttackInput from "./AttackInput.vue";
import RangeInput from "./RangeInput.vue";
import ReloadCountInput from "./ReloadCountInput.vue";
import ResistanceInput from "./ResistanceInput.vue";
import type { WeaponPropertiesModel, WeaponTrait } from "@/types/items";

const { rt, t, tm } = useI18n();

type RangeKey = "ammunition.normal" | "ammunition.long" | "thrown.normal" | "thrown.long";
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

function setRange(key: RangeKey, value?: number): void {
  const properties: WeaponPropertiesModel = { ...props.modelValue };
  switch (key) {
    case "ammunition.long":
      console.log(key + ": " + value); // TODO(fpion): implement
      break;
    case "ammunition.normal":
      console.log(key + ": " + value); // TODO(fpion): implement
      break;
    case "thrown.long":
      console.log(key + ": " + value); // TODO(fpion): implement
      break;
    case "thrown.normal":
      console.log(key + ": " + value); // TODO(fpion): implement
      break;
    default:
      throw new Error(`The weapon range key '${key}' is not supported.`);
  }
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
    <div class="row">
      <div class="col-lg-6">
        <h5>{{ t("items.weapon.range.ammunition") }}</h5>
        <div class="row">
          <RangeInput
            class="col"
            id="ammunition-normal-range"
            label="items.weapon.range.normal"
            :model-value="modelValue.ammunitionRange?.normal"
            placeholder="items.weapon.range.normal"
            @update:model-value="setRange('ammunition.normal', $event)"
          />
          <RangeInput
            class="col"
            id="ammunition-long-range"
            label="items.weapon.range.long"
            :model-value="modelValue.ammunitionRange?.long"
            placeholder="items.weapon.range.long"
            @update:model-value="setRange('ammunition.long', $event)"
          />
        </div>
      </div>
      <div class="col-lg-6">
        <h5>{{ t("items.weapon.range.thrown") }}</h5>
        <div class="row">
          <RangeInput
            class="col"
            id="thrown-normal-range"
            label="items.weapon.range.normal"
            :model-value="modelValue.thrownRange?.normal"
            placeholder="items.weapon.range.normal"
            @update:model-value="setRange('thrown.normal', $event)"
          />
          <RangeInput
            class="col"
            id="thrown-long-range"
            label="items.weapon.range.long"
            :model-value="modelValue.thrownRange?.long"
            placeholder="items.weapon.range.long"
            @update:model-value="setRange('thrown.long', $event)"
          />
        </div>
      </div>
    </div>
  </div>
</template>
