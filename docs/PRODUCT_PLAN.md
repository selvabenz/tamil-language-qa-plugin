# Verse-by-verse Tamil Sandhi review

This plan records the owner's requirements of 13 November 2026. It supersedes the earlier diagnostic-only release goal: the intended product includes explicit, human-triggered safe application. The supplied reference package remains unchanged. These are target requirements; see [implementation status](IMPLEMENTATION_STATUS.md) for implemented behavior and outstanding gates.

## Scope and scan behavior

Select the active editable project and one scope: current verse, selected passage, chapter, book, or selected books / whole project. Show a reference summary and verse count before scanning. A partial verse selection stays limited to its selected text; surrounding text may be read for analysis but never becomes an edit target. Reject invalid or unsupported ranges explicitly.

Resolve references using the project's versification, including verse bridges, suffixes, empty and missing verses. Process each selected verse in canonical order, reporting progress, cancellation, completed / skipped / failed counts, and partial results. A passage may supply sentence context across a verse boundary, but findings and replacement spans remain within selected editable text. Do not generate a cross-verse patch; mark such cases for manual review. Headings, footnotes, cross-references, attributes, and structural markers are excluded from editing in the first release.

Capture project ID, scope, exact source revision/hash, rule catalog version, configuration version, and plugin build for each scan. Project switching or text changes invalidate affected findings. Incremental rescans must remove obsolete findings and re-evaluate context without duplicating results.

## Review actions

| Action | Behavior |
| --- | --- |
| Apply | Preview the exact before/after difference and apply this occurrence only after an explicit reviewer action and successful safety checks. |
| Reject | Record that the suggestion is wrong for this occurrence; do not change Scripture or turn the rejection into a grammar rule. |
| Ignore | Dismiss this occurrence for the current scan by default. Offer an explicit project-persistent occurrence ignore with a visible scope and an undo/reset control. |
| Modify & Apply | Let the reviewer edit the proposed replacement inside the allowed text span, preview it in context, validate it, and apply explicitly. Record that the replacement was reviewer-authored. |
| Leave open | Keep unresolved or ambiguous findings pending for later review. |

Persisted decisions are keyed by project, reference, rule, source/context fingerprint, and relevant versions. Reconsider decisions after changed context or changed rules; never silently suppress a new occurrence. Do not offer global ignore or automatic Apply All in the first release.

## Plugin window

Use the supported Paratext plugin window/control APIs and standard Windows controls, with Paratext-compatible spacing, terminology, focus behavior, project font, Tamil shaping, selection colors, and reference navigation. Honor host appearance and scaling where the SDK exposes them. Verify the result alongside native Paratext checking windows on the exact target host.

The layout contains:

1. Project and scope selector, Scan / Cancel, progress, and a compact visible plugin version.
2. Verse-linked findings with reference, original text, suggestion, status, and confidence category. Filter by open / applied / rejected / ignored / needs review.
3. A context pane showing the verse, relevant sentence context, highlighted target, and exact proposed difference.
4. A short Tamil explanation beginning `காரணம்:`, rule ID, grammatical evidence, source reference, and applicable project exception.
5. Apply, Reject, Ignore, Modify & Apply, and Previous / Next controls with keyboard access. Disable Apply when no concrete validated replacement exists.

Synchronize verse navigation with Paratext without stealing the caret while the user is typing. Explain why an action is unavailable (read-only project, stale finding, missing permission, lock unavailable, or uncertain analysis). Preserve focus after actions and refresh the changed verse.

## Inline annotations and context menu

Use the supported annotation and annotation-menu APIs to mark the exact finding span and expose the same commands. A short menu may open the context preview before applying. Both surfaces call the same review and patch services and immediately reflect each other's state.

The compatibility spike must prove annotation spans, context menus, accessibility, refresh, and coexistence with native annotations on Paratext 9.5.110.1. If the supported API cannot provide an operation, document the limitation and route it to the plugin window. Inline support is not complete until demonstrated on the host; do not simulate it by intercepting editor keystrokes or modifying Paratext files directly.

## Safe application transaction

