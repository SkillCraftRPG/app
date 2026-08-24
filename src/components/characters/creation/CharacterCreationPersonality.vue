<template>
  <form @submit.prevent="handleSubmit(submit)">
    <h2 class="h3">{{ t("characters.personality.title") }}</h2>
    <p class="text-body-secondary">{{ t("characters.personality.help") }}</p>
    <section>
      <h3 class="h5">{{ t("characters.alignment.label") }}</h3>
      <p class="text-body-secondary">{{ t("characters.alignment.help") }}</p>
      <AlignmentRadio class="mb-3" v-model="alignment" />
      <p>{{ t(`characters.alignment.tendencies.${alignment ?? "Unaligned"}`) }}</p>
    </section>
    <section>
      <h2 class="h3">{{ t("characters.personality.detail.title") }}</h2>
      <p class="text-body-secondary">{{ t("characters.personality.detail.help") }}</p>
      <div class="row">
        <div class="col-md-4">
          <PersonalityField class="mb-3" v-model="traits" />
        </div>
        <div class="col-md-4">
          <IdealsField class="mb-3" v-model="ideals" />
        </div>
        <div class="col-md-4">
          <FlawsField class="mb-3" v-model="flaws" />
        </div>
      </div>
    </section>
    <div class="d-flex justify-content-between">
      <div class="d-flex gap-2">
        <TarButton icon="fas fa-xmark" outline :text="t('actions.abandon')" variant="danger" @click="$emit('abandon')" />
        <TarButton icon="fas fa-arrow-left" outline :text="t('actions.previous')" variant="secondary" @click="character.goBack" />
      </div>
      <TarButton icon="fas fa-arrow-right" :text="t('actions.next')" type="submit" />
    </div>
  </form>
</template>

<script setup lang="ts">
import { onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import AlignmentRadio from "@/components/characters/profile/AlignmentRadio.vue";
import FlawsField from "@/components/characters/profile/FlawsField.vue";
import IdealsField from "@/components/characters/profile/IdealsField.vue";
import PersonalityField from "@/components/characters/profile/PersonalityField.vue";
import TarButton from "@/components/tar/TarButton.vue";
import type { Alignment, CharacterPersonality } from "@/types/characters";
import { useCharacterStore } from "@/stores/character";
import { useForm } from "@/forms";

const character = useCharacterStore();
const { t } = useI18n();

defineEmits<{
  (e: "abandon"): void;
}>();

const alignment = ref<Alignment | null>(null);
const flaws = ref<string>("");
const ideals = ref<string>("");
const traits = ref<string>("");

const { handleSubmit } = useForm();
function submit(): void {
  const personality: CharacterPersonality = {
    traits: traits.value,
    ideals: ideals.value,
    flaws: flaws.value,
  };
  character.savePersonality(alignment.value, personality);
}

onMounted(() => {
  alignment.value = character.creation.alignment ?? null;
  traits.value = character.creation.personality.traits ?? "";
  ideals.value = character.creation.personality.ideals ?? "";
  flaws.value = character.creation.personality.flaws ?? "";
});
</script>
