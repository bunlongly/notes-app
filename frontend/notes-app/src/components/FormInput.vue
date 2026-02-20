<template>
  <div class="form-group">
    <label
      v-if="label"
      :for="id"
      class="block text-sm font-medium text-gray-700 mb-1"
    >
      {{ label }}
      <span v-if="required" class="text-red-500">*</span>
    </label>
    <input
      :id="id"
      :type="type"
      :value="modelValue"
      :placeholder="placeholder"
      :disabled="disabled"
      @input="
        $emit('update:modelValue', ($event.target as HTMLInputElement).value)
      "
      @blur="$emit('blur')"
      :class="[
        'w-full px-4 py-2 border rounded-lg outline-none transition-colors',
        error
          ? 'border-red-500 focus:ring-2 focus:ring-red-200 focus:border-red-500'
          : 'border-gray-300 focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500',
        disabled ? 'bg-gray-100 cursor-not-allowed' : 'bg-white'
      ]"
    />
    <p v-if="error" class="mt-1 text-sm text-red-600">
      {{ error }}
    </p>
    <div v-if="hint && !error" class="mt-1 text-sm text-gray-500">
      {{ hint }}
    </div>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  id: string;
  label?: string;
  type?: string;
  modelValue: string;
  placeholder?: string;
  error?: string;
  hint?: string;
  required?: boolean;
  disabled?: boolean;
}>();

defineEmits<{
  'update:modelValue': [value: string];
  blur: [];
}>();
</script>
