<script setup lang="ts">
const props = defineProps<{
  text: string
  type: 'button' | 'submit'
  disabled?: boolean
}>()
</script>

<template>
  <button
    :class="`button`"
    :type="type"
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
  padding: rem(12) rem(16);
  color: var(--color-light);
  background: var(--color-accent-gradient);
  border: none;
  border-radius: var(--border-radius-small);
  box-shadow: var(--shadow-accent);
  transition-duration: var(--transition-duration);

  @include hover {
    scale: 1.01;
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
