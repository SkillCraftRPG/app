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

const toasts = useToastStore();
const { t } = useI18n();

const props = defineProps<{
  character: CharacterModel;
  id?: string;
}>();

const attribute = ref<Attribute>();
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);

const hasChanges = computed<boolean>(() => Boolean(attribute.value));
const hasReachedMaximumAttributeScore = computed<boolean>(() => typeof score.value === "number" && score.value >= 20);
const modalId = computed<string>(() => props.id ?? "level-up");
const score = computed<number | undefined>(() => {
  switch (attribute.value) {
    case "Agility":
      return props.character.attributes.agility.score;
    case "Coordination":
      return props.character.attributes.coordination.score;
    case "Intellect":
      return props.character.attributes.intellect.score;
    case "Presence":
      return props.character.attributes.presence.score;
    case "Sensitivity":
      return props.character.attributes.sensitivity.score;
    case "Spirit":
      return props.character.attributes.spirit.score;
    case "Vigor":
      return props.character.attributes.vigor.score;
    default:
      return undefined;
  }
});

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
  <TarModal v-if="id" :close="t('actions.close')" :id="id" ref="modalRef" :title="t('characters.levelUp.label')">
    <form @submit.prevent="onSubmit">
      <AttributeSelect required v-model="attribute" />
    </form>
    <p v-if="hasReachedMaximumAttributeScore" class="text-danger">{{ t("characters.levelUp.maximumScoreReached") }}</p>
    <template #footer>
      <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="onCancel" />
      <TarButton
        :disabled="isSubmitting || !hasChanges || hasReachedMaximumAttributeScore"
        icon="fas fa-trophy"
        :loading="isSubmitting"
        :status="t('loading')"
        :text="t('characters.levelUp.label')"
        variant="success"
        @click="onSubmit"
      />
    </template>
  </TarModal>
  <span v-else>
    <TarButton
      :disabled="!character.canLevelUp"
      icon="fas fa-trophy"
      :text="t('characters.levelUp.label')"
      variant="success"
      data-bs-toggle="modal"
      :data-bs-target="`#${modalId}`"
    />
    <TarModal :close="t('actions.close')" :id="modalId" ref="modalRef" :title="t('characters.levelUp.label')">
      <form @submit.prevent="onSubmit">
        <AttributeSelect required v-model="attribute" />
      </form>
      <p v-if="hasReachedMaximumAttributeScore" class="text-danger">{{ t("characters.levelUp.maximumScoreReached") }}</p>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="onCancel" />
        <TarButton
          :disabled="isSubmitting || !hasChanges || hasReachedMaximumAttributeScore"
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
