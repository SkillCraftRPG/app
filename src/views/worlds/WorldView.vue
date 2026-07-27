<template>
  <main class="container worlds-page d-flex flex-column flex-grow-1">
    <LoadingSpinner v-if="isLoading" />
    <div v-else-if="world">
      <h1>{{ world.name ?? world.key }}</h1>
      <!-- TODO(fpion): edit button right to the title that opens big modal with key and name inputs, and description textarea -->
      <!-- TODO(fpion): createdBy and createdOn -->
      <!-- TODO(fpion): updatedBy and updatedOn -->
      <!-- TODO(fpion): tiles -->
    </div>
  </main>
</template>

<script setup lang="ts">
import { inject, onMounted, ref } from "vue";
import { useRoute, useRouter } from "vue-router";

import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import type { World } from "@/types/worlds";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { handleErrorKey } from "@/inject";
import { readWorld } from "@/api/worlds";
import { useDocument } from "@/composables/document";

const document = useDocument();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();

const isLoading = ref<boolean>(true);
const status = ref<string>("");
const world = ref<World>();

onMounted(async () => {
  status.value = (Array.isArray(route.query.status) ? route.query.status[0] : route.query.status) ?? "";

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
