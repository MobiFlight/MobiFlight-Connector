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

If you want to run the tests outside the dev container the Playwright dependencies will need to be installed manually:

```sh
npm install
sudo npx playwright install-deps
npx playwright install
```

## Environment Variables

Create a `.env` file in the frontend project root (`src/MobiFlightConnector/frontend/`, next to `.env.template`) with the credentials for the accounts the tests sign in with:

```
# Club member account
TESTS_MEMBER_EMAIL=your-member-test-email@example.com
TESTS_MEMBER_PASSWORD=your-member-password
TESTS_MEMBER_NAME=Your Member Name

# Basic account
TESTS_BASIC_EMAIL=your-basic-test-email@example.com
TESTS_BASIC_PASSWORD=your-basic-password
TESTS_BASIC_NAME=Your Basic Name
```

> [!NOTE] Never commit your real `.env` file to version control.

Each account type is optional. If its variables are missing, the corresponding authentication setup and the tests that depend on it are **skipped**.
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

```

---

For more information, see the [Playwright documentation](https://playwright.dev/).
```
