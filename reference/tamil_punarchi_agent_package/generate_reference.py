import json,pathlib,collections
p=pathlib.Path('/mnt/data/tamil_punarchi_agent_package')
d=json.loads((p/'rules.json').read_text(encoding='utf8'))
categories={
'classification':'01. Fundamentals / புணர்ச்சி வகைகள்',
'vowel':'02. Vowel-endings / உயிரீற்றுப் புணர்ச்சி',
'short_u':'03. குற்றியலுகரம், முற்றியலுகரம் and related rules',
'consonant':'04. General consonant endings / மெய்யீறு',
'consonant_m':'05. மகர ஈற்றுப் புணர்ச்சி',
'consonant_l':'06. லகர / ளகர ஈறு',
'consonant_n':'07. னகர / ணகர ஈறு',
'consonant_yrz':'08. யகர / ரகர / ழகர ஈறு',
'consonant_v':'09. வகர ஈறு',
'case':'10. Case relations / வேற்றுமை',
'vallinam_add':'11. வல்லினம் மிகும் இடங்கள்',
'vallinam_no':'12. வல்லினம் மிகா இடங்கள்',
'lexical':'13. Special lexical compounds / சிறப்புப் புணர்ச்சிகள்',
'sariyai':'14. சாரியைகள் and special declensions',
'safety':'15. Engineering safeguards and project-specific policy',
}
by=collections.defaultdict(list)
for r in d['rules']:by[r['category']].append(r)
out=['# Complete bilingual rule reference / தமிழ்ப் புணர்ச்சி சரிபார்ப்பு விதிகள்','',
  f'Contains **{d["rule_count"]} catalog entries**. Each entry has an ID, grammatical condition, expected decision, sample correct forms where available, evidence sources and implementation tier. This is a conservative rule inventory, *not* a statement that each entry can be reduced to a regex.','',
  '**Key:** `candidate` = suggest when parser proves the conditions; `review` = morphological/lexicon/human review; `block` = protection against incorrect insertion. None permit automatic authoritative-text edits.','']
for cat,label in categories.items():
 out.extend([f'## {label}',''])
 for r in by[cat]:
  out.append(f'### `{r["id"]}` — {r["name_ta"]} / {r["name_en"]}')
  out.append(f'**Decision:** `{r["decision"]}` · **Layer:** `{r["layer"]}` · **Tier:** `{r["tier"]}`')
  out.append('')
  out.append('**Condition:** '+r['preconditions'])
  if r['requires_annotations']:out.append('\n**Required analysis:** '+', '.join('`'+x+'`' for x in r['requires_annotations']))
  if r['examples_correct']:
   out.append('\n**Correct examples:** '+'; '.join(r['examples_correct']))
  if r['examples_incorrect']:
   out.append('\n**Examples needing review/correction:** '+'; '.join(r['examples_incorrect']))
  if r['acceptable_alternatives']:
   out.append('\n**Acceptable variants:** '+'; '.join(r['acceptable_alternatives']))
  if r['notes']:out.append('\n**Note:** '+r['notes'])
  out.append('\n**Sources:** '+', '.join(f'`{x}`' for x in r['source_ids']))
  out.append('')
(p/'RULE_REFERENCE.md').write_text('\n'.join(out)+'\n',encoding='utf8')
print('reference bytes:',(p/'RULE_REFERENCE.md').stat().st_size)
