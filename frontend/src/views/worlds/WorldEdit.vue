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
import SlugInput from "@/components/worlds/SlugInput.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { ApiError } from "@/types/api";
import type { CreateOrReplaceWorldPayload, WorldModel } from "@/types/worlds";
import { handleErrorKey } from "@/inject/App";
import { readWorld, replaceWorld } from "@/api/worlds";
import { useToastStore } from "@/stores/toast";
import { useWorldStore } from "@/stores/world";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const worldStore = useWorldStore();
const { t } = useI18n();

const description = ref<string>("");
const name = ref<string>("");
const slug = ref<string>("");
const world = ref<WorldModel>();

const hasChanges = computed<boolean>(
  () => !!world.value && (slug.value !== world.value.slug || name.value !== (world.value.name ?? "") || description.value !== (world.value.description ?? "")),
);

function setModel(model: WorldModel): void {
  world.value = model;
  description.value = model.description ?? "";
  name.value = model.name ?? "";
  slug.value = model.slug;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  if (world.value) {
    try {
      const payload: CreateOrReplaceWorldPayload = {
        slug: slug.value,
        name: name.value,
        description: description.value,
      };
      const model: WorldModel = await replaceWorld(world.value.id, payload, world.value.version);
      worldStore.save(model);
      toasts.success("worlds.updated");
      if (world.value.slug === model.slug) {
        setModel(model);
      } else {
        router.replace({ name: "WorldEdit", params: { slug: model.slug } });
      }
    } catch (e: unknown) {
      handleError(e);
    }
  }
});

onMounted(async () => {
  try {
    const slug = route.params.slug?.toString();
    if (slug) {
      const world: WorldModel = await readWorld(slug);
      setModel(world);
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
    <template v-if="world">
      <h1>{{ world.name ?? world.slug }}</h1>
      <AppBreadcrumb :current="t('worlds.edit')" :world="world" @error="handleError" />
      <StatusDetail :aggregate="world" />
      <form @submit.prevent="onSubmit">
        <div class="row">
          <NameInput class="col-lg-6" required v-model="name" />
          <SlugInput class="col-lg-6" :name="name" v-model="slug" />
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
