﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YellowstonePathology.UI.Surgical
{
    public class PathologistSignoutPath
    {
        private YellowstonePathology.Business.Test.AccessionOrder m_AccessionOrder;
        private YellowstonePathology.Business.Test.Surgical.SurgicalTestOrder m_SurgicalTestOrder;
        private YellowstonePathology.Business.Persistence.ObjectTracker m_ObjectTracker;
        private YellowstonePathology.Business.User.SystemIdentity m_SystemIdentity;

        private List<Action> m_ActionList;
        private int m_ActionIndex;
        private List<string> m_AuditMessages;
        private List<string> m_ColonCancerMessages;

        private YellowstonePathology.Business.Audit.Model.AuditCollection m_AuditCollection;
        private YellowstonePathology.Business.Audit.Model.AuditResult m_AuditResult;
        private YellowstonePathology.Business.Audit.Model.AuditResult m_MessageAuditResult;
        private PathologistSignoutDialog m_PathologistSignoutDialog;
        private PQRSSignoutPage m_PQRSSignoutPage;

        public PathologistSignoutPath(YellowstonePathology.Business.Test.AccessionOrder accessionOrder,
            YellowstonePathology.Business.Test.Surgical.SurgicalTestOrder surgicalTestOrder,
            YellowstonePathology.Business.Persistence.ObjectTracker objectTracker,
            YellowstonePathology.Business.User.SystemIdentity systemIdentity)
        {
            this.m_AccessionOrder = accessionOrder;
            this.m_SurgicalTestOrder = surgicalTestOrder;
            this.m_ObjectTracker = objectTracker;
            this.m_SystemIdentity = systemIdentity;

            this.m_AuditCollection = new Business.Audit.Model.AuditCollection();
            this.m_AuditCollection.Add(new Business.Audit.Model.AncillaryStudiesAreHandledAudit(this.m_SurgicalTestOrder));
            this.m_AuditCollection.Add(new Business.Audit.Model.SurgicalCaseHasQuestionMarksAudit(this.m_SurgicalTestOrder));
            this.m_AuditCollection.Add(new Business.Audit.Model.SigningUserIsAssignedUserAudit(this.m_SurgicalTestOrder, this.m_SystemIdentity));
            this.m_AuditCollection.Add(new Business.Audit.Model.SvhCaseHasMRNAndAccountNoAudit(this.m_AccessionOrder));
            this.m_AuditCollection.Add(new Business.Audit.Model.CaseHasNotFoundClientAudit(this.m_AccessionOrder));
            this.m_AuditCollection.Add(new Business.Audit.Model.CaseHasNotFoundProviderAudit(this.m_AccessionOrder));
            this.m_AuditCollection.Add(new Business.Audit.Model.CaseHasUnfinaledPeerReviewAudit(this.m_AccessionOrder));
            this.m_AuditCollection.Add(new Business.Audit.Model.GradedStainsAreHandledAudit(this.m_SurgicalTestOrder));
            this.m_AuditCollection.Add(new Business.Audit.Model.IntraoperativeConsultationCorrelationAudit(this.m_SurgicalTestOrder));
            this.m_AuditCollection.Add(new Business.Audit.Model.PapCorrelationAudit(this.m_AccessionOrder));
            this.m_AuditCollection.Add(new Business.Audit.Model.PQRSIsRequiredAudit(this.m_AccessionOrder));
            this.m_AuditCollection.Add(new Business.Audit.Model.LynchSyndromeAudit(this.m_AccessionOrder));
            this.m_AuditCollection.Add(new Business.Audit.Model.CCCPAudit(this.m_AccessionOrder));
        }

        public void Start()
        {
            if (this.m_AuditResult.Status == Business.Audit.Model.AuditStatusEnum.Failure)
            {
                this.m_PathologistSignoutDialog = new PathologistSignoutDialog();
                this.SetActionList();
                this.m_ActionList[0].Invoke();
                this.m_PathologistSignoutDialog.ShowDialog();
            }
        }

        public YellowstonePathology.Business.Audit.Model.AuditResult CaseCanBeSignedOut()
        {
            this.m_AuditResult = this.m_AuditCollection.Run2();
            return this.m_AuditResult;
        }

        private void SetActionList()
        {
            this.m_ActionList = new List<Action>();
            this.m_ActionIndex = 0;
            this.m_AuditMessages = new List<string>();
            this.m_ColonCancerMessages = new List<string>();
            this.m_MessageAuditResult = new Business.Audit.Model.AuditResult();
            this.m_MessageAuditResult.Status = Business.Audit.Model.AuditStatusEnum.OK;
            this.m_MessageAuditResult.Message = string.Empty;

            foreach(YellowstonePathology.Business.Audit.Model.Audit audit in this.m_AuditCollection)
            {
                audit.Run();
                if (audit.Status == Business.Audit.Model.AuditStatusEnum.Failure)
                {
                    Type auditType = audit.GetType();
                    switch (auditType.FullName)
                    {
                        case "YellowstonePathology.Business.Audit.Model.PapCorrelationAudit":
                            {
                                this.m_ActionList.Add(HandlePapCorrelation);
                                break;
                            }
                        case "YellowstonePathology.Business.Audit.Model.PQRSIsRequiredAudit":
                            {
                                this.m_ActionList.Add(HandlePQRS);
                                break;
                            }
                        case "YellowstonePathology.Business.Audit.Model.LynchSyndromeAudit":
                        case "YellowstonePathology.Business.Audit.Model.CCCPAudit":
                            {
                                this.m_ColonCancerMessages.Add(audit.Message.ToString());
                                if (this.m_ActionList.Contains(HandleColorectalCancer) == false)
                                {
                                    this.m_ActionList.Add(HandleColorectalCancer);
                                }
                                break;
                            }
                        default:
                            {
                                this.m_AuditMessages.Add(audit.Message.ToString());
                                this.m_MessageAuditResult.Status = audit.Status;
                                if(this.m_ActionList.Contains(HandleAuditMessages) == false)
                                {
                                    this.m_ActionList.Add(HandleAuditMessages);
                                }
                                break;
                            }
                    }
                }
            }
        }

        private void MoveForward(object sender, EventArgs e)
        {
            this.m_ActionIndex++;
            if (this.m_ActionIndex >= this.m_ActionList.Count)
            {
                this.m_PathologistSignoutDialog.Close();
            }
            else
            {
                this.m_ActionList[this.m_ActionIndex].Invoke();
            }
        }

        private void MoveBack(object sender, EventArgs e)
        {
            this.m_ActionIndex--;
            if (this.m_ActionIndex < 0)
            {
                this.m_PathologistSignoutDialog.Close();
            }
            else
            {
                this.m_ActionList[this.m_ActionIndex].Invoke();
            }
        }

        private void HandlePapCorrelation()
        {
            //this.m_SurgicalTestOrder.PapCorrelationRequired = true;
            PapCorrelationPage papCorrelationPage = new PapCorrelationPage(this.m_AccessionOrder, this.m_SurgicalTestOrder, this.m_ObjectTracker);
            papCorrelationPage.Next += this.MoveForward;
            papCorrelationPage.Back += this.MoveBack;
            this.m_PathologistSignoutDialog.PageNavigator.Navigate(papCorrelationPage);
            //this.m_SurgicalTestOrder.PapCorrelationRequired = false;
            //this.m_SurgicalTestOrder.PapCorrelation = 0;
        }

        private void HandlePQRS()
        {
            if (this.m_PQRSSignoutPage == null)
            {
                bool result = false;
                YellowstonePathology.Business.Surgical.PQRSMeasureCollection pqrsCollection = YellowstonePathology.Business.Surgical.PQRSMeasureCollection.GetAll();
                foreach (YellowstonePathology.Business.Test.Surgical.SurgicalSpecimen surgicalSpecimen in this.m_SurgicalTestOrder.SurgicalSpecimenCollection)
                {
                    foreach (YellowstonePathology.Business.Surgical.PQRSMeasure pqrsMeasure in pqrsCollection)
                    {
                        int patientAge = YellowstonePathology.Business.Helper.PatientHelper.GetAge(this.m_AccessionOrder.PBirthdate.Value);
                        if (pqrsMeasure.DoesMeasureApply(this.m_SurgicalTestOrder, surgicalSpecimen, patientAge) == true)
                        {
                            this.m_PQRSSignoutPage = new PQRSSignoutPage(pqrsMeasure, surgicalSpecimen, this.m_SurgicalTestOrder, this.m_AccessionOrder, this.m_ObjectTracker);
                            this.m_PQRSSignoutPage.Next += this.MoveForward;
                            this.m_PQRSSignoutPage.Back +=this.MoveBack;
                            this.m_PathologistSignoutDialog.PageNavigator.Navigate(this.m_PQRSSignoutPage);
                            result = true;
                            break;
                        }
                    }
                    if (result == true) break;
                }
            }
            else
            {
                this.m_PathologistSignoutDialog.PageNavigator.Navigate(this.m_PQRSSignoutPage);
            }
        }

        private void HandleColorectalCancer()
        {
            ColorectalCancerOrderPage colorectalCancerOrderPage = new ColorectalCancerOrderPage(this.m_AccessionOrder, this.m_ColonCancerMessages);
            colorectalCancerOrderPage.Next += this.MoveForward;
            colorectalCancerOrderPage.Back += this.MoveBack;
            colorectalCancerOrderPage.OrderCCCP += ColorectalCancerOrderPage_OrderCCCP;
            colorectalCancerOrderPage.OrderLynchSyndrome += ColorectalCancerOrderPage_OrderLynchSyndrome;
            this.m_PathologistSignoutDialog.PageNavigator.Navigate(colorectalCancerOrderPage);
        }

        private void ColorectalCancerOrderPage_OrderCCCP(object sender, EventArgs e)
        {
            YellowstonePathology.Business.Test.ComprehensiveColonCancerProfile.ComprehensiveColonCancerProfileTest comprehensiveColonCancerProfileTest = new Business.Test.ComprehensiveColonCancerProfile.ComprehensiveColonCancerProfileTest();
            this.StartReportOrderPath(comprehensiveColonCancerProfileTest);
        }

        private void ColorectalCancerOrderPage_OrderLynchSyndrome(object sender, EventArgs e)
        {
            YellowstonePathology.Business.Test.LynchSyndrome.LynchSyndromeEvaluationTest lynchSyndromeEvaluationTest = new Business.Test.LynchSyndrome.LynchSyndromeEvaluationTest();
            this.StartReportOrderPath(lynchSyndromeEvaluationTest);
        }

        private void StartReportOrderPath(YellowstonePathology.Business.PanelSet.Model.PanelSet panelSet)
        {
            YellowstonePathology.Business.Test.TestOrderInfo testOrderInfo = new Business.Test.TestOrderInfo();
            testOrderInfo.PanelSet = panelSet;
            testOrderInfo.OrderTargetIsKnown = false;

            YellowstonePathology.UI.Login.Receiving.ReportOrderPath reportOrderPath = new Login.Receiving.ReportOrderPath(this.m_AccessionOrder, this.m_ObjectTracker, this.m_SystemIdentity, this.m_PathologistSignoutDialog.PageNavigator, PageNavigationModeEnum.Inline);
            reportOrderPath.Finish += ReportOrderPath_Finish;
            reportOrderPath.Start(testOrderInfo);
        }

        private void ReportOrderPath_Finish(object sender, CustomEventArgs.TestOrderInfoEventArgs e)
        {
            this.HandleColorectalCancer();
        }

        private void HandleAuditMessages()
        {
            PathologistSignoutAuditMessagePage pathologistSignoutAuditMessagePage = new PathologistSignoutAuditMessagePage(this.m_AuditMessages);
            pathologistSignoutAuditMessagePage.Next += this.MoveForward;
            pathologistSignoutAuditMessagePage.Back += this.MoveBack;
            this.m_PathologistSignoutDialog.PageNavigator.Navigate(pathologistSignoutAuditMessagePage);
        }
    }
}
