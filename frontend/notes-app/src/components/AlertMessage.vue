<template>
  <div
    v-if="message"
    :class="[
      'px-4 py-3 rounded-lg text-sm flex items-start justify-between gap-3',
      variantClasses
    ]"
  >
    <span class="flex-1">{{ message }}</span>
    <button
      v-if="dismissible"
      @click="$emit('close')"
      class="text-current opacity-70 hover:opacity-100 font-bold leading-none flex-shrink-0"
      aria-label="Close"
    >
      ×
    </button>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';

const props = withDefaults(
  defineProps<{
    message: string;
    variant?: 'error' | 'success' | 'warning' | 'info';
    dismissible?: boolean;
  }>(),
  {
    variant: 'error',
    dismissible: true
  }
);

defineEmits<{
  close: [];
}>();

const variantClasses = computed(() => {
  switch (props.variant) {
    case 'error':
      return 'bg-red-50 border border-red-200 text-red-700';
    case 'success':
      return 'bg-green-50 border border-green-200 text-green-700';
    case 'warning':
      return 'bg-yellow-50 border border-yellow-200 text-yellow-700';
    case 'info':
      return 'bg-blue-50 border border-blue-200 text-blue-700';
    default:
      return 'bg-red-50 border border-red-200 text-red-700';
  }
});
</script>
