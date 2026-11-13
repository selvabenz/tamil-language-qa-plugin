# Install the development preview

Target: Windows with Paratext **9.5.110.1** and .NET Framework 4.8. The plugin is a development preview with limited linguistic coverage. It does not establish 100% accuracy. Review each suggestion in context and first test on a disposable project.

## From a release ZIP

1. Close Paratext normally.
2. Extract the ZIP to a new folder. Verify its files against `SHA256SUMS.txt`.
3. Create `C:\Program Files\Paratext 9\plugins\TamilSandhiQA` (administrator access may be required).
4. Copy `TamilSandhi.ptxplg`, `TamilSandhi.Core.dll`, `rules.json`, and `build.json` into that folder. Do not copy or replace any Paratext interface assemblies.
5. Start Paratext. In an open Scripture project's menu, choose **Tools > Checks > Tamil Sandhi QA**. Menu grouping may depend on host menu settings; search for Tamil Sandhi if necessary.

## Build and install from source

No NuGet packages or network services are required to compile the Windows plugin against an installed host.

```powershell
powershell -NoProfile -File scripts/build.ps1 -BuildNumber 1
powershell -NoProfile -File scripts/install.ps1
```

`build.ps1 -CoreOnly` compiles and runs the host-independent regression and transaction tests without a Paratext installation. The full build references the installed host's assemblies; these proprietary assemblies are never distributed in the ZIP or repository.

## Use

Choose the scope, then Scan. Findings display verse context, proposed replacement, evidence, Tamil explanation, and state. Review the before/after preview before choosing Apply or Modify & Apply. Reject and Ignore never write Scripture. Revert last edit is guarded: it refuses if the chapter has changed afterward.

To show inline highlights, enable **Tamil sandhi findings** in the project's annotation menu after opening and scanning in the checker window. Click a highlight for contextual review commands. The Scripture context-menu entry can open the checker for an exact selected passage.

Project decisions, opt-in house style, and the durable transaction journal stay under `%LOCALAPPDATA%\TamilSandhiQA\<hashed-project-ID>`. They contain local project text and must not be published. Reset saved decisions clears only stored occurrence decisions. House style uses exact no-insertion lexemes and is disabled by default.

The current source also supports exact raw-USFM selected passages across verses/chapters, verse-unit counts and progress, local review history, and schema-validated findings export. Selection mapping fails closed when the host's selected raw text cannot be matched exactly. Exported numerical confidence remains uncalibrated (zero); read the evidence and explanation rather than treating it as a correctness probability.

If a write is interrupted, the next edit reconciles its recorded before/after chapter hashes. An unrecognized outcome blocks further writes for that chapter; inspect the local journal and current text before recovery. Do not delete the journal to bypass this check.

## Remove or update

Close Paratext normally before changing plugin files. To remove the plugin, remove only the `TamilSandhiQA` plugin folder; retain local journals when recovery may be needed. To update, replace the four plugin files together. The source installer retains `.previous` copies of replaced files.
