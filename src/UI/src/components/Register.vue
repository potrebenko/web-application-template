<template>
  <v-container class="fill-height" fluid>
    <v-row class="justify-center align-center">
      <v-col cols="12" sm="8" md="4">
        <v-card class="elevation-12">
          <v-card-title class="text-center">Register</v-card-title>
          <v-card-text>
            <v-form @submit.prevent="handleSubmit">
              <v-text-field
                v-model="state.name"
                :error-messages="v$.name.$errors.map(e => e.$message).join(', ')"
                label="Name"
                required
                prepend-icon="mdi-account"
                @blur="v$.name.$touch()"
                @input="v$.name.$touch()"
              ></v-text-field>
              <v-text-field
                v-model="state.email"
                :error-messages="v$.email.$errors.map(e => e.$message).join(', ')"
                label="Email"
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

              <v-text-field
                v-model="state.confirmPassword"
                :error-messages="v$.confirmPassword.$errors.map(e => e.$message).join(', ')"
                :append-icon="showConfirmPassword ? 'mdi-eye' : 'mdi-eye-off'"
                :type="showConfirmPassword ? 'text' : 'password'"
                label="Confirm Password"
                required
                prepend-icon="mdi-lock-check"
                @click:append="showConfirmPassword = !showConfirmPassword"
                @blur="v$.confirmPassword.$touch()"
                @input="v$.confirmPassword.$touch()"
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
                  Register
                </v-btn>
              </v-card-actions>

              <div class="text-center mt-3">
                Already have an account?
                <router-link to="/login">Login here</router-link>
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
import { required, email, sameAs } from '@vuelidate/validators'
import api from '../services/api'

const router = useRouter()
const errorMessage = ref('')
const loading = ref(false)
const showPassword = ref(false)
const showConfirmPassword = ref(false)

const state = reactive({
  name: '',
  email: '',
  password: '',
  confirmPassword: ''
})

const rules = computed(() => ({
  name: { required },
  email: { required, email },
  password: { required },
  confirmPassword: {
    required,
    sameAsPassword: sameAs(state.password)
  }
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

    await api.post('/users/register', {
      name: state.name,
      email: state.email,
      password: state.password
    })

    router.push('/login')
  } catch (error: any) {
    errorMessage.value = error.response?.data?.message || 'An error occurred during registration'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
/* Vuetify handles most styling */
</style>
