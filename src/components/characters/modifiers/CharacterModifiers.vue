<template>
  <div>
    <div class="mb-3">
      <TarButton icon="fas fa-plus" size="large" :text="t('actions.add')" @click="add" />
    </div>
    <div class="row">
      <div class="col-md-6 col-lg-3">
        <!-- TODO(fpion): Search filter -->
      </div>
      <div class="col-md-6 col-lg-3">
        <!-- TODO(fpion): Kind filter -->
      </div>
      <div class="col-md-6 col-lg-3">
        <!-- TODO(fpion): Target filter -->
      </div>
      <div class="col-md-6 col-lg-3">
        <!-- TODO(fpion): Bonus vs. Penalty filter -->
      </div>
    </div>
    <div v-if="modifiers.length" class="row">
      <div v-for="modifier in modifiers" :key="modifier.id" class="col-md-6 col-lg-4 col-xl-3">
        <CharacterModifier class="mb-3" :modifier="modifier" @edit="edit(modifier)" @remove="remove(modifier)" />
      </div>
    </div>
    <p v-else>{{ t("characters.modifiers.empty") }}</p>
    <!-- TODO(fpion): clear filters if any -->
    <EditCharacterModifierModal
      :character="character"
      :modifier="modifier"
      ref="editModal"
      @error="$emit('error', $event)"
      @updated="$emit('updated', $event)"
    />
    <RemoveCharacterModifierModal
      v-if="modifier"
      :character="character"
      :modifier="modifier"
      ref="removeModal"
      @error="$emit('error', $event)"
      @updated="$emit('updated', $event)"
    />
  </div>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed, nextTick, ref } from "vue";
import { useI18n } from "vue-i18n";

import CharacterModifier from "./CharacterModifier.vue";
import EditCharacterModifierModal from "./EditCharacterModifierModal.vue";
import RemoveCharacterModifierModal from "./RemoveCharacterModifierModal.vue";
import TarButton from "@/components/tar/TarButton.vue";
import type { Character, CharacterModifier as CharacterModifierT } from "@/types/characters";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = defineProps<{
  character: Character;
}>();

defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Character): void;
}>();

const editModal = ref<InstanceType<typeof EditCharacterModifierModal> | null>(null);
const removeModal = ref<InstanceType<typeof RemoveCharacterModifierModal> | null>(null);
const modifier = ref<CharacterModifierT>();

const modifiers = computed<CharacterModifierT[]>(() => orderBy(props.character.modifiers, "name")); // TODO(fpion): sort key

function add(): void {
  modifier.value = undefined;
  editModal.value?.open();
}
function edit(value: CharacterModifierT): void {
  modifier.value = value;
  editModal.value?.open();
}
function remove(value: CharacterModifierT): void {
  modifier.value = value;
  nextTick(() => removeModal.value?.open());
}
</script>
