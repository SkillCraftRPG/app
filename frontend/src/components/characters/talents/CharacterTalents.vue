<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import CharacterTalentEdit from "./CharacterTalentEdit.vue";
import CharacterTalentRemove from "./CharacterTalentRemove.vue";
import TalentIcon from "@/components/talents/TalentIcon.vue";
import type { CharacterModel, CharacterTalentModel } from "@/types/characters";
import type { SearchResults } from "@/types/search";
import type { SearchTalentsPayload, TalentModel } from "@/types/talents";
import { calculateTalentPoints } from "@/helpers/gameUtils";
import { searchTalents } from "@/api/talents";

type SortedTalent = CharacterTalentModel & {
  sort: string;
};

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = defineProps<{
  character: CharacterModel;
}>();

const talents = ref<TalentModel[]>([]);

const acquiredTalentIds = computed<Set<string>>(() => new Set<string>(props.character.talents.map(({ talent }) => talent.id)));
const availableTalents = computed<TalentModel[]>(() =>
  talents.value.filter(
    ({ allowMultiplePurchases, id, requiredTalent }) =>
      (allowMultiplePurchases || !acquiredTalentIds.value.has(id)) && (!requiredTalent || acquiredTalentIds.value.has(requiredTalent.id)),
  ),
);
const sortedTalents = computed<SortedTalent[]>(() =>
  orderBy(
    props.character.talents.map((item) => ({ ...item, sort: [item.talent.tier, item.talent.name, item.precision].join(".") })),
    "sort",
  ),
);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: CharacterModel): void;
}>();

onMounted(async () => {
  try {
    const payload: SearchTalentsPayload = {
      ids: [],
      search: { terms: [], operator: "And" },
      tier: { values: [props.character.tier], operator: "lte" },
      sort: [{ field: "Name", isDescending: false }],
      skip: 0,
      limit: 0,
    };
    const results: SearchResults<TalentModel> = await searchTalents(payload);
    talents.value = [...results.items];
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>

<template>
  <div>
    <p>{{ t("characters.talents.remainingPoints", { n: calculateTalentPoints(character).remaining }) }}</p>
    <div class="mb-3">
      <CharacterTalentEdit :character="character" :talents="availableTalents" @error="$emit('error', $event)" @updated="$emit('updated', $event)" />
    </div>
    <table v-if="sortedTalents.length > 0" class="table table-striped">
      <thead>
        <tr>
          <th scope="col">{{ t("game.tier.label") }}</th>
          <th scope="col">{{ t("characters.talents.cost") }}</th>
          <th scope="col">{{ t("name") }}</th>
          <th scope="col">{{ t("characters.talents.precision") }}</th>
          <th scope="col">{{ t("characters.talents.notes") }}</th>
          <th scope="col"></th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="talent in sortedTalents" :key="talent.talent.id">
          <td>{{ talent.talent.tier }}</td>
          <td>{{ talent.cost }}</td>
          <td>
            <RouterLink :to="{ name: 'TalentEdit', params: { id: talent.talent.id } }" target="_blank"><TalentIcon />{{ talent.talent.name }}</RouterLink>
          </td>
          <td>{{ talent.precision ?? "—" }}</td>
          <td class="notes">{{ talent.notes ?? "—" }}</td>
          <td>
            <CharacterTalentEdit class="me-1" :character="character" :talent="talent" @error="$emit('error', $event)" @updated="$emit('updated', $event)" />
            <CharacterTalentRemove class="ms-1" :character="character" :talent="talent" @error="$emit('error', $event)" @updated="$emit('updated', $event)" />
          </td>
        </tr>
      </tbody>
    </table>
    <p v-else>{{ t("characters.talents.empty") }}</p>
  </div>
</template>

<style scoped>
.notes {
  max-width: 480px;
}
</style>
