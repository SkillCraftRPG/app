<template>
  <div class="h-100">
    <TarCard class="clickable h-100" @click="open">
      <div class="card-text d-flex justify-content-between align-items-center gap-2">
        <div class="fw-semibold">{{ label }}</div>
        <div class="text-alcohol">{{ n(character.bloodAlcoholContent, "integer") }} / {{ n(total, "integer") }}</div>
      </div>
      <TarProgress :aria-label="label" class="progress-alcohol mt-1" :label="progress.card.label" :value="progress.card.value" />
    </TarCard>
    <TarModal centered :close="t('actions.close')" fade scrollable ref="modal" size="large" :title="label">
      <form @submit.prevent="handleSubmit(submit)">
        <div class="row">
          <div class="col-md-6 mb-3">
            <AlcoholField id="current" label="characters.bloodAlcoholContent.current" :max="total" v-model="current" />
          </div>
          <div class="col-md-6 mb-3">
            <div class="small text-body-secondary">{{ t("characters.bloodAlcoholContent.total") }}</div>
            <div>{{ n(total, "integer") }}</div>
          </div>
        </div>
        <TarProgress :aria-label="label" class="progress-alcohol" :label="progress.form.label" :value="progress.form.value" />
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
import { computed, nextTick, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import AlcoholField from "./AlcoholField.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarCard from "@/components/tar/TarCard.vue";
import TarModal from "@/components/tar/TarModal.vue";
import TarProgress from "@/components/tar/TarProgress.vue";
import type { Character, UpdateCharacterPayload } from "@/types/characters";
import type { ProgressData } from "@/types/progress";
import { calculateProgress } from "@/utils/progress";
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

const hasChanges = computed<boolean>(() => current.value !== props.character.bloodAlcoholContent);
const label = computed<string>(() => t("characters.bloodAlcoholContent.label"));
const total = computed<number>(() => props.character.attributes.health.total + 5);

type ProgressPair = {
  card: ProgressData;
  form: ProgressData;
};
const progress = computed<ProgressPair>(() => ({
  card: calculateProgress(props.character.bloodAlcoholContent / total.value, n),
  form: calculateProgress(current.value / total.value, n),
}));

const { handleSubmit, reinitialize, reset } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const payload: UpdateCharacterPayload = {
        bloodAlcoholContent: current.value,
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
    current.value = character.bloodAlcoholContent;
    nextTick(reinitialize);
  },
  { deep: true, immediate: true },
);
</script>

<style scoped>
.progress-alcohol {
  --bs-progress-bar-bg: var(--bs-orange);
}

.text-alcohol {
  color: var(--bs-orange);
}
</style>
