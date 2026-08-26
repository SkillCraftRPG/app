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
          <div class="flex-fill text-start">{{ n(minimum, "integer") }}</div>
          <div class="flex-fill text-center fw-semibold">{{ n(character.experience, "integer") }}</div>
          <div class="flex-fill text-end">{{ n(maximum, "integer") }}</div>
        </div>
        <TarProgress :aria-label="label" class="my-1" :label="progress.label" :value="progress.value" variant="warning" />
        <div class="mb-3">
          {{ t("characters.experience.toNextLevel") }}&nbsp;<span class="fw-semibold">{{ n(toNextLevel, "integer") }}</span>
        </div>
      </section>
      <form @submit.prevent="handleSubmit(submit)">
        <ExperienceField class="mb-3" label="characters.experience.gain" required v-model="experience">
          <template #append>
            <TarButton :disabled="level >= MAXIMUM_LEVEL" icon="fas fa-arrow-turn-up" outline :text="t('characters.level.up')" @click="levelUp" />
          </template>
        </ExperienceField>
      </form>
      <table class="table table-sm">
        <tbody>
          <tr v-for="impact in impacts" :key="impact.key">
            <td class="w-third text-body-secondary">{{ impact.label }}</td>
            <td class="w-third text-center">{{ impact.current }}</td>
            <td class="w-sixth text-center" :class="{ 'text-primary': impact.delta }">{{ formatSignedInteger(impact.delta, n) }}</td>
            <td class="w-sixth text-end fw-semibold" :class="{ 'text-primary': impact.delta }">{{ impact.result }}</td>
          </tr>
        </tbody>
      </table>
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
import type { Attribute } from "@/types/game";
import type { Character, GainCharacterExperiencePayload } from "@/types/characters";
import type { ProgressData } from "@/types/progress";
import { MAXIMUM_LEVEL, getLevel, getThreshold } from "@/utils/experience";
import { calculateAttributePoints, getAttributeTotals } from "@/utils/character";
import { calculateProgress } from "@/utils/progress";
import { calculateStatisticNew } from "@/utils/game";
import { formatSignedInteger } from "@/utils/format";
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

const progress = computed<ProgressData>(() => calculateProgress((props.character.experience - minimum.value) / (maximum.value - minimum.value), n));

type Impact = {
  key: string;
  label: string;
  current: string;
  delta: number;
  result: string;
};
const attributes = computed<Map<Attribute, number>>(() => getAttributeTotals(props.character));
const level = computed<number>(() => getLevel(props.character.experience + experience.value));
const impacts = computed<Impact[]>(() => {
  const attributeDelta: number = calculateAttributePoints(level.value) - calculateAttributePoints(props.character.level);
  const vitality: number = calculateStatisticNew("Vitality", attributes.value, level.value, props.character.modifiers);
  const stamina: number = calculateStatisticNew("Stamina", attributes.value, level.value, props.character.modifiers);
  const learning: number = calculateStatisticNew("Learning", attributes.value, level.value, props.character.modifiers);
  return [
    {
      key: "level",
      label: t("characters.level.label"),
      current: n(props.character.level, "integer"),
      delta: level.value - props.character.level,
      result: n(level.value, "integer"),
    },
    {
      key: "attributes",
      label: t("characters.attributes.title"),
      current: formatSignedInteger(props.character.points.attributes, n),
      delta: attributeDelta,
      result: formatSignedInteger(props.character.points.attributes + attributeDelta, n),
    },
    {
      key: "vitality",
      label: t("game.statistic.options.Vitality"),
      current: n(props.character.statistics.vitality.total, "integer"),
      delta: vitality - props.character.statistics.vitality.total,
      result: n(vitality, "integer"),
    },
    {
      key: "stamina",
      label: t("game.statistic.options.Stamina"),
      current: n(props.character.statistics.stamina.total, "integer"),
      delta: stamina - props.character.statistics.stamina.total,
      result: n(stamina, "integer"),
    },
    {
      key: "learning",
      label: t("game.statistic.options.Learning"),
      current: n(props.character.statistics.learning.total, "integer"),
      delta: learning - props.character.statistics.learning.total,
      result: n(learning, "integer"),
    },
  ];
});

function levelUp(): void {
  if (level.value < MAXIMUM_LEVEL) {
    experience.value = getThreshold(level.value + 1) - props.character.experience;
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
