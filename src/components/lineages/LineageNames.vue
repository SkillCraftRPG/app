<template>
  <form @submit.prevent="handleSubmit(submit)">
    <TagContainer class="mb-3" id="family" :label="t('lineages.names.family')" v-model="family" />
    <TagContainer class="mb-3" id="female" :label="t('lineages.names.female')" v-model="female" />
    <TagContainer class="mb-3" id="male" :label="t('lineages.names.male')" v-model="male" />
    <TagContainer class="mb-3" id="unisex" :label="t('lineages.names.unisex')" v-model="unisex" />
    <h3 class="h5">{{ t("lineages.names.custom.lead") }}</h3>
    <p class="text-body-secondary">{{ t("lineages.names.custom.help") }}</p>
    <div class="row">
      <div class="col-md-6" @keydown.enter.prevent="add" @keydown.esc.prevent="category = ''">
        <TarInput class="mb-3" floating id="category" :label="addLabel" max="100" :placeholder="addLabel" v-model="category">
          <template #append>
            <TarButton :disabled="!category" icon="fas fa-plus" :text="t('actions.add')" @click="add" />
          </template>
        </TarInput>
      </div>
    </div>
    <TagContainer v-for="(category, index) in custom" :key="index" class="mb-3" :id="`custom-${index}`" :label="category.category" v-model="category.values">
      <TarButton icon="fas fa-xmark" outline :text="t('actions.remove')" variant="danger" @click="remove(index)" />
    </TagContainer>
    <h3 class="h5">{{ t("lineages.names.content.lead") }}</h3>
    <p class="text-body-secondary">{{ t("lineages.names.content.help") }}</p>
    <ContentField class="mb-3" v-model="content" />
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

import ContentField from "@/components/shared/ContentField.vue";
import TagContainer from "@/components/shared/TagContainer.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarInput from "@/components/tar/TarInput.vue";
import type { Lineage, NameCategory, UpdateLineagePayload } from "@/types/lineages";
import { updateLineage } from "@/api/lineages";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  lineage: Lineage;
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Lineage): void;
}>();

const category = ref<string>("");
const content = ref<string>("");
const custom = ref<NameCategory[]>([]);
const family = ref<string[]>([]);
const female = ref<string[]>([]);
const isLoading = ref<boolean>(false);
const male = ref<string[]>([]);
const unisex = ref<string[]>([]);

const addLabel = computed<string>(() => t("lineages.names.custom.add"));
const hasChanges = computed<boolean>(
  () =>
    JSON.stringify(props.lineage.names.family) !== JSON.stringify(family.value) ||
    JSON.stringify(props.lineage.names.female) !== JSON.stringify(female.value) ||
    JSON.stringify(props.lineage.names.male) !== JSON.stringify(male.value) ||
    JSON.stringify(props.lineage.names.unisex) !== JSON.stringify(unisex.value) ||
    JSON.stringify(props.lineage.names.custom) !== JSON.stringify(custom.value) ||
    (props.lineage.names.content ?? "") !== content.value,
);

function add(): void {
  const value: string = category.value.trim();
  if (value) {
    custom.value.push({ category: value, values: [] });
  }
  category.value = "";
}
function remove(index: number): void {
  custom.value.splice(index, 1);
}

const { handleSubmit, reinitialize } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const payload: UpdateLineagePayload = {
        names: {
          family: family.value,
          female: female.value,
          male: male.value,
          unisex: unisex.value,
          custom: custom.value,
          content: content.value,
        },
      };
      const lineage: Lineage = await updateLineage(props.lineage.id, payload);
      reinitialize();
      emit("updated", lineage);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

watch(
  () => props.lineage,
  (lineage) => {
    family.value = [...lineage.names.family];
    female.value = [...lineage.names.female];
    male.value = [...lineage.names.male];
    unisex.value = [...lineage.names.unisex];
    custom.value = lineage.names.custom.map((category) => ({ ...category }));
    content.value = lineage.names.content ?? "";
  },
  { deep: true, immediate: true },
);
</script>
