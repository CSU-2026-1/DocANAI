<script setup lang="ts">
import { ref } from 'vue'

defineOptions({ inheritAttrs: false })

const props = withDefaults(defineProps<{
  id: string
  modelValue?: string | number
  placeholder?: string
  type: 'text' | 'password'
  required?: boolean
}>(), {
  modelValue: '',
  placeholder: '',
  required: false
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: string | number): void
}>()

const isFocused = ref(false)
const inputRef = ref<HTMLInputElement | null>(null)

const onInput = (event: Event) => {
  const target = event.target as HTMLInputElement
  emit('update:modelValue', target.value)
}

defineExpose({ focus: () => inputRef.value?.focus() })
</script>

<template>
  <div
    class="input"
    :class="{'input--focused': isFocused, 'input--filled': modelValue}"
  >
    <div v-if="$slots.icon" class="input__icon">
      <slot name="icon"></slot>
    </div>
    <input
      class="input__field"
      :id="id"
      :type="type"
      ref="inputRef"
      :value="modelValue"
      :placeholder="placeholder"
      v-bind="$attrs"
      @input="onInput"
      @focus="isFocused = true"
      @blur="isFocused = false"
    >
  </div>
</template>

<style scoped lang="scss">
@use '../../styles/helpers/' as *;

.input {
  display: flex;
  align-items: center;
  position: relative;
  width: 100%;
  border: var(--border);
  border-radius: var(--border-radius-small);
  transition-duration: var(--transition-duration);

  @include hover {
    border-color: var(--color-accent-1);
  }

  &--focused {
    border-color: var(--color-accent-1);

    .input__icon,
    .input__field {
      color: var(--color-dark);
    }
  }

  &--filled {
    .input__icon,
    .input__field {
      color: var(--color-dark);
    }
  }

  &__icon {
    @include flex-center;

    padding-left: rem(12);
    color: var(--color-gray);
    transition-duration: var(--transition-duration);
  }

  &__field {
    flex: 1;
    padding: rem(12) rem(12) rem(12) rem(24);
    width: 100%;
    color: var(--color-gray);
    background-color: transparent;
    border: none;
    outline: none;
  }
}
</style>
