using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Paratext.PluginInterfaces;

namespace TamilSandhi.Plugin
{
    public sealed class ReviewControl : EmbeddedPluginControl
    {
        private IPluginChildWindow parent;
        private IWindowPluginHost host;
        private IProject project;
        private IVerseRef reference;
        private readonly IScriptureTextSelection initialSelection;
        private readonly bool selectionRequested;
        private SandhiEngine engine;
        private HouseStyle style=new HouseStyle();
        private IJournal journal;
        private readonly List<Finding> findings=new List<Finding>();
        private readonly ConcurrentDictionary<string,string> sourceCache=new ConcurrentDictionary<string,string>();
        private volatile Finding[] publishedFindings=new Finding[0];
        private readonly Dictionary<string,ReviewState> decisions=new Dictionary<string,ReviewState>();
        private readonly ComboBox scope=new ComboBox {DropDownStyle=ComboBoxStyle.DropDownList,Width=160,AccessibleName="Scripture scope"};
        private readonly TextBox books=new TextBox {Width=100,AccessibleName="Selected book codes",Text="GEN"};
        private readonly ComboBox filter=new ComboBox {DropDownStyle=ComboBoxStyle.DropDownList,Width=130,AccessibleName="Finding status filter"};
        private readonly ListView list=new ListView {View=View.Details,FullRowSelect=true,HideSelection=false,Dock=DockStyle.Fill,MultiSelect=false,AccessibleName="Sandhi findings"};
        private readonly RichTextBox context=new RichTextBox {ReadOnly=true,Dock=DockStyle.Fill,AccessibleName="Verse context and explanation",DetectUrls=false};
        private readonly Label status=new Label {AutoSize=true,Text="Ready. Limited linguistic coverage; review every suggestion."};
        private readonly Button scan=new Button {Text="&Scan",AutoSize=true};
        private readonly Button cancel=new Button {Text="&Cancel",AutoSize=true,Enabled=false};
        private readonly Button apply=new Button {Text="&Apply...",AutoSize=true,Enabled=false};
        private readonly Button modify=new Button {Text="&Modify && Apply...",AutoSize=true,Enabled=false};
        private readonly Button revert=new Button {Text="&Revert last edit...",AutoSize=true,Enabled=false};
        private bool scanning,cancelRequested,disposed,applying;
        private string projectFolder;
        private AuditRecord lastApplied;
        public static readonly ConcurrentDictionary<string,ReviewControl> Sessions=new ConcurrentDictionary<string,ReviewControl>();
        internal static event EventHandler ResultsChanged;
        private static void Changed(){var handler=ResultsChanged;if(handler!=null)handler(null,EventArgs.Empty);}
        public ReviewControl() : this(null,false) { }
        public ReviewControl(IParatextChildState initial,bool selection)
        {
            initialSelection=initial==null||initial.Selections == null ? null : initial.Selections.OfType<IScriptureTextSelection>().FirstOrDefault();
            selectionRequested=selection;reference=initial==null?null:initial.VerseRef;
            Dock=DockStyle.Fill;AutoScaleMode=AutoScaleMode.Dpi;Font=SystemFonts.MessageBoxFont;
            scope.Items.AddRange(new object[]{"Current verse","Selected passage","Current chapter","Current book","Selected books","Whole project"});scope.SelectedIndex=selection?1:0;
            filter.Items.AddRange(new object[]{"All","Open","NeedsReview","Applied","Rejected","Ignored","Stale"});filter.SelectedIndex=0;
            list.Columns.Add("Reference",100);list.Columns.Add("Original",200);list.Columns.Add("Suggestion",200);list.Columns.Add("Status",100);list.Columns.Add("Rule",140);
            var toolbar=new FlowLayoutPanel {Dock=DockStyle.Top,AutoSize=true,WrapContents=true,Padding=new Padding(6)};
            toolbar.Controls.AddRange(new Control[]{new Label{Text="Scope:",AutoSize=true,Padding=new Padding(0,7,0,0)},scope,books,scan,cancel,filter,new Button{Text="About "+BuildInfo.Version.Split('+')[0],AutoSize=true}});
            ((Button)toolbar.Controls[6]).Click+=delegate {MessageBox.Show(this,"Tamil Sandhi QA\n"+BuildInfo.Version+"\nTarget: Paratext 9.5.110.1\nCatalog: 2026-11-13\nConfig: "+style.Hash+"\nLocal data: "+projectFolder+"\nDevelopment preview. Four supported suggestion families; remaining analyses need review.","About Tamil Sandhi QA",MessageBoxButtons.OK,MessageBoxIcon.Information);};
            var actions=new FlowLayoutPanel {Dock=DockStyle.Bottom,AutoSize=true,Padding=new Padding(6)};
            var reject=Button("&Reject",delegate{Decide(ReviewState.Rejected,false);});
            var ignore=Button("&Ignore",delegate{Decide(ReviewState.Ignored,false);});
            var persist=Button("Ignore occurrence (project)",delegate{Decide(ReviewState.Ignored,true);});
            var reopen=Button("&Leave open",delegate{Decide(ReviewState.Open,true);});
            var reset=Button("Reset saved decisions",delegate{decisions.Clear();SaveDecisions();foreach(var f in findings.Where(x=>x.State==ReviewState.Rejected||x.State==ReviewState.Ignored))f.State=f.Suggested==null?ReviewState.NeedsReview:ReviewState.Open;RefreshList();Changed();});
            actions.Controls.AddRange(new Control[]{apply,modify,reject,ignore,persist,reopen,Button("Previous",delegate{NavigateFinding(-1);}),Button("Next",delegate{NavigateFinding(1);}),revert,reset,Button("House style...",EditHouseStyle)});
            var split=new SplitContainer {Dock=DockStyle.Fill,Orientation=Orientation.Horizontal,SplitterDistance=200};split.Panel1.Controls.Add(list);split.Panel2.Controls.Add(context);
            var footer=new FlowLayoutPanel {Dock=DockStyle.Bottom,AutoSize=true,Padding=new Padding(6)};footer.Controls.Add(status);
            Controls.Add(split);Controls.Add(footer);Controls.Add(actions);Controls.Add(toolbar);
            scan.Click+=delegate{StartScan();};cancel.Click+=delegate{cancelRequested=true;};filter.SelectedIndexChanged+=delegate{RefreshList();};list.SelectedIndexChanged+=delegate{ShowFinding();};
            apply.Click+=delegate{Preview(false);};modify.Click+=delegate{Preview(true);};revert.Click+=Revert;
            list.ContextMenuStrip=MenuForSelection();
            scope.SelectedIndexChanged+=delegate{books.Enabled=scope.SelectedIndex==4;};books.Enabled=scope.SelectedIndex==4;
        }
        private static Button Button(string text,EventHandler click){var b=new Button{Text=text,AutoSize=true};b.Click+=click;return b;}
        public override void OnAddedToParent(IPluginChildWindow window,IWindowPluginHost pluginHost,string state)
        {
            parent=window;host=pluginHost;parent.SetTitle("Tamil Sandhi QA "+BuildInfo.Version.Split('+')[0]);
            if(reference==null)reference=parent.CurrentState.VerseRef;
            parent.AllowUserToChangeProject=true;
            parent.ProjectChanged+=ProjectChanged;parent.VerseRefChanged+=ReferenceChanged;
            SetProject(parent.CurrentState.Project);
        }
        public override string GetState(){return null;}
        public override void DoLoad(IProgressInfo progress){BeginInvoke((Action)delegate{if(selectionRequested)StartScan();});}
        private void ProjectChanged(IPluginChildWindow sender,IProject next){reference=sender.CurrentState.VerseRef;SetProject(next);}
        private void ReferenceChanged(IPluginChildWindow sender,IVerseRef oldRef,IVerseRef newRef){reference=newRef;}
        private void SetProject(IProject next)
        {
            cancelRequested=true;
            if(project!=null){project.ScriptureDataChanged-=ScriptureChanged;ReviewControl removed;Sessions.TryRemove(project.ID,out removed);}
            project=next;engine=null;findings.Clear();sourceCache.Clear();decisions.Clear();lastApplied=null;revert.Enabled=false;
            if(project==null)return;
            Sessions[project.ID]=this;project.ScriptureDataChanged+=ScriptureChanged;
            projectFolder=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"TamilSandhiQA",TextIdentity.Hash(project.ID));Directory.CreateDirectory(projectFolder);
            journal=new FileJournal(Path.Combine(projectFolder,"transactions.jsonl"));
            try
            {
                string catalogFile=Path.Combine(Path.GetDirectoryName(typeof(TamilSandhiPlugin).Assembly.Location),"rules.json");
                engine=new SandhiEngine(Json.Read<Catalog>(File.ReadAllText(catalogFile,Encoding.UTF8)));
                string config=Path.Combine(projectFolder,"house-style.json");style=File.Exists(config)?Json.Read<HouseStyle>(File.ReadAllText(config,Encoding.UTF8)):new HouseStyle();
                if(style==null||style.NoInsertBefore==null||String.IsNullOrWhiteSpace(style.Version))throw new InvalidDataException("Invalid house style configuration.");
                string saved=Path.Combine(projectFolder,"decisions.json");if(File.Exists(saved))foreach(var kv in Json.Read<Dictionary<string,ReviewState>>(File.ReadAllText(saved,Encoding.UTF8)))decisions[kv.Key]=kv.Value;
                var history=journal.Read().GroupBy(r=>r.FindingId).Select(g=>g.Last()).Where(r=>r.State=="applied").OrderByDescending(r=>r.Timestamp).FirstOrDefault();lastApplied=history;revert.Enabled=history!=null;
                var font=project.Language.Font;context.Font=new Font(font.FontFamily,Math.Max(font.Size,12));list.Font=context.Font;
                status.Text="Project: "+project.ShortName+" | Ready. Suggestions cover a limited lexical subset.";
                books.Text=reference==null?"GEN":reference.BookCode;
                scan.Enabled=true;
            }
            catch(Exception ex){scan.Enabled=false;Error(ex);}
            RefreshList();Changed();
        }
        private void ScriptureChanged(IProject p,int book,int chapter)
        {
            if(disposed)return;
            if(InvokeRequired){BeginInvoke((Action)delegate{ScriptureChanged(p,book,chapter);});return;}
            foreach(var f in findings.Where(f=>f.Project==p.ID&&(book<=0||f.Book==book)&&(chapter<=0||f.Chapter==chapter)&&f.State!=ReviewState.Applied))f.State=ReviewState.Stale;
            RefreshList();Changed();
        }
        private sealed class Work
        {
            public int Book,Chapter;public List<TextSpan> Scope;public ChapterSnapshot Snapshot;
        }
        private List<Work> CreateWork(IProject selected,IVerseRef at,int selectedScope)
        {
            if(at==null)throw new InvalidOperationException("Choose a Scripture reference first.");
            var work=new List<Work>();
            var chosen=selected.AvailableBooks.Where(b=>b.InProjectScope).OrderBy(b=>b.Number).ToList();
            if(selectedScope<=3)chosen=chosen.Where(b=>b.Number==at.BookNum).ToList();
            if(selectedScope==4)
            {
                var codes=books.Text.ToUpperInvariant().Split(new[]{',',' ',';'},StringSplitOptions.RemoveEmptyEntries);
                if(codes.Length==0||codes.Any(c=>!chosen.Any(b=>b.Code==c)))throw new InvalidOperationException("Enter valid selected book codes separated by commas.");
                chosen=chosen.Where(b=>codes.Contains(b.Code)).ToList();
            }
            if(selectedScope==1)
            {
                if(initialSelection==null || String.IsNullOrEmpty(initialSelection.SelectedText))throw new InvalidOperationException("Open the checker from a selected Scripture passage using the context menu.");
                // Multi-verse partial selections require exact endpoints; this preview refuses rather than expands them.
                if(!initialSelection.VerseRefStart.Equals(initialSelection.VerseRefEnd))throw new InvalidOperationException("This preview supports exact single-verse text selections. Use chapter/book scope for larger passages.");
                chosen=selected.AvailableBooks.Where(b=>b.Number==initialSelection.VerseRefStart.BookNum).ToList();
            }
            foreach(var book in chosen)
            {
                int first=1,last=selected.Versification.GetLastChapter(book.Number);
                if(selectedScope<=2){first=last=selectedScope==1?initialSelection.VerseRefStart.ChapterNum:at.ChapterNum;}
                for(int chapter=first;chapter<=last;chapter++)work.Add(new Work{Book=book.Number,Chapter=chapter});
            }
            if(work.Count==0)throw new InvalidOperationException("No Scripture books in this scope.");
            return work;
        }
        private async void StartScan()
        {
            if(scanning||project==null||engine==null)return;
            scanning=true;cancelRequested=false;scan.Enabled=false;cancel.Enabled=true;apply.Enabled=modify.Enabled=false;findings.Clear();sourceCache.Clear();
            IProject selected=project;var at=reference;int mode=scope.SelectedIndex;var selectedEngine=engine;var selectedStyle=style;
            try
            {
                var work=CreateWork(selected,at,mode);int completed=0,failed=0,verseCount=0;
                status.Text="Scope: "+selected.ShortName+" | "+scope.Text+" | "+work.Count+" chapter(s). Preparing verse count...";
                RefreshList();Changed();
                foreach(var item in work)
                {
                    if(cancelRequested||disposed||project!=selected)break;
                    try
                    {
                        // SDK calls stay on the host UI thread. Pure text analysis runs off-thread.
                        item.Snapshot=new ChapterSnapshot{Project=selected.ID,Book=item.Book,Chapter=item.Chapter,Text=selected.GetUSFM(item.Book,item.Chapter)};
                        var segments=Usfm.Segments(item.Snapshot.Text);
                        item.Scope=segments;
                        if(mode==0)item.Scope=segments.Where(s=>Usfm.VerseContains(s.Verse,at.VerseNum)).ToList();
                        if(mode==1)
                        {
                            var sr=initialSelection.VerseRefStart;string verseText=selected.GetUSFM(sr.BookNum,sr.ChapterNum,sr.VerseNum);
                            int verseStart=item.Snapshot.Text.IndexOf(verseText,StringComparison.Ordinal);
                            if(verseStart<0||item.Snapshot.Text.IndexOf(verseText,verseStart+1,StringComparison.Ordinal)>=0)throw new InvalidOperationException("Selection cannot be mapped exactly to the chapter. No scope expansion is allowed.");
                            int start=verseStart+initialSelection.Offset;
                            if(start<0||start+initialSelection.SelectedText.Length>item.Snapshot.Text.Length||item.Snapshot.Text.Substring(start,initialSelection.SelectedText.Length)!=initialSelection.SelectedText)throw new InvalidOperationException("Selection changed or cannot be mapped. Select it again.");
                            item.Scope=new List<TextSpan>{new TextSpan{Start=start,Length=initialSelection.SelectedText.Length}};
                        }
                        verseCount+=segments.Where(s=>item.Scope.Any(t=>s.Start<t.End&&s.End>t.Start)).Select(s=>s.Verse).Distinct().Count();
                        var found=await Task.Run(()=>selectedEngine.Analyze(item.Snapshot,selectedStyle,item.Scope,null));
                        if(disposed||project!=selected||cancelRequested)break;
                        if(selected.GetUSFM(item.Book,item.Chapter)!=item.Snapshot.Text)foreach(var f in found)f.State=ReviewState.Stale;
                        foreach(var f in found){ReviewState saved;if(f.State!=ReviewState.Stale&&decisions.TryGetValue(f.Id,out saved))f.State=saved;findings.Add(f);}
                        sourceCache[item.Book+":"+item.Chapter]=item.Snapshot.Text;completed++;
                    }
                    catch(Exception ex){failed++;status.Text="Chapter "+item.Book+" "+item.Chapter+" skipped: "+ex.Message;}
                    status.Text=(cancelRequested?"Cancelled. ":"Scanning. ")+completed+"/"+work.Count+" chapters; "+verseCount+" verses; "+failed+" failed; "+findings.Count+" findings.";
                    RefreshList();await Task.Delay(1);
                }
                status.Text=(cancelRequested?"Cancelled; partial results. ":"Scan finished. ")+completed+" chapters; "+verseCount+" verses; "+failed+" failed; "+findings.Count+" findings. No Scripture changed.";
                Changed();
            }
            catch(Exception ex){Error(ex);}
            finally{scanning=false;if(!disposed){scan.Enabled=engine!=null;cancel.Enabled=false;ShowFinding();}}
        }
        private Finding Selected {get{return list.SelectedItems.Count==0?null:list.SelectedItems[0].Tag as Finding;}}
        private void RefreshList()
        {
            publishedFindings=findings.ToArray();
            var prior=Selected;list.BeginUpdate();list.Items.Clear();
            foreach(var f in findings.Where(f=>filter.SelectedIndex<=0||f.State.ToString()==filter.Text))
            {
                var row=new ListViewItem(new[]{f.Reference,f.Original,f.Suggested??"Human review",f.State.ToString(),f.Rule});row.Tag=f;list.Items.Add(row);if(prior!=null&&f.Id==prior.Id)row.Selected=true;
            }
            list.EndUpdate();if(list.SelectedItems.Count==0&&list.Items.Count>0)list.Items[0].Selected=true;ShowFinding();
        }
        private void ShowFinding()
        {
            var f=Selected;apply.Enabled=modify.Enabled=!scanning&&f!=null&&f.Actionable;
            if(f==null){context.Clear();return;}
            context.Text=f.Context+"\n\n"+f.Original+"\n→ "+(f.Suggested??"No verified replacement — leave open for human review")+"\n\n"+f.Reason+"\n\n"+f.Rule+" | "+f.State+"\n"+f.Evidence+"\n"+String.Join("\n",f.Sources);
            int target=context.Text.IndexOf(f.Original,StringComparison.Ordinal);if(target>=0){context.Select(target,f.Original.Length);context.SelectionBackColor=SystemColors.Info;context.Select(0,0);}
        }
        private void NavigateFinding(int delta)
        {
            if(list.Items.Count==0)return;int i=list.SelectedIndices.Count==0?0:list.SelectedIndices[0];i=Math.Max(0,Math.Min(list.Items.Count-1,i+delta));list.Items[i].Selected=true;list.Items[i].EnsureVisible();
            var f=Selected;if(f!=null){var vr=project.Versification.CreateReference(f.Book,f.Chapter,Int32.Parse(System.Text.RegularExpressions.Regex.Match(f.Verse,@"\d+").Value));host.SetReferenceForSyncGroup(vr,parent.CurrentState.SyncReferenceGroup);}
        }
        private void SaveDecisions(){File.WriteAllText(Path.Combine(projectFolder,"decisions.json"),Json.Write(decisions),new UTF8Encoding(false));}
        private void Decide(ReviewState state,bool persist)
        {
            var f=Selected;if(f==null||scanning||applying)return;
            if(f.State==ReviewState.Applied||f.State==ReviewState.Uncertain||f.State==ReviewState.Stale){status.Text="Rescan this verse before changing its review state.";return;}
            try
            {
                journal.Append(new AuditRecord{Transaction=Guid.NewGuid().ToString("N"),FindingId=f.Id,Project=f.Project,Book=f.Book,Chapter=f.Chapter,Verse=f.Verse,Start=f.Start,Before=f.Original,BeforeHash=f.ChapterHash,Action=state.ToString().ToLowerInvariant()+(persist?"-project":"-occurrence"),State="reviewed",Reviewer=host.UserInfo==null?null:host.UserInfo.Name,Build=BuildInfo.Version,Rule=f.Rule,Catalog=f.CatalogVersion,Config=f.ConfigHash,Evidence=f.Evidence,Timestamp=DateTime.UtcNow.ToString("o")});
                f.State=state==ReviewState.Open&&f.Suggested==null?ReviewState.NeedsReview:state;
                if(persist||state==ReviewState.Rejected){if(state==ReviewState.Open)decisions.Remove(f.Id);else decisions[f.Id]=state;SaveDecisions();}
            }
            catch(Exception ex){Error(ex);return;}
            RefreshList();Changed();
        }
        private void Preview(bool modified)
        {
            var f=Selected;if(f==null||!f.Actionable||scanning||applying)return;
            using(var dialog=new Form{Text=modified?"Modify & Apply — "+f.Reference:"Apply suggestion — "+f.Reference,Width=700,Height=400,StartPosition=FormStartPosition.CenterParent,MinimizeBox=false,MaximizeBox=false})
            {
                var before=new TextBox{Text=f.Context+Environment.NewLine+Environment.NewLine+"Original: "+f.Original,Multiline=true,ReadOnly=true,Dock=DockStyle.Top,Height=120,Font=context.Font};
                var replacement=new TextBox{Text=f.Suggested,Multiline=true,ReadOnly=!modified,Dock=DockStyle.Fill,Font=context.Font,AccessibleName="Replacement preview"};
                var buttons=new FlowLayoutPanel{Dock=DockStyle.Bottom,AutoSize=true,FlowDirection=FlowDirection.RightToLeft};var confirm=Button(modified?"Modify && Apply":"Apply",delegate{dialog.DialogResult=DialogResult.OK;});var close=Button("Cancel",delegate{dialog.DialogResult=DialogResult.Cancel;});buttons.Controls.AddRange(new Control[]{confirm,close});dialog.Controls.Add(replacement);dialog.Controls.Add(before);dialog.Controls.Add(buttons);dialog.AcceptButton=confirm;dialog.CancelButton=close;
                if(dialog.ShowDialog(this)!=DialogResult.OK)return;
                try
                {
                    applying=true;var service=new PatchService(new HostStore(project,this),journal,BuildInfo.Version);
                    lastApplied=service.Apply(f,replacement.Text,host.UserInfo==null?null:host.UserInfo.Name,modified,style.Hash);revert.Enabled=true;
                    foreach(var other in findings.Where(o=>o.Book==f.Book&&o.Chapter==f.Chapter&&o!=f&&o.State!=ReviewState.Applied))other.State=ReviewState.Stale;
                    status.Text="Applied one reviewed occurrence. Rescan the chapter to refresh context.";
                }
                catch(Exception ex){Error(ex);}
                finally{applying=false;RefreshList();Changed();}
            }
        }
        private void Revert(object sender,EventArgs e)
        {
            if(lastApplied==null||scanning||applying)return;
            if(MessageBox.Show(this,lastApplied.After+"\n→ "+lastApplied.Before+"\n\nRevert this edit only if its chapter has not changed?","Revert last edit",MessageBoxButtons.OKCancel,MessageBoxIcon.Question)!=DialogResult.OK)return;
            try{new PatchService(new HostStore(project,this),journal,BuildInfo.Version).Revert(lastApplied,host.UserInfo==null?null:host.UserInfo.Name);lastApplied=null;revert.Enabled=false;status.Text="Edit reverted. Rescan to refresh findings.";}
            catch(Exception ex){Error(ex);}
        }
        private void EditHouseStyle(object sender,EventArgs e)
        {
            if(scanning||applying)return;
            using(var d=new Form{Text="Project house style — explicit no-insertion lexemes",Width=600,Height=350,StartPosition=FormStartPosition.CenterParent})
            {
                var text=new TextBox{Multiline=true,Dock=DockStyle.Fill,Text=String.Join(Environment.NewLine,style.NoInsertBefore),Font=context.Font,AccessibleName="No-insertion lexemes, one per line"};
                var save=Button("Save for this project",delegate{d.DialogResult=DialogResult.OK;});save.Dock=DockStyle.Bottom;d.Controls.Add(text);d.Controls.Add(save);
                if(d.ShowDialog(this)==DialogResult.OK){style=new HouseStyle{Version=DateTime.UtcNow.ToString("o"),NoInsertBefore=text.Lines.Select(l=>l.Trim().Normalize()).Where(l=>l.Length>0).Distinct().ToList()};File.WriteAllText(Path.Combine(projectFolder,"house-style.json"),Json.Write(style),new UTF8Encoding(false));foreach(var f in findings)f.State=ReviewState.Stale;RefreshList();Changed();status.Text="Project house style saved. Rescan required.";}
            }
        }
        private ContextMenuStrip MenuForSelection()
        {
            var menu=new ContextMenuStrip();menu.Items.Add("Apply...",null,delegate{Preview(false);});menu.Items.Add("Modify & Apply...",null,delegate{Preview(true);});menu.Items.Add("Reject",null,delegate{Decide(ReviewState.Rejected,false);});menu.Items.Add("Ignore this scan",null,delegate{Decide(ReviewState.Ignored,false);});menu.Items.Add("Leave open",null,delegate{Decide(ReviewState.Open,true);});
            menu.Opening+=delegate{var f=Selected;menu.Items[0].Enabled=menu.Items[1].Enabled=f!=null&&f.Actionable&&!scanning;};return menu;
        }
        internal void InlineMenu(Finding finding,Point screenPoint)
        {
            if(disposed)return;if(InvokeRequired){BeginInvoke((Action)delegate{InlineMenu(finding,screenPoint);});return;}
            filter.SelectedIndex=0;foreach(ListViewItem row in list.Items)row.Selected=((Finding)row.Tag).Id==finding.Id;
            MenuForSelection().Show(screenPoint);
        }
        internal IList<Finding> CurrentFindings {get{return publishedFindings;}}
        internal string ChapterSource(Finding f){string text;return sourceCache.TryGetValue(f.Book+":"+f.Chapter,out text)?text:null;}
        private void Error(Exception ex){status.Text=ex.Message;MessageBox.Show(this,ex.Message,"Tamil Sandhi QA",MessageBoxButtons.OK,MessageBoxIcon.Warning);}
        protected override void Dispose(bool disposing)
        {
            disposed=true;cancelRequested=true;
            if(disposing){if(project!=null){project.ScriptureDataChanged-=ScriptureChanged;ReviewControl current;if(Sessions.TryGetValue(project.ID,out current)&&current==this){ReviewControl removed;Sessions.TryRemove(project.ID,out removed);}}if(parent!=null){parent.ProjectChanged-=ProjectChanged;parent.VerseRefChanged-=ReferenceChanged;}}
            base.Dispose(disposing);
        }
    }
}
