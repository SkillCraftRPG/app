<template>
  <section>
    <h2 class="h3">{{ t("talents.title") }}</h2>
    <template v-if="!isLoading">
      <section v-if="talents.length">
        <p class="text-body-secondary">{{ t("characters.talents.help") }}</p>
        <ul class="list-unstyled mt-2 mb-3 small">
          <li v-for="rule in evaluation" :key="rule.key" :class="getClasses(rule)">
            <font-awesome-icon :icon="rule.success ? 'fas fa-check' : 'fas fa-xmark'" />&nbsp;{{ rule.text }}
          </li>
        </ul>
        <AddCharacterTalent v-if="context" class="mb-3" :context="context" :talents="talents" @added="add" />
        <template v-if="acquisitions.length">
          <div class="row">
            <div v-for="(acquisition, index) in acquisitions" :key="index" class="col-md-6 col-lg-4 col-xl-3 mb-3">
              <CharacterTalentCard class="d-flex flex-column h-100" :acquisition="acquisition" @edit="openEdit(index)" @remove="openRemove(index)" />
            </div>
          </div>
          <RemoveCharacterTalentModal v-if="acquisition" :acquisition="acquisition" ref="removeModal" @confirm="remove(index)" />
        </template>
        <p v-else>{{ t("characters.talents.empty") }}</p>
      </section>
      <TarAlert v-else class="d-flex justify-content-between" show variant="warning">
        <div>
          <strong>{{ t("characters.creation.talents.empty.lead") }}</strong> {{ t("characters.creation.talents.empty.help") }}
        </div>
        <RouterLink :to="{ name: 'Talents' }" class="btn btn-primary">
          <font-awesome-icon aria-hidden="true" icon="fas fa-code-branch" />&nbsp;{{ t("talents.title") }}
        </RouterLink>
      </TarAlert>
    </template>
    <LoadingSpinner v-if="isLoading" />
    <div class="d-flex justify-content-between">
      <div class="d-flex gap-2">
        <TarButton icon="fas fa-xmark" outline :text="t('actions.abandon')" variant="danger" @click="$emit('abandon')" />
        <TarButton icon="fas fa-arrow-left" outline :text="t('actions.previous')" variant="secondary" @click="character.goBack" />
      </div>
      <TarButton :disabled="!canSubmit" icon="fas fa-arrow-right" :text="t('actions.next')" @click="submit" />
    </div>
  </section>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed, nextTick, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import AddCharacterTalent from "@/components/characters/talents/AddCharacterTalent.vue";
import CharacterTalentCard from "@/components/characters/talents/CharacterTalentCard.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import RemoveCharacterTalentModal from "@/components/characters/talents/RemoveCharacterTalentModal.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import TarButton from "@/components/tar/TarButton.vue";
import type { Caste } from "@/types/castes";
import type { CharacterTalent, CharacterTalentContext } from "@/types/characters";
import type { Education } from "@/types/educations";
import type { Lineage } from "@/types/lineages";
import type { SearchResults } from "@/types/search";
import type { SearchTalentsPayload, Talent } from "@/types/talents";
import type { Skill } from "@/types/game";
import { calculateCost } from "@/utils/talent";
import { searchTalents } from "@/api/talents";
import { useCharacterStore } from "@/stores/character";

const MAX_POINTS: number = 12;
const MIN_POINTS: number = 10;
const REQUIRED_SKILLS: number = 6;
const character = useCharacterStore();
const { orderBy } = arrayUtils;
const { t } = useI18n();

const emit = defineEmits<{
  (e: "abandon"): void;
  (e: "error", value: unknown): void;
}>();

const acquisitions = ref<CharacterTalent[]>([]);
const index = ref<number>(0);
const isLoading = ref<boolean>(true);
const removeModal = ref<InstanceType<typeof RemoveCharacterTalentModal> | null>(null);
const talents = ref<Talent[]>([]);
const touched = ref<boolean>(false);

