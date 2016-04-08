﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YellowstonePathology.UI.Test
{
    public class API2MALT1ByPCRResultPath : ResultPath
    {
        API2MALT1ByPCRResultPage m_ResultPage;
        YellowstonePathology.Business.Test.AccessionOrder m_AccessionOrder;
        YellowstonePathology.Business.Test.API2MALT1ByPCR.API2MALT1ByPCRTestOrder m_PanelSetOrder;

        public API2MALT1ByPCRResultPath(string reportNo,
            YellowstonePathology.Business.Test.AccessionOrder accessionOrder,
            YellowstonePathology.UI.Navigation.PageNavigator pageNavigator,
            System.Windows.Window window)
            : base(pageNavigator, window)
        {
            this.m_AccessionOrder = accessionOrder;
            this.m_PanelSetOrder = (YellowstonePathology.Business.Test.API2MALT1ByPCR.API2MALT1ByPCRTestOrder)this.m_AccessionOrder.PanelSetOrderCollection.GetPanelSetOrder(reportNo);
        }

        protected override void ShowResultPage()
        {
            this.m_ResultPage = new API2MALT1ByPCRResultPage((YellowstonePathology.Business.Test.API2MALT1ByPCR.API2MALT1ByPCRTestOrder)this.m_PanelSetOrder, this.m_AccessionOrder, this.m_SystemIdentity);
            this.m_ResultPage.Next += new API2MALT1ByPCRResultPage.NextEventHandler(ResultsPage_Next);
            this.m_PageNavigator.Navigate(this.m_ResultPage);
        }

        private void ResultsPage_Next(object sender, EventArgs e)
        {
            this.Finished();
        }
    }
}
