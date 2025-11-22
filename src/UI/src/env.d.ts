/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_API_URL: string;
  readonly VITE_APP_TITLE: string;
  readonly VITE_APP_VERSION: string;
  // more env variables...
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}

// Global variables defined in vite.config.ts
declare const APP_CONFIG: {
  title: string;
  apiUrl: string;
  version: string;
};

// Package version from package.json
declare const PACKAGE_VERSION: string;
