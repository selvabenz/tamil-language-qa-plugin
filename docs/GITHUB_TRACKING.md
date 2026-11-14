# GitHub delivery tracking

[Delivery epic](https://github.com/selvabenz/tamil-language-qa-plugin/issues/1) | [Project board](https://github.com/users/selvabenz/projects/4)

The board is linked to this repository. All 14 implementation tasks are In Progress, with source, test evidence and remaining gates recorded in issue comments. The epic remains open until their acceptance gates pass. Priority, phase and planned release fields organize delivery; area/type/priority/phase labels provide repository filtering. Dependencies are recorded as issue relationships and in each task body.

| Phase | Task | Issue |
| --- | --- | --- |
| P0 | SDK and native integration | [#2](https://github.com/selvabenz/tamil-language-qa-plugin/issues/2) |
| P0 | Shared contracts | [#7](https://github.com/selvabenz/tamil-language-qa-plugin/issues/7) |
| P0 | Build identity and packaging | [#14](https://github.com/selvabenz/tamil-language-qa-plugin/issues/14) |
| P1 | USFM and Tamil source spans | [#4](https://github.com/selvabenz/tamil-language-qa-plugin/issues/4) |
| P1 | Scope and verse scanning | [#3](https://github.com/selvabenz/tamil-language-qa-plugin/issues/3) |
| P1 | Offline linguistic analysis | [#5](https://github.com/selvabenz/tamil-language-qa-plugin/issues/5) |
| P1 | Project house style | [#6](https://github.com/selvabenz/tamil-language-qa-plugin/issues/6) |
| P1 | Native-style review window | [#8](https://github.com/selvabenz/tamil-language-qa-plugin/issues/8) |
| P2 | Review decisions | [#10](https://github.com/selvabenz/tamil-language-qa-plugin/issues/10) |
| P2 | Safe locked application | [#11](https://github.com/selvabenz/tamil-language-qa-plugin/issues/11) |
| P2 | Audit, revert and recovery | [#12](https://github.com/selvabenz/tamil-language-qa-plugin/issues/12) |
| P2 | Inline annotations and menus | [#9](https://github.com/selvabenz/tamil-language-qa-plugin/issues/9) |
| P3 | Offline resources and responsiveness | [#13](https://github.com/selvabenz/tamil-language-qa-plugin/issues/13) |
| P3 | Release validation | [#15](https://github.com/selvabenz/tamil-language-qa-plugin/issues/15) |

The [development prerelease](https://github.com/selvabenz/tamil-language-qa-plugin/releases/tag/v0.1.0-dev.6) provides the last installed host-tested build. See [implementation status](IMPLEMENTATION_STATUS.md) for its 403 current-source passing assertions, partial linguistic coverage and pending exact-host acceptance. A prerelease is not production completion.

## Review fixes

- [#16](https://github.com/selvabenz/tamil-language-qa-plugin/issues/16): resume checking after closed protected notes.
- [#17](https://github.com/selvabenz/tamil-language-qa-plugin/issues/17): wrapped boundaries and exact whitespace-preserving Apply/Revert.
- [#18](https://github.com/selvabenz/tamil-language-qa-plugin/issues/18): clean source and catalog provenance before packaging.

These bounded defects have regression coverage; broader host acceptance remains in #15. See [developer handoff](DEVELOPER_HANDOFF.md).

Whole-app review issues [#19–#25](https://github.com/selvabenz/tamil-language-qa-plugin/issues?q=is%3Aissue+label%3Abug) have individual labels, build 6 board targets and completion evidence. Their fixes do not close the broader acceptance gates in original issues #1–#15. See [review fix matrix](WHOLE_APP_REVIEW_FIXES.md).
