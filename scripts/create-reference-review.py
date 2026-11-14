"""Create a private, offline adjudication view; never modifies Scripture files."""
import argparse
import json
from pathlib import Path

parser = argparse.ArgumentParser()
parser.add_argument('findings', nargs='+', type=Path)
parser.add_argument('--output', required=True, type=Path)
args = parser.parse_args()
rows = []
for source in args.findings:
    rows.extend(row for row in json.loads(source.read_text(encoding='utf-8-sig')) if row.get('suggested'))
payload = json.dumps(rows, ensure_ascii=True).replace('<', '\\u003c').replace('>', '\\u003e').replace('&', '\\u0026')
page = r'''<!doctype html><html lang="en"><meta charset="utf-8"><title>Private BSI OV sandhi review</title>
<style>body{font:16px system-ui;margin:24px;max-width:1100px;background:#f6f6f4;color:#222}header{position:sticky;top:0;background:#f6f6f4;padding:12px 0}input,select,button{font:inherit;padding:8px}article{background:white;border:1px solid #ddd;border-radius:6px;padding:18px;margin:14px 0}pre{white-space:pre-wrap;font:18px/1.7 system-ui}small{color:#555}</style>
<header><h1>Private BSI OV sandhi review</h1><p>Unadjudicated proposals, not confirmed errors. Review verse meaning, edition style and grammar. This page makes no Scripture changes and uses no network services.</p><input id="search" placeholder="Filter reference, rule or text"><button id="export">Save review decisions</button><p id="count"></p></header><main id="rows"></main>
<script>const data=PAYLOAD;
const storageKey='private-bsi-sandhi-review-v1';let decisions={};try{decisions=JSON.parse(localStorage.getItem(storageKey)||'{}')}catch(e){}
function key(r){return JSON.stringify([r.book,r.chapter,r.verse,r.rule,r.original,r.suggested])}
function render(){const root=document.getElementById('rows');root.replaceChildren();const q=document.getElementById('search').value.toLowerCase();let count=0;for(const r of data){if(!JSON.stringify(r).toLowerCase().includes(q))continue;count++;const a=document.createElement('article');const h=document.createElement('h3');h.textContent='Book '+r.book+' '+r.chapter+':'+r.verse+' | '+r.rule+' | '+r.operation;a.append(h);const p=document.createElement('pre');p.textContent=r.original+'\n→ '+r.suggested;a.append(p);const d=document.createElement('details');const s=document.createElement('summary');s.textContent='Verse context';const c=document.createElement('pre');c.textContent=r.context;d.append(s,c);a.append(d);const select=document.createElement('select');for(const [v,t] of [['open','Needs adjudication'],['correction','Approve modern correction'],['edition_variant','Keep reference: edition/style variant'],['false_positive','Reject: incorrect suggestion'],['uncertain','Uncertain: linguistic review needed']]){const o=document.createElement('option');o.value=v;o.textContent=t;select.append(o)}select.value=decisions[key(r)]||'open';select.onchange=()=>{decisions[key(r)]=select.value;try{localStorage.setItem(storageKey,JSON.stringify(decisions))}catch(e){}};a.append(select);root.append(a)}document.getElementById('count').textContent=count+' shown / '+data.length+' proposals. Decisions remain local until you save a file.'}
document.getElementById('search').oninput=render;document.getElementById('export').onclick=()=>{const blob=new Blob([JSON.stringify({independentGold:false,proposals:data.map(r=>({book:r.book,chapter:r.chapter,verse:r.verse,rule:r.rule,original:r.original,suggested:r.suggested,decision:decisions[key(r)]||'open'}))},null,2)],{type:'application/json'});const a=document.createElement('a');a.href=URL.createObjectURL(blob);a.download='private-bsi-sandhi-adjudication.json';a.click();setTimeout(()=>URL.revokeObjectURL(a.href),1000)};render();</script></html>'''
args.output.parent.mkdir(parents=True, exist_ok=True)
args.output.write_text(page.replace('PAYLOAD', payload), encoding='utf-8')
print(f'Private review created: {len(rows)} proposals; no reference files changed.')
