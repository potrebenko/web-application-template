# Styles Directory

This directory contains global styling files for the application.

## settings.scss

The `settings.scss` file contains global SCSS variables and Vuetify theme customizations. It uses the modern Sass module system with `@use` instead of the legacy `$variable` syntax.

### Key Features:

- **Vuetify Theme Customization**: Configures Vuetify's theme variables using the `@use` rule
- **CSS Custom Properties**: Defines global CSS variables using `:root` for consistent styling
- **Global Styles**: Sets up base styles for HTML and body elements
- **Utility Classes**: Provides commonly used utility classes

## Usage

To use these styles in your components:

1. For Vuetify theme customization, the settings are automatically applied
2. For custom CSS variables, access them in your component styles:

```scss
.my-component {
  color: var(--primary-color);
  background-color: var(--secondary-color);
}
```

3. For utility classes, simply add them to your elements:

```html
<div class="flex-center full-height">
  <p class="text-center">Centered content</p>
</div>
```

## Modern Sass Best Practices

This project uses the modern Sass module system to avoid the "Legacy JS API is deprecated" warning:

- Use `@use` instead of `@import`
- Use CSS custom properties (variables) with `--name` syntax
- Place global variables in `:root` selector
- Import Sass built-in modules with `@use "sass:math"`, etc.