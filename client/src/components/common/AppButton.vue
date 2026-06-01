<script setup lang="ts">
const props = defineProps<{
  text: string
  type: 'button' | 'submit'
  light?: boolean
  disabled?: boolean
}>()
</script>

<template>
  <button
    class="button"
    :class="{ 'button--light': light }"
    :type="type"
    :disabled="disabled"
  >
    <div class="button__text">{{ text }}</div>
    <div class="button__icon">
      <slot name="icon"></slot>
    </div>
  </button>
</template>

<style scoped lang="scss">
@use '../../styles/helpers/' as *;

.button {
  display: flex;
  align-items: center;
  justify-content: center;
  column-gap: rem(12);
  padding: rem(12) rem(36);
  color: var(--color-light);
  background-color: var(--color-accent-2);
  border: none;
  border-radius: var(--border-radius-small);
  transition-duration: var(--transition-duration);

  @include hover {
    scale: 1.01;
    box-shadow: var(--shadow-accent);
  }

  &--light {
    color: var(--color-dark);
    background-color: var(--color-light);

    @include hover {
      box-shadow: var(--shadow-dark);
    }
  }

  &:disabled {
    opacity: 0.3;
    cursor: not-allowed;

    @include hover {
      scale: 1;
      box-shadow: none;
    }
  }

  &__text {
    @include fluid-text(20, 16);

    font-weight: 60;
  }

  &__icon {
    @include square(24);
  }
}
</style>
