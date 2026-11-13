# Development implementation status

The first implementation is a development preview, not a production release or a demonstration of 100% linguistic accuracy.

## Implemented

- C# .NET Framework 4.8 core and native WinForms control embedded through Paratext's window plugin interface.
- Host menus, scope controls, verse-linked findings, Tamil explanations, context preview, status filters, and visible informational build version.
- Current verse, chapter, book, selected books and whole-project scans. Exact single-verse selections require unambiguous original-text mapping.
- Immutable scans, conservative USFM segments, UTF-8/grapheme boundary checks, stable findings and explicit abstention for nominated unresolved boundaries.
- Four limited suggestion families: demonstrative + known noun, known overt accusative + known verb, known overt dative + known verb, and known vanthodar participle + known verb. The small built-in lexical sets still require linguistic review.
- Shared Apply / Modify & Apply service with source-hash validation under the host lock, span/permission checks, durable intent and save verification.
- Reject, scan-only Ignore, project-persistent occurrence Ignore, Leave open, saved-decision reset, and local review audit records.
- Guarded Revert using a full post-edit chapter hash; durable pending/uncertain transaction reconciliation before subsequent writes.
- Annotation provider and context commands sharing window findings/services. Ambiguous host verse mapping fails closed.
- Opt-in exact house lexemes, local journals/configuration, offline runtime and no third-party runtime dependencies.
- Build/install/package scripts and Windows CI using the official compile-only SDK pinned to 2.0.100.

## Validation evidence so far

- Local build against Paratext 9.5.110.1's installed assemblies succeeds with warnings treated as errors.
- 322 core assertions pass, consuming all 99 supplied scenarios for preservation, protected examples, finding contracts and limited prediction coverage.
- Of 24 conditional suggestion scenarios, 10 receive the expected suggestion without external annotations; 14 are unimplemented or abstained. This is development-fixture coverage, not independent accuracy evidence.
- Host-contract tests load both interfaces, enumerate menus and construct/layout the native control against SDK 2.0.100. They are not in-host acceptance tests.
- Program Files installation initially failed with Windows access denied. Administrative installation and exact-host acceptance remain pending until recorded in issue evidence.

## Remaining release gates

- Verify installation, discovery, navigation, annotation mapping/context menus and save/revert inside the exact host on a disposable project.
- Extend exact passage mapping to multi-verse/multi-chapter partial selections. The preview refuses these rather than widening the scope.
- Add a full pre-scan verse count, finer verse progress, structured export and broader review-history recovery UI.
- Expand morphology/syntax and licensed lexicons, independently review word classifications, and complete remaining suggestion and compound analyses.
- Evaluate precision, recall and abstention on held-out expert-reviewed text. No independent evaluation has occurred.
- Complete accessibility, appearance, long-scope performance, offline-host, concurrent-write and restart/recovery acceptance; establish measured budgets.

Issues stay open until their full acceptance criteria have evidence. A prerelease ZIP does not mean all 14 tasks are finished. The [product plan](PRODUCT_PLAN.md) remains the target behavior.
