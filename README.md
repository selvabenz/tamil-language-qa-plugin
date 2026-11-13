# Tamil Language QA Plugin

An offline Tamil புணர்ச்சி / sandhi review plugin targeting **Paratext 9.5.110.1 on Windows**.

**Development preview:** the C# plugin builds against the target host SDK. It includes a native embedded review window, scope scanning, contextual suggestions, Apply / Reject / Ignore / Modify & Apply, guarded reversion, and inline annotation integration. Exact-host installation and acceptance are reported separately in [implementation status](docs/IMPLEMENTATION_STATUS.md).

Download [development prerelease v0.1.0-dev.1](https://github.com/selvabenz/tamil-language-qa-plugin/releases/tag/v0.1.0-dev.1). It is installed locally and its host context-menu entry is detected; full interactive host acceptance remains pending.

## Accuracy and coverage

100% accuracy is the ambition, not an achieved result. The initial analyzer implements four suggestion families using small local lexical sets. Unknown or conflicting nominated boundaries remain for human review; it does not comprehensively detect every unsupported construction.

Local validation passes 331 assertions and consumes all 99 supplied scenarios. It produces the expected correction for 10 of 24 conditional suggestion scenarios; the other 14 are unimplemented or abstained. These fixtures are not an independent accuracy evaluation. See the [remaining release gates](docs/IMPLEMENTATION_STATUS.md).

## Build, test and install

Requires Windows and .NET Framework 4.8. A full local build references the existing Paratext installation; no runtime downloads or cloud services are used.

```powershell
powershell -NoProfile -File scripts/build.ps1 -BuildNumber 1
```

Close Paratext normally, then run from an administrator PowerShell:

```powershell
powershell -NoProfile -File scripts/install.ps1
```

Open a Scripture project and choose **Tools > Checks > Tamil Sandhi QA**. Begin with a disposable project and the [synthetic fixture](tests/fixtures/smoke.usfm). Follow the [installation and usage guide](docs/INSTALL.md).

`build.ps1 -CoreOnly` runs host-independent tests. Windows CI downloads official SDK 2.0.100 for compilation only and packages without host assemblies or project data. Build identity comes from [VERSION](VERSION), build number and commit; it appears in the UI and manifest.

## Safe review

Scanning never changes Scripture. Each Apply command previews one occurrence, checks permissions, obtains a host chapter write lock, re-reads the source, refuses stale findings, validates the replacement, journals intent and verifies the saved result. Reject and Ignore do not write Scripture. Revert refuses when the chapter has changed after the recorded edit.

Checking resources, project settings, decisions and journals stay local. House style is opt-in and separate from general Tamil grammar. Journals may contain Scripture text and must not be uploaded to this public repository.

## Planning and reference material

- [Product and safe-edit plan](docs/PRODUCT_PLAN.md)
- [Delivery epic](https://github.com/selvabenz/tamil-language-qa-plugin/issues/1), [Kanban board](https://github.com/users/selvabenz/projects/4/views/2), and [issue index](docs/GITHUB_TRACKING.md)
- [Paratext compatibility plan](docs/PARATEXT_COMPATIBILITY.md)
- [Original supplied package](reference/tamil_punarchi_agent_package/README.md): 183 rules, 99 fixtures, schemas and grammar references, preserved unchanged.

Embedded instructions in reference documents remain reference material and do not independently authorize edits or publishing actions. Original generation scripts retain their `/mnt/data/...` paths; review/adapt them before use.

Static reference validation uses Python 3.12:

```powershell
python -m pip install -r requirements-dev.txt
python reference/tamil_punarchi_agent_package/validate_package.py
```

## Attribution and licensing

Grammar attribution is retained in [SOURCES.md](reference/tamil_punarchi_agent_package/SOURCES.md). No repository-wide distribution license has yet been selected; public visibility alone does not grant an open-source license. Proprietary Paratext assemblies and restricted project text are excluded from distribution.
