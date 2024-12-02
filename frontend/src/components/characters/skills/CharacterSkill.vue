<script setup lang="ts">
import { TarButton, TarCard, TarModal } from "logitar-vue3-ui";
import { computed, ref, watch } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import RankInput from "./RankInput.vue";
import type { Attribute, Skill } from "@/types/game";
import type { BonusModel, CharacterModel, CharacterSkillModel, UpdateCharacterPayload } from "@/types/characters";
import { increaseCharacterSkillRank, updateCharacter } from "@/api/characters";
import { useToastStore } from "@/stores/toast";

type AttributeBonus = {
  attribute: Attribute;
  bonus: number;
};

const toasts = useToastStore();
const { t } = useI18n();

const props = defineProps<{
  character: CharacterModel;
  skill: Skill;
  text: string;
}>();

const isLoading = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const rankInput = ref<number>(0);

const characterSkill = computed<CharacterSkillModel>(() => {
  switch (props.skill) {
    case "Acrobatics":
      return props.character.skills.acrobatics;
    case "Athletics":
      return props.character.skills.athletics;
    case "Craft":
      return props.character.skills.craft;
    case "Deception":
      return props.character.skills.deception;
    case "Diplomacy":
      return props.character.skills.diplomacy;
    case "Discipline":
      return props.character.skills.discipline;
    case "Insight":
      return props.character.skills.insight;
    case "Investigation":
      return props.character.skills.investigation;
    case "Knowledge":
      return props.character.skills.knowledge;
    case "Linguistics":
      return props.character.skills.linguistics;
    case "Medicine":
      return props.character.skills.medicine;
    case "Melee":
      return props.character.skills.melee;
    case "Occultism":
      return props.character.skills.occultism;
    case "Orientation":
      return props.character.skills.orientation;
    case "Perception":
      return props.character.skills.perception;
    case "Performance":
      return props.character.skills.performance;
    case "Resistance":
      return props.character.skills.resistance;
    case "Stealth":
      return props.character.skills.stealth;
    case "Survival":
      return props.character.skills.survival;
    case "Thievery":
      return props.character.skills.thievery;
    default:
      throw new Error(`The skill '${props.skill}' is not supported.`);
  }
});
const attribute = computed<AttributeBonus>(() => {
  switch (props.skill) {
    case "Acrobatics":
    case "Melee":
    case "Stealth":
      return { attribute: "Agility", bonus: props.character.attributes.agility.temporaryModifier };
    case "Craft":
    case "Orientation":
    case "Thievery":
      return { attribute: "Coordination", bonus: props.character.attributes.coordination.temporaryModifier };
    case "Investigation":
    case "Knowledge":
    case "Linguistics":
      return { attribute: "Intellect", bonus: props.character.attributes.intellect.temporaryModifier };
    case "Deception":
    case "Diplomacy":
    case "Performance":
      return { attribute: "Presence", bonus: props.character.attributes.presence.temporaryModifier };
    case "Insight":
    case "Medicine":
    case "Perception":
    case "Survival":
      return { attribute: "Sensitivity", bonus: props.character.attributes.sensitivity.temporaryModifier };
    case "Discipline":
    case "Occultism":
      return { attribute: "Spirit", bonus: props.character.attributes.spirit.temporaryModifier };
    case "Athletics":
    case "Resistance":
      return { attribute: "Vigor", bonus: props.character.attributes.vigor.temporaryModifier };
    default:
      throw new Error(`The skill '${props.skill}' is not supported.`);
  }
});
const bonus = computed<number>(() => characterSkill.value.total);
const bonuses = computed<BonusModel[]>(() => props.character.bonuses.filter(({ category, target }) => category === "Skill" && target === props.skill));
const hasChanges = computed<boolean>(() => rankInput.value !== rank.value);
const isTrained = computed<boolean>(() => characterSkill.value.isTrained);
const neutral = computed<number>(() => characterSkill.value.total + 10);
const rank = computed<number>(() => props.character.skillRanks.find(({ skill }) => skill === props.skill)?.rank ?? 0);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: CharacterModel): void;
}>();

function formatBonus(bonus: number): string {
  return bonus > 0 ? `+${bonus}` : bonus.toString();
}

function hide(): void {
  modalRef.value?.hide();
}

function onCancel(): void {
  rankInput.value = rank.value;
  hide();
}

async function onIncrease(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const character: CharacterModel = await increaseCharacterSkillRank(props.character.id, props.skill);
      toasts.success("characters.skills.rank.increased");
      emit("updated", character);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    const payload: UpdateCharacterPayload = {
      skillRanks: [{ skill: props.skill, rank: rankInput.value }],
    };
    const character: CharacterModel = await updateCharacter(props.character.id, payload);
    toasts.success("characters.skills.rank.updated");
    emit("updated", character);
  } catch (e: unknown) {
    emit("error", e);
  }
  onCancel();
});

watch(rank, (rank) => (rankInput.value = rank), { immediate: true });
</script>

<template>
  <span>
    <TarCard :title="text">
      <template v-if="isTrained" #subtitle-override>
        <h6 class="card-subtitle mb-2 text-body-secondary"><font-awesome-icon icon="fas fa-check" /> {{ t("characters.skills.trained") }}</h6>
      </template>
      <div class="mb-3">
        <span>{{ t("characters.skills.rank.format", { rank }) }}</span>
        <span class="float-end"> {{ formatBonus(bonus) }} / {{ neutral }} </span>
      </div>
      <div class="float-end">
        <TarButton
          class="me-1"
          :disabled="isLoading || character.skillPoints.remaining < 1 || rank >= character.maximumSkillRank"
          icon="fas fa-plus"
          :loading="isLoading"
          :text="t('characters.skills.rank.increase')"
          variant="warning"
          @click="onIncrease"
        />
        <TarButton class="ms-1" icon="fas fa-list" :text="t('characters.skills.detail')" data-bs-toggle="modal" :data-bs-target="`#${skill}-detail`" />
      </div>
      <TarModal :close="t('actions.close')" :id="`${skill}-detail`" ref="modalRef" :title="text">
        <div>
          <template v-if="isTrained">{{ t("characters.skills.rank.trained", { rank, bonus: formatBonus(rank) }) }}</template>
          <template v-else>{{ t("characters.skills.rank.untrained", { rank, bonus: formatBonus(Math.floor(rank / 2)) }) }}</template>
        </div>
        <div>{{ t(`game.attribute.options.${attribute.attribute}`) }} ({{ formatBonus(attribute.bonus) }})</div>
        <div v-for="bonus in bonuses" :key="bonus.id">{{ bonus.precision ?? t("characters.bonus") }} ({{ formatBonus(bonus.value) }})</div>
        <div class="my-3">
          <strong>{{ t("characters.skills.totalFormat", { bonus: formatBonus(bonus), neutral }) }}</strong>
        </div>
        <form @submit.prevent="onSubmit">
          <RankInput :max="character.maximumSkillRank" :skill="skill" v-model="rankInput" />
        </form>
        <template #footer>
          <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="onCancel" />
          <TarButton
            :disabled="isSubmitting || !hasChanges"
            icon="fas fa-save"
            :loading="isSubmitting"
            :status="t('loading')"
            :text="t('actions.save')"
            @click="onSubmit"
          />
        </template>
      </TarModal>
    </TarCard>
  </span>
</template>
