<script setup lang="ts">
const props = defineProps<{
  currentStep: number
  steps: { title: string }[]
  isLastStepCompleted: boolean
}>()
</script>

<template>
  <div class="steps">
    <div
      v-for="(_, index) in steps" 
      class="steps__item"
      :key="index"
      :class="{ 'active': index + 1 === currentStep, 'completed': index + 1 < currentStep || isLastStepCompleted }"
    >
      <div class="steps__circle">
        <svg
          v-if="index + 1 < currentStep || isLastStepCompleted"
          width="16" height="16" viewBox="0 0 16 16"
          fill="none"
        >
          <g clip-path="url(#clip0_121_383)">
            <path
              fill-rule="evenodd" clip-rule="evenodd"
              d="M8 16C3.5816 16 0 12.4184 0 8C0 3.5816 3.5816 0 8 0C12.4184 0 16 3.5816 16 8C16 12.4184 12.4184 16 8 16ZM7.0584 9.712L4.8464 7.4984L4 8.3448L6.4952 10.8416C6.64522 10.9916 6.84867 11.0758 7.0608 11.0758C7.27293 11.0758 7.47638 10.9916 7.6264 10.8416L12.388 6.0816L11.5384 5.232L7.0584 9.712Z"
              fill="currentColor"
            />
          </g>
          <defs>
            <clipPath id="clip0_121_383">
              <rect width="16" height="16" fill="white"/>
            </clipPath>
          </defs>
        </svg>
        <span v-else></span>
      </div>
      <div
        v-if="index < steps.length - 1"
        class="steps__line"
        :class="{ 'active': index + 2 === currentStep, 'completed': index + 2 < currentStep || isLastStepCompleted }"
      ></div>
    </div>
  </div>
  <div class="step">
    <h1 class="step__title h1">{{ steps[currentStep - 1].title }}</h1>
  </div>
  <slot name="content"></slot>
</template>

<style scoped lang="scss">
@use '../../styles/helpers/' as *;

.steps {
  display: flex;
  align-items: center;
  justify-content: center;
  column-gap: rem(16);

  &__item {
    display: flex;
    align-items: center;

    &.active .steps__circle {
      background-color: var(--color-accent-1);
      box-shadow: var(--shadow-dark);
    }

    &.completed .steps__circle {
      background-color: var(--color-accent-2);

      svg {
        background-color: var(--color-light);
        color: var(--color-accent-2);
        transition-duration: var(--transition-duration);
      }
    }

    &:not(.completed):not(.active) + & .steps__line {
      background-color: var(--color-gray);
    }
  }

  &__circle {
    @include square(16);
    @include flex-center;

    background-color: var(--color-gray);
    border-radius: 50%;
    transition-duration: var(--transition-duration);
  }

  &__line {
    margin-left: rem(16);
    width: rem(120);
    height: rem(2);
    background-color: var(--color-gray);
    border-radius: rem(2);
    transition-duration: var(--transition-duration);

    &.active {
      background-color: var(--color-accent-1);
    }

    &.completed {
      background-color: var(--color-accent-2);
    }
  }
}

.step {
  display: flex;
  flex-direction: column;
  align-items: center;
  row-gap: rem(30);

  &:not(:first-child) {
    margin-top: rem(30);
  }
}
</style>
