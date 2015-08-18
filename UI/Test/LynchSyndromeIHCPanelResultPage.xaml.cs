﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Xml.Linq;

namespace YellowstonePathology.UI.Test
{
	/// <summary>
	/// Interaction logic for LynchSyndromeResultPage.xaml
	/// </summary>
	public partial class LynchSyndromeIHCPanelResultPage : UserControl, INotifyPropertyChanged, Business.Interface.IPersistPageChanges
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public delegate void NextEventHandler(object sender, EventArgs e);
		public event NextEventHandler Next;        

		private YellowstonePathology.Business.User.SystemIdentity m_SystemIdentity;
		private YellowstonePathology.Business.Test.AccessionOrder m_AccessionOrder;
		private YellowstonePathology.Business.Persistence.ObjectTracker m_ObjectTracker;
		private string m_PageHeaderText;

		private YellowstonePathology.Business.Test.LynchSyndrome.LynchSyndromeIHCPanelTestOrder m_PanelSetOrderLynchSyndromeIHC;
		private string m_OrderedOnDescription;

        public LynchSyndromeIHCPanelResultPage(YellowstonePathology.Business.Test.LynchSyndrome.LynchSyndromeIHCPanelTestOrder panelSetOrderLynchSyndromeIHC,
			YellowstonePathology.Business.Test.AccessionOrder accessionOrder,
			YellowstonePathology.Business.Persistence.ObjectTracker objectTracker,
			YellowstonePathology.Business.User.SystemIdentity systemIdentity)
		{
			this.m_PanelSetOrderLynchSyndromeIHC = panelSetOrderLynchSyndromeIHC;
			this.m_AccessionOrder = accessionOrder;
			this.m_SystemIdentity = systemIdentity;
			this.m_ObjectTracker = objectTracker;

            this.m_PageHeaderText = "Lynch Syndrome IHC Results For: " + this.m_AccessionOrder.PatientDisplayName + " (" + this.m_PanelSetOrderLynchSyndromeIHC.ReportNo + ")";

			YellowstonePathology.Business.Specimen.Model.SpecimenOrder specimenOrder = this.m_AccessionOrder.SpecimenOrderCollection.GetByAliquotOrderId(this.m_PanelSetOrderLynchSyndromeIHC.OrderedOnId);
			YellowstonePathology.Business.Test.AliquotOrder aliquotOrder = this.m_AccessionOrder.SpecimenOrderCollection.GetAliquotOrder(this.m_PanelSetOrderLynchSyndromeIHC.OrderedOnId);
			this.m_OrderedOnDescription = specimenOrder.Description + ": " + aliquotOrder.Label;

			InitializeComponent();

			DataContext = this;
		}

		public string OrderedOnDescription
		{
			get { return this.m_OrderedOnDescription; }
		}

		public YellowstonePathology.Business.Test.LynchSyndrome.LynchSyndromeIHCPanelTestOrder PanelSetOrder
		{
			get { return this.m_PanelSetOrderLynchSyndromeIHC; }
		}

		public void NotifyPropertyChanged(String info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}

		public string PageHeaderText
		{
			get { return this.m_PageHeaderText; }
		}

		public bool OkToSaveOnNavigation(Type pageNavigatingTo)
		{
			return true;
		}

		public bool OkToSaveOnClose()
		{
			return true;
		}

		public void Save()
		{
			this.m_ObjectTracker.SubmitChanges(this.m_AccessionOrder);
		}

		public void UpdateBindingSources()
		{

		}			

		private void ButtonNext_Click(object sender, RoutedEventArgs e)
		{			
			if (this.Next != null) this.Next(this, new EventArgs());
		}        

        private void HyperLinkNoLossOfNuclearExpression_Click(object sender, RoutedEventArgs e)
        {
            YellowstonePathology.Business.Test.LynchSyndrome.IHCResultNoLossOfNuclearExpression ihcResult = new Business.Test.LynchSyndrome.IHCResultNoLossOfNuclearExpression();
            ihcResult.SetResults(this.m_PanelSetOrderLynchSyndromeIHC);
        }

        private void HyperLinkLossOfNuclearExpressionMSH2MSH6_Click(object sender, RoutedEventArgs e)
        {
            YellowstonePathology.Business.Test.LynchSyndrome.IHCResult ihcResult = new Business.Test.LynchSyndrome.IHCResultLossOfNuclearExpressionMSH2MSH6();
            ihcResult.SetResults(this.m_PanelSetOrderLynchSyndromeIHC);
        }

        private void HyperLinkLossOfNuclearExpressionMSH6_Click(object sender, RoutedEventArgs e)
        {
            YellowstonePathology.Business.Test.LynchSyndrome.IHCResult ihcResult = new Business.Test.LynchSyndrome.IHCResultLossOfNuclearExpressionMSH6();
            ihcResult.SetResults(this.m_PanelSetOrderLynchSyndromeIHC);
        }

