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

namespace YellowstonePathology.UI.Test
{
	/// <summary>
	/// Interaction logic for MDSByFishResultPage.xaml
	/// </summary>
	public partial class MDSByFishResultPage : UserControl, INotifyPropertyChanged, Business.Interface.IPersistPageChanges
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public delegate void NextEventHandler(object sender, EventArgs e);
		public event NextEventHandler Next;

		private YellowstonePathology.Business.User.SystemIdentity m_SystemIdentity;
		private YellowstonePathology.Business.Test.AccessionOrder m_AccessionOrder;
		private string m_PageHeaderText;

		private YellowstonePathology.Business.Test.MDSByFish.PanelSetOrderMDSByFish m_PanelSetOrder;
		private string m_OrderedOnDescription;

		public MDSByFishResultPage(YellowstonePathology.Business.Test.MDSByFish.PanelSetOrderMDSByFish panelSetOrderMDSByFish,
			YellowstonePathology.Business.Test.AccessionOrder accessionOrder,
			YellowstonePathology.Business.User.SystemIdentity systemIdentity)
		{
			this.m_PanelSetOrder = panelSetOrderMDSByFish;
			this.m_AccessionOrder = accessionOrder;
			this.m_SystemIdentity = systemIdentity;

			this.m_PageHeaderText = "MDS by Fish Analysis Result For: " + this.m_AccessionOrder.PatientDisplayName;

			YellowstonePathology.Business.Specimen.Model.SpecimenOrder specimenOrder = this.m_AccessionOrder.SpecimenOrderCollection.GetSpecimenOrder(this.m_PanelSetOrder.OrderedOn, this.m_PanelSetOrder.OrderedOnId);
			this.m_OrderedOnDescription = specimenOrder.Description;

			InitializeComponent();

			DataContext = this;

            Loaded += MDSByFishResultPage_Loaded;
            Unloaded += MDSByFishResultPage_Unloaded;
		}

        private void MDSByFishResultPage_Loaded(object sender, RoutedEventArgs e)
        {
             
        }

        private void MDSByFishResultPage_Unloaded(object sender, RoutedEventArgs e)
        {
             
        }

        public string OrderedOnDescription
		{
			get { return this.m_OrderedOnDescription; }
		}

		public YellowstonePathology.Business.Test.MDSByFish.PanelSetOrderMDSByFish PanelSetOrder
		{
			get { return this.m_PanelSetOrder; }
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

		public void Save(bool releaseLock)
		{
            YellowstonePathology.Business.Persistence.ObjectGateway.Instance.SubmitChanges(this.m_AccessionOrder, false);
        }

        public void UpdateBindingSources()
		{

		}

		private void HyperLinkNormal_Click(object sender, RoutedEventArgs e)
		{
			YellowstonePathology.Business.Test.MDSByFish.MDSByFishNormalResult result = new Business.Test.MDSByFish.MDSByFishNormalResult();
			result.SetResults(this.m_PanelSetOrder);
			this.NotifyPropertyChanged("PanelSetOrder");
		}

		private void HyperLinkAbnormal_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("This result is not yet implemented.", "Not implemented yet.");
		}

		private void HyperLinkShowDocument_Click(object sender, RoutedEventArgs e)
		{
			this.Save(false);
			YellowstonePathology.Business.Test.MDSByFish.MDSByFishWordDocument report = new Business.Test.MDSByFish.MDSByFishWordDocument();
			report.Render(this.m_AccessionOrder.MasterAccessionNo, this.m_PanelSetOrder.ReportNo, Business.Document.ReportSaveModeEnum.Draft);

			YellowstonePathology.Business.OrderIdParser orderIdParser = new Business.OrderIdParser(this.m_PanelSetOrder.ReportNo);
			string fileName = YellowstonePathology.Business.Document.CaseDocument.GetDraftDocumentFilePath(orderIdParser);
			YellowstonePathology.Business.Document.CaseDocument.OpenWordDocumentWithWordViewer(fileName);
		}

		private void HyperLinkFinalizeResults_Click(object sender, RoutedEventArgs e)
		{
			if (this.m_PanelSetOrder.Final == false)
			{
				this.m_PanelSetOrder.Finalize(this.m_SystemIdentity.User);
			}
			else
			{
				MessageBox.Show("This case cannot be finalized because it is already final.");
			}
		}

		private void HyperLinkUnfinalResults_Click(object sender, RoutedEventArgs e)
		{
			if (this.m_PanelSetOrder.Final == true)
			{
				this.m_PanelSetOrder.Unfinalize();
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
			YellowstonePathology.Business.Rules.MethodResult result = this.m_PanelSetOrder.IsOkToAccept();
			if (result.Success == true)
			{
				this.m_PanelSetOrder.Accept(this.m_SystemIdentity.User);
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
			YellowstonePathology.Business.Rules.MethodResult result = this.m_PanelSetOrder.IsOkToUnaccept();
			if (result.Success == true)
			{
				this.m_PanelSetOrder.Unaccept();
			}
			else
			{
				MessageBox.Show(result.Message);
			}
		}

		private void ButtonNext_Click(object sender, RoutedEventArgs e)
		{
			if (this.Next != null) this.Next(this, new EventArgs());
		}
	}
}
