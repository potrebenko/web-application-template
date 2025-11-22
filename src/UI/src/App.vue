<template>
  <v-app>
    <v-app-bar v-if="isUserAuthenticated" app color="primary" dark>
      <v-toolbar-title>{{ appTitle }} (v{{ packageVersion }})</v-toolbar-title>
      <v-spacer></v-spacer>
      <v-btn @click="handleLogout" :loading="isLoading">
        Logout
      </v-btn>
    </v-app-bar>
    <v-main>
      <router-view />
    </v-main>
    <v-footer app>
      &copy; {{ new Date().getFullYear() }} Net Template {{ packageVersion }}
    </v-footer>
  </v-app>
</template>

<script lang="ts" setup>
import { ref, onMounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import { isAuthenticated, logout } from '@/utils/auth'
// Using the global APP_CONFIG defined in vite.config.ts
const appTitle = APP_CONFIG.title
// Using the package version from package.json
const packageVersion = PACKAGE_VERSION

const router = useRouter()
const isUserAuthenticated = ref<boolean>(false)
const isLoading = ref<boolean>(false)

// Check authentication status
const checkAuth = async (): Promise<void> => {
  isUserAuthenticated.value = await isAuthenticated()
}

// Handle logout
const handleLogout = async (): Promise<void> => {
  try {
    isLoading.value = true
    await logout()
    isUserAuthenticated.value = false
    router.push('/login')
  } catch (error) {
    console.error('Logout failed:', error)
  } finally {
    isLoading.value = false
  }
}

// Check auth on component mount
onMounted(async () => {
  await checkAuth()
})

// Watch for route changes to update auth status
watch(() => router.currentRoute.value, async () => {
  await checkAuth()
}, { deep: true })

</script>

<style>

</style>