const acquisition = computed<CharacterTalent | undefined>(() => acquisitions.value[index.value]);
const spent = computed<number>(() => acquisitions.value.reduce((sum, acquisition) => sum + calculateCost(acquisition.talent, acquisition.discounts), 0));
const trained = computed<Set<Skill>>(() => {
  const trained: Set<Skill> = new Set();
  acquisitions.value.forEach(({ talent }) => {
    if (talent.skill) {
      trained.add(talent.skill);
    }
  });
  return trained;
});

const lineage = computed<Lineage | undefined>(() =>
  character.creation.ethnicity ? { ...character.creation.ethnicity, parent: character.creation.species } : character.creation.species,
);
const context = computed<CharacterTalentContext | undefined>(() =>
  lineage.value
    ? {
        tier: 0,
        lineage: lineage.value,
        customizations: character.creation.customizations,
        talents: acquisitions.value,
      }
    : undefined,
);

type Rule = {
  key: string;
  success: boolean;
  text: string;
};
const evaluation = computed<Rule[]>(() => {
  const rules: Rule[] = [
    {
      key: "min",
      success: spent.value >= MIN_POINTS,
      text: t("characters.talents.rules.min", { spent: spent.value, min: MIN_POINTS }),
    },
    {
      key: "max",
      success: spent.value <= MAX_POINTS,
      text: t("characters.talents.rules.max", { spent: spent.value, max: MAX_POINTS }),
    },
    {
      key: "skills",
      success: trained.value.size >= REQUIRED_SKILLS,
      text: t("characters.talents.rules.skills", { trained: trained.value.size, required: REQUIRED_SKILLS }),
    },
  ];
  const caste: Caste | undefined = character.creation.caste;
  if (caste?.skill) {
    rules.push({
      key: "caste",
      success: trained.value.has(caste.skill),
      text: t("characters.talents.rules.caste", {
        skill: t(`game.skill.options.${caste.skill}`),
        caste: caste.name,
      }),
    });
  }
  const education: Education | undefined = character.creation.education;
  if (education?.skill) {
    rules.push({
      key: "education",
      success: trained.value.has(education.skill),
      text: t("characters.talents.rules.education", {
        skill: t(`game.skill.options.${education.skill}`),
        education: education.name,
      }),
    });
  }
  return rules;
});
const canSubmit = computed<boolean>(() => evaluation.value.every((rule) => rule.success));

function getClasses(rule: Rule): string[] {
  if (!touched.value) {
    return ["text-secondary"];
  }
  return [rule.success ? "text-success" : "text-danger"];
}

function add(acquisition: CharacterTalent): void {
  acquisitions.value.push(acquisition);
  touched.value = true;
}
function remove(index: number): void {
  acquisitions.value.splice(index, 1);
}

function openEdit(targetIndex: number): void {
  index.value = targetIndex;
  // TODO(fpion): nextTick(() => editModal.value?.open());
}
function openRemove(targetIndex: number): void {
  index.value = targetIndex;
  nextTick(() => removeModal.value?.open());
}

function submit(): void {
  if (canSubmit.value) {
    character.saveTalents(acquisitions.value);
  }
}

onMounted(async () => {
  try {
    const payload: SearchTalentsPayload = {
      ids: [],
      search: { terms: [], operator: "And" },
      tiers: [0],
      sort: [],
      skip: 0,
      limit: 0,
    };
    const results: SearchResults<Talent> = await searchTalents(payload);
    talents.value = orderBy(results.items, "name");

    const talentsById: Map<string, Talent> = new Map();
    talents.value.forEach((talent) => talentsById.set(talent.id, talent));
    character.creation.talents.forEach((acquisition) => {
      const talent: Talent | undefined = talentsById.get(acquisition.talent.id);
      if (talent) {
        acquisitions.value.push({ ...acquisition, talent });
      }
    });
    touched.value = acquisitions.value.length > 0;
  } catch (e: unknown) {
    emit("error", e);
  } finally {
    isLoading.value = false;
  }
});
</script>
