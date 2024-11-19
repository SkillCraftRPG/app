import { defineStore } from "pinia";
import { ref } from "vue";

import type { CharacterCreation, Step1, Step2, Step3, Step4, Step5, Step6 } from "@/types/characters";

export const useCharacterStore = defineStore(
  "character",
  () => {
    const creation = ref<CharacterCreation>({});
    const step = ref<number>(1);

    function goBack(): void {
      if (step.value === 1) {
        creation.value = {};
      } else {
        step.value--;
      }
    }

    function setStep1(step1: Step1): void {
      creation.value = { ...creation.value, step1 };
      step.value++;
    }
    function setStep2(step2: Step2): void {
      creation.value = { ...creation.value, step2 };
      step.value++;
    }
    function setStep3(step3: Step3): void {
      creation.value = { ...creation.value, step3 };
      step.value++;
    }
    function setStep4(step4: Step4): void {
      creation.value = { ...creation.value, step4 };
      step.value++;
    }
    function setStep5(step5: Step5): void {
      creation.value = { ...creation.value, step5 };
      step.value++;
    }
    function setStep6(step6: Step6): void {
      creation.value = { ...creation.value, step6 };
    }

    // TODO(fpion): separate step increment from setSteps?
    // TODO(fpion): clear store when character successfully created?

    return { creation, step, goBack, setStep1, setStep2, setStep3, setStep4, setStep5, setStep6 };
  },
  {
    persist: true,
  },
);
