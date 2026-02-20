<template>
  <Transition name="toast">
    <div
      v-if="show"
      :class="[
        'fixed top-4 right-4 z-50 max-w-sm w-full shadow-lg rounded-lg p-4 flex items-start gap-3',
        variantClasses
      ]"
    >
      <div class="flex-shrink-0">
        <svg
          v-if="variant === 'success'"
          class="w-6 h-6"
          fill="none"
          stroke="currentColor"
          viewBox="0 0 24 24"
        >
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="2"
            d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"
          />
        </svg>
        <svg
          v-else-if="variant === 'error'"
          class="w-6 h-6"
          fill="none"
          stroke="currentColor"
          viewBox="0 0 24 24"
        >
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="2"
            d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z"
          />
        </svg>
        <svg
          v-else
          class="w-6 h-6"
          fill="none"
          stroke="currentColor"
          viewBox="0 0 24 24"
        >
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="2"
            d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
          />
        </svg>
      </div>
      <div class="flex-1">
        <p class="font-medium">{{ message }}</p>
      </div>
      <button
        @click="close"
        class="flex-shrink-0 opacity-70 hover:opacity-100 transition-opacity"
      >
        <svg
          class="w-5 h-5"
          fill="none"
          stroke="currentColor"
          viewBox="0 0 24 24"
        >
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="2"
            d="M6 18L18 6M6 6l12 12"
          />
        </svg>
      </button>
    </div>
  </Transition>
</template>

<script setup lang="ts">
import { computed, watch } from 'vue';

interface Props {
  show: boolean;
  message: string;
  variant?: 'success' | 'error' | 'info';
  duration?: number;
}

const props = withDefaults(defineProps<Props>(), {
  variant: 'info',
  duration: 3000
});

const emit = defineEmits<{
  close: [];
}>();

const variantClasses = computed(() => {
  switch (props.variant) {
    case 'success':
      return 'bg-green-50 text-green-800 border border-green-200';
    case 'error':
      return 'bg-red-50 text-red-800 border border-red-200';
    default:
      return 'bg-blue-50 text-blue-800 border border-blue-200';
  }
});

const close = () => {
  emit('close');
};

watch(
  () => props.show,
  newVal => {
    if (newVal && props.duration > 0) {
      setTimeout(() => {
        close();
      }, props.duration);
    }
  }
);
</script>

<style scoped>
.toast-enter-active,
.toast-leave-active {
  transition: all 0.3s ease;
}

.toast-enter-from {
  opacity: 0;
  transform: translateX(100%);
}

.toast-leave-to {
  opacity: 0;
  transform: translateY(-20px);
}
</style>
