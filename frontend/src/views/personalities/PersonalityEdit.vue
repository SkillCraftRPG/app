<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useRoute, useRouter } from "vue-router";

import AttributeSelect from "@/components/game/AttributeSelect.vue";
import BackButton from "@/components/shared/BackButton.vue";
import CustomizationSelect from "@/components/customizations/CustomizationSelect.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import NameInput from "@/components/shared/NameInput.vue";
import SaveButton from "@/components/shared/SaveButton.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { ApiError } from "@/types/api";
import type { Attribute } from "@/types/game";
import type { CreateOrReplacePersonalityPayload, PersonalityModel } from "@/types/personalities";
import type { CustomizationModel } from "@/types/customizations";
import { handleErrorKey } from "@/inject/App";
import { readPersonality, replacePersonality } from "@/api/personalities";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();

const attribute = ref<Attribute>();
const description = ref<string>("");
const gift = ref<CustomizationModel>();
const hasLoaded = ref<boolean>(false);
const name = ref<string>("");
const personality = ref<PersonalityModel>();

const hasChanges = computed<boolean>(
  () =>
    !!personality.value &&
    (name.value !== personality.value.name ||
      attribute.value !== personality.value.attribute ||
      gift.value?.id !== personality.value.gift?.id ||
      description.value !== (personality.value.description ?? "")),
);

function setModel(model: PersonalityModel): void {
  personality.value = model;
  attribute.value = model.attribute;
  description.value = model.description ?? "";
  gift.value = model.gift;
  name.value = model.name;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  if (personality.value) {
    try {
      const payload: CreateOrReplacePersonalityPayload = {
        name: name.value,
        description: description.value,
        attribute: attribute.value,
        giftId: gift.value?.id,
      };
      const model: PersonalityModel = await replacePersonality(personality.value.id, payload, personality.value.version);
      setModel(model);
      toasts.success("personalities.updated");
    } catch (e: unknown) {
      handleError(e);
    }
  }
});

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      const personality: PersonalityModel = await readPersonality(id);
      setModel(personality);
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
    <template v-if="personality">
      <h1>{{ personality.name }}</h1>
      <StatusDetail :aggregate="personality" />
      <form @submit.prevent="onSubmit">
        <div class="row">
          <NameInput class="col-md-4" required v-model="name" />
          <AttributeSelect class="col-md-4" v-model="attribute" />
          <CustomizationSelect class="col-md-4" type="Gift" :model-value="gift?.id" @selected="gift = $event" />
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
