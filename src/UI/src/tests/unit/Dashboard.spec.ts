import { describe, it, expect, vi } from 'vitest'
import { mount } from '@vue/test-utils'
import Dashboard from '@/components/Dashboard.vue'

// Mock Vuetify components
vi.mock('vuetify/components', () => {
  return {
    VContainer: {
      name: 'v-container',
      render: (h) => h('div', { class: 'v-container' }, [h('slot')])
    },
    VRow: {
      name: 'v-row',
      render: (h) => h('div', { class: 'v-row' }, [h('slot')])
    },
    VCol: {
      name: 'v-col',
      render: (h) => h('div', { class: 'v-col' }, [h('slot')])
    }
  }
})

describe('Dashboard.vue', () => {
  it('renders the dashboard title correctly', async () => {
    const wrapper = mount(Dashboard, {
      global: {
        stubs: {
          'v-container': true,
          'v-row': true,
          'v-col': true
        }
      }
    })
    
    // Check if the title is rendered
    expect(wrapper.find('h1').text()).toBe('Dashboard')
  })

  it('contains paragraphs of text', async () => {
    const wrapper = mount(Dashboard, {
      global: {
        stubs: {
          'v-container': true,
          'v-row': true,
          'v-col': true
        }
      }
    })
    
    // Check if paragraphs exist
    const paragraphs = wrapper.findAll('p')
    expect(paragraphs.length).toBeGreaterThan(0)
  })
})