1. Bind the command to the reviewed project, reference, finding, exact original span, expected text, context fingerprint, and scan revision. A menu opened earlier must not act on a later selection.
2. Check host permissions and editability. Obtain the supported write lock; never hold it during scanning or while waiting for reviewer input.
3. Re-read the latest chapter under the lock. Validate revision/content, source span, context, selected scope, grapheme boundaries, and absence of overlapping/conflicting edits. If stale, refuse the edit and offer a rescan; do not silently rebase it.
4. Reject replacements that introduce USFM markers, change verse identifiers, cross protected structures, exceed the selected span, or corrupt Unicode. Revalidate reviewer-modified text too.
5. Apply the minimal patch to the latest chapter representation, preserve unrelated content, and write through the supported Paratext API. Scripture writing is chapter-based in the documented API, so verse-level intent needs a chapter-level preservation check.
6. Verify the saved result and record a local transaction outcome. Distinguish pending, applied, failed, and uncertain outcomes; a crash or verification error must not be reported as success. Release locks on every path and refresh affected results.

No background or scan operation writes Scripture. A confirmed Apply / Modify & Apply command is the per-occurrence authorization; avoid redundant confirmation dialogs after the user has reviewed the exact change. If the proposed replacement changes, show the updated preview before committing.

## Audit and recovery

Keep a local journal with genuine host reviewer identity when available, transaction ID, timestamp, project/reference, original and replacement, rule/evidence, source hashes, action, and plugin/catalog/config versions. Do not invent reviewer identity. Do not commit or upload journals or project text to this public repository.

Verify whether Paratext exposes supported undo integration. Otherwise provide an explicit Revert operation that restores only the recorded span after validating the current post-edit text and context under a new write lock. Concurrent changes must block stale reversion. Retain enough state to reconcile interrupted writes on restart without blindly writing or retrying an edit. Explain the verified recovery capability in release notes.

## Offline core and linguistic quality

The host-independent core parses USFM, maps Tamil graphemes to exact source spans, consumes morphology/syntax evidence, evaluates protective rules and opt-in house style, and emits typed findings. Do not infer grammatical meaning solely from a suffix. Unknown or conflicting evidence produces a needs-review result without an actionable replacement.

Keep approved lexicons and rule data local with documented licenses. No cloud inference, runtime downloads, telemetry, or automatic linguistic updates. Scans and review decisions must remain usable with networking disabled. Versioned changes to rule data must be explicit and auditable.

100% accuracy remains an aspiration. Measure precision, recall, false positives, false negatives, and abstention on independent expert-reviewed data; report coverage by construction and rule. Structural fixture validation is not evidence of prediction accuracy.

## Version and build identity

[`VERSION`](../VERSION) starts at `0.1.0-dev`, identifying the development baseline, not a released binary. Future CI builds derive a SemVer informational version such as `0.1.0-dev.42+gabc1234` from the base, CI run number, and commit. Use one version source for assembly/file metadata, plugin metadata, artifact names, About, and diagnostics. The window shows a compact version; About exposes the full build, commit, catalog/config versions, and tested Paratext build. Never label an untested build as compatible.

## Delivery order and release gates

| Phase | Deliverable | Gate |
| --- | --- | --- |
| P0 Foundation | SDK capability spike, data contracts, build/version infrastructure | Minimal versioned plugin loads in 9.5.110.1; required host APIs verified. |
| P1 Read-only preview | Scope scanner, Unicode/USFM mapping, evidence-backed core, native-style window | Correct scope and verse navigation; zero scan mutations; uncertain cases remain open. |
| P2 Safe review | Shared decisions, locked apply, journal/revert, inline integration | Permissions, stale edits, failures, recovery, and both surfaces verified on copied projects. |
| P3 Release candidate | Offline packaging, accessibility/performance, independent linguistic and host QA | Published measured coverage and limitations; exact-host offline tests pass. |

Work starts with P0. Unimplemented tasks remain Todo; dependencies are recorded on the issues. A task reaches Done only with acceptance evidence. No completion dates or performance thresholds are invented: establish representative datasets and measured budgets during implementation.

## API research references

- [Plugin API 2.0 index](https://github.com/ubsicap/paratext_demo_plugins/wiki): host windows, annotations, selection, and synchronization research entry points.
- [Writing Scripture](https://github.com/ubsicap/paratext_demo_plugins/wiki/Writing-Scripture): chapter-level writes, host permissions, and mandatory write locks.
- [Demo plugins](https://github.com/ubsicap/paratext_demo_plugins): verify actual supported signatures and behavior against the installed SDK.

The documentation establishes design constraints, not verified compatibility. The P0 spike must resolve remaining host capabilities before implementation depends on them.
