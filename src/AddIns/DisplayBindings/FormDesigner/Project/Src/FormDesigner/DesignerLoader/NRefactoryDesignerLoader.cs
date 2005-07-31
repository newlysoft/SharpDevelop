﻿// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.IO;
using System.ComponentModel.Design;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms;
using System.Windows.Forms.Design;


using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

using ICSharpCode.FormDesigner.Services;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;
using ICSharpCode.NRefactory.PrettyPrinter;

namespace ICSharpCode.FormDesigner
{
	public class DefaultMemberRelationshipService : MemberRelationshipService
	{
		public override bool SupportsRelationship(MemberRelationship source, MemberRelationship relationship)
		{
			return true;
		}
		protected override MemberRelationship GetRelationship(MemberRelationship source)
		{
			return base.GetRelationship(source);
		}
	}
	
	public class NRefactoryDesignerLoader : CodeDomDesignerLoader, ICodeDomDesignerReload
	{
		bool                  loading               = true;
		IDesignerLoaderHost   designerLoaderHost    = null;
		ITypeResolutionService typeResolutionService = null;
		SupportedLanguages    language;
		Microsoft.CSharp.CSharpCodeProvider provider = new Microsoft.CSharp.CSharpCodeProvider();
		
		protected Hashtable resources = null;
		bool isReloadNeeded  = false;
		
		TextEditorControl textEditorControl;
		
		public string TextContent {
			get {
				return textEditorControl.Document.TextContent;
			}
		}
		
		public override bool Loading {
			get {
				return loading;
			}
		}
		
		public IDesignerLoaderHost DesignerLoaderHost {
			get {
				return designerLoaderHost;
			}
		}
		
		protected override CodeDomProvider CodeDomProvider {
			get {
				return provider;
			}
		}
		
		protected override ITypeResolutionService TypeResolutionService {
			get {
				Console.WriteLine("type resolution service");
				return typeResolutionService;
			}
		}
		
		protected override bool IsReloadNeeded()
		{
			return isReloadNeeded | base.IsReloadNeeded();
		}
		
		public NRefactoryDesignerLoader(SupportedLanguages language, TextEditorControl textEditorControl)
		{
			this.language = language;
			this.textEditorControl = textEditorControl;
		}
		
		#region System.ComponentModel.Design.Serialization.ICodeDomDesignerReload interface implementation
		public bool ShouldReloadDesigner(CodeCompileUnit newTree) 
		{
			Console.Write("AskReload");
			return  IsReloadNeeded();
		}
		#endregion
		
		public override void BeginLoad(IDesignerLoaderHost host) 
		{
			this.loading = true;
			typeResolutionService = (ITypeResolutionService)host.GetService(typeof(ITypeResolutionService));
			base.BeginLoad(host);
		}
		protected override void OnEndLoad(bool successful, ICollection errors)
		{
			this.loading = false;
			base.OnEndLoad(successful, errors);
		}
 

		
		protected override CodeCompileUnit Parse()
		{
			Console.Write("ParseCompileUnit");
			isReloadNeeded = false;
			ICSharpCode.NRefactory.Parser.IParser p = ICSharpCode.NRefactory.Parser.ParserFactory.CreateParser(language, new StringReader(TextContent));
			p.Parse();
			
			CodeDOMVisitor visitor = new CodeDOMVisitor();
			visitor.Visit(p.CompilationUnit, null);
			
			// output generated CodeDOM to the console :
//			CodeDOMVerboseOutputGenerator outputGenerator = new CodeDOMVerboseOutputGenerator();
//			outputGenerator.GenerateCodeFromMember(visitor.codeCompileUnit.Namespaces[0].Types[0], Console.Out, null);
			
//			provider.GenerateCodeFromCompileUnit(visitor.codeCompileUnit, Console.Out, null);
			
			return visitor.codeCompileUnit;
		}
		
		protected override void Write(CodeCompileUnit unit)
		{
			provider.GenerateCodeFromCompileUnit(unit, Console.Out, null);
		}
		
//		public void Reload()
//		{
//			base.Reload(BasicDesignerLoader.ReloadFlags.Default);
//		}
//		public override void Flush()
//		{
//			base.Flush();
//		}
		
//		void InitializeExtendersForProject(IDesignerHost host)
//		{
//			IExtenderProviderService elsi = (IExtenderProviderService)host.GetService(typeof(IExtenderProviderService));
//			elsi.AddExtenderProvider(new ICSharpCode.FormDesigner.Util.NameExtender());
//		}
		
		public override void Dispose()
		{
			base.Dispose();
			if (this.resources != null) {
				foreach (DesignerResourceService.ResourceStorage storage in this.resources.Values) {
					storage.Dispose();
				}
				resources.Clear();
			}
			resources = null;
		}
	}
}
