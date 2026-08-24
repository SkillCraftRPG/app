<template>
  <form @submit.prevent="handleSubmit(submit)">
    <section>
      <h3 class="h5">{{ t("lineages.languages.granted.lead") }}</h3>
      <p class="text-body-secondary">{{ t("lineages.languages.granted.help") }}</p>
      <div class="row">
        <div class="col-md-6">
          <LanguageSelect
            class="mb-3"
            :exclude="exclude"
            :model-value="language?.id ?? ''"
            placeholder="languages.placeholder"
            @error="$emit('error', $event)"
            @selected="language = $event"
          >
            <template #append>
              <TarButton :disabled="!language" icon="fas fa-plus" :text="t('actions.add')" @click="add" />
            </template>
          </LanguageSelect>
        </div>
      </div>
      <div v-if="languages.length" class="row">
        <div v-for="language in languages" :key="language.id" class="col-md-6 col-lg-4 col-xl-3 mb-3">
          <LanguageCard class="h-100" :language="language">
            <div class="d-flex justify-content-end mt-2 gap-2">
              <RouterLink class="btn btn-outline-primary" target="_blank" :to="{ name: 'Language', params: { id: language.id } }">
                <font-awesome-icon icon="fas fa-edit" />&nbsp;{{ t("actions.edit") }}
              </RouterLink>
              <TarButton icon="fas fa-xmark" outline :text="t('actions.remove')" variant="danger" @click="remove(language)" />
            </div>
          </LanguageCard>
        </div>
      </div>
      <p>{{ t("lineages.languages.granted.empty") }}</p>
    </section>
    <section>
      <h3 class="h5">{{ t("lineages.languages.extra.lead") }}</h3>
      <p class="text-body-secondary">{{ t("lineages.languages.extra.help") }}</p>
      <div class="row">
        <div class="col-md-6">
          <ExtraLanguagesField class="mb-3" v-model="extra" />
        </div>
      </div>
    </section>
    <section>
      <h3 class="h5">{{ t("lineages.languages.content.lead") }}</h3>
      <p class="text-body-secondary">{{ t("lineages.languages.content.help") }}</p>
      <ContentField class="mb-3" v-model="content" />
    </section>
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
import ExtraLanguagesField from "@/components/languages/ExtraLanguagesField.vue";
import LanguageCard from "@/components/languages/LanguageCard.vue";
import LanguageSelect from "@/components/languages/LanguageSelect.vue";
import TarButton from "@/components/tar/TarButton.vue";
import type { Language } from "@/types/languages";
import type { Lineage, UpdateLineagePayload } from "@/types/lineages";
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

const content = ref<string>("");
const extra = ref<number>(0);
const isLoading = ref<boolean>(false);
const language = ref<Language>();
const languages = ref<Language[]>([]);

const exclude = computed<string[]>(() => languages.value.map(({ id }) => id));
const hasChanges = computed<boolean>(
  () =>
    JSON.stringify(props.lineage.languages.granted.map(({ id }) => id)) !== JSON.stringify(languages.value.map(({ id }) => id)) ||
    props.lineage.languages.extra !== extra.value ||
    (props.lineage.languages.content ?? "") !== content.value,
);

function add(): void {
  if (language.value) {
    languages.value.push(language.value);
    language.value = undefined;
  }
}

function remove(language: Language): void {
  const index: number = languages.value.findIndex(({ id }) => id === language.id);
  if (index >= 0) {
    languages.value.splice(index, 1);
  }
}

const { handleSubmit, reinitialize } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const payload: UpdateLineagePayload = {
        languages: {
          ids: languages.value.map(({ id }) => id),
          extra: extra.value,
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
    languages.value = [...lineage.languages.granted];
    extra.value = lineage.languages.extra;
    content.value = lineage.languages.content ?? "";
  },
  { deep: true, immediate: true },
);
</script>
