<template>
  <div class="text-body-secondary">
    <StatusInfo :actor="subject.createdBy" :date="subject.createdOn" format="status.createdOn" />
    <br />
    <StatusInfo :actor="subject.updatedBy" :date="subject.updatedOn" format="status.updatedOn" relative />
    <slot></slot>
  </div>
</template>

<script setup lang="ts">
import StatusInfo from "./StatusInfo.vue";
import type { Aggregate } from "@/types/api";

defineProps<{
  subject: Aggregate;
}>();
</script>
