﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace YellowstonePathology.UI.Test
{
    public class MissingInformationResultPath : ResultPath
    {        
        private MissingInformationResultPage m_ResultPage;
        private YellowstonePathology.Business.Test.AccessionOrder m_AccessionOrder;
		private YellowstonePathology.Business.Persistence.ObjectTracker m_ObjectTracker;
		private YellowstonePathology.Business.Test.MissingInformation.MissingInformtionTestOrder m_MissingInformationTestOrder;

        public MissingInformationResultPath(string reportNo, YellowstonePathology.Business.Test.AccessionOrder accessionOrder,            
            YellowstonePathology.Business.Persistence.ObjectTracker objectTracker,
            YellowstonePathology.UI.Navigation.PageNavigator pageNavigator)
            : base(pageNavigator)
        {
            this.m_AccessionOrder = accessionOrder;
            this.m_ObjectTracker = objectTracker;            
			this.m_MissingInformationTestOrder = (YellowstonePathology.Business.Test.MissingInformation.MissingInformtionTestOrder)this.m_AccessionOrder.PanelSetOrderCollection.GetPanelSetOrder(reportNo);
		}

        public override void Start()
        {            
            this.ShowResultPage();
        }		

		private void ShowResultPage()
		{
			this.m_ResultPage = new MissingInformationResultPage(this.m_MissingInformationTestOrder, this.m_AccessionOrder, this.m_ObjectTracker, this.m_SystemIdentity);            
            this.m_ResultPage.Next += new MissingInformationResultPage.NextEventHandler(ResultPage_Next);
            this.m_PageNavigator.Navigate(this.m_ResultPage);
		}

        private void ResultPage_Next(object sender, EventArgs e)
        {
            base.Finished();
        }        
    }
}