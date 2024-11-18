<script setup lang="ts">
import { useI18n } from "vue-i18n";

import AttributeBonusInput from "@/components/lineages/AttributeBonusInput.vue";
import ExtraAttributesInput from "@/components/lineages/ExtraAttributesInput.vue";
import type { AttributeBonusesModel } from "@/types/lineages";
import type { Attribute } from "@/types/game";

const { t } = useI18n();

const props = defineProps<{
  modelValue: AttributeBonusesModel;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: AttributeBonusesModel): void;
}>();

function setAttribute(attribute: Attribute, value?: number): void {
  const attributes: AttributeBonusesModel = { ...props.modelValue };
  switch (attribute) {
    case "Agility":
      attributes.agility = value ?? 0;
      break;
    case "Coordination":
      attributes.coordination = value ?? 0;
      break;
    case "Spirit":
      attributes.spirit = value ?? 0;
      break;
    case "Intellect":
      attributes.intellect = value ?? 0;
      break;
    case "Presence":
      attributes.presence = value ?? 0;
      break;
    case "Sensitivity":
      attributes.sensitivity = value ?? 0;
      break;
    case "Vigor":
      attributes.vigor = value ?? 0;
      break;
    default:
      throw new Error(`The attribute "${attribute}" is not supported.`);
  }
  emit("update:model-value", attributes);
}
function setExtra(extra: number): void {
  const attributes: AttributeBonusesModel = { ...props.modelValue, extra };
  emit("update:model-value", attributes);
}
</script>

<template>
  <div>
    <h3>{{ t("lineages.attributes.label") }}</h3>
    <div class="row">
      <!-- TODO(fpion): order -->
      <div class="col-attributes">
        <AttributeBonusInput attribute="Agility" :model-value="modelValue.agility" @update:model-value="setAttribute('Agility', $event)" />
      </div>
      <div class="col-attributes">
        <AttributeBonusInput attribute="Coordination" :model-value="modelValue.coordination" @update:model-value="setAttribute('Coordination', $event)" />
      </div>
      <div class="col-attributes">
        <AttributeBonusInput attribute="Spirit" :model-value="modelValue.spirit" @update:model-value="setAttribute('Spirit', $event)" />
      </div>
      <div class="col-attributes">
        <AttributeBonusInput attribute="Intellect" :model-value="modelValue.intellect" @update:model-value="setAttribute('Intellect', $event)" />
      </div>
      <div class="col-attributes">
        <AttributeBonusInput attribute="Presence" :model-value="modelValue.presence" @update:model-value="setAttribute('Presence', $event)" />
      </div>
      <div class="col-attributes">
        <AttributeBonusInput attribute="Sensitivity" :model-value="modelValue.sensitivity" @update:model-value="setAttribute('Sensitivity', $event)" />
      </div>
      <div class="col-attributes">
        <AttributeBonusInput attribute="Vigor" :model-value="modelValue.vigor" @update:model-value="setAttribute('Vigor', $event)" />
      </div>
      <div class="col-attributes">
        <ExtraAttributesInput :attributes="modelValue" :model-value="modelValue.extra" @update:model-value="setExtra($event ?? 0)" />
      </div>
    </div>
  </div>
</template>

<style scoped>
@media (min-width: 992px) {
  .col-attributes {
    flex: 0 0 auto;
    width: 12.5%;
  }
}
</style>
