<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import AttributeSelect from "@/components/game/AttributeSelect.vue";
import type { Attribute } from "@/types/game";
import type { CharacterModel, LevelUpCharacterPayload } from "@/types/characters";
import { levelUpCharacter } from "@/api/characters";
import { useToastStore } from "@/stores/toast";
import { getTotalExperience } from "@/helpers/gameUtils";

const toasts = useToastStore();
const { t } = useI18n();

const props = defineProps<{
  character: CharacterModel;
}>();

const attribute = ref<Attribute>();
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);

const canLevelUp = computed<boolean>(() => props.character.level < 20 && props.character.experience >= getTotalExperience(props.character.level + 1));
const hasChanges = computed<boolean>(() => Boolean(attribute.value));

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: CharacterModel): void;
}>();

function hide(): void {
  modalRef.value?.hide();
}

function onCancel(): void {
  attribute.value = undefined;
  hide();
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  if (attribute.value) {
    try {
      const payload: LevelUpCharacterPayload = {
        attribute: attribute.value,
      };
      const character: CharacterModel = await levelUpCharacter(props.character.id, payload);
      toasts.success("characters.leveledUp");
      emit("updated", character);
    } catch (e: unknown) {
      emit("error", e);
    }
  }
  onCancel();
});
</script>

<template>
  <span>
    <TarButton
      :disabled="!canLevelUp"
      icon="fas fa-trophy"
      :text="t('characters.levelUp.label')"
      variant="success"
      data-bs-toggle="modal"
      data-bs-target="#level-up"
    />
    <TarModal :close="t('actions.close')" id="level-up" ref="modalRef" :title="t('characters.levelUp.label')">
      <form @submit.prevent="onSubmit">
        <AttributeSelect required v-model="attribute" />
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="onCancel" />
        <TarButton
          :disabled="isSubmitting || !hasChanges"
          icon="fas fa-trophy"
          :loading="isSubmitting"
          :status="t('loading')"
          :text="t('characters.levelUp.label')"
          variant="success"
          @click="onSubmit"
        />
      </template>
    </TarModal>
  </span>
</template>
