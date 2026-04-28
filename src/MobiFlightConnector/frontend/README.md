# Initial setup of frontend project

## vite / react / tailwindcss / shadcn
Followed the information from the vite and tailwind websites, e.g. https://ui.shadcn.com/docs/installation/vite

## playwright test
Follow information for playwright installation, e.g. https://playwright.dev/docs/intro

## prettier for tailwind
```
npm install --save-dev --save-exact prettier prettier-plugin-tailwindcss
```
## react-i18next
Follow information from website: https://github.com/i18next/react-i18next
Add http backend, `npm install i18next-http-backend`

## zustand
https://www.npmjs.com/package/zustand

## lodash-es
https://www.npmjs.com/package/lodash-es
and @types/lodash-es

## Community feed loading
The community feed uses a hybrid strategy:

- Immediate fallback: `feed:community` from bundled i18n files in `public/locales/*/feed.json`
- Background refresh: optional remote JSON loaded per language
- Replacement rule: remote feed replaces fallback only when remote `community` contains at least one valid item
- Fallback rule: timeout, network errors, invalid payloads, or empty remote arrays keep bundled fallback visible

Environment variables:

- `VITE_FEED_REMOTE_BASE_URL`: remote base URL in the format `{base}/{language}/feed.json`