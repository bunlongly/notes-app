<template>
  <div
    class="min-h-screen flex items-center justify-center bg-gray-100 py-12 px-4"
  >
    <div class="max-w-md w-full">
      <div class="text-center mb-8">
        <h1 class="text-3xl font-bold text-gray-900 mb-2">Create Account</h1>
        <p class="text-gray-600">Sign up to get started</p>
      </div>

      <div class="bg-white rounded-lg shadow-md p-8">
        <form @submit.prevent="handleRegister" class="space-y-5">
          <FormInput
            id="fullName"
            v-model="formData.fullName"
            label="Full Name"
            type="text"
            placeholder="John Doe"
            :error="errors.fullName"
            :required="true"
          />

          <FormInput
            id="email"
            v-model="formData.email"
            label="Email Address"
            type="email"
            placeholder="you@example.com"
            :error="errors.email"
            :required="true"
          />

          <FormInput
            id="password"
            v-model="formData.password"
            label="Password"
            type="password"
            placeholder="••••••••"
            :error="errors.password"
            :required="true"
          />
          <div
            v-if="formData.password && passwordMissing.length > 0"
            class="text-xs text-red-600 space-y-1 -mt-3"
          >
            <p class="font-medium">Password still needs:</p>
            <ul class="list-disc list-inside ml-2">
              <li v-for="missing in passwordMissing" :key="missing">
                {{ missing }}
              </li>
            </ul>
          </div>
          <div
            v-else-if="formData.password && passwordMissing.length === 0"
            class="text-xs text-green-600 -mt-3"
          >
            ✓ Password meets all requirements
          </div>

          <AlertMessage
            v-if="authStore.error"
            :message="authStore.error"
            variant="error"
          />

          <BaseButton
            type="submit"
            variant="primary"
            :loading="authStore.isLoading"
            loading-text="Creating account..."
            class="w-full"
          >
            Sign Up
          </BaseButton>
        </form>

        <div class="mt-6 text-center">
          <p class="text-gray-600">
            Already have an account?
            <router-link
              to="/login"
              class="text-indigo-600 hover:text-indigo-700 font-medium"
            >
              Sign in
            </router-link>
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onUnmounted, computed, watch } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import { registerSchema, validatePassword } from '@/utils/validation';
import { logger } from '@/utils/logger';
import FormInput from '@/components/FormInput.vue';
import BaseButton from '@/components/BaseButton.vue';
import AlertMessage from '@/components/AlertMessage.vue';

const router = useRouter();
const authStore = useAuthStore();

const formData = ref({
  email: '',
  password: '',
  fullName: ''
});

const errors = ref<Record<string, string>>({});
const passwordMissing = ref<string[]>([]);

// Watch password for real-time validation feedback
watch(
  () => formData.value.password,
  newPassword => {
    if (newPassword) {
      passwordMissing.value = validatePassword(newPassword);
    } else {
      passwordMissing.value = [];
    }
  }
);

// Clear errors when component unmounts (fixes persistence bug)
onUnmounted(() => {
  errors.value = {};
  authStore.error = null;
});

const handleRegister = async () => {
  logger.info('Register form submitted', formData.value);
  errors.value = {};

  // Check for empty fields
  if (!formData.value.fullName?.trim()) {
    errors.value.fullName = 'Full name is required';
    return;
  }
  if (!formData.value.email?.trim()) {
    errors.value.email = 'Email is required';
    return;
  }
  if (!formData.value.password?.trim()) {
    errors.value.password = 'Password is required';
    return;
  }

  // Validate form data with Zod
  logger.validation.start('Register Form');
  const validation = registerSchema.safeParse(formData.value);

  if (!validation.success) {
    logger.validation.errors(
      'Register Form',
      validation.error.flatten().fieldErrors
    );
    // Show validation errors
    if (validation.error && validation.error.errors) {
      validation.error.errors.forEach(err => {
        if (err.path && err.path[0]) {
          errors.value[err.path[0] as string] = err.message;
        }
      });
    }
    return;
  }

  logger.validation.success('Register Form');

  // If validation passes, register the user
  logger.info('Calling register API...');
  const success = await authStore.register(formData.value);

  if (success) {
    logger.info('Registration successful, redirecting to /notes');
    router.push('/notes');
  } else {
    logger.error('Registration failed', authStore.error);
  }
};
</script>
