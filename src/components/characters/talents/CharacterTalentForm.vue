<template>
  <div>
    <TalentCard class="mb-3" :talent="talent" />
    <form @submit.prevent="submit">
      <section>
        <NameField class="mb-3" id="qualifier" label="characters.talents.qualifier" v-model="qualifier" />
        <NotesField class="mb-3" v-model="notes" />
      </section>
      <section class="row mb-3 text-center">
        <div class="col">
          <div class="fw-bold">{{ t("characters.talents.cost.base") }}</div>
          <div>{{ 2 + talent.tier }}</div>
        </div>
        <div class="col">
          <div class="fw-bold">{{ t("characters.talents.cost.effective") }}</div>
          <div>{{ effectiveCost }}</div>
        </div>
      </section>
      <section>
        <h2 class="h6">{{ t("characters.talents.discount.title") }}</h2>
        <div class="mb-3">
          <TarButton icon="fas fa-plus" outline :text="t('actions.add')" @click="add" />
        </div>
        <EditCharacterTalentDiscount
          v-for="(discount, index) in discounts"
          :key="index"
          class="mb-3"
          :customizations="customizations"
          :id="`discount-${index}`"
          :lineage="lineage"
          :max="baseCost"
          :model-value="discount"
          @update:model-value="update(index, $event)"
          @removed="remove(index)"
        />
      </section>
    </form>
  </div>
</template>

<script setup lang="ts">
import { computed, nextTick, ref } from "vue";
import { useI18n } from "vue-i18n";

import EditCharacterTalentDiscount from "./EditCharacterTalentDiscount.vue";
import NameField from "@/components/shared/NameField.vue";
import NotesField from "@/components/shared/NotesField.vue";
import TalentCard from "@/components/talents/TalentCard.vue";
import TarButton from "@/components/tar/TarButton.vue";
import type { CharacterTalentDetail, CharacterTalentDiscount } from "@/types/characters";
import type { Customization } from "@/types/customizations";
import type { Lineage } from "@/types/lineages";
import type { Talent } from "@/types/talents";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    customizations?: Customization[];
    lineage: Lineage;
    talent: Talent;
  }>(),
  {
    customizations: () => [],
  },
);

const emit = defineEmits<{
  (e: "saved", value: CharacterTalentDetail): void;
}>();

const discounts = ref<CharacterTalentDiscount[]>([]);
const notes = ref<string>("");
const qualifier = ref<string>("");

const baseCost = computed<number>(() => 2 + props.talent.tier);
const effectiveCost = computed<number>(() => Math.max(baseCost.value - discounts.value.reduce((sum, discount) => sum + discount.amount, 0), 0));

function add(): void {
  discounts.value.push({ source: "Lineage", target: "", amount: 1 });
}
function remove(index: number): void {
  discounts.value.splice(index, 1);
}
function update(index: number, value: CharacterTalentDiscount): void {
  discounts.value.splice(index, 1, value);
}

const { handleSubmit, reset: resetForm } = useForm();
function reset(): void {
  qualifier.value = "";
  notes.value = "";
  discounts.value = [];
  nextTick(resetForm);
}
function onSubmit(): void {
  emit("saved", {
    qualifier: qualifier.value,
    notes: notes.value,
    discounts: discounts.value,
  });
  reset();
}

function submit(): void {
  handleSubmit(onSubmit);
}
defineExpose({ reset, submit });
</script>
