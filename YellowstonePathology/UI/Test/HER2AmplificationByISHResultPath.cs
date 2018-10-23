﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YellowstonePathology.UI.Test
{
    public class HER2AmplificationByISHResultPath : ResultPath
    {
        HER2AmplificationByISHResultPage m_ResultPage;
        YellowstonePathology.Business.Test.AccessionOrder m_AccessionOrder;
        YellowstonePathology.Business.Test.HER2AmplificationByISH.HER2AmplificationByISHTestOrder m_PanelSetOrder;

        public HER2AmplificationByISHResultPath(string reportNo,
            YellowstonePathology.Business.Test.AccessionOrder accessionOrder,
            YellowstonePathology.UI.Navigation.PageNavigator pageNavigator,
            System.Windows.Window window)
            : base(pageNavigator, window)
        {
            this.m_AccessionOrder = accessionOrder;
            this.m_PanelSetOrder = (YellowstonePathology.Business.Test.HER2AmplificationByISH.HER2AmplificationByISHTestOrder)this.m_AccessionOrder.PanelSetOrderCollection.GetPanelSetOrder(reportNo);
        }

        protected override void ShowResultPage()
        {
            this.m_ResultPage = new HER2AmplificationByISHResultPage(this.m_PanelSetOrder, this.m_AccessionOrder, this.m_SystemIdentity, this.m_PageNavigator);
            this.m_ResultPage.Next += new HER2AmplificationByISHResultPage.NextEventHandler(ResultPage_Next);
            this.m_ResultPage.SpecimenDetail += new HER2AmplificationByISHResultPage.SpecimenDetailEventHandler(ResultPage_SpecimenDetail);
            this.m_ResultPage.OrderHER2IHCAndSummary += ResultPage_OrderHER2IHCAndSummary;

            this.RegisterCancelATest(this.m_ResultPage);
            this.m_PageNavigator.Navigate(this.m_ResultPage);
        }

        private void ResultPage_OrderHER2IHCAndSummary(object sender, EventArgs e)
        {
            YellowstonePathology.Business.Test.Her2AmplificationByIHC.Her2AmplificationByIHCTest her2AmplificationByIHCTest = new Business.Test.Her2AmplificationByIHC.Her2AmplificationByIHCTest();
            YellowstonePathology.Business.Interface.IOrderTarget orderTargetIHC = this.m_AccessionOrder.SpecimenOrderCollection.GetOrderTarget(this.m_PanelSetOrder.OrderedOnId);
            YellowstonePathology.Business.Test.TestOrderInfo testOrderInfoIHC = new YellowstonePathology.Business.Test.TestOrderInfo(her2AmplificationByIHCTest, orderTargetIHC, false);
            YellowstonePathology.Business.Visitor.OrderTestOrderVisitor orderVisitorIHC = new Business.Visitor.OrderTestOrderVisitor(testOrderInfoIHC);
            this.m_AccessionOrder.TakeATrip(orderVisitorIHC);
            orderVisitorIHC.PanelSetOrder.Distribute = true;

            YellowstonePathology.Business.Task.Model.TaskOrder taskOrderIHC = this.m_AccessionOrder.CreateTask(testOrderInfoIHC);
            this.m_AccessionOrder.TaskOrderCollection.Add(taskOrderIHC);

            YellowstonePathology.Business.Test.HER2AmplificationSummary.HER2AmplificationSummaryTest her2AmplificationSummaryTest = new Business.Test.HER2AmplificationSummary.HER2AmplificationSummaryTest();
            YellowstonePathology.Business.Interface.IOrderTarget orderTargetSum = this.m_AccessionOrder.SpecimenOrderCollection.GetOrderTarget(this.m_PanelSetOrder.OrderedOnId);
            YellowstonePathology.Business.Test.TestOrderInfo testOrderInfoSum = new YellowstonePathology.Business.Test.TestOrderInfo(her2AmplificationSummaryTest, orderTargetSum, false);
            YellowstonePathology.Business.Visitor.OrderTestOrderVisitor orderVisitorSum = new Business.Visitor.OrderTestOrderVisitor(testOrderInfoSum);
            this.m_AccessionOrder.TakeATrip(orderVisitorSum);
            orderVisitorSum.PanelSetOrder.Distribute = true;

            YellowstonePathology.Business.Task.Model.TaskOrder taskOrderSum = this.m_AccessionOrder.CreateTask(testOrderInfoSum);
            this.m_AccessionOrder.TaskOrderCollection.Add(taskOrderSum);
        }

        private void ResultPage_Next(object sender, EventArgs e)
        {
            if (this.ShowReflexTestPage() == false)
            {

                if (this.ShowAmendmentPage() == false)
                {
                    this.Finished();
                }
            }
        }

        private void ResultPage_SpecimenDetail(object sender, EventArgs e)
        {
            YellowstonePathology.Business.Specimen.Model.SpecimenOrder specimenOrder = this.m_AccessionOrder.SpecimenOrderCollection.GetSpecimenOrderByOrderTarget(this.m_PanelSetOrder.OrderedOnId);
            Login.SpecimenOrderDetailsPath specimenOrderDetailsPath = new Login.SpecimenOrderDetailsPath(specimenOrder, this.m_AccessionOrder, this.m_PageNavigator);
            specimenOrderDetailsPath.Finish += new Login.SpecimenOrderDetailsPath.FinishEventHandler(SpecimenOrderDetailsPath_Finish);
            specimenOrderDetailsPath.Start();
        }

        private void SpecimenOrderDetailsPath_Finish(object sender, EventArgs e)
        {
            this.ShowResultPage();
        }

        private bool ShowReflexTestPage()
        {
            bool result = false;
            YellowstonePathology.Business.Test.HER2AmplificationSummary.HER2AmplificationSummaryTest her2AmplificationSummaryTest = new Business.Test.HER2AmplificationSummary.HER2AmplificationSummaryTest();
            YellowstonePathology.Business.Test.InvasiveBreastPanel.InvasiveBreastPanelTest panelSetInvasiveBreastPanel = new YellowstonePathology.Business.Test.InvasiveBreastPanel.InvasiveBreastPanelTest();
            if (this.m_AccessionOrder.PanelSetOrderCollection.Exists(her2AmplificationSummaryTest.PanelSetId, this.m_PanelSetOrder.OrderedOnId, true) == true)
            {
                YellowstonePathology.Business.Test.PanelSetOrder panelSetOrder = this.m_AccessionOrder.PanelSetOrderCollection.GetPanelSetOrder(her2AmplificationSummaryTest.PanelSetId, this.m_PanelSetOrder.OrderedOnId, true);
                result = true;
                YellowstonePathology.UI.Test.HER2AmplificationSummaryResultPath resultPath = new Test.HER2AmplificationSummaryResultPath(panelSetOrder.ReportNo, this.m_AccessionOrder, this.m_PageNavigator, this.m_Window);
                resultPath.Finish += new Test.HER2AmplificationSummaryResultPath.FinishEventHandler(ResultPath_Finish);
                resultPath.Start();
            }
            else if (this.m_AccessionOrder.PanelSetOrderCollection.Exists(panelSetInvasiveBreastPanel.PanelSetId, this.m_PanelSetOrder.OrderedOnId, true) == true)
            {
                YellowstonePathology.Business.Test.PanelSetOrder panelSetOrder = this.m_AccessionOrder.PanelSetOrderCollection.GetPanelSetOrder(panelSetInvasiveBreastPanel.PanelSetId, this.m_PanelSetOrder.OrderedOnId, true);
                result = true;
                YellowstonePathology.UI.Test.InvasiveBreastPanelPath resultPath = new Test.InvasiveBreastPanelPath(panelSetOrder.ReportNo, this.m_AccessionOrder, this.m_PageNavigator, this.m_Window);
                resultPath.Finish += new Test.InvasiveBreastPanelPath.FinishEventHandler(ResultPath_Finish);
                resultPath.Start();
            }
            return result;
        }

        private void ResultPath_Finish(object sender, EventArgs e)
        {
            if (this.ShowAmendmentPage() == false)
            {
                base.Finished();
            }
        }

        private bool ShowAmendmentPage()
        {
            bool result = false;
            if (this.m_AccessionOrder.PanelSetOrderCollection.HasSurgical() == true)
            {
                YellowstonePathology.Business.Test.Surgical.SurgicalTestOrder surgicalTestOrder = this.m_AccessionOrder.PanelSetOrderCollection.GetSurgical();
                if (surgicalTestOrder.AmendmentCollection.HasAmendmentForReferenceReportNo(this.m_PanelSetOrder.ReportNo) == true)
                {
                    result = true;                                        
                    YellowstonePathology.Business.Amendment.Model.Amendment amendment = surgicalTestOrder.AmendmentCollection.GetAmendmentForReferenceReportNo(this.m_PanelSetOrder.ReportNo);
                    AmendmentPage amendmentPage = new AmendmentPage(this.m_AccessionOrder, amendment, this.m_SystemIdentity);
                    amendmentPage.Back += AmendmentPage_Back;
                    amendmentPage.Finish += AmendmentPage_Finish;
                    this.m_PageNavigator.Navigate(amendmentPage);
                }
            }
            return result;
        }

        private void AmendmentPage_Finish(object sender, EventArgs e)
        {
            base.Finished();
        }

        private void AmendmentPage_Back(object sender, EventArgs e)
        {
            if (this.ShowReflexTestPage() == false)
            {
                this.ShowResultPage();
            }
        }
    }
}
