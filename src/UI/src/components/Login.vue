<template>
  <v-container class="fill-height" fluid>
    <v-row class="justify-center align-center">
      <v-col cols="12" sm="8" md="4">
        <v-card class="elevation-12">
          <v-card-title class="text-center">Login</v-card-title>
          <v-card-text>
            <v-form @submit.prevent="handleSubmit">
              <v-text-field
                v-model="state.email"
                :error-messages="v$.email.$errors.map(e => e.$message).join(', ')"
                label="Email"
                type="text"
                required
                prepend-icon="mdi-email"
                @blur="v$.email.$touch()"
                @input="v$.email.$touch()"
              ></v-text-field>

              <v-text-field
                v-model="state.password"
                :error-messages="v$.password.$errors.map(e => e.$message).join(', ')"
                :append-icon="showPassword ? 'mdi-eye' : 'mdi-eye-off'"
                :type="showPassword ? 'text' : 'password'"
                label="Password"
                required
                prepend-icon="mdi-lock"
                @click:append="showPassword = !showPassword"
                @blur="v$.password.$touch()"
                @input="v$.password.$touch()"
              ></v-text-field>

              <v-alert
                v-if="errorMessage"
                type="error"
                class="mt-3"
              >
                {{ errorMessage }}
              </v-alert>

              <v-card-actions class="justify-center">
                <v-btn
                  color="primary"
                  type="submit"
                  :loading="loading"
                  block
                >
                  Login
                </v-btn>
              </v-card-actions>

              <div class="text-center mt-3">
                Don't have an account?
                <router-link to="/register">Register here</router-link>
              </div>
            </v-form>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts" setup>
import { reactive, ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useVuelidate } from '@vuelidate/core'
import { required, email } from '@vuelidate/validators'
import api from '../services/api'
import { setToken } from '../utils/auth'

const router = useRouter()
const errorMessage = ref('')
const loading = ref(false)
const showPassword = ref(false)

const state = reactive({
  email: '',
  password: ''
})

const rules = computed(() => ({
  email: { required, email },
  password: { required }
}))

const v$ = useVuelidate(rules, state)

const handleSubmit = async () => {
  const result = await v$.value.$validate()
  if (!result) {
    return
  }

  try {
    loading.value = true
    errorMessage.value = ''

    const response = await api.post('/users/login', {
      email: state.email,
      password: state.password
    }, {
     withCredentials: true // Necessary to receive cookies
    })

    setToken(response.data.token)
    router.push('/dashboard')
  } catch (error: any) {
    errorMessage.value = error.response?.data?.message || 'An error occurred during login'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
/* Vuetify handles most styling */
</style>
