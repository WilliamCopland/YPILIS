﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YellowstonePathology.Business.Test.LymphocyteSubsetPanel5
{
    // Lymphocyte subset Panel 5
    public class LymphocyteSubsetPanel5Test : YellowstonePathology.Business.PanelSet.Model.PanelSet
    {
        public LymphocyteSubsetPanel5Test()
        {
            this.m_PanelSetId = 283;
            this.m_PanelSetName = "Lymphocyte Subset Panel 5";
            this.m_Abbreviation = "Lymphocyte Subset Panel 5";
            this.m_CaseType = YellowstonePathology.Business.CaseType.Molecular;
            this.m_HasTechnicalComponent = true;
            this.m_HasProfessionalComponent = true;
            this.m_ResultDocumentSource = YellowstonePathology.Business.PanelSet.Model.ResultDocumentSourceEnum.PublishedDocument;
            this.m_ReportNoLetter = new YellowstonePathology.Business.ReportNoLetterR();
            this.m_Active = true;
            this.m_AttemptOrderTargetLookup = true;
            this.m_PanelSetOrderClassName = typeof(YellowstonePathology.Business.Test.PanelSetOrder).AssemblyQualifiedName;
            this.m_WordDocumentClassName = typeof(YellowstonePathology.Business.Document.ReferenceLabReport).AssemblyQualifiedName;
            this.m_AllowMultiplePerAccession = true;
            this.m_ExpectedDuration = new TimeSpan(5, 0, 0, 0);

            string taskDescription = "Gather materials and send out to Neo.";
            this.m_TaskCollection.Add(new YellowstonePathology.Business.Task.Model.TaskFedexShipment(YellowstonePathology.Business.Task.Model.TaskAssignment.Flow, taskDescription, new Facility.Model.NeogenomicsIrvine()));

            this.m_TechnicalComponentFacility = new YellowstonePathology.Business.Facility.Model.ARUP();
            this.m_TechnicalComponentBillingFacility = new YellowstonePathology.Business.Facility.Model.ARUP();

            this.m_ProfessionalComponentFacility = new YellowstonePathology.Business.Facility.Model.ARUP();
            this.m_ProfessionalComponentBillingFacility = new YellowstonePathology.Business.Facility.Model.ARUP();
            
            this.m_RequireAssignmentOnOrder = true;

            this.m_UniversalServiceIdCollection.Add(new YellowstonePathology.Business.ClientOrder.Model.UniversalServiceDefinitions.UniversalServiceMiscellaneous());
        }
    }
}