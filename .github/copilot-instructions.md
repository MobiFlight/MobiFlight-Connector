## Definition of Done for MobiFlight PRs
### General Guidelines
- Follow existing design patterns and architecture
- Consistent with existing code style and conventions
- Backward compatibility unless a breaking change is explicitly intended, use migration routines where necessary
- Create unit tests to cover existing functionality BEFORE starting code refactoring.

### Pull request checklist
Always check for the following items before marking a PR as ready to merge, if applicable:
- PR description is clear and provides context and serves as release notes for end users
- PR description describes the problem being solved and the feature being added
- PR description "fixes" or "closes" related issues
- PR description mentions any breaking changes
- PR description provides a section with technical details of the implementation
- PR description includes screenshots or screen recordings for UI changes
- All new code is covered by unit tests where applicable
- All new code is covered by integration or end-to-end tests where applicable
- All existing and new tests pass successfully
- Always use i18n for user-facing strings
- Add label `docs` if documentation updates are required