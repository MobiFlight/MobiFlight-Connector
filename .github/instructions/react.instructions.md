---
description: 'ReactJS development standards and best practices'
applyTo: '**/*.jsx, **/*.tsx, **/*.js, **/*.ts, **/*.css, **/*.scss'
---

# ReactJS Development Instructions

Instructions for building high-quality ReactJS applications with modern patterns, hooks, and best practices following the official React documentation at https://react.dev.

## Project Context
- React 19+ with React Compiler (babel-plugin-react-compiler)
- TypeScript for type safety with strict mode
- Vite as the build tool with path aliases (`@/` maps to `./src/`)
- Functional components with hooks as default
- shadcn/ui component library with Radix UI primitives
- Tailwind CSS v4 for styling with CSS variables
- Zustand for global state management
- react-i18next for internationalization
- Playwright for end-to-end testing, Vitest for unit tests
- ESLint with typescript-eslint and react-hooks plugin
- Prettier with tailwindcss plugin (no semicolons)

## Development Standards

### Architecture
- Use functional components with hooks as the primary pattern
- Organize by feature: `components/`, `pages/`, `stores/`, `lib/`, `types/`
- UI primitives go in `components/ui/` (shadcn/ui convention)
- Custom hooks go in `lib/hooks/`
- Zustand stores go in `stores/` with pattern: `use[Name]Store`
- Type definitions go in `types/` as `.d.ts` or `.ts` files
- Use `@/` path alias for imports (e.g., `@/components/ui/button`)

### TypeScript Integration
- Define types in `types/` directory, export via `types/index.ts`
- Use TypeScript interfaces for props, state, and store definitions
- Prefer `interface` for object shapes, `type` for unions and intersections
- Use `React.ComponentProps<"element">` for extending HTML element props
- Define separate interfaces for state and actions in Zustand stores

### Component Design
- Use default exports for page components and feature components
- Follow the single responsibility principle for components
- Use descriptive and consistent naming conventions
- Implement proper prop validation with TypeScript or PropTypes
- Design components to be testable and reusable
- Keep components small and focused on a single concern
- Use composition patterns (render props, children as functions)
- Use named exports for UI primitives and utilities
- Follow shadcn/ui patterns: use `cn()` utility for conditional classes
- Use `class-variance-authority` (cva) for component variants
- Accept `className` prop and merge with `cn()` for style overrides
- Use `data-testid` attributes for Playwright test selectors

### State Management
- Use Zustand for global/shared state (stores pattern)
- Define stores with separate state and actions interfaces:
  ```typescript
  interface StoreState { ... }
  interface StoreActions { ... }
  export const useStore = create<StoreState & StoreActions>((set) => ({ ... }))
  ```
- Create selector hooks for commonly accessed state slices
- Use `useState` for local component state only
- Use `useRef` for mutable values that don't trigger re-renders

### Hooks and Effects
- React Compiler handles memoization - avoid manual `useMemo`/`useCallback` unless profiling shows need
- Use `useEffect` with proper dependency arrays
- Implement cleanup functions in effects for event listeners
- Create custom hooks in `lib/hooks/` for reusable logic
- Follow naming convention: `use[Feature]` (e.g., `useAppMessage`, `useTheme`)
- Use `useRef` for accessing DOM elements and storing mutable values

### Styling
- Use Tailwind CSS v4 with CSS custom properties for theming
- Use `cn()` from `@/lib/utils` for merging class names
- Use `tailwind-merge` to handle class conflicts
- Follow mobile-first responsive design
- Use shadcn/ui component variants via `cva` definitions in `components/ui/variants.ts`
- Prettier auto-sorts Tailwind classes (via prettier-plugin-tailwindcss)

### Icons
- Use `@tabler/icons-react` for icons (e.g., `IconPlus`, `IconTrash`)
- Custom SVG icons go in `components/icons/`
- Pass `className` prop to icons for styling consistency

### Performance Optimization
- Implement virtual scrolling for large lists
- Profile components with React DevTools to identify performance bottlenecks

### Internationalization
- Use `react-i18next` with `useTranslation()` hook
- Translation keys follow dot notation: `"Namespace.Section.Key"`
- Translation files in `public/locales/{lang}/translation.json`
- Support multiple namespaces: `translation`, `feed`
- Use `t()` function for all user-visible strings

### Data Fetching & Communication
- Use custom `useAppMessage` hook for WebView message handling
- Use `publishOnMessageExchange()` to send messages to backend
- Define message types in `types/messages.d.ts` and `types/commands.d.ts`
- Handle loading, error, and success states appropriately

### Routing
- Use React Router v7 (`react-router`, `react-router-dom`)
- Define routes in `Routes.tsx`
- Use `useParams()`, `useSearchParams()`, `useNavigate()` hooks
- Page components go in `pages/` directory

### Testing
- E2E tests use Playwright in `tests/` directory
- Unit tests use Vitest in `__tests__/` directories or `.test.ts` files
- Use Page Object pattern for Playwright: fixtures in `tests/fixtures/`
- Use `data-testid` attributes for reliable test selectors
- Test user interactions, not implementation details
- Test accessibility features and keyboard navigation

### Error Handling
- Use Error Boundaries for component-level error handling
- Handle async errors in effects and event handlers
- Provide meaningful error states in UI components
- Log errors appropriately in development mode

### Accessibility
- Use semantic HTML elements appropriately
- Implement proper ARIA attributes and roles
- Ensure keyboard navigation works for all interactive elements
- Provide alt text for images and descriptive text for icons

### Code Style
- No semicolons (Prettier config)
- Use double quotes for strings in JSX, single quotes in JS/TS
- Arrow functions for components: `const Component = () => { ... }`
- Destructure props in function signature
- Keep components focused and single-purpose

## File Structure
```
frontend/
├── src/
│   ├── components/
│   │   ├── ui/           # shadcn/ui primitives
│   │   ├── icons/        # Custom SVG icons
│   │   ├── modals/       # Dialog/modal components
│   │   └── [feature]/    # Feature-specific components
│   ├── pages/            # Route page components
│   ├── stores/           # Zustand stores
│   │   └── __tests__/    # Store unit tests
│   ├── lib/
│   │   ├── hooks/        # Custom hooks
│   │   └── utils.ts      # Utility functions (cn, etc.)
│   ├── types/            # TypeScript type definitions
│   └── assets/           # Static assets
├── tests/                # Playwright E2E tests
│   ├── fixtures/         # Page objects and test helpers
│   └── data/             # Test data files
└── public/
    └── locales/          # i18n translation files
```

## Implementation Process
1. Define TypeScript interfaces in `types/`
2. Create or update Zustand store if needed
3. Build UI with shadcn/ui components and Tailwind
4. Add translations to locale files
5. Implement business logic with custom hooks
6. Add `data-testid` attributes for testing
7. Write Playwright tests for user flows
8. Run `npm run lint` and `npm run check:i18n` before commit

## Additional Guidelines
- Use existing shadcn/ui components before creating custom ones
- Keep bundle size in mind - use dynamic imports for large features
- Follow existing patterns in the codebase for consistency
- Use `lodash-es` (aliased as `lodash`) for utility functions
- Test with `npm run dev` and verify in browser before committing

## Common Patterns
- Provider pattern for context-based state sharing (ThemeProvider, TooltipProvider)
- Custom hooks for reusable logic extraction (`useAppMessage`, `useKeyAccelerators`)
- Zustand selector hooks for optimized re-renders
- Page Object pattern for E2E tests
- Message exchange pattern for WebView communication