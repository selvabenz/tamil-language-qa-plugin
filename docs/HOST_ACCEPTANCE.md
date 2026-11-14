# Disposable in-host acceptance

`tests-host/InHostAcceptance.cs` is a developer-only startup plugin. The normal product build and release ZIP exclude it. It tests the production `HostStore` source and core transaction service through real Paratext APIs.

Build the product, then run `scripts/build-host-acceptance.ps1`. This creates a unique ignored `artifacts/host-acceptance-*` directory containing a helper assembly and a one-shot request. The generated helper is bound to that exact local directory; it is not portable or a release deliverable.

The runner refuses any host other than 9.5.110.1 and any project other than `NSTest`, with full name `Note Suggestions Disposable Test`, a non-resource project whose Genesis 1 contains `Synthetic test only`. It must have edit permission. It backs up the original chapter and SHA-256 locally before acquiring a write lock and replacing the disposable chapter with the synthetic fixture. It tests protected USFM scanning, Apply/readback, stale-finding refusal, Revert, and Modify & Apply/Revert. Restoration acquires a fresh lock and refuses to overwrite an unexpected intervening change. The original backup is retained even after a successful restoration.

With Paratext closed normally, install only the generated helper beside the already installed product plugin. Reopen Paratext to consume the request once. Inspect `report.json` and verify every test and exact original restoration before recording a pass. A compiled helper or consumed request alone is not evidence of success. Remove the helper from the installation with Paratext closed after testing; never package it.

The local chapter backup and transaction journal may contain project text. Keep the entire generated run directory private. Publish only a reviewed test summary without project IDs, paths, backups, or journal contents.

This runner does not validate window interaction, annotations, every scope, independent linguistic accuracy, or offline host behavior. Those remain separate acceptance gates.
