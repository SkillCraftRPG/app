<template>
  <TarModal :close="t('actions.close')" fade ref="modal" size="large" :title="title">
    <form @submit.prevent="handleSubmit(submit)">
      <div class="row">
        <div class="col-md-6">
          <div v-if="modifier" class="mb-3">
            <div class="small text-body-secondary">{{ t("characters.modifiers.kind.label") }}</div>
            <div class="fw-semibold">{{ kindText }}</div>
          </div>
          <CharacterModifierKindField v-else class="mb-3" :model-value="kind" required @update:model-value="updateKind" />
        </div>
        <div class="col-md-6">
          <div v-if="modifier" class="mb-3">
            <div class="small text-body-secondary">{{ kindText }}</div>
            <div class="fw-semibold">{{ targetText }}</div>
          </div>
          <AttributeField v-else-if="kind === 'Attribute'" class="mb-3" required v-model="target" />
          <SkillField v-else-if="kind === 'Skill'" class="mb-3" required v-model="target" />
          <SpeedKindField v-else-if="kind === 'Speed'" class="mb-3" required v-model="target" />
          <StatisticField v-else-if="kind === 'Statistic'" class="mb-3" required v-model="target" />
        </div>
        <div class="col-md-6">
          <NameField class="mb-3" v-model="name" />
        </div>
        <div class="col-md-6">
          <CharacterModifierValueField class="mb-3" required v-model="value" />
        </div>
      </div>
      <NotesField class="mb-3" v-model="notes" />
    </form>
    <template #footer>
      <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
      <TarButton :disabled="isLoading" :icon="submitIcon" :loading="isLoading" :status="t('loading')" :text="submitText" @click="handleSubmit(submit)" />
    </template>
  </TarModal>
</template>

<script setup lang="ts">
import { computed, nextTick, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import AttributeField from "@/components/game/AttributeField.vue";
import CharacterModifierKindField from "./CharacterModifierKindField.vue";
import CharacterModifierValueField from "./CharacterModifierValueField.vue";
import NameField from "@/components/shared/NameField.vue";
import NotesField from "@/components/shared/NotesField.vue";
import SkillField from "@/components/game/SkillField.vue";
import SpeedKindField from "@/components/game/SpeedKindField.vue";
import StatisticField from "@/components/game/StatisticField.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarModal from "@/components/tar/TarModal.vue";
import type { Character, CharacterModifier, CharacterModifierKind, CreateOrReplaceCharacterModifierPayload } from "@/types/characters";
import { createCharacterModifier, replaceCharacterModifier } from "@/api/characters";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  character: Character;
  modifier?: CharacterModifier;
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Character): void;
}>();

const isLoading = ref<boolean>(false);
const kind = ref<string>("");
const modal = ref<InstanceType<typeof TarModal> | null>(null);
const name = ref<string>("");
const notes = ref<string>("");
const target = ref<string>("");
const value = ref<number>(0);

const submitIcon = computed<string>(() => (props.modifier ? "fas fa-floppy-disk" : "fas fa-plus"));
const submitText = computed<string>(() => t(props.modifier ? "actions.save" : "actions.add"));
const title = computed<string>(() => t(props.modifier ? "characters.modifiers.edit" : "characters.modifiers.create"));

const kindText = computed<string | undefined>(() => {
  if (props.modifier) {
    switch (props.modifier.kind) {
      case "Attribute":
        return t("game.attribute.label");
      case "Skill":
        return t("game.skill.label");
      case "Speed":
        return t("game.speed.label");
      case "Statistic":
        return t("game.statistic.label");
    }
  }
});
const targetText = computed<string | undefined>(() => {
  if (props.modifier) {
    switch (props.modifier.kind) {
      case "Attribute":
        return t(`game.attribute.options.${props.modifier.target}`);
      case "Skill":
        return t(`game.skill.options.${props.modifier.target}`);
      case "Speed":
        return t(`game.speed.kind.options.${props.modifier.target}`);
      case "Statistic":
        return t(`game.statistic.options.${props.modifier.target}`);
    }
  }
});

const { handleSubmit, reinitialize, reset } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const payload: CreateOrReplaceCharacterModifierPayload = {
        kind: kind.value as CharacterModifierKind,
        target: target.value,
        value: value.value,
        name: name.value,
        notes: notes.value,
      };
      const character: Character = props.modifier
        ? await replaceCharacterModifier(props.character.id, props.modifier.id, payload)
        : await createCharacterModifier(props.character.id, payload);
      emit("updated", character);
      modal.value?.hide();
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

function cancel(): void {
  reset();
  modal.value?.hide();
}

function updateKind(value: string): void {
  kind.value = value;
  target.value = "";
}

watch(
  () => props.modifier,
  (modifier) => {
    kind.value = modifier?.kind ?? "";
    target.value = modifier?.target ?? "";
    value.value = modifier?.value ?? 0;
    name.value = modifier?.name ?? "";
    notes.value = modifier?.notes ?? "";
    nextTick(reinitialize);
  },
  { deep: true, immediate: true },
);

function open(): void {
  modal.value?.show();
}
defineExpose({ open });
</script>
