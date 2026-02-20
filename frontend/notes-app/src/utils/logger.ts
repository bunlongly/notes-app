/**
 * Logger utility for consistent logging across the application
 */

const isDevelopment = import.meta.env.DEV;

export const logger = {
  info: (message: string, ...args: any[]) => {
    if (isDevelopment) {
      console.log(`[INFO] ${message}`, ...args);
    }
  },

  error: (message: string, error?: any) => {
    if (isDevelopment) {
      console.error(`[ERROR] ${message}`, error);
    }
  },

  warn: (message: string, ...args: any[]) => {
    if (isDevelopment) {
      console.warn(`[WARN] ${message}`, ...args);
    }
  },

  debug: (message: string, ...args: any[]) => {
    if (isDevelopment) {
      console.debug(`[DEBUG] ${message}`, ...args);
    }
  },

  api: {
    request: (method: string, url: string, data?: any) => {
      if (isDevelopment) {
        console.log(`[API REQUEST] ${method} ${url}`, data || '');
      }
    },
    response: (method: string, url: string, status: number, data?: any) => {
      if (isDevelopment) {
        console.log(`[API RESPONSE] ${method} ${url} - ${status}`, data || '');
      }
    },
    error: (method: string, url: string, error: any) => {
      if (isDevelopment) {
        console.error(`[API ERROR] ${method} ${url}`, error);
      }
    }
  },

  validation: {
    start: (formName: string) => {
      if (isDevelopment) {
        console.log(`[VALIDATION] Starting validation for ${formName}`);
      }
    },
    errors: (formName: string, errors: Record<string, string>) => {
      if (isDevelopment) {
        console.log(`[VALIDATION] ${formName} validation failed:`, errors);
      }
    },
    success: (formName: string) => {
      if (isDevelopment) {
        console.log(`[VALIDATION] ${formName} validation passed`);
      }
    }
  }
};
