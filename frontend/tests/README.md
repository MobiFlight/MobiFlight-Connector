# MobiFlight Frontend Tests

This folder contains end-to-end and integration tests for the MobiFlight frontend, using [Playwright](https://playwright.dev/).

## Structure

- **fixtures/**: Custom Playwright fixtures and helpers.
- **auth.setup.ts**: Handles authentication setup for tests that require a logged-in user.
- **.auth/**: Stores authentication state (e.g., user.json) for Playwright.
- **Other test files**: Contain actual test cases for UI and features.

> [!IMPORTANT]  
>  Tests that require secrets will be **skipped** if the necessary environment variables are not set.

## Running the Tests

### Inside the Development Container (devContainer)

All dependencies are automatically installed when you open this project in the dev container.  
You can simply run:

```sh
npx playwright test
```

Or, you can use the VSCode extension for Playwright to run the tests.

### Without the Dev Container

If you want to run the tests outside the dev container, check the required steps in `.devcontainer.json` under `postCreateCommand`.  
Typically, you need to run:

```sh
npm install
sudo npx playwright install-deps
npx playwright install
```

## Environment Variables

Create a `.env` file in the project root (see `.env.template` for required variables):

```
TESTS_USER_EMAIL=your-test-email@example.com
TESTS_USER_PASSWORD=your-password
TESTS_USER_NAME=Your Name
```

> [!NOTE] Never commit your real `.env` file to version control.

Tests that require secrets will be **skipped** if the necessary environment variables are not set.

## CI and Secrets

- In CI, secrets are injected as environment variables.
- For security, secrets are **not available** to PRs from forks. In such cases, secret-dependent tests are skipped automatically.

## Contributing

- If you want to contribute tests that require authentication, create your own `.env` file locally.
- Secret-dependent tests will run for maintainers or trusted branches in CI.
- Please see the root `CONTRIBUTING.md` for more details.

## Useful Commands

- Run all tests:  
  ```sh
  npx playwright test
  ```
- Run a specific test file:  
  ```sh
  npx playwright test tests/example.spec.ts
  ```
- Open Playwright Test UI:  
  ```sh
  npx playwright test --ui
  ```

---

For more information, see the [Playwright documentation](https://playwright.dev/).