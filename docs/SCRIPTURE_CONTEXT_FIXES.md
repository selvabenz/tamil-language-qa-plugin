# Scripture-context reliability changes

The `scripture-modern-1` profile fixes the five bounded findings from the [sandhi review](SANDHI_RULES_REVIEW.md). It is a conservative modern written Tamil profile, not a complete sentence parser or a certification of Scripture accuracy.

| Finding | Issue | Change | Verification |
|---|---|---|---|
| SR-01 | [#26](https://github.com/selvabenz/tamil-language-qa-plugin/issues/26) | Ambiguous `அவரை` stays review-only; external tags cannot assert away the homograph. Reasons state the lexical/semantic limitation. | Separate pronoun/crop contexts, no actionable patch, including asserted annotations. |
| SR-02 | [#27](https://github.com/selvabenz/tamil-language-qa-plugin/issues/27) | Insert missing, replace wrong, and remove unnecessary ஒற்று only in bounded supported constructions. | Wrong demonstrative/dative consonants; positive/negative relative participles, adjectives, soft/medial participles; exact locked Apply/Revert; CRLF/tab preservation. |
| SR-03 | [#28](https://github.com/selvabenz/tamil-language-qa-plugin/issues/28) | 11 bounded addition families and 5 non-addition checks; counts for supported, unresolved, unassessed and suppressed word pairs. | All 22 unambiguous correction-needed B fixtures required; 43 Scripture-style context cases; native-control coverage disclosure. |
| SR-04 | [#29](https://github.com/selvabenz/tamil-language-qa-plugin/issues/29) | Maintained expectation overrides separate ambiguous B004 and already-correct B010. Original reference import unchanged. | Every supported B prediction is mandatory; no silently accepted misses in that set. |
| SR-05 | [#30](https://github.com/selvabenz/tamil-language-qa-plugin/issues/30) | Modern register/decision/tier gates; explicit bounded promotions; names policy kept editorial; profile identity included in findings. | Classical/tier/decision changes cannot activate patches; deity-name inflection and editorial suppression; all finding exports schema-validated. |

## Grammar and policy provenance

Addition families follow the [Tamil Virtual Academy modern addition lesson](https://www.tamilvu.org/ta/courses-degree-c021-c0214-html-c0214661-16317). Non-addition checks follow its [modern non-addition lesson](https://www.tamilvu.org/ta/courses-degree-c021-c0214-html-c0214662-16318). See [capability matrix](RULE_CAPABILITIES.md) for every imported entry.

The reference marks locative adverbs, இ/ய்/hard-u/ஆக participles and clipped negatives as `review` tier. The runtime explicitly promotes these six IDs **only inside the enumerated lexical constructions** documented in `ScriptureRuleProfile.cs`; this is an implementation decision supported by the above instructional examples, not a change to the imported catalog. Other review/classical entries remain inactive.

`NO-NAMES` is an editorial protection concept, not a universal no-sandhi rule. `கர்த்தர் சொன்னார்` is left unchanged; the known dative `கர்த்தருக்கு சொன்னார்` can suggest `கர்த்தருக்குச் சொன்னார்`. An opt-in `NoInsertBefore` override suppresses the entire matching boundary, including replacement/removal, and is counted separately. It remains a coarse exact-word project policy, not automatic named-entity recognition.

Unknown morphology, homographs, joined compounds, special lexical derivations, and broader register differences remain unverified. The engine does not infer case from an arbitrary ஐ/க்கு suffix. It nominates some unresolved boundaries for review and explicitly counts other pairs as unassessed. Protected USFM, punctuation, poetry markers and verse boundaries remain barriers. Review decisions from the previous profile do not silently carry into this profile because finding identity includes the profile version.

## Evidence and real-resource evaluation

The development suite passes 661 core assertions, the SDK/native-control suite, original reference validators, exported finding schemas, and build/package-integrity regressions. The 24 imported B rows comprise 22 expected corrections, one correction-needed but ambiguous input, and one already-correct input. These are development cases, not held-out accuracy evidence.

The user selected **Tamil BSI OV Edited** and **OTCV** in Paratext as reviewed usage references. `tests-host/ReadOnlyResourceBenchmark.cs` uses only the public SDK, targets those identities, reads representative chapters across narrative/poetry/prophecy/Gospel/epistle books, and verifies unchanged readback hashes. It has no Scripture-write API calls. Host resource-access restrictions must be respected; no resource token is fabricated or retrieved from another plugin.

Run `scripts/build-resource-benchmark.ps1` after building the product. Install its helper only for the authorized local run and remove it with Paratext closed afterwards. The helper and all resource findings stay in ignored `artifacts/resource-benchmark-*`. Do not publish resource text, private snippets, resource files, local inventories or journals. Only a reviewed aggregate report may be copied to public docs.

Baseline proposed changes to a reviewed resource are disagreements requiring adjudication, not automatically errors in the resource. Controlled missing-ஒற்று recovery is a coverage measurement, not independent natural-error recall. No 100% accuracy claim or production release gate is satisfied by this suite. Broader evaluation remains in issues #5 and #15.
