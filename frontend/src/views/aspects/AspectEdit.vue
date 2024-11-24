<script setup lang="ts">
import { TarAlert } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import AttributeSelect from "@/components/game/AttributeSelect.vue";
import BackButton from "@/components/shared/BackButton.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import NameInput from "@/components/shared/NameInput.vue";
import SaveButton from "@/components/shared/SaveButton.vue";
import SkillSelect from "@/components/game/SkillSelect.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { ApiError } from "@/types/api";
import type { Attribute, Skill } from "@/types/game";
import type { CreateOrReplaceAspectPayload, AspectModel } from "@/types/aspects";
import { handleErrorKey } from "@/inject/App";
import { readAspect, replaceAspect } from "@/api/aspects";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const aspect = ref<AspectModel>();
const description = ref<string>("");
const discountedSkill1 = ref<Skill>();
const discountedSkill2 = ref<Skill>();
const mandatoryAttribute1 = ref<Attribute>();
const mandatoryAttribute2 = ref<Attribute>();
const name = ref<string>("");
const optionalAttribute1 = ref<Attribute>();
const optionalAttribute2 = ref<Attribute>();

const areAttributesValid = computed<boolean>(() => {
  const attributes: Attribute[] = [];
  const distinct = new Set<Attribute>();
  if (mandatoryAttribute1.value) {
    attributes.push(mandatoryAttribute1.value);
    distinct.add(mandatoryAttribute1.value);
  }
  if (mandatoryAttribute2.value) {
    attributes.push(mandatoryAttribute2.value);
    distinct.add(mandatoryAttribute2.value);
  }
  if (optionalAttribute1.value) {
    attributes.push(optionalAttribute1.value);
    distinct.add(optionalAttribute1.value);
  }
  if (optionalAttribute2.value) {
    attributes.push(optionalAttribute2.value);
    distinct.add(optionalAttribute2.value);
  }
  return attributes.length === distinct.size;
});
const areSkillsValid = computed<boolean>(() => !discountedSkill1.value || discountedSkill1.value !== discountedSkill2.value);
const hasChanges = computed<boolean>(
  () =>
    !!aspect.value &&
    (name.value !== aspect.value.name ||
      description.value !== (aspect.value.description ?? "") ||
      mandatoryAttribute1.value !== (aspect.value.attributes.mandatory1 ?? undefined) ||
      mandatoryAttribute2.value !== (aspect.value.attributes.mandatory2 ?? undefined) ||
      optionalAttribute1.value !== (aspect.value.attributes.optional1 ?? undefined) ||
      optionalAttribute2.value !== (aspect.value.attributes.optional2 ?? undefined) ||
      discountedSkill1.value !== (aspect.value.skills.discounted1 ?? undefined) ||
      discountedSkill2.value !== (aspect.value.skills.discounted2 ?? undefined)),
);

function setModel(model: AspectModel): void {
  aspect.value = model;
  description.value = model.description ?? "";
  discountedSkill1.value = model.skills.discounted1 ?? undefined;
  discountedSkill2.value = model.skills.discounted2 ?? undefined;
  mandatoryAttribute1.value = model.attributes.mandatory1 ?? undefined;
  mandatoryAttribute2.value = model.attributes.mandatory2 ?? undefined;
  name.value = model.name;
  optionalAttribute1.value = model.attributes.optional1 ?? undefined;
  optionalAttribute2.value = model.attributes.optional2 ?? undefined;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  if (aspect.value) {
    try {
      const payload: CreateOrReplaceAspectPayload = {
        name: name.value,
        description: description.value,
        attributes: {
          mandatory1: mandatoryAttribute1.value,
          mandatory2: mandatoryAttribute2.value,
          optional1: optionalAttribute1.value,
          optional2: optionalAttribute2.value,
        },
        skills: {
          discounted1: discountedSkill1.value,
          discounted2: discountedSkill2.value,
        },
      };
      const model: AspectModel = await replaceAspect(aspect.value.id, payload, aspect.value.version);
      setModel(model);
      toasts.success("aspects.updated");
    } catch (e: unknown) {
      handleError(e);
    }
  }
});

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      const aspect: AspectModel = await readAspect(id);
      setModel(aspect);
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
    <template v-if="aspect">
      <h1>{{ aspect.name }}</h1>
      <AppBreadcrumb :current="aspect.name" :parent="{ route: { name: 'AspectList' }, text: t('aspects.list') }" :world="aspect.world" @error="handleError" />
      <StatusDetail :aggregate="aspect" />
      <form @submit.prevent="onSubmit">
        <NameInput required v-model="name" />
        <DescriptionTextarea v-model="description" />
        <h3>{{ t("game.attributes") }}</h3>
        <TarAlert :show="!areAttributesValid" variant="danger">
          <font-awesome-icon icon="fas fa-triangle-exclamation" /> {{ t("aspects.attributes.invalid") }}
        </TarAlert>
        <div class="row">
          <AttributeSelect class="col-lg-3" id="mandatory-attribute-1" label="aspects.attributes.mandatory1" v-model="mandatoryAttribute1" />
          <AttributeSelect class="col-lg-3" id="mandatory-attribute-2" label="aspects.attributes.mandatory2" v-model="mandatoryAttribute2" />
          <AttributeSelect class="col-lg-3" id="optional-attribute-1" label="aspects.attributes.optional1" v-model="optionalAttribute1" />
          <AttributeSelect class="col-lg-3" id="optional-attribute-2" label="aspects.attributes.optional2" v-model="optionalAttribute2" />
        </div>
        <h3>{{ t("game.skills") }}</h3>
        <TarAlert :show="!areSkillsValid" variant="danger">
          <font-awesome-icon icon="fas fa-triangle-exclamation" /> {{ t("aspects.skills.invalid") }}
        </TarAlert>
        <div class="row">
          <SkillSelect class="col-lg-6" id="discounted-skill-1" label="aspects.skills.discounted1" v-model="discountedSkill1" />
          <SkillSelect class="col-lg-6" id="discounted-skill-2" label="aspects.skills.discounted2" v-model="discountedSkill2" />
        </div>
        <div>
          <SaveButton class="me-1" :disabled="!areAttributesValid || !areSkillsValid || isSubmitting || !hasChanges" :loading="isSubmitting" />
          <BackButton class="ms-1" :has-changes="hasChanges" />
        </div>
      </form>
    </template>
  </main>
</template>
