<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { computed, ref, watchEffect } from "vue";
import { nanoid } from "nanoid";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import NameInput from "@/components/shared/NameInput.vue";
import TalentCostInput from "./TalentCostInput.vue";
import type { CharacterModel, CharacterTalentModel, CharacterTalentPayload } from "@/types/characters";
import type { TalentModel } from "@/types/talents";
import { addCharacterTalent, saveCharacterTalent } from "@/api/characters";
import { useToastStore } from "@/stores/toast";

const toasts = useToastStore();
const { t } = useI18n();

const props = defineProps<{
  character: CharacterModel;
  talent?: CharacterTalentModel;
}>();

const cost = ref<number>(0);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const notes = ref<string>("");
const precision = ref<string>("");
const selectedTalent = ref<TalentModel>();

const hasChanges = computed<boolean>(
  () =>
    selectedTalent.value?.id !== (props.talent?.talent.id ?? undefined) ||
    cost.value !== (props.talent?.cost ?? 0) ||
    precision.value !== (props.talent?.precision ?? "") ||
    notes.value !== (props.talent?.notes ?? ""),
);
const id = computed<string>(() => (props.talent ? `edit-talent-${props.talent.id ?? nanoid()}` : "add-talent"));

function hide(): void {
  modalRef.value?.hide();
}

function setModel(model?: CharacterTalentModel): void {
  cost.value = model?.cost ?? 0;
  notes.value = model?.notes ?? "";
  precision.value = model?.precision ?? "";
  selectedTalent.value = model?.talent ?? undefined;
}

function onCancel(): void {
  setModel(props.talent);
  hide();
}

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: CharacterModel): void;
}>();

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  if (selectedTalent.value) {
    try {
      const payload: CharacterTalentPayload = {
        talentId: selectedTalent.value?.id,
        cost: cost.value,
        precision: precision.value,
        notes: notes.value,
      };
      let character: CharacterModel | undefined = undefined;
      if (props.talent) {
        character = await saveCharacterTalent(props.character.id, props.talent.id, payload);
        toasts.success("characters.talents.edited");
      } else {
        character = await addCharacterTalent(props.character.id, payload);
        toasts.success("characters.talents.added");
      }
      emit("updated", character);
    } catch (e: unknown) {
      emit("error", e);
    }
  }
  onCancel();
});

watchEffect(() => {
  const talent: CharacterTalentModel | undefined = props.talent;
  setModel(talent);
});
</script>

<template>
  <span>
    <TarButton
      :icon="talent ? 'fas fa-edit' : 'fas fa-plus'"
      :text="t(talent ? 'actions.edit' : 'actions.add')"
      :variant="talent ? 'primary' : 'success'"
      data-bs-toggle="modal"
      :data-bs-target="`#${id}`"
    />
    <TarModal :close="t('actions.close')" :id="id" ref="modalRef" :title="t(talent ? 'characters.talents.edit' : 'characters.talents.add')">
      <form @submit.prevent="onSubmit">
        <TalentCostInput v-if="selectedTalent" :tier="selectedTalent.tier" v-model="cost" />
        <NameInput id="precision" label="characters.talents.precision" placeholder="characters.talents.precision" v-model="precision" />
        <DescriptionTextarea id="notes" label="characters.talents.notes" placeholder="characters.talents.notes" rows="5" v-model="notes" />
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="onCancel" />
        <TarButton
          :disabled="isSubmitting || !hasChanges"
          :icon="talent ? 'fas fa-save' : 'fas fa-plus'"
          :loading="isSubmitting"
          :status="t('loading')"
          :text="t(talent ? 'actions.save' : 'actions.add')"
          :variant="talent ? 'primary' : 'success'"
          @click="onSubmit"
        />
      </template>
    </TarModal>
  </span>
</template>
