<template>
  <div>
    <TarButton icon="fas fa-plus" size="large" :text="t('actions.add')" @click="open" />
    <TarModal centered :close="t('actions.close')" fade ref="modal" scrollable size="x-large" :title="t('characters.talents.add')">
      <SelectCharacterTalent v-if="step === 'select'" :context="context" ref="select" :selected="talent" :talents="talents" @toggle="toggle" />
      <CharacterTalentForm v-else-if="talent" :context="context" ref="form" :talent="talent" @saved="submit" />
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
        <TarButton v-if="step === 'select'" :disabled="!talent" icon="fas fa-arrow-right" :text="t('actions.next')" @click="next" />
        <template v-else>
          <TarButton icon="fas fa-arrow-left" outline :text="t('actions.previous')" @click="previous" />
          <TarButton icon="fas fa-plus" :text="t('actions.add')" @click="add" />
        </template>
      </template>
    </TarModal>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import CharacterTalentForm from "./CharacterTalentForm.vue";
import SelectCharacterTalent from "./SelectCharacterTalent.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarModal from "@/components/tar/TarModal.vue";
import type { CharacterTalent, CharacterTalentContext, CharacterTalentDetail } from "@/types/characters";
import type { Talent } from "@/types/talents";

type Step = "select" | "form";

const { t } = useI18n();

defineProps<{
  context: CharacterTalentContext;
  talents: Talent[];
}>();

const emit = defineEmits<{
  (e: "added", value: CharacterTalent): void;
}>();

const form = ref<InstanceType<typeof CharacterTalentForm> | null>(null);
const modal = ref<InstanceType<typeof TarModal> | null>(null);
const select = ref<InstanceType<typeof SelectCharacterTalent> | null>(null);
const step = ref<Step>("select");
const talent = ref<Talent>();

function add(): void {
  form.value?.submit();
}

function cancel(): void {
  select.value?.clearFilters();
  form.value?.reset();
  step.value = "select";
  talent.value = undefined;
  modal.value?.hide();
}

function next(): void {
  if (step.value === "select") {
    step.value = "form";
  }
}

function previous(): void {
  if (step.value === "form") {
    step.value = "select";
  }
}

function submit(detail: CharacterTalentDetail): void {
  if (talent.value) {
    emit("added", {
      id: "",
      talent: talent.value,
      qualifier: detail.qualifier.trim(),
      notes: detail.notes,
      discounts: detail.discounts,
    });
    step.value = "select";
    talent.value = undefined;
    modal.value?.hide();
  }
}

function toggle(value: Talent): void {
  if (talent.value?.id === value.id) {
    talent.value = undefined;
  } else {
    talent.value = value;
  }
}

function open(): void {
  modal.value?.show();
}
</script>
