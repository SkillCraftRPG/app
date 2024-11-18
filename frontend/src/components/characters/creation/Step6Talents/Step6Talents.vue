<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import TalentCard from "@/components/talents/TalentCard.vue";
import TalentSelect from "@/components/talents/TalentSelect.vue";
import type { Step6 } from "@/types/characters";
import type { TalentModel } from "@/types/talents";

const { t } = useI18n();

const talent = ref<TalentModel>();
const talents = ref<TalentModel[]>([]);

const isCompleted = computed<boolean>(() => requiredTalents.value === 0);
const requiredTalents = computed<number>(() => 2 - talents.value.length);

const emit = defineEmits<{
  (e: "back"): void;
  (e: "continue", value: Step6): void;
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
const onSubmit = handleSubmit(() => emit("continue", { talents: talents.value }));
</script>

<template>
  <div>
    <h3>{{ t("characters.steps.talents") }}</h3>
    <form @submit="onSubmit">
      <TalentSelect
        :disabled="requiredTalents === 0"
        :exclude="talents"
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
        <div v-for="talent in talents" :key="talent.id" class="col-lg-6">
          <TalentCard remove :talent="talent" view @removed="removeTalent(talent)" />
        </div>
      </div>
      <!-- TODO(fpion): Talents -->
      <TarButton class="me-1" icon="fas fa-arrow-left" :text="t('actions.back')" variant="secondary" @click="$emit('back')" />
      <TarButton class="ms-1" :disabled="!isCompleted" icon="fas fa-plus" :text="t('actions.create')" type="submit" variant="success" />
    </form>
  </div>
</template>
