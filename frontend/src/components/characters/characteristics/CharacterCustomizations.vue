<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import type { CharacterModel } from "@/types/characters";
import type { CustomizationModel } from "@/types/customizations";
import CustomizationCard from "@/components/customizations/CustomizationCard.vue";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = defineProps<{
  character: CharacterModel;
}>();

type CharacterCustomization = {
  source: "customization" | "nature";
  customization: CustomizationModel;
};

const customizations = computed<CharacterCustomization[]>(() => {
  const customizations: CharacterCustomization[] = [];
  if (props.character.nature.gift) {
    customizations.push({ source: "nature", customization: props.character.nature.gift });
  }
  customizations.push(
    ...orderBy(
      props.character.customizations.filter(({ type }) => type === "Gift"),
      "name",
    ).map((customization) => ({ source: "customization", customization }) as CharacterCustomization),
  );
  customizations.push(
    ...orderBy(
      props.character.customizations.filter(({ type }) => type === "Disability"),
      "name",
    ).map((customization) => ({ source: "customization", customization }) as CharacterCustomization),
  );
  return customizations;
});
</script>

<template>
  <div>
    <h3>{{ t("customizations.list") }}</h3>
    <div class="mb-3 row">
      <div class="col-lg-3" v-for="item in customizations" :key="item.customization.id">
        <CustomizationCard :customization="item.customization" :nature="item.source === 'nature'" view />
      </div>
    </div>
  </div>
</template>
