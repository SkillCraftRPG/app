<template>
  <TarModal centered :close="t('actions.close')" fade scrollable ref="modal" size="x-large" :title="title">
    <form @submit.prevent="handleSubmit(submit)">
      <SelectCharacterTalent v-if="step === 'select'" :context="context" ref="select" :selected="talent" :talents="talents" @toggle="toggle" />
      <EditCharacterTalentDetail v-else-if="step === 'detail' && talent" :context="context" :talent="talent" v-model="detail" />
    </form>
    <template #footer>
      <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
      <TarButton v-if="canGoBack" icon="fas fa-arrow-left" outline :text="t('actions.previous')" @click="goBack" />
      <TarButton :disabled="!canSubmit" :icon="submitIcon" :text="submitText" @click="handleSubmit(submit)" />
    </template>
  </TarModal>
</template>

<script setup lang="ts">
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import EditCharacterTalentDetail from "./EditCharacterTalentDetail.vue";
import SelectCharacterTalent from "./SelectCharacterTalent.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarModal from "@/components/tar/TarModal.vue";
import type { CharacterTalent, CharacterTalentContext, CharacterTalentDetail } from "@/types/characters";
import type { Talent } from "@/types/talents";
import { SYSTEM } from "@/types/api";
import { calculateCost } from "@/utils/talent";
import { useForm } from "@/forms";

type Step = "select" | "detail";

const { t } = useI18n();

const props = defineProps<{
  acquisition?: CharacterTalent;
  context: CharacterTalentContext;
  talents: Talent[];
}>();

const emit = defineEmits<{
  (e: "confirm", value: CharacterTalent): void;
}>();

const detail = ref<CharacterTalentDetail>({ qualifier: "", notes: "", discounts: [] });
const modal = ref<InstanceType<typeof TarModal> | null>(null);
const select = ref<InstanceType<typeof SelectCharacterTalent> | null>(null);
const step = ref<Step>();
const talent = ref<Talent>();

const canGoBack = computed<boolean>(() => !props.acquisition && step.value === "detail");
const canSubmit = computed<boolean>(() => Boolean(step.value === "detail" || talent.value));
const submitIcon = computed<string>(() => {
  if (step.value === "select") {
    return "fas fa-arrow-right";
  }
  return props.acquisition ? "fas fa-floppy-disk" : "fas fa-plus";
});
const submitText = computed<string>(() => {
  if (step.value === "select") {
    return t("actions.next");
  }
  return props.acquisition ? t("actions.save") : t("actions.add");
});
const title = computed<string>(() => t(props.acquisition ? "characters.talents.edit" : "characters.talents.add"));

function clear(): void {
  select.value?.clearFilters();
  if (props.acquisition) {
    step.value = "detail";
  } else {
    talent.value = undefined;
    step.value = "select";
  }
  detail.value = { qualifier: "", notes: "", discounts: [] };
  reset();
}

function cancel(): void {
  clear();
  modal.value?.hide();
}

function goBack(): void {
  if (canGoBack.value) {
    step.value = "select";
  }
}

function toggle(value: Talent): void {
  if (talent.value?.id === value.id) {
    talent.value = undefined;
  } else {
    talent.value = value;
  }
}

const { handleSubmit, reset } = useForm();
function submit(): void {
  if (canSubmit.value) {
    switch (step.value) {
      case "detail":
        if (talent.value) {
          const acquisition: CharacterTalent = {
            id: props.acquisition?.id ?? "",
            talent: talent.value,
            qualifier: detail.value.qualifier,
            notes: detail.value.notes,
            discounts: detail.value.discounts,
            cost: calculateCost(talent.value, detail.value.discounts),
            createdBy: props.acquisition?.createdBy ?? SYSTEM,
            createdOn: props.acquisition?.createdOn ?? "",
            updatedBy: props.acquisition?.updatedBy ?? SYSTEM,
            updatedOn: props.acquisition?.updatedOn ?? "",
          };
          emit("confirm", acquisition);
          clear();
          modal.value?.hide();
        }
        break;
      case "select":
        step.value = "detail";
        break;
    }
  }
}

watch(
  () => props.acquisition,
  (acquisition) => {
    talent.value = acquisition?.talent;
    detail.value = {
      qualifier: acquisition?.qualifier ?? "",
      notes: acquisition?.notes ?? "",
      discounts: acquisition?.discounts.map((discount) => ({ ...discount })) ?? [],
    };
    step.value = talent.value ? "detail" : "select";
  },
  { deep: true, immediate: true },
);

function open(): void {
  modal.value?.show();
}
defineExpose({ open });

// TODO(fpion): see EditCharacterModifierModal for fix(es)
// TODO(fpion): rename `detail` to `acquisition`
</script>
