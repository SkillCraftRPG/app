import { computed, ref } from "vue";
import { defineStore } from "pinia";

export const useEventStore = defineStore("event", () => {
  const events = ref<string[]>([]);

  const isEmpty = computed<boolean>(() => events.value.length === 0);
  const size = computed<number>(() => events.value.length);

  function peek(): string | undefined {
    return events.value[0];
  }

  function push(event: string): void {
    events.value.push(event);
  }

  function shift(): string | undefined {
    return events.value.shift();
  }

  function toArray(): string[] {
    return [...events.value];
  }

  return { isEmpty, size, peek, push, shift, toArray };
});
