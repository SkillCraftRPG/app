<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useRoute, useRouter } from "vue-router";

import BackButton from "@/components/shared/BackButton.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import NameInput from "@/components/shared/NameInput.vue";
import SaveButton from "@/components/shared/SaveButton.vue";
import ScriptInput from "@/components/languages/ScriptInput.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import TypicalSpeakersInput from "@/components/languages/TypicalSpeakersInput.vue";
import type { ApiError } from "@/types/api";
import type { CreateOrReplaceLanguagePayload, LanguageModel } from "@/types/languages";
import { handleErrorKey } from "@/inject/App";
import { readLanguage, replaceLanguage } from "@/api/languages";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();

const language = ref<LanguageModel>();
const description = ref<string>("");
const hasLoaded = ref<boolean>(false);
const name = ref<string>("");
const script = ref<string>("");
const typicalSpeakers = ref<string>("");

const hasChanges = computed<boolean>(
  () =>
    !!language.value &&
    (name.value !== language.value.name ||
      script.value !== (language.value.script ?? "") ||
      typicalSpeakers.value !== (language.value.typicalSpeakers ?? "") ||
      description.value !== (language.value.description ?? "")),
);

function setModel(model: LanguageModel): void {
  language.value = model;
  description.value = model.description ?? "";
  name.value = model.name;
  script.value = model.script ?? "";
  typicalSpeakers.value = model.typicalSpeakers ?? "";
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  if (language.value) {
    try {
      const payload: CreateOrReplaceLanguagePayload = {
        name: name.value,
        description: description.value,
        script: script.value,
        typicalSpeakers: typicalSpeakers.value,
      };
      const model: LanguageModel = await replaceLanguage(language.value.id, payload, language.value.version);
      setModel(model);
      toasts.success("languages.updated");
    } catch (e: unknown) {
      handleError(e);
    }
  }
});

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      const language: LanguageModel = await readLanguage(id);
      setModel(language);
    }
  } catch (e: unknown) {
    const { status } = e as ApiError;
    if (status === 404) {
      router.push({ path: "/not-found" });
    } else {
      handleError(e);
    }
  }
  hasLoaded.value = true;
});
</script>

<template>
  <main class="container">
    <template v-if="language">
      <h1>{{ language.name }}</h1>
      <StatusDetail :aggregate="language" />
      <form @submit.prevent="onSubmit">
        <div class="row">
          <NameInput class="col-lg-6" required v-model="name" />
          <ScriptInput class="col-lg-6" v-model="script" />
        </div>
        <TypicalSpeakersInput v-model="typicalSpeakers" />
        <DescriptionTextarea v-model="description" />
        <div>
          <SaveButton class="me-1" :disabled="isSubmitting || !hasChanges" :loading="isSubmitting" />
          <BackButton class="ms-1" :has-changes="hasChanges" />
        </div>
      </form>
    </template>
  </main>
</template>
