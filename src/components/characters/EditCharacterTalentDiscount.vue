<template>
  <TarCard>
    <div class="card-text">
      <div class="row">
        <div class="col-lg-4">
          <CharacterTalentDiscountSourceField class="mb-3" :id="`${id}-source`" :model-value="modelValue.source" required @update:model-value="updateSource" />
        </div>
        <div class="col-lg-4">
          <div class="mb-3">
            <CharacterTalentDiscountTargetField
              class="mb-3"
              :id="`${id}-target`"
              :label="label"
              :model-value="modelValue.target"
              :options="options"
              :placeholder="placeholder"
              required
              @update:model-value="updateTarget"
            />
          </div>
        </div>
        <div class="col-lg-4">
          <CharacterTalentDiscountAmountField class="mb-3" :id="`${id}-amount`" :model-value="modelValue.amount" required @update:model-value="updateAmount" />
        </div>
      </div>
    </div>
    <div class="d-flex justify-content-end">
      <TarButton icon="fas fa-xmark" outline :text="t('actions.remove')" variant="danger" @click="$emit('removed')" />
    </div>
  </TarCard>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import CharacterTalentDiscountAmountField from "./CharacterTalentDiscountAmountField.vue";
import CharacterTalentDiscountSourceField from "./CharacterTalentDiscountSourceField.vue";
import CharacterTalentDiscountTargetField from "./CharacterTalentDiscountTargetField.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarCard from "@/components/tar/TarCard.vue";
import type { CharacterTalentDiscount, CharacterTalentDiscountSource } from "@/types/characters";
import type { Customization } from "@/types/customizations";
import type { Lineage } from "@/types/lineages";
import type { SelectOption } from "@/types/tar/select";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    customizations?: Customization[];
    id: string;
    lineage: Lineage;
    modelValue: CharacterTalentDiscount;
  }>(),
  {
    customizations: () => [],
  },
);

const emit = defineEmits<{
  (e: "removed"): void;
  (e: "update:model-value", value: CharacterTalentDiscount): void;
}>();

const label = computed<string | undefined>(() => {
  switch (props.modelValue.source) {
    case "Customization":
      return "customizations.label";
    case "Lineage":
      return "lineages.label";
  }
});
const options = computed<SelectOption[] | undefined>(() => {
  switch (props.modelValue.source) {
    case "Customization":
      return orderBy(
        props.customizations.map(({ id, name }) => ({ text: name, value: id })),
        "text",
      );
    case "Lineage":
      const lineages: Lineage[] = [props.lineage];
      if (props.lineage.parent) {
        lineages.push(props.lineage.parent);
      }
      return orderBy(
        lineages.map(({ id, name }) => ({ text: name, value: id })),
        "text",
      );
  }
});
const placeholder = computed<string | undefined>(() => {
  switch (props.modelValue.source) {
    case "Customization":
      return "customizations.placeholder";
    case "Lineage":
      return "lineages.placeholder";
  }
});

function updateAmount(amount: number): void {
  emit("update:model-value", { ...props.modelValue, amount });
}
function updateSource(source: string): void {
  emit("update:model-value", { ...props.modelValue, source: source as CharacterTalentDiscountSource, target: "" });
}
function updateTarget(target: string): void {
  emit("update:model-value", { ...props.modelValue, target });
}
</script>
