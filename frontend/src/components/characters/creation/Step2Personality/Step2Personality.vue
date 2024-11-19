<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import AttributeSelect from "@/components/game/AttributeSelect.vue";
import CustomizationCard from "@/components/customizations/CustomizationCard.vue";
import CustomizationSelect from "@/components/customizations/CustomizationSelect.vue";
import MarkdownText from "@/components/shared/MarkdownText.vue";
import PersonalitySelect from "@/components/personalities/PersonalitySelect.vue";
import type { CustomizationModel } from "@/types/customizations";
import type { PersonalityModel } from "@/types/personalities";
import type { Step2 } from "@/types/characters";
import { useCharacterStore } from "@/stores/character";

type Customization = {
  customization: CustomizationModel;
  source: "extra" | "personality";
};

const character = useCharacterStore();
const { t } = useI18n();

const customization = ref<CustomizationModel>();
const customizations = ref<CustomizationModel[]>([]);
const personality = ref<PersonalityModel>();

const allCustomizations = computed<Customization[]>(() => {
  const allCustomizations: Customization[] = [];
  if (personality.value?.gift) {
    allCustomizations.push({ customization: personality.value.gift, source: "personality" } as Customization);
  }
  allCustomizations.push(...customizations.value.map((customization) => ({ customization, source: "extra" }) as Customization));
  return allCustomizations;
});
const disabilityCount = computed<number>(() => customizations.value.filter(({ type }) => type === "Disability").length);
const excludedCustomizations = computed<CustomizationModel[]>(() => allCustomizations.value.map(({ customization }) => customization));
const giftCount = computed<number>(() => customizations.value.filter(({ type }) => type === "Gift").length);
const isCompleted = computed<boolean>(() => giftCount.value === disabilityCount.value);
const requiredDisabilities = computed<number>(() => {
  const count: number = giftCount.value - disabilityCount.value;
  return count < 0 ? 0 : count;
});
const requiredGifts = computed<number>(() => {
  const count: number = disabilityCount.value - giftCount.value;
  return count < 0 ? 0 : count;
});

function addCustomization(): void {
  if (customization.value) {
    customizations.value.push(customization.value);
    customization.value = undefined;
  }
}
function removeCustomization(customization: CustomizationModel): void {
  const index: number = customizations.value.findIndex(({ id }) => id === customization.id);
  if (index >= 0) {
    customizations.value.splice(index, 1);
  }
}

function setPersonality(value?: PersonalityModel): void {
  personality.value = value;
  customizations.value = [];
}

defineEmits<{
  (e: "error", value: unknown): void;
}>();

const { handleSubmit } = useForm();
const onSubmit = handleSubmit(() => {
  if (personality.value) {
    const payload: Step2 = {
      personality: personality.value,
      customizations: customizations.value,
    };
    character.setStep2(payload);
    character.next();
  }
});

onMounted(() => {
  const step2: Step2 | undefined = character.creation.step2;
  if (step2) {
    setPersonality(step2.personality);
    customizations.value = [...step2.customizations];
  }
});
</script>

<template>
  <div>
    <h3>{{ t("characters.steps.personality") }}</h3>
    <form @submit="onSubmit">
      <div class="row">
        <PersonalitySelect class="col" :model-value="personality?.id" required @error="$emit('error', $event)" @selected="setPersonality" />
        <AttributeSelect
          v-if="personality?.attribute"
          class="col"
          disabled
          label="characters.personalityAttributeBonus"
          :model-value="personality.attribute"
          validation="server"
        />
      </div>
      <MarkdownText v-if="personality?.description" :text="personality.description" />
      <template v-if="allCustomizations.length > 0">
        <h5>{{ t("customizations.list") }}</h5>
        <CustomizationSelect :exclude="excludedCustomizations" :model-value="customization?.id" validation="server" @selected="customization = $event">
          <template #append>
            <TarButton :disabled="!customization" icon="fas fa-plus" :text="t('actions.add')" variant="success" @click="addCustomization" />
          </template>
        </CustomizationSelect>
        <div class="mb-3 row">
          <div v-for="customization in allCustomizations" :key="customization.customization.id" class="col-lg-3">
            <CustomizationCard
              :customization="customization.customization"
              :remove="customization.source === 'extra'"
              view
              @removed="removeCustomization(customization.customization)"
            />
          </div>
        </div>
        <p v-if="requiredDisabilities > 0" class="text-danger">
          <font-awesome-icon icon="fas fa-triangle-exclamation" /> {{ t("characters.customizations.select.Disability", { n: requiredDisabilities }) }}
        </p>
        <p v-if="requiredGifts > 0" class="text-danger">
          <font-awesome-icon icon="fas fa-triangle-exclamation" /> {{ t("characters.customizations.select.Gift", { n: requiredGifts }) }}
        </p>
      </template>
      <TarButton class="me-1" icon="fas fa-arrow-left" :text="t('actions.back')" variant="secondary" @click="character.back()" />
      <TarButton class="ms-1" :disabled="!isCompleted" icon="fas fa-arrow-right" :text="t('actions.continue')" type="submit" />
    </form>
  </div>
</template>
