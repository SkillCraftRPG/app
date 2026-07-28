<template>
  <main class="container worlds-page d-flex flex-column flex-grow-1">
    <LoadingSpinner v-if="isLoading" />
    <div v-else-if="world">
      <div class="d-flex flex-column flex-sm-row justify-content-between align-items-sm-start gap-3">
        <h1 class="mb-0">{{ world.name ?? world.key }}</h1>
        <EditWorld class="mb-3" :world="world" @error="handleError" @updated="onUpdate" />
      </div>
      <TarAlert :close="t('actions.close')" dismissible variant="success" v-model="isCreated">
        <strong>{{ t("worlds.created.lead") }}</strong> {{ t("worlds.created.help", { name: world.name ?? world.key }) }}
      </TarAlert>
      <StatusDetail :subject="world" />
    </div>
  </main>
</template>

<script setup lang="ts">
import { inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import EditWorld from "@/components/worlds/EditWorld.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import type { World } from "@/types/worlds";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { handleErrorKey } from "@/inject";
import { readWorld } from "@/api/worlds";
import { useDocument } from "@/composables/document";
import { useEventStore } from "@/stores/event";
import { useToastStore } from "@/stores/toast";

const document = useDocument();
const events = useEventStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const isCreated = ref<boolean>(events.shift() === "created");
const isLoading = ref<boolean>(true);
const world = ref<World>();

function onUpdate(value: World): void {
  world.value = value;
  toasts.success("saved");
}

onMounted(async () => {
  try {
    const id: string = (Array.isArray(route.params.id) ? route.params.id[0] : route.params.id) ?? "";
    world.value = await readWorld(id);
    document.setTitle(world.value.name ?? world.value.key);
  } catch (e: unknown) {
    const failure = e as ApiFailure;
    if (failure.status === StatusCodes.NotFound) {
      router.push("/not-found");
    } else {
      handleError(e);
    }
  } finally {
    isLoading.value = false;
  }
});
</script>
