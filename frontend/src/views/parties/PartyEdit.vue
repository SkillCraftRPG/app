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
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { ApiError } from "@/types/api";
import type { CreateOrReplacePartyPayload, PartyModel } from "@/types/parties";
import { handleErrorKey } from "@/inject/App";
import { readParty, replaceParty } from "@/api/parties";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const description = ref<string>("");
const hasLoaded = ref<boolean>(false);
const name = ref<string>("");
const party = ref<PartyModel>();

const hasChanges = computed<boolean>(() => !!party.value && (name.value !== party.value.name || description.value !== (party.value.description ?? "")));

function setModel(model: PartyModel): void {
  party.value = model;
  description.value = model.description ?? "";
  name.value = model.name;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  if (party.value) {
    try {
      const payload: CreateOrReplacePartyPayload = {
        name: name.value,
        description: description.value,
      };
      const model: PartyModel = await replaceParty(party.value.id, payload, party.value.version);
      setModel(model);
      toasts.success("parties.updated");
    } catch (e: unknown) {
      handleError(e);
    }
  }
});

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      const party: PartyModel = await readParty(id);
      setModel(party);
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
    <template v-if="party">
      <h1>{{ party.name }}</h1>
      <AppBreadcrumb :current="party.name" :parent="{ route: { name: 'PartyList' }, text: t('parties.list') }" :world="party.world" @error="handleError" />
      <StatusDetail :aggregate="party" />
      <form @submit.prevent="onSubmit">
        <NameInput required v-model="name" />
        <DescriptionTextarea v-model="description" />
        <div>
          <SaveButton class="me-1" :disabled="isSubmitting || !hasChanges" :loading="isSubmitting" />
          <BackButton class="ms-1" :has-changes="hasChanges" />
        </div>
      </form>
    </template>
  </main>
</template>