        private void HyperLinkLossOfNuclearExpressionPMS2_Click(object sender, RoutedEventArgs e)
        {
            YellowstonePathology.Business.Test.LynchSyndrome.IHCResult ihcResult = new Business.Test.LynchSyndrome.IHCResultLossOfNuclearExpressionPMS2();
            ihcResult.SetResults(this.m_PanelSetOrderLynchSyndromeIHC);
        }

        private void HyperLinkLossOfNuclearExpressionMLH1PMS2_Click(object sender, RoutedEventArgs e)
        {
            YellowstonePathology.Business.Test.LynchSyndrome.IHCResult ihcResult = new Business.Test.LynchSyndrome.IHCResultLossOfNuclearExpressionMLH1PMS2();
            ihcResult.SetResults(this.m_PanelSetOrderLynchSyndromeIHC);
        }

		private void HyperLinkLossOfNuclearExpressionMLH1_Click(object sender, RoutedEventArgs e)
		{
			YellowstonePathology.Business.Test.LynchSyndrome.IHCResult ihcResult = new Business.Test.LynchSyndrome.IHCResultLossOfNuclearExpressionMLH1();
			ihcResult.SetResults(this.m_PanelSetOrderLynchSyndromeIHC);
		}

		private void HyperLinkLossOfNuclearExpressionMSH2_Click(object sender, RoutedEventArgs e)
		{
			YellowstonePathology.Business.Test.LynchSyndrome.IHCResult ihcResult = new Business.Test.LynchSyndrome.IHCResultLossOfNuclearExpressionMSH2();
			ihcResult.SetResults(this.m_PanelSetOrderLynchSyndromeIHC);
		}

		private void HyperLinkFinalizeResults_Click(object sender, RoutedEventArgs e)
		{
			if (this.m_PanelSetOrderLynchSyndromeIHC.Final == false)
			{
				this.m_PanelSetOrderLynchSyndromeIHC.Finalize(this.m_SystemIdentity.User);
			}
			else
			{
				MessageBox.Show("This case cannot be finalized because it is already final.");
			}
		}

		private void HyperLinkShowDocument_Click(object sender, RoutedEventArgs e)
		{
			this.Save();
			YellowstonePathology.Business.Test.LynchSyndrome.LynchSyndromeIHCPanelWordDocument report = new Business.Test.LynchSyndrome.LynchSyndromeIHCPanelWordDocument();
			report.Render(this.m_AccessionOrder.MasterAccessionNo, this.m_PanelSetOrderLynchSyndromeIHC.ReportNo, Business.Document.ReportSaveModeEnum.Draft);

			YellowstonePathology.Business.OrderIdParser orderIdParser = new Business.OrderIdParser(this.m_PanelSetOrderLynchSyndromeIHC.ReportNo);
			string fileName = YellowstonePathology.Business.Document.CaseDocument.GetDraftDocumentFilePath(orderIdParser);
			YellowstonePathology.Business.Document.CaseDocument.OpenWordDocumentWithWordViewer(fileName);
		}

		private void HyperLinkUnfinalResults_Click(object sender, RoutedEventArgs e)
		{
			if (this.m_PanelSetOrderLynchSyndromeIHC.Final == true)
			{
				this.m_PanelSetOrderLynchSyndromeIHC.Unfinalize();
			}
			else
			{
				MessageBox.Show("This case cannot be unfinalized because it is not final.");
			}
		}

		private void HyperLinkAcceptResults_Click(object sender, RoutedEventArgs e)
		{
			//if (this.ComboBoxResult.SelectedItem != null)
			//{
			YellowstonePathology.Business.Rules.MethodResult result = this.m_PanelSetOrderLynchSyndromeIHC.IsOkToAccept();
			if (result.Success == true)
			{
				this.m_PanelSetOrderLynchSyndromeIHC.Accept(this.m_SystemIdentity.User);
			}
			else
			{
				MessageBox.Show(result.Message);
			}
			//}
			//else
			//{
			//	MessageBox.Show("A result must be selected before it can be accepted.");
			//}
		}

		private void HyperLinkUnacceptResults_Click(object sender, RoutedEventArgs e)
		{
			YellowstonePathology.Business.Rules.MethodResult result = this.m_PanelSetOrderLynchSyndromeIHC.IsOkToUnaccept();
			if (result.Success == true)
			{
				this.m_PanelSetOrderLynchSyndromeIHC.Unaccept();
			}
			else
			{
				MessageBox.Show(result.Message);
			}
		}
	}
}
