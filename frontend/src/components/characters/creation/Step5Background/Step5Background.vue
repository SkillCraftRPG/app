<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import CasteDetail from "./CasteDetail.vue";
import CasteSelect from "@/components/castes/CasteSelect.vue";
import EducationSelect from "@/components/educations/EducationSelect.vue";
import ItemSelect from "@/components/items/ItemSelect.vue";
import MarkdownText from "@/components/shared/MarkdownText.vue";
import QuantityInput from "./QuantityInput.vue";
import SkillSelect from "@/components/game/SkillSelect.vue";
import type { CasteModel } from "@/types/castes";
import type { EducationModel } from "@/types/educations";
import type { ItemModel } from "@/types/items";
import type { SearchResults } from "@/types/search";
import type { SearchTalentsPayload, TalentModel } from "@/types/talents";
import type { Skill } from "@/types/game";
import type { Step5 } from "@/types/characters";
import { roll } from "@/helpers/gameUtils";
import { searchTalents } from "@/api/talents";
import { useCharacterStore } from "@/stores/character";

const character = useCharacterStore();
const { t } = useI18n();

const caste = ref<CasteModel>();
const education = ref<EducationModel>();
const item = ref<ItemModel>();
const quantity = ref<number>(0);
const talents = ref<Set<Skill>>(new Set<Skill>());

const isCasteValid = computed<boolean>(() => (caste.value?.skill ? talents.value.has(caste.value.skill) : false));
const isCompleted = computed<boolean>(() => isCasteValid.value && isEducationValid.value && caste.value?.skill !== education.value?.skill);
const isEducationValid = computed<boolean>(() => (education.value?.skill ? talents.value.has(education.value.skill) : false));
const startingWealth = computed<string | undefined>(() =>
  caste.value?.wealthRoll && education.value?.wealthMultiplier ? [caste.value.wealthRoll, education.value.wealthMultiplier].join(" × ") : undefined,
);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
}>();

function rollStartingWealth(): void {
  if (caste.value?.wealthRoll && education.value?.wealthMultiplier) {
    quantity.value = roll(caste.value.wealthRoll);
    quantity.value *= education.value.wealthMultiplier;
  }
}

const { handleSubmit } = useForm();
const onSubmit = handleSubmit(() => {
  if (caste.value && education.value) {
    const payload: Step5 = {
      caste: caste.value,
      education: education.value,
      item: item.value,
      quantity: quantity.value,
    };
    character.setStep5(payload);
    character.next();
  }
});

onMounted(async () => {
  const step5: Step5 | undefined = character.creation.step5;
  if (step5) {
    caste.value = step5.caste;
    education.value = step5.education;
    item.value = step5.item;
    quantity.value = step5.quantity;
  }
  try {
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
        talents.value.add(talent.skill);
      }
    });
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>

<template>
  <div>
    <h3>{{ t("characters.steps.background") }}</h3>
    <form @submit="onSubmit">
      <h5>{{ t("castes.select.label") }}</h5>
      <div class="row">
        <CasteSelect class="col" :model-value="caste?.id" required @selected="caste = $event" />
        <SkillSelect v-if="caste?.skill" class="col" disabled id="caste-skill" :model-value="caste?.skill" validation="server" />
      </div>
      <template v-if="caste">
        <MarkdownText v-if="caste.description" :text="caste.description" />
        <CasteDetail v-if="caste.features.length > 0" :caste="caste" />
        <p v-if="!isCasteValid" class="text-danger">
          <font-awesome-icon icon="fas fa-triangle-exclamation" /> {{ t(caste.skill ? "characters.caste.invalid.talent" : "characters.caste.invalid.skill") }}
        </p>
      </template>
      <h5>{{ t("educations.select.label") }}</h5>
      <div class="row">
        <EducationSelect class="col" :model-value="education?.id" required @selected="education = $event" />
        <SkillSelect v-if="education?.skill" class="col" disabled id="education-skill" :model-value="education?.skill" validation="server" />
      </div>
      <template v-if="education">
        <MarkdownText v-if="education.description" :text="education.description" />
        <p v-if="!isEducationValid" class="text-danger">
          <font-awesome-icon icon="fas fa-triangle-exclamation" />
          {{ t(education.skill ? "characters.education.invalid.talent" : "characters.education.invalid.skill") }}
        </p>
      </template>
      <template v-if="caste && education">
        <h5>{{ t("game.startingWealth") }}</h5>
        <div class="row">
          <ItemSelect category="Money" class="col" :model-value="item?.id" :value="{ values: [1], operator: 'eq' }" @selected="item = $event" />
          <QuantityInput class="col" v-model="quantity">
            <template v-if="startingWealth" #prepend>
              <TarButton :disabled="!item" icon="fas fa-dice" :text="startingWealth" @click="rollStartingWealth" />
            </template>
          </QuantityInput>
        </div>
        <p v-if="caste.skill === education.skill" class="text-danger">
          <font-awesome-icon icon="fas fa-triangle-exclamation" /> {{ t("characters.background.invalid") }}
        </p>
      </template>
      <TarButton class="me-1" icon="fas fa-arrow-left" :text="t('actions.back')" variant="secondary" @click="character.back()" />
      <TarButton class="ms-1" :disabled="!isCompleted" icon="fas fa-arrow-right" :text="t('actions.continue')" type="submit" />
    </form>
  </div>
</template>
