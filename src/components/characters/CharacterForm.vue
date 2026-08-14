<template>
  <form @submit.prevent="handleSubmit(submit)">
    <div class="row">
      <div class="col-md-6">
        <NameField class="mb-3" required v-model="name" />
      </div>
      <div class="col-md-6">
        <DominantHandRadio class="mb-3" v-model="dominantHand" />
      </div>
    </div>
    <div class="d-flex justify-content-end mb-3">
      <TarButton
        :disabled="!hasChanges || isLoading"
        icon="fas fa-floppy-disk"
        :loading="isLoading"
        size="large"
        :status="t('loading')"
        :text="t('actions.save')"
        type="submit"
      />
    </div>
  </form>
</template>

<script setup lang="ts">
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import DominantHandRadio from "./DominantHandRadio.vue";
import NameField from "@/components/shared/NameField.vue";
import TarButton from "@/components/tar/TarButton.vue";
import type { Character, DominantHand, UpdateCharacterPayload } from "@/types/characters";
import { updateCharacter } from "@/api/characters";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  character: Character;
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Character): void;
}>();

const dominantHand = ref<DominantHand | null>(null);
const isLoading = ref<boolean>(false);
const name = ref<string>("");

const hasChanges = computed<boolean>(() => props.character.name !== name.value || (props.character.dominantHand ?? null) !== dominantHand.value);

const { handleSubmit, reinitialize } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const payload: UpdateCharacterPayload = {
        name: name.value,
        dominantHand: { value: dominantHand.value },
      };
      const character: Character = await updateCharacter(props.character.id, payload);
      emit("updated", character);
      reinitialize();
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

watch(
  () => props.character,
  (character) => {
    name.value = character.name;
    dominantHand.value = character.dominantHand ?? null;
  },
  { deep: true, immediate: true },
);
</script>
