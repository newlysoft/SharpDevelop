﻿/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 08.06.2011
 * Time: 19:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Documents;

using ICSharpCode.Reports.Core.Exporter.ExportRenderer;

namespace ICSharpCode.Reports.Core.WpfReportViewer
{
	/// <summary>
	/// Description of PreviewViewModel.
	/// </summary>
	public class PreviewViewModel:INotifyPropertyChanged
	{
		
		private IDocumentPaginatorSource document;
		
		public PreviewViewModel(ReportSettings reportSettings, PagesCollection pages)
		{
			this.Pages = pages;
//			Stopwatch s = new Stopwatch();
//			s.Start();
//			Console.WriteLine("-------------");
			FixedDocumentRenderer renderer =  FixedDocumentRenderer.CreateInstance(reportSettings,Pages);
			
//			Console.WriteLine("\tfinish render {0}",s.Elapsed);
//			Console.WriteLine("-------------");
//			
			renderer.Start();
//				Console.WriteLine("\tfinsih start {0}",s.Elapsed);
				
			renderer.RenderOutput();
//				Console.WriteLine("\tfinish renderoutput {0}",s.Elapsed);
			renderer.End();
//				Console.WriteLine("\tfinish end {0}",s.Elapsed);
			this.Document = renderer.Document;
//				Console.WriteLine("\tfinish document {0}",s.Elapsed);
//				s.Stop();
		}
		
		public PagesCollection Pages {get;private set;}
		
		public IDocumentPaginatorSource Document
		{
			get {return document;}
			set {
				this.document = value;
				OnNotifyPropertyChanged ("Document");
				}	
		}
		
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		void OnNotifyPropertyChanged(string num0)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this,new PropertyChangedEventArgs(num0));
			}
		}
	}
}
