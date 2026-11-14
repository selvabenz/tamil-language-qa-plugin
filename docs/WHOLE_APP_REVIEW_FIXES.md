# Whole-app review fixes

The seven findings from the second whole-app review are tracked separately from the original delivery gates. Build 6 is the corrected candidate; installation and exact-host results will be recorded in the developer handoff after verification.

| Issue | Fix | Regression evidence |
| --- | --- | --- |
| [#19](https://github.com/selvabenz/tamil-language-qa-plugin/issues/19) | Protect f/fe/x containers even when their close is missing. | Paragraph, verse and chapter markers cannot re-enable checking or patching inside an unclosed note. |
| [#20](https://github.com/selvabenz/tamil-language-qa-plugin/issues/20) | Validate journal identity, explicit schema, reference, hashes, timestamp and action/outcome. | Incomplete pending records block a new write and release the lock. Unknown schema and truncated JSON remain blocked. |
| [#21](https://github.com/selvabenz/tamil-language-qa-plugin/issues/21) | Clear empty/null host selections and retain the captured passage during scan preparation. | The production selection handler clears the old passage; raw selection mapping still refuses ambiguity. |
| [#22](https://github.com/selvabenz/tamil-language-qa-plugin/issues/22) | Atomically save shared house style, notify all matching windows and validate fresh settings under the chapter lock. | Another settings-store update and a change during lock acquisition prevent writes. The production window invalidates findings on notification. |
| [#23](https://github.com/selvabenz/tamil-language-qa-plugin/issues/23) | Mark linked findings Reverted, clear applied text, refresh the window/annotations and link history transactions. | Exact restoration, non-accepted export and reverted original-history outcome. |
| [#24](https://github.com/selvabenz/tamil-language-qa-plugin/issues/24) | Continue readable chapters after a preparation failure; refuse incomplete exact passage mapping. | The production async review control completes two readable chapters around one failed chapter and reports one finding and one failure. |
| [#25](https://github.com/selvabenz/tamil-language-qa-plugin/issues/25) | Verify build-time plugin/core/catalog hashes and the complete package checksum list before installation. | Replaced binaries, tampered manifests and incomplete checksum lists are refused. Core-only builds cannot be packaged or installed. |

Local checks pass 403 core assertions, native-control regression tests, artifact/source-integrity tests and the unchanged reference/schema validators. The WinForms regression runner explicitly supplies a UI synchronization context and waits for asynchronous completion before disposal.

Journal records with missing fields now fail closed even if the JSON parses. Back up the local journal before investigating any corruption; never delete it merely to bypass a blocked write. Older packages without the new build-time artifact hashes are refused by the updated installer; rebuild from the intended source or use their original deployment procedure when investigating a rollback.

Shared settings operations are serialized across review windows in the host process. Direct external editing of the configuration file during a write is unsupported; the plugin rereads configuration under its chapter lock and invalidates plugin-managed changes across windows.

These fixes do not complete independent linguistic evaluation, all inline/accessibility/offline-host checks, custom-versification acceptance or startup/recovery stress testing. Original delivery issue #15 remains the release gate.
