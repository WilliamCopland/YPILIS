﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YellowstonePathology.UI.Test
{
	public class JAK2V617FResultPath : ResultPath
	{
		JAK2V617FResultPage m_ResultPage;
		YellowstonePathology.Business.Test.AccessionOrder m_AccessionOrder;
		YellowstonePathology.Business.Test.JAK2V617F.JAK2V617FTestOrder m_PanelSetOrder;
		private YellowstonePathology.Business.Persistence.ObjectTracker m_ObjectTracker;

		public JAK2V617FResultPath(string reportNo,
            YellowstonePathology.Business.Test.AccessionOrder accessionOrder,
			YellowstonePathology.Business.Persistence.ObjectTracker objectTracker,
            YellowstonePathology.UI.Navigation.PageNavigator pageNavigator) 
            : base(pageNavigator)
        {
            this.m_AccessionOrder = accessionOrder;
			this.m_PanelSetOrder = (YellowstonePathology.Business.Test.JAK2V617F.JAK2V617FTestOrder)this.m_AccessionOrder.PanelSetOrderCollection.GetPanelSetOrder(reportNo);
			this.m_ObjectTracker = objectTracker;
			this.Authenticated += new AuthenticatedEventHandler(ResultPath_Authenticated);
		}

		private void ResultPath_Authenticated(object sender, EventArgs e)
		{
			this.ShowResultPage();
		}

        private void ShowResultPage()
        {
			this.m_ResultPage = new JAK2V617FResultPage(this.m_PanelSetOrder, this.m_AccessionOrder, this.m_ObjectTracker, this.m_SystemIdentity, this.m_PageNavigator);
			this.m_ResultPage.Next += new JAK2V617FResultPage.NextEventHandler(ResultPage_Next);
			this.m_PageNavigator.Navigate(this.m_ResultPage);
        }

		private void ResultPage_Next(object sender, EventArgs e)
        {
			if (this.ShowReflexTestPage() == false)
			{
				this.Finished();
			}
		}

		private bool ShowReflexTestPage()
		{
			bool result = false;
			YellowstonePathology.Business.Test.MPNStandardReflex.MPNStandardReflexTest panelSetMPNStandardReflex = new YellowstonePathology.Business.Test.MPNStandardReflex.MPNStandardReflexTest();
			YellowstonePathology.Business.Test.MPNExtendedReflex.MPNExtendedReflexTest panelSetMPNExtendedReflex = new YellowstonePathology.Business.Test.MPNExtendedReflex.MPNExtendedReflexTest();
			if (this.m_AccessionOrder.PanelSetOrderCollection.Exists(panelSetMPNStandardReflex.PanelSetId) == true)
			{
				result = true;
				YellowstonePathology.Business.Test.MPNStandardReflex.MPNStandardReflexTestOrder panelSetOrderMPNStandardReflex = (YellowstonePathology.Business.Test.MPNStandardReflex.MPNStandardReflexTestOrder)this.m_AccessionOrder.PanelSetOrderCollection.GetPanelSetOrder(panelSetMPNStandardReflex.PanelSetId);
				Test.MPNStandardReflexPath MPNStandardReflexPath = new Test.MPNStandardReflexPath(panelSetOrderMPNStandardReflex.ReportNo, this.m_AccessionOrder, this.m_ObjectTracker, this.m_PageNavigator);
				MPNStandardReflexPath.Finish += new Test.MPNStandardReflexPath.FinishEventHandler(MPNStandardReflexPath_Finish);
				MPNStandardReflexPath.Back += new MPNStandardReflexPath.BackEventHandler(MPNStandardReflexPath_Back);
				MPNStandardReflexPath.Start();
			}
			else if (this.m_AccessionOrder.PanelSetOrderCollection.Exists(panelSetMPNExtendedReflex.PanelSetId) == true)
			{
				result = true;
				YellowstonePathology.Business.Test.MPNExtendedReflex.MPNExtendedReflexTestOrder panelSetOrderMPNExtendedReflex = (YellowstonePathology.Business.Test.MPNExtendedReflex.MPNExtendedReflexTestOrder)this.m_AccessionOrder.PanelSetOrderCollection.GetPanelSetOrder(panelSetMPNExtendedReflex.PanelSetId);
				Test.MPNExtendedReflexPath MPNExtendedReflexPath = new Test.MPNExtendedReflexPath(panelSetOrderMPNExtendedReflex.ReportNo, this.m_AccessionOrder, this.m_ObjectTracker, this.m_PageNavigator);
				MPNExtendedReflexPath.Finish += new Test.MPNExtendedReflexPath.FinishEventHandler(MPNExtendedReflexPath_Finish);
				MPNExtendedReflexPath.Back += new MPNExtendedReflexPath.BackEventHandler(MPNExtendedReflexPath_Back);
				MPNExtendedReflexPath.Start();
			}
			return result;
		}

		private void MPNStandardReflexPath_Back(object sender, EventArgs e)
		{
			this.ShowResultPage();
		}

		private void MPNStandardReflexPath_Finish(object sender, EventArgs e)
		{
			base.Finished();
		}

		private void MPNExtendedReflexPath_Back(object sender, EventArgs e)
		{
			this.ShowResultPage();
		}

		private void MPNExtendedReflexPath_Finish(object sender, EventArgs e)
		{
			base.Finished();
		}

		private void ReportOrderPath_Finish(object sender, EventArgs e)
		{
			this.ShowReflexTestPage();
		}
	}
}
