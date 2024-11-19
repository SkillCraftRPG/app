<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import CasteDetail from "./CasteDetail.vue";
import CasteSelect from "@/components/castes/CasteSelect.vue";
import EducationSelect from "@/components/castes/EducationSelect.vue";
import ItemSelect from "@/components/items/ItemSelect.vue";
import MarkdownText from "@/components/shared/MarkdownText.vue";
import QuantityInput from "./QuantityInput.vue";
import SkillSelect from "@/components/game/SkillSelect.vue";
import type { CasteModel } from "@/types/castes";
import type { EducationModel } from "@/types/educations";
import type { ItemModel } from "@/types/items";
import type { Step5 } from "@/types/characters";
import { roll } from "@/helpers/gameUtils";
import { useCharacterStore } from "@/stores/character";

const character = useCharacterStore();
const { t } = useI18n();

const caste = ref<CasteModel>();
const education = ref<EducationModel>();
const item = ref<ItemModel>();
const quantity = ref<number>(0);

const startingWealth = computed<string | undefined>(() =>
  caste.value?.wealthRoll && education.value?.wealthMultiplier ? [caste.value.wealthRoll, education.value.wealthMultiplier].join(" × ") : undefined,
);

defineEmits<{
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
  }
});

onMounted(() => {
  const step5: Step5 | undefined = character.creation.step5;
  if (step5) {
    caste.value = step5.caste;
    education.value = step5.education;
    item.value = step5.item;
    quantity.value = step5.quantity;
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
        <CasteDetail :caste="caste" />
      </template>
      <h5>{{ t("educations.select.label") }}</h5>
      <div class="row">
        <EducationSelect class="col" :model-value="education?.id" required @selected="education = $event" />
        <SkillSelect v-if="education?.skill" class="col" disabled id="education-skill" :model-value="education?.skill" validation="server" />
      </div>
      <MarkdownText v-if="education?.description" :text="education.description" />
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
      </template>
      <TarButton class="me-1" icon="fas fa-arrow-left" :text="t('actions.back')" variant="secondary" @click="character.goBack()" />
      <TarButton class="ms-1" icon="fas fa-arrow-right" :text="t('actions.continue')" type="submit" />
    </form>
  </div>
</template>
