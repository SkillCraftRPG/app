<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import AppInput from "@/components/shared/AppInput.vue";
import LineageIcon from "@/components/lineages/LineageIcon.vue";
import type { CharacterModel } from "@/types/characters";
import type { LineageModel } from "@/types/lineages";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  character: CharacterModel;
  nation?: boolean | string;
}>();

const isNation = computed<boolean>(() => parseBoolean(props.nation) ?? false);
const id = computed<string>(() => (isNation.value ? "nation" : "species"));
const label = computed<string>(() => `lineages.${isNation.value ? "nation" : "species"}.label`);
const lineage = computed<LineageModel | undefined>(() => {
  if (isNation.value) {
    return props.character.lineage.species ? props.character.lineage : undefined;
  }
  return props.character.lineage.species ? props.character.lineage.species : props.character.lineage;
});
const value = computed<string>(() => lineage.value?.name ?? t("lineages.nation.none"));
</script>

<template>
  <AppInput disabled floating :id="id" :label="label" :model-value="value" validation="server">
    <template #append v-if="lineage">
      <RouterLink :to="{ name: 'LineageEdit', params: { id: lineage.id } }" class="btn btn-primary" target="_blank">
        <LineageIcon /> {{ t("actions.view") }}
      </RouterLink>
    </template>
  </AppInput>
</template>
