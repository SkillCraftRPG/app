<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import BackButton from "@/components/shared/BackButton.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import NameInput from "@/components/shared/NameInput.vue";
import SaveButton from "@/components/shared/SaveButton.vue";
import SkillSelect from "@/components/game/SkillSelect.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import WealthMultiplierInput from "@/components/educations/WealthMultiplierInput.vue";
import type { ApiError } from "@/types/api";
import type { CreateOrReplaceEducationPayload, EducationModel } from "@/types/educations";
import type { Skill } from "@/types/game";
import { handleErrorKey } from "@/inject/App";
import { readEducation, replaceEducation } from "@/api/educations";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const description = ref<string>("");
const education = ref<EducationModel>();
const name = ref<string>("");
const skill = ref<Skill>();
const wealthMultiplier = ref<number>();

const hasChanges = computed<boolean>(
  () =>
    !!education.value &&
    (name.value !== education.value.name ||
      skill.value !== (education.value.skill ?? undefined) ||
      wealthMultiplier.value !== (education.value.wealthMultiplier ?? undefined) ||
      description.value !== (education.value.description ?? "")),
);

function setModel(model: EducationModel): void {
  education.value = model;
  description.value = model.description ?? "";
  name.value = model.name;
  skill.value = model.skill ?? undefined;
  wealthMultiplier.value = model.wealthMultiplier ?? undefined;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  if (education.value) {
    try {
      const payload: CreateOrReplaceEducationPayload = {
        name: name.value,
        description: description.value,
        skill: skill.value,
        wealthMultiplier: wealthMultiplier.value,
      };
      const model: EducationModel = await replaceEducation(education.value.id, payload, education.value.version);
      setModel(model);
      toasts.success("educations.updated");
    } catch (e: unknown) {
      handleError(e);
    }
  }
});

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      const education: EducationModel = await readEducation(id);
      setModel(education);
    }
  } catch (e: unknown) {
    const { status } = e as ApiError;
    if (status === 404) {
      router.push({ path: "/not-found" });
    } else {
      handleError(e);
    }
  }
});
</script>

<template>
  <main class="container">
    <template v-if="education">
      <h1>{{ education.name }}</h1>
      <AppBreadcrumb
        :current="education.name"
        :parent="{ route: { name: 'EducationList' }, text: t('educations.list') }"
        :world="education.world"
        @error="handleError"
      />
      <StatusDetail :aggregate="education" />
      <form @submit.prevent="onSubmit">
        <div class="row">
          <NameInput class="col-lg-4" required v-model="name" />
          <SkillSelect class="col-lg-4" v-model="skill" />
          <WealthMultiplierInput class="col-lg-4" v-model="wealthMultiplier" />
        </div>
        <DescriptionTextarea v-model="description" />
        <div>
          <SaveButton class="me-1" :disabled="isSubmitting || !hasChanges" :loading="isSubmitting" />
          <BackButton class="ms-1" :has-changes="hasChanges" />
        </div>
      </form>
    </template>
  </main>
</template>
