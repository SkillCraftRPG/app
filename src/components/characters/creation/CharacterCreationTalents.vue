<template>
  <section>
    <h2 class="h3">{{ t("talents.title") }}</h2>
    <template v-if="!isLoading">
      <section v-if="talents.length">
        <p>TODO(fpion): implement</p>
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
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import TarButton from "@/components/tar/TarButton.vue";
import type { SearchResults } from "@/types/search";
import type { SearchTalentsPayload, Talent } from "@/types/talents";
import { searchTalents } from "@/api/talents";
import { useCharacterStore } from "@/stores/character";

const character = useCharacterStore();
const { orderBy } = arrayUtils;
const { t } = useI18n();

const emit = defineEmits<{
  (e: "abandon"): void;
  (e: "error", value: unknown): void;
}>();

const isLoading = ref<boolean>(true);
const talents = ref<Talent[]>([]);

const canSubmit = computed<boolean>(() => false);

function submit(): void {
  if (canSubmit.value) {
    console.log("Submitting…");
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
  } catch (e: unknown) {
    emit("error", e);
  } finally {
    isLoading.value = false;
  }
});
</script>
