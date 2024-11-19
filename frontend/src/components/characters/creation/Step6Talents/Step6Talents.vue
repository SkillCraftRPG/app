<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import TalentCard from "@/components/talents/TalentCard.vue";
import TalentSelect from "@/components/talents/TalentSelect.vue";
import type { SearchResults } from "@/types/search";
import type { SearchTalentsPayload, TalentModel } from "@/types/talents";
import type { Skill } from "@/types/game";
import type { Step5, Step6 } from "@/types/characters";
import { searchTalents } from "@/api/talents";
import { useCharacterStore } from "@/stores/character";

const character = useCharacterStore();
const { t } = useI18n();

const backgroundTalents = ref<TalentModel[]>([]);
const talent = ref<TalentModel>();
const talents = ref<TalentModel[]>([]);

const excludedTalents = computed<TalentModel[]>(() => backgroundTalents.value.concat(talents.value));
const isCompleted = computed<boolean>(() => requiredTalents.value === 0);
const requiredTalents = computed<number>(() => 2 - talents.value.length);

const emit = defineEmits<{
  (e: "complete"): void;
  (e: "error", value: unknown): void;
}>();

function addTalent(): void {
  if (talent.value) {
    talents.value.push(talent.value);
    talent.value = undefined;
  }
}
function removeTalent(talent: TalentModel): void {
  const index: number = talents.value.findIndex(({ id }) => id === talent.id);
  if (index >= 0) {
    talents.value.splice(index, 1);
  }
}

const { handleSubmit } = useForm();
const onSubmit = handleSubmit(() => {
  const payload: Step6 = { talents: talents.value };
  character.setStep6(payload);
  emit("complete");
});

onMounted(async () => {
  const step5: Step5 | undefined = character.creation.step5;
  if (step5) {
    try {
      const skillTalents = new Map<Skill, TalentModel>();
      const payload: SearchTalentsPayload = {
        hasSkill: true,
        ids: [],
        search: { terms: [], operator: "And" },
        tier: { values: [0], operator: "eq" },
        sort: [],
        skip: 0,
        limit: 0,
      };
      const results: SearchResults<TalentModel> = await searchTalents(payload);
      results.items.forEach((talent) => {
        if (talent.skill) {
          skillTalents.set(talent.skill, talent);
        }
      });
      if (step5.caste.skill) {
        const talent: TalentModel | undefined = skillTalents.get(step5.caste.skill);
        if (talent) {
          backgroundTalents.value.push(talent);
        }
      }
      if (step5.education.skill) {
        const talent: TalentModel | undefined = skillTalents.get(step5.education.skill);
        if (talent) {
          backgroundTalents.value.push(talent);
        }
      }
    } catch (e: unknown) {
      emit("error", e);
    }
  }
  const step6: Step6 | undefined = character.creation.step6;
  if (step6) {
    talents.value = [...step6.talents];
  }
});
</script>

<template>
  <div>
    <h3>{{ t("characters.steps.talents") }}</h3>
    <form @submit="onSubmit">
      <TalentSelect
        :disabled="requiredTalents === 0"
        :exclude="excludedTalents"
        :max-tier="0"
        :model-value="talent?.id"
        skill
        validation="server"
        @selected="talent = $event"
      >
        <template #append>
          <TarButton icon="fas fa-plus" :disabled="!talent" :text="t('actions.add')" variant="success" @click="addTalent" />
        </template>
      </TalentSelect>
      <p v-if="requiredTalents !== 0" class="text-danger">
        <font-awesome-icon icon="fas fa-triangle-exclamation" /> {{ t("characters.talents.select", { n: requiredTalents }) }}
      </p>
      <div v-if="talents.length > 0" class="mb-3 row">
        <div v-for="talent in talents" :key="talent.id" class="col-lg-3">
          <TalentCard remove :talent="talent" view @removed="removeTalent(talent)" />
        </div>
      </div>
      <TarButton class="me-1" icon="fas fa-arrow-left" :text="t('actions.back')" variant="secondary" @click="character.goBack()" />
      <TarButton class="ms-1" :disabled="!isCompleted" icon="fas fa-plus" :text="t('actions.create')" type="submit" variant="success" />
    </form>
  </div>
</template>
