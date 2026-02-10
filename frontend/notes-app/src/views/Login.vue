<template>
  <div
    class="min-h-screen flex items-center justify-center bg-gray-100 py-12 px-4"
  >
    <div class="max-w-md w-full">
      <div class="text-center mb-8">
        <h1 class="text-3xl font-bold text-gray-900 mb-2">Welcome Back</h1>
        <p class="text-gray-600">Sign in to your account</p>
      </div>

      <div class="bg-white rounded-lg shadow-md p-8">
        <form @submit.prevent="handleLogin" class="space-y-5">
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

          <AlertMessage
            v-if="authStore.error"
            :message="authStore.error"
            variant="error"
          />

          <BaseButton
            type="submit"
            variant="primary"
            :loading="authStore.isLoading"
            loading-text="Signing in..."
            class="w-full"
          >
            Sign In
          </BaseButton>
        </form>

        <div class="mt-6 text-center">
          <p class="text-gray-600">
            Don't have an account?
            <router-link
              to="/register"
              class="text-indigo-600 hover:text-indigo-700 font-medium"
            >
              Sign up
            </router-link>
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onUnmounted } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import { loginSchema } from '@/utils/validation';
import { logger } from '@/utils/logger';
import FormInput from '@/components/FormInput.vue';
import BaseButton from '@/components/BaseButton.vue';
import AlertMessage from '@/components/AlertMessage.vue';

const router = useRouter();
const authStore = useAuthStore();

const formData = ref({
  email: '',
  password: ''
});

const errors = ref<Record<string, string>>({});

// Clear errors when component unmounts (fixes persistence bug)
onUnmounted(() => {
  errors.value = {};
  authStore.error = null;
});

const handleLogin = async () => {
  logger.info('Login form submitted', formData.value);
  errors.value = {};

  // Check for empty fields
  if (!formData.value.email?.trim()) {
    errors.value.email = 'Email is required';
    return;
  }
  if (!formData.value.password?.trim()) {
    errors.value.password = 'Password is required';
    return;
  }

  // Validate form data with Zod
  logger.validation.start('Login Form');
  const validation = loginSchema.safeParse(formData.value);

  if (!validation.success) {
    logger.validation.errors(
      'Login Form',
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

  logger.validation.success('Login Form');

  // If validation passes, login the user
  logger.info('Calling login API...');
  const success = await authStore.login(formData.value);

  if (success) {
    logger.info('Login successful, redirecting to /notes');
    router.push('/notes');
  } else {
    logger.error('Login failed', authStore.error);
  }
};
</script>
