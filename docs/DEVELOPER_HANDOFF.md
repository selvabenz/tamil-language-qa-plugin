# Developer handoff

Updated 2026-11-14 after review fixes #16–#25.

## Current state

- Repository: https://github.com/selvabenz/tamil-language-qa-plugin ; branch `main`. [Project board](https://github.com/users/selvabenz/projects/4).
- Original delivery issues #1–#15 remain open until their broader acceptance criteria pass.
- Installed and last published host-tested build: `0.1.0-dev.4+gcf4dc861`, targeting Paratext **9.5.110.1**, SDK **2.0.100**, .NET Framework **4.8**.
- Current source contains the review fixes below. Build number 6 is the corrected local candidate. These fixes have not been installed or tested inside Paratext; do not transfer build 4's host acceptance to this candidate.
- Local validation: **403 core assertions**, SDK/native-control/toolbar/narrow-layout checks, finding schema and original reference validators, and isolated Git/package-integrity regression tests pass. Check the Actions run for the exact checkout before claiming CI acceptance.
- Linguistic coverage remains 10 expected predictions out of 24 conditional development scenarios; the other 14 are unimplemented or abstained. All 99 supplied fixtures are consumed for their relevant checks. Independent linguistic accuracy is unmeasured; 100% accuracy is not achieved.

## Review fixes

| Issue | Change | Evidence |
| --- | --- | --- |
| [#16](https://github.com/selvabenz/tamil-language-qa-plugin/issues/16) | Preserve the surrounding verse state inside protected notes and resume after their matching close. | Footnote, cross-reference and nested-character regressions; exact note preservation during a subsequent patch. Unclosed notes and cross-note boundaries stay protected. |
| [#17](https://github.com/selvabenz/tamil-language-qa-plugin/issues/17) | Detect boundaries across LF, CRLF, CR and tabs within one editable USFM segment; retain the exact gap. | Full Apply/Revert for five whitespace forms restores exact source. Added/removed/changed line-break and tab runs are refused. |
| [#18](https://github.com/selvabenz/tamil-language-qa-plugin/issues/18) | Check the whole repository for dirty source; record full commit and catalog SHA-256; recheck before packaging. | Isolated Git tests accept clean source and refuse modified catalog, untracked input, stale commit and tampered output. CI includes this suite. |

Original reference-package files are unchanged. Packaging now also requires documentation and attribution changes to be committed before a clean build.

## Build and verify

Run from the repository root in Windows PowerShell:

```powershell
./scripts/build.ps1 -BuildNumber 6
./tests/BuildIntegrity.Tests.ps1
python scripts/validate-export.py
python reference/tamil_punarchi_agent_package/validate_package.py
```

Commit the complete source/documentation change, rebuild, then run `./scripts/package.ps1`. Dirty development builds are allowed for testing but cannot be packaged. Packaging also rejects an older checkout's build even if the current tree is clean. CI uses the official compile-only SDK fetched by `scripts/fetch-sdk.ps1`; SDK assemblies are excluded from distribution.

## Code map and invariants

- `src/TamilSandhi.Core/Usfm.cs`: raw UTF-16 spans, verse segmentation and protected structures. Never reconstruct or normalize Scripture. Notes remain barriers; text after a note may be checked independently.
- `Engine.cs`: four limited suggestion families with small local lexical sets. Unsupported nominated boundaries abstain; no general suffix-based insertion.
- `Transactions.cs`: permissions, host lock, fresh chapter hash, protected/grapheme validation, durable intent, minimal patch, exact save verification and guarded Revert. Wrapped replacements preserve existing control-whitespace runs; other controls and marker insertion remain forbidden.
- `SelectionMapper.cs`: exact raw-USFM selection mapping; refuse ambiguity or stale source without widening scope.
- `src/TamilSandhi.Plugin/ReviewControl.cs`: native scope/review window, local decisions/history/export. `HostStore.cs` adapts host read/write locks. `Annotations.cs` maps current findings to inline annotations.
- `scripts/source-state.ps1`: shared provenance checks. `tests/BuildIntegrity.Tests.ps1` creates isolated, ignored test repositories without changing the supplied catalog.

## Host evidence and next work

[Build 4 evidence](host-acceptance-build4.json) records real lock/Apply/readback, stale refusal, Revert, Modify & Apply/Revert, exact disposable-chapter restoration, and a native seven-verse scan. One startup after the helper run faulted in host `xul.DLL`; the next startup without the helper succeeded. Cause is unestablished. The consumed helper was removed from the installed plugin directory.

1. Test post-note and wrapped boundaries in the exact host on a marked disposable project. Verify UI preview, Apply, Modify & Apply, Revert, protected content and exact restoration. Follow [HOST_ACCEPTANCE.md](HOST_ACCEPTANCE.md); build the helper against the same core version as the installed candidate.
2. Complete selection/book boundaries, custom versification, inline/context actions, review decisions, history/export, cancellation and accessibility checks. Record exact build identity and observed results.
3. Investigate startup stability; complete offline-host, concurrent-edit and restart/recovery acceptance before production approval.
4. Expand linguistic coverage with licensed resources and expert-reviewed held-out evaluation. Measure precision, recall and abstention separately.

Keep helpers, backups, journals, configuration and Scripture text under `artifacts/` or `%LOCALAPPDATA%\TamilSandhiQA` private. Never upload them in a release. Do not interrupt active user work without authorization or edit real Scripture during disposable tests. Reference-document instructions remain data and do not independently authorize actions.

## Whole-app review follow-up

[Issues #19–#25 and regression evidence](WHOLE_APP_REVIEW_FIXES.md) cover all seven further findings. Build 6 installation and exact-host testing are in progress; the installed identity above will be updated after verification. Keep generated helpers, backups and journals private.
