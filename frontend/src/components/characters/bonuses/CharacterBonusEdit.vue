<script setup lang="ts">
import { TarButton, TarCheckbox, TarModal } from "logitar-vue3-ui";
import { computed, ref, watchEffect } from "vue";
import { nanoid } from "nanoid";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import AttributeSelect from "@/components/game/AttributeSelect.vue";
import BonusCategorySelect from "./BonusCategorySelect.vue";
import BonusValueInput from "./BonusValueInput.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import MiscellaneousBonusSelect from "./MiscellaneousBonusSelect.vue";
import NameInput from "@/components/shared/NameInput.vue";
import SkillSelect from "@/components/game/SkillSelect.vue";
import SpeedSelect from "@/components/game/SpeedSelect.vue";
import StatisticSelect from "@/components/game/StatisticSelect.vue";
import type { Attribute, Skill, Speed, Statistic } from "@/types/game";
import type { BonusCategory, BonusModel, BonusPayload, CharacterModel, MiscellaneousBonusTarget } from "@/types/characters";
import { addBonus, saveBonus } from "@/api/characters";
import { useToastStore } from "@/stores/toast";

const toasts = useToastStore();
const { t } = useI18n();

const props = defineProps<{
  bonus?: BonusModel;
  character: CharacterModel;
}>();

const category = ref<BonusCategory>();
const isTemporary = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const notes = ref<string>("");
const precision = ref<string>("");
const target = ref<string>();
const value = ref<number>(0);

// TODO(fpion): BEGIN REFACTOR
const attribute = computed<Attribute | undefined>(() => (target.value ? (target.value as Attribute) : undefined));
const miscellaneous = computed<MiscellaneousBonusTarget | undefined>(() => (target.value ? (target.value as MiscellaneousBonusTarget) : undefined));
const skill = computed<Skill | undefined>(() => (target.value ? (target.value as Skill) : undefined));
const speed = computed<Speed | undefined>(() => (target.value ? (target.value as Speed) : undefined));
const statistic = computed<Statistic | undefined>(() => (target.value ? (target.value as Statistic) : undefined));
// TODO(fpion): END REFACTOR

const hasChanges = computed<boolean>(
  () =>
    category.value !== (props.bonus?.category ?? undefined) ||
    (target.value ?? "") !== (props.bonus?.target ?? "") ||
    value.value !== (props.bonus?.value ?? 0) ||
    isTemporary.value !== (props.bonus?.isTemporary ?? false) ||
    precision.value !== (props.bonus?.precision ?? "") ||
    notes.value !== (props.bonus?.notes ?? ""),
);
const id = computed<string>(() => (props.bonus ? `edit-bonus-${props.bonus.id ?? nanoid()}` : "add-bonus"));

function hide(): void {
  modalRef.value?.hide();
}

function setModel(model?: BonusModel): void {
  category.value = model?.category;
  isTemporary.value = model?.isTemporary ?? false;
  notes.value = model?.notes ?? "";
  precision.value = model?.precision ?? "";
  target.value = model?.target ?? undefined;
  value.value = model?.value ?? 0;
}

function onCancel(): void {
  setModel(props.bonus);
  hide();
}

function setCategory(value?: BonusCategory): void {
  category.value = value;
  target.value = undefined;
}

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: CharacterModel): void;
}>();

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  if (category.value && target.value) {
    try {
      const payload: BonusPayload = {
        category: category.value,
        target: target.value,
        value: value.value,
        isTemporary: isTemporary.value,
        precision: precision.value,
        notes: notes.value,
      };
      let character: CharacterModel | undefined = undefined;
      if (props.bonus) {
        character = await saveBonus(props.character.id, props.bonus.id, payload);
        toasts.success("characters.bonuses.edited");
      } else {
        character = await addBonus(props.character.id, payload);
        toasts.success("characters.bonuses.added");
      }
      emit("updated", character);
    } catch (e: unknown) {
      emit("error", e);
    }
  }
  onCancel();
});

watchEffect(() => {
  const bonus: BonusModel | undefined = props.bonus;
  setModel(bonus);
});
</script>

<template>
  <span>
    <TarButton
      :icon="bonus ? 'fas fa-edit' : 'fas fa-plus'"
      :text="t(bonus ? 'actions.edit' : 'actions.add')"
      :variant="bonus ? 'primary' : 'success'"
      data-bs-toggle="modal"
      :data-bs-target="`#${id}`"
    />
    <TarModal :close="t('actions.close')" :id="id" ref="modalRef" size="x-large" :title="t(bonus ? 'characters.bonuses.edit' : 'characters.bonuses.add')">
      <form @submit.prevent="onSubmit">
        <div class="row">
          <BonusCategorySelect
            class="col"
            :disabled="Boolean(bonus)"
            :model-value="category"
            :required="!bonus"
            :validation="bonus ? 'server' : undefined"
            @update:model-value="setCategory"
          />
          <AttributeSelect
            v-if="category === 'Attribute'"
            class="col"
            :disabled="Boolean(bonus)"
            :model-value="attribute"
            :required="!bonus"
            :validation="bonus ? 'server' : undefined"
            @update:model-value="target = $event"
          />
          <MiscellaneousBonusSelect
            v-else-if="category === 'Miscellaneous'"
            class="col"
            :disabled="Boolean(bonus)"
            :model-value="miscellaneous"
            :required="!bonus"
            :validation="bonus ? 'server' : undefined"
            @update:model-value="target = $event"
          />
          <SkillSelect
            v-else-if="category === 'Skill'"
            class="col"
            :disabled="Boolean(bonus)"
            :model-value="skill"
            :required="!bonus"
            :validation="bonus ? 'server' : undefined"
            @update:model-value="target = $event"
          />
          <SpeedSelect
            v-else-if="category === 'Speed'"
            class="col"
            :disabled="Boolean(bonus)"
            :model-value="speed"
            :required="!bonus"
            :validation="bonus ? 'server' : undefined"
            @update:model-value="target = $event"
          />
          <StatisticSelect
            v-else-if="category === 'Statistic'"
            class="col"
            :disabled="Boolean(bonus)"
            :model-value="statistic"
            :required="!bonus"
            :validation="bonus ? 'server' : undefined"
            @update:model-value="target = $event"
          />
        </div>
        <div class="row">
          <BonusValueInput class="col" required v-model="value" />
          <NameInput class="col" id="precision" label="characters.bonuses.precision" placeholder="characters.bonuses.precision" v-model="precision" />
        </div>
        <TarCheckbox class="mb-3" id="temporary" :label="t('characters.bonuses.temporary')" v-model="isTemporary" />
        <DescriptionTextarea id="notes" label="characters.bonuses.notes" placeholder="characters.bonuses.notes" rows="5" v-model="notes" />
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="onCancel" />
        <TarButton
          :disabled="isSubmitting || !hasChanges"
          :icon="bonus ? 'fas fa-save' : 'fas fa-plus'"
          :loading="isSubmitting"
          :status="t('loading')"
          :text="t(bonus ? 'actions.save' : 'actions.add')"
          :variant="bonus ? 'primary' : 'success'"
          @click="onSubmit"
        />
      </template>
    </TarModal>
  </span>
</template>
