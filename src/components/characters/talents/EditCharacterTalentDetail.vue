<template>
  <div>
    <TalentCard class="mb-3" :talent="talent" />
    <section>
      <NameField class="mb-3" id="qualifier" label="characters.talents.qualifier" :model-value="modelValue.qualifier" @update:model-value="updateQualifier" />
      <NotesField class="mb-3" :model-value="modelValue.qualifier" @update:model-value="updateNotes" />
    </section>
    <section class="row mb-3 text-center">
      <div class="col">
        <div class="fw-bold">{{ t("characters.talents.cost.base") }}</div>
        <div>{{ n(talent.cost, "integer") }}</div>
      </div>
      <div class="col">
        <div class="fw-bold">{{ t("characters.talents.cost.effective") }}</div>
        <div>{{ n(effectiveCost, "integer") }}</div>
      </div>
    </section>
    <section>
      <h2 class="h6">{{ t("characters.talents.discount.title") }}</h2>
      <div class="mb-3">
        <TarButton icon="fas fa-plus" outline :text="t('actions.add')" @click="addDiscount" />
      </div>
      <EditCharacterTalentDiscount
        v-for="(discount, index) in modelValue.discounts"
        :key="index"
        class="mb-3"
        :context="context"
        :id="`discount-${index}`"
        :max="talent.cost"
        :model-value="discount"
        @remove="removeDiscount(index)"
        @update:model-value="updateDiscount(index, $event)"
      />
    </section>
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import EditCharacterTalentDiscount from "./EditCharacterTalentDiscount.vue";
import NameField from "@/components/shared/NameField.vue";
import NotesField from "@/components/shared/NotesField.vue";
import TalentCard from "@/components/talents/TalentCard.vue";
import TarButton from "@/components/tar/TarButton.vue";
import type { CharacterTalentContext, CharacterTalentDetail, CharacterTalentDiscount } from "@/types/characters";
import type { Talent } from "@/types/talents";
import { calculateCost } from "@/utils/talent";

const { n, t } = useI18n();

const props = defineProps<{
  context: CharacterTalentContext;
  modelValue: CharacterTalentDetail;
  talent: Talent;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: CharacterTalentDetail): void;
}>();

const effectiveCost = computed<number>(() => calculateCost(props.talent, props.modelValue.discounts));

function updateNotes(notes: string): void {
  emit("update:model-value", { ...props.modelValue, notes });
}

function updateQualifier(qualifier: string): void {
  emit("update:model-value", { ...props.modelValue, qualifier });
}

function addDiscount(): void {
  const discounts: CharacterTalentDiscount[] = props.modelValue.discounts.map((discount) => ({ ...discount }));
  discounts.push({ source: "Lineage", target: "", amount: 1 });
  emit("update:model-value", { ...props.modelValue, discounts });
}
function removeDiscount(index: number): void {
  const discounts: CharacterTalentDiscount[] = props.modelValue.discounts.map((discount) => ({ ...discount }));
  discounts.splice(index, 1);
  emit("update:model-value", { ...props.modelValue, discounts });
}
function updateDiscount(index: number, discount: CharacterTalentDiscount): void {
  const discounts: CharacterTalentDiscount[] = props.modelValue.discounts.map((discount) => ({ ...discount }));
  discounts.splice(index, 1, { ...discount });
  emit("update:model-value", { ...props.modelValue, discounts });
}

// TODO(fpion): rename `detail` to `acquisition`
</script>
