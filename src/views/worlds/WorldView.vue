<template>
  <main class="container worlds-page d-flex flex-column flex-grow-1">
    <LoadingSpinner v-if="isLoading" />
    <div v-else-if="world">
      <h1>{{ world.name ?? world.key }}</h1>
      <!-- TODO(fpion): edit button right to the title that opens big modal with key and name inputs, and description textarea -->
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

import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import type { World } from "@/types/worlds";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { handleErrorKey } from "@/inject";
import { readWorld } from "@/api/worlds";
import { useDocument } from "@/composables/document";
import { useEventStore } from "@/stores/event";

const document = useDocument();
const events = useEventStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { t } = useI18n();

const isCreated = ref<boolean>(events.shift() === "created");
const isLoading = ref<boolean>(true);
const world = ref<World>();

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
