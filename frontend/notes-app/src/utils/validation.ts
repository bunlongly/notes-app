import { z } from 'zod';

// Custom email validator - ensures only one @ symbol
const emailSchema = z
  .string()
  .min(1, 'Email is required')
  .email('Please enter a valid email address')
  .refine(
    email => {
      const atCount = (email.match(/@/g) || []).length;
      return atCount === 1;
    },
    { message: 'Email must contain exactly one @ symbol' }
  );

// Custom password validator with detailed feedback
export function validatePassword(password: string): string[] {
  const errors: string[] = [];

  if (password.length < 8) {
    errors.push('At least 8 characters');
  }
  if (!/[A-Z]/.test(password)) {
    errors.push('One uppercase letter (A-Z)');
  }
  if (!/[a-z]/.test(password)) {
    errors.push('One lowercase letter (a-z)');
  }
  if (!/[0-9]/.test(password)) {
    errors.push('One number (0-9)');
  }
  if (!/[^A-Za-z0-9]/.test(password)) {
    errors.push('One special character (!@#$%^&*)');
  }

  return errors;
}

// Login validation schema
export const loginSchema = z.object({
  email: emailSchema,
  password: z.string().min(1, 'Password is required')
});

// Register validation schema with strong password requirements
export const registerSchema = z.object({
  fullName: z
    .string()
    .min(1, 'Full name is required')
    .min(2, 'Full name must be at least 2 characters')
    .max(100, 'Full name must not exceed 100 characters'),
  email: emailSchema,
  password: z
    .string()
    .min(8, 'Password must be at least 8 characters')
    .regex(/[A-Z]/, 'Password must contain at least one uppercase letter')
    .regex(/[a-z]/, 'Password must contain at least one lowercase letter')
    .regex(/[0-9]/, 'Password must contain at least one number')
    .regex(
      /[^A-Za-z0-9]/,
      'Password must contain at least one special character'
    )
});

// Note validation schema
export const noteSchema = z.object({
  title: z
    .string()
    .min(1, 'Title is required')
    .max(200, 'Title must not exceed 200 characters'),
  content: z
    .string()
    .max(5000, 'Content must not exceed 5000 characters')
    .optional()
});

export type LoginFormData = z.infer<typeof loginSchema>;
export type RegisterFormData = z.infer<typeof registerSchema>;
export type NoteFormData = z.infer<typeof noteSchema>;
