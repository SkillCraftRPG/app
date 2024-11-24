<script setup lang="ts">
import { TarBadge } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import CharacterBonusEdit from "./CharacterBonusEdit.vue";
import CharacterBonusRemove from "./CharacterBonusRemove.vue";
import type { BonusModel, CharacterModel } from "@/types/characters";

type SortedBonus = BonusModel & {
  sort: string;
};

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = defineProps<{
  character: CharacterModel;
}>();

const bonuses = computed<SortedBonus[]>(() =>
  orderBy(
    props.character.bonuses.map((bonus) => ({ ...bonus, sort: [bonus.category, bonus.target].join(".") })),
    "sort",
  ),
);

defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: CharacterModel): void;
}>();
</script>

<template>
  <div>
    <div class="mb-3">
      <CharacterBonusEdit :character="character" @error="$emit('error', $event)" @updated="$emit('updated', $event)" />
    </div>
    <table v-if="bonuses.length > 0" class="table table-striped">
      <thead>
        <tr>
          <th scope="col">{{ t("characters.bonuses.target") }}</th>
          <th scope="col">{{ t("characters.bonuses.value") }}</th>
          <th scope="col">{{ t("characters.bonuses.precision") }}</th>
          <th scope="col">{{ t("characters.bonuses.notes") }}</th>
          <th scope="col"></th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="bonus in bonuses" :key="bonus.id">
          <td>
            {{ t(`characters.bonuses.category.options.${bonus.category}`) }}
            <br />
            <template v-if="bonus.category === 'Attribute'">{{ t(`game.attribute.options.${bonus.target}`) }}</template>
            <template v-else-if="bonus.category === 'Miscellaneous'">{{ t(`characters.bonuses.miscellaneous.options.${bonus.target}`) }}</template>
            <template v-else-if="bonus.category === 'Skill'">{{ t(`game.skill.options.${bonus.target}`) }}</template>
            <template v-else-if="bonus.category === 'Speed'">{{ t(`game.speed.options.${bonus.target}`) }}</template>
            <template v-else-if="bonus.category === 'Statistic'">{{ t(`game.statistic.options.${bonus.target}`) }}</template>
          </td>
          <td>
            {{ bonus.value > 0 ? `+${bonus.value}` : bonus.value }}
            <template v-if="bonus.isTemporary">
              <br />
              <TarBadge pill>{{ t("characters.bonuses.temporary") }}</TarBadge>
            </template>
          </td>
          <td>{{ bonus.precision ?? "—" }}</td>
          <td class="notes">{{ bonus.notes ?? "—" }}</td>
          <td>
            <CharacterBonusEdit class="me-1" :bonus="bonus" :character="character" @error="$emit('error', $event)" @updated="$emit('updated', $event)" />
            <CharacterBonusRemove class="ms-1" :bonus="bonus" :character="character" @error="$emit('error', $event)" @updated="$emit('updated', $event)" />
          </td>
        </tr>
      </tbody>
    </table>
    <p v-else>{{ t("characters.bonuses.empty") }}</p>
  </div>
</template>

<style scoped>
.notes {
  max-width: 480px;
}
</style>
