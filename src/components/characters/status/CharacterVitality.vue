<template>
  <div class="h-100">
    <TarCard class="clickable h-100" @click="open">
      <div class="card-text d-flex justify-content-between align-items-center gap-2">
        <div class="fw-semibold">{{ label }}</div>
        <div class="text-danger">{{ n(character.vitality.current, "integer") }} / {{ n(total, "integer") }}</div>
      </div>
      <div class="card-text d-flex justify-content-between align-items-center gap-2">
        <div>{{ t("game.rest.long") }}</div>
        <div>+{{ n(regeneration, "integer") }}</div>
      </div>
      <TarProgress :aria-label="label" class="mt-1" :label="progress.card.label" :value="progress.card.value" variant="danger" />
    </TarCard>
    <TarModal centered :close="t('actions.close')" fade ref="modal" size="large" :title="label">
      <form @submit.prevent="handleSubmit(submit)">
        <div class="row">
          <div class="col-md-6">
            <VitalityField id="current" label="characters.vitality.current" :max="total" v-model="current" />
          </div>
          <div class="col-md-6">
            <div class="d-flex justify-content-between align-items-center gap-2">
              <div>{{ t("characters.vitality.total") }}</div>
              <div>{{ n(total, "integer") }}</div>
            </div>
            <div class="d-flex justify-content-between align-items-center gap-2">
              <div>{{ t("game.rest.long") }}</div>
              <div>+{{ n(regeneration, "integer") }}</div>
            </div>
          </div>
        </div>
        <TarProgress :aria-label="label" class="mb-3 mt-1" :label="progress.form.label" :value="progress.form.value" variant="danger" />
        <div class="row">
          <div class="col-md-6">
            <VitalityField class="mb-3" id="temporary" label="characters.vitality.temporary" v-model="temporary" />
          </div>
          <div class="col-md-6">
            <VitalityField class="mb-3" id="stun" label="characters.vitality.stun" v-model="stun" />
          </div>
        </div>
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
        <TarButton
          :disabled="!hasChanges || isLoading"
          icon="fas fa-floppy-disk"
          :loading="isLoading"
          :status="t('loading')"
          :text="t('actions.save')"
          @click="handleSubmit(submit)"
        />
      </template>
    </TarModal>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import TarButton from "@/components/tar/TarButton.vue";
import TarCard from "@/components/tar/TarCard.vue";
import TarModal from "@/components/tar/TarModal.vue";
import TarProgress from "@/components/tar/TarProgress.vue";
import VitalityField from "./VitalityField.vue";
import type { Character, UpdateCharacterPayload } from "@/types/characters";
import { updateCharacter } from "@/api/characters";
import { useForm } from "@/forms";

const { n, t } = useI18n();

const props = defineProps<{
  character: Character;
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Character): void;
}>();

const current = ref<number>(0);
const isLoading = ref<boolean>(false);
const modal = ref<InstanceType<typeof TarModal> | null>(null);
const stun = ref<number>(0);
const temporary = ref<number>(0);

const hasChanges = computed<boolean>(
  () =>
    current.value !== props.character.vitality.current ||
    temporary.value !== props.character.vitality.temporary ||
    stun.value !== props.character.vitality.stun,
);
const label = computed<string>(() => t("game.statistic.options.Vitality"));
const total = computed<number>(() => props.character.statistics.vitality.total);
const regeneration = computed<number>(() => Math.round(total.value / 7));
const progress = computed(() => {
  const card: number = Math.floor((props.character.vitality.current * 100) / total.value);
  const form: number = Math.floor((current.value * 100) / total.value);
  return {
    card: { label: n(card / 100, "percentage"), value: card },
    form: { label: n(form / 100, "percentage"), value: form },
  };
});

const { handleSubmit, reinitialize, reset } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const payload: UpdateCharacterPayload = {
        vitality: {
          current: current.value,
          temporary: temporary.value,
          stun: stun.value,
        },
      };
      const character: Character = await updateCharacter(props.character.id, payload);
      emit("updated", character);
      modal.value?.hide();
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

function cancel(): void {
  reset();
  modal.value?.hide();
}

function open(): void {
  modal.value?.show();
}

watch(
  () => props.character,
  (character) => {
    current.value = character.vitality.current;
    temporary.value = character.vitality.temporary;
    stun.value = character.vitality.stun;
    reinitialize();
  },
  { deep: true, immediate: true },
);

// TODO(fpion): show temporary HP and stun damage in card
</script>
