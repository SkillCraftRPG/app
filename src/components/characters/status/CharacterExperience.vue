<template>
  <div class="h-100">
    <TarCard class="clickable h-100" @click="open">
      <div class="d-flex justify-content-between align-items-center gap-2">
        <div class="fw-semibold">{{ t("characters.level.label") }}&nbsp;{{ n(character.level, "integer") }}</div>
        <div class="fw-semibold">{{ t("characters.tier") }}&nbsp;{{ n(character.tier, "integer") }}</div>
      </div>
      <div class="d-flex justify-content-between align-items-center gap-2">
        <div class="fw-semibold">{{ label }}</div>
        <div class="text-warning">{{ n(character.experience, "integer") }} / {{ n(maximum, "integer") }}</div>
      </div>
      <TarProgress :aria-label="label" class="mt-1" :label="progress.label" :value="progress.value" variant="warning" />
    </TarCard>
    <TarModal centered :close="t('actions.close')" fade scrollable ref="modal" :title="label">
      <section>
        <div class="d-flex align-items-center">
          <div class="flex-fill text-start">{{ minimum }}</div>
          <div class="flex-fill text-center fw-semibold">{{ n(character.experience, "integer") }}</div>
          <div class="flex-fill text-end">{{ maximum }}</div>
        </div>
        <TarProgress :aria-label="label" class="my-1" :label="progress.label" :value="progress.value" variant="warning" />
        <div class="mb-3">
          {{ t("characters.experience.toNextLevel") }}&nbsp;<span class="fw-semibold">{{ n(toNextLevel, "integer") }}</span>
        </div>
      </section>
      <form @submit.prevent="handleSubmit(submit)">
        <ExperienceField class="mb-3" label="characters.experience.gain" required v-model="experience">
          <template #append>
            <TarButton :disabled="expected.level === MAXIMUM_LEVEL" icon="fas fa-arrow-turn-up" outline :text="t('characters.level.up')" @click="levelUp" />
          </template>
        </ExperienceField>
      </form>
      <section>
        <div class="row">
          <div class="col">{{ t("characters.level.label") }}</div>
          <div class="col text-center">{{ n(character.level, "integer") }}</div>
          <div class="col d-flex justify-content-between align-items-center gap-2">
            <div>→</div>
            <div class="fw-semibold" :class="{ 'text-primary': isLevelUp }">{{ n(expected.level, "integer") }}</div>
          </div>
        </div>
        <div class="row">
          <div class="col">{{ t("game.statistic.options.Vitality") }}</div>
          <div class="col text-center">{{ n(character.statistics.vitality.total, "integer") }}</div>
          <div class="col d-flex justify-content-between align-items-center gap-2">
            <div>→</div>
            <div class="fw-semibold" :class="{ 'text-primary': isLevelUp }">{{ n(expected.vitality, "integer") }}</div>
          </div>
        </div>
        <div class="row">
          <div class="col">{{ t("game.statistic.options.Stamina") }}</div>
          <div class="col text-center">{{ n(character.statistics.stamina.total, "integer") }}</div>
          <div class="col d-flex justify-content-between align-items-center gap-2">
            <div>→</div>
            <div class="fw-semibold" :class="{ 'text-primary': isLevelUp }">{{ n(expected.stamina, "integer") }}</div>
          </div>
        </div>
        <div class="row">
          <div class="col">{{ t("game.statistic.options.Learning") }}</div>
          <div class="col text-center">{{ n(character.statistics.learning.total, "integer") }}</div>
          <div class="col d-flex justify-content-between align-items-center gap-2">
            <div>→</div>
            <div class="fw-semibold" :class="{ 'text-primary': isLevelUp }">{{ n(expected.learning, "integer") }}</div>
          </div>
        </div>
      </section>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
        <TarButton
          :disabled="!experience || isLoading"
          icon="fas fa-floppy-disk"
          :loading="isLoading"
          :status="t('loading')"
          :text="t('actions.save')"
          @click="handleSubmit(submit)"
        />
      </template>
    </TarModal>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import ExperienceField from "./ExperienceField.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarCard from "@/components/tar/TarCard.vue";
import TarModal from "@/components/tar/TarModal.vue";
import TarProgress from "@/components/tar/TarProgress.vue";
import type { Character, GainCharacterExperiencePayload } from "@/types/characters";
import { MAXIMUM_LEVEL, getLevel, getThreshold } from "@/utils/experience";
import { gainCharacterExperience } from "@/api/characters";
import { useForm } from "@/forms";

const { n, t } = useI18n();

const props = defineProps<{
  character: Character;
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Character): void;
}>();

const experience = ref<number>(0);
const isLoading = ref<boolean>(false);
const modal = ref<InstanceType<typeof TarModal> | null>(null);

const label = computed<string>(() => t("characters.experience.label"));
const minimum = computed<number>(() => getThreshold(props.character.level));
const maximum = computed<number>(() => getThreshold(Math.min(props.character.level + 1, MAXIMUM_LEVEL)));
const toNextLevel = computed<number>(() => maximum.value - props.character.experience);
const progress = computed(() => {
  const value: number = Math.floor(((props.character.experience - minimum.value) / (maximum.value - minimum.value)) * 100);
  return { label: n(value / 100, "percentage"), value };
});
const expected = computed(() => {
  const level: number = getLevel(props.character.experience + experience.value);
  const constitution: number = Math.floor(((25 + level) * (5 + props.character.attributes.health.total)) / 5);
  let vitality: number = constitution;
  let stamina: number = constitution;
  let learning: number = Math.max(
    Math.floor(5 + props.character.attributes.intellect.total + (level / 5) * (2 + props.character.attributes.intellect.total)),
    Math.floor(5 + level / 5),
  );
  props.character.modifiers.forEach((modifier) => {
    if (modifier.kind === "Statistic") {
      switch (modifier.target) {
        case "Learning":
          learning += modifier.value;
          break;
        case "Stamina":
          stamina += modifier.value;
          break;
        case "Vitality":
          vitality += modifier.value;
          break;
      }
    }
  });
  return { level, vitality, stamina, learning };
});
const isLevelUp = computed<boolean>(() => props.character.level < expected.value.level);

function levelUp(): void {
  if (expected.value.level < MAXIMUM_LEVEL) {
    experience.value = getThreshold(expected.value.level + 1) - props.character.experience;
  }
}

const { handleSubmit, reinitialize, reset } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const payload: GainCharacterExperiencePayload = {
        experience: experience.value,
      };
      const character: Character = await gainCharacterExperience(props.character.id, payload);
      emit("updated", character);
      experience.value = 0;
      reinitialize();
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

function open(): void {
  modal.value?.show();
}
</script>
