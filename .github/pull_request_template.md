<!-- 
please use branch name following the convention: 
#GITHUB_ISSUE/[descriptive-branch-name]
-->
### Summary
<!-- Describes the solution and what problem it addresses solved -->
<!-- This is also for regular, non-technical users -->

### Screenshots / recordings
<!-- Before/After For UI changes; delete this section if not applicable -->

### Acceptance criteria
<!-- List specific testable items here, e.g. use cases, features -->
<!-- This also serves as checklist during development -->
- [ ] X added
- [ ] Y still working
- [ ] Z changed

### DoD Checklist
<!-- ~~strike irrelevant items~~ -->
- [ ] Unit tests available
  - [ ] .NET backend
  - [ ] Frontend
- [ ] Frontend tests
- [ ] i18n - All user-facing strings are translated
  - [ ] core (en,de,es)
  - [ ] additional langs
- [ ] documentation
  - [ ] User docs (docs.mobiflight.com)
  - [ ] Developer docs (e.g., readme.md)
     
## Related issues
<!-- fixes, relates to existing #issues -->
fixes #ISSUE_NUMBER

### Out-of-scope
<!-- mention what is not covered by this PR -->

### Notes
<!-- provide further information that might be of interest -->

<!-- Leave this here in the end as a quick help for users wanting to test this PR later -->
> [!tip] 
> ### How to test a Pull Request build?
> If you would like to provide feedback on a PR build like this one, here's how:
> 1. Scroll down to the bottom of this page, to find the _last_ comment from _github-actions_
> 2. Download the MobiFlightConnector.zip file link on that comment and open it from your browser downloads
> 3. Navigate inside the zip with the Windows file manager and locate MFConnector.exe and double click it to open
> 4. Choose "Extract all" and select the default suggested destination, it will save to your Documents/MobiFlightConnector-0.0.NNNN.XXXX where NNNN is the pull request number, so you can later know which is which, if you are testing many PR builds.
>
> Now you can open the MFConnector.exe from the created folder. The PR build will have its own settings, and profile history, so it does not mess up your default MobiFlight installation.
