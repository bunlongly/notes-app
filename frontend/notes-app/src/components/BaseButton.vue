<template>
  <button
    :type="type"
    :disabled="disabled || loading"
    :class="[
      'px-4 py-2.5 rounded-lg font-medium transition-colors focus:outline-none focus:ring-4',
      variantClasses,
      disabled || loading ? 'opacity-50 cursor-not-allowed' : ''
    ]"
  >
    <span v-if="loading" class="flex items-center justify-center">
      <svg
        class="animate-spin -ml-1 mr-2 h-5 w-5"
        xmlns="http://www.w3.org/2000/svg"
        fill="none"
        viewBox="0 0 24 24"
      >
        <circle
          class="opacity-25"
          cx="12"
          cy="12"
          r="10"
          stroke="currentColor"
          stroke-width="4"
        ></circle>
        <path
          class="opacity-75"
          fill="currentColor"
          d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
        ></path>
      </svg>
      {{ loadingText }}
    </span>
    <span v-else>
      <slot></slot>
    </span>
  </button>
</template>

<script setup lang="ts">
import { computed } from 'vue';

const props = withDefaults(
  defineProps<{
    type?: 'button' | 'submit' | 'reset';
    variant?: 'primary' | 'secondary' | 'outline' | 'danger';
    loading?: boolean;
    loadingText?: string;
    disabled?: boolean;
  }>(),
  {
    type: 'button',
    variant: 'primary',
    loading: false,
    loadingText: 'Loading...',
    disabled: false
  }
);

const variantClasses = computed(() => {
  switch (props.variant) {
    case 'primary':
      return 'bg-indigo-600 text-white hover:bg-indigo-700 focus:ring-indigo-200';
    case 'secondary':
      return 'bg-gray-600 text-white hover:bg-gray-700 focus:ring-gray-200';
    case 'outline':
      return 'border-2 border-indigo-600 text-indigo-600 hover:bg-indigo-50 focus:ring-indigo-200';
    case 'danger':
      return 'bg-red-600 text-white hover:bg-red-700 focus:ring-red-200';
    default:
      return 'bg-indigo-600 text-white hover:bg-indigo-700 focus:ring-indigo-200';
  }
});
</script>
