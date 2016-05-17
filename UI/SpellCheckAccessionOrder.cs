﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace YellowstonePathology.UI
{
    public class SpellCheckAccessionOrder
    {
        private List<SpellCheckProperty> m_PropertyList;
        private System.Text.RegularExpressions.MatchCollection m_Matches;
        private System.Text.RegularExpressions.Regex m_Regex;        
        private int m_CurrentPropertyListIndex;
       
        public SpellCheckAccessionOrder(Business.Test.AccessionOrder accessionOrder)
        {
            this.m_PropertyList = new List<SpellCheckProperty>();
            YellowstonePathology.Business.Test.Surgical.SurgicalTestOrder surgicalTestOrder = accessionOrder.PanelSetOrderCollection.GetSurgical();

            PropertyInfo clinicalInfoProperty = typeof(YellowstonePathology.Business.Test.AccessionOrder).GetProperty("ClinicalHistory");
            SpellCheckProperty clinicalInfo = new SpellCheckProperty(clinicalInfoProperty, accessionOrder, "Clinical History");
            this.m_PropertyList.Add(clinicalInfo);

            PropertyInfo grossXProperty = typeof(YellowstonePathology.Business.Test.Surgical.SurgicalTestOrder).GetProperty("GrossX");
            SpellCheckProperty grossX = new SpellCheckProperty(grossXProperty, surgicalTestOrder, "Gross Description");
            this.m_PropertyList.Add(grossX);

            PropertyInfo microscopicXProperty = typeof(YellowstonePathology.Business.Test.Surgical.SurgicalTestOrder).GetProperty("MicroscopicX");
            SpellCheckProperty microscopicX = new SpellCheckProperty(microscopicXProperty, surgicalTestOrder, "Microscopic");
            this.m_PropertyList.Add(microscopicX);

            foreach (YellowstonePathology.Business.Specimen.Model.SpecimenOrder specimenOrder in accessionOrder.SpecimenOrderCollection)
            {
                PropertyInfo specimenDescriptionProperty = typeof(YellowstonePathology.Business.Specimen.Model.SpecimenOrder).GetProperty("Description");
                SpellCheckProperty specimenDescription = new SpellCheckProperty(specimenDescriptionProperty, specimenOrder, "Specimen Description");
                this.m_PropertyList.Add(specimenDescription);
            }

            foreach (YellowstonePathology.Business.Test.Surgical.SurgicalSpecimen surgicalSpecimen in surgicalTestOrder.SurgicalSpecimenCollection)
            {
                PropertyInfo diagnosisProperty = typeof(YellowstonePathology.Business.Test.Surgical.SurgicalSpecimen).GetProperty("Diagnosis");
                SpellCheckProperty diagnosis = new SpellCheckProperty(diagnosisProperty, surgicalSpecimen, "Specimen Diagnosis");
                this.m_PropertyList.Add(diagnosis);
            }
            
            this.m_CurrentPropertyListIndex = -1;
            this.m_Regex = new System.Text.RegularExpressions.Regex(@"\b\w+\b");
        }

        public int CurrentPropertyListIndex
        {
            get { return this.m_CurrentPropertyListIndex; }
        }

        public List<SpellCheckProperty> PropertyList
        {
            get { return this.m_PropertyList; }
        }

        public bool HasNextProperty()
        {
            bool result = false;            
            if (this.m_CurrentPropertyListIndex < this.m_PropertyList.Count - 1)
            {
                result = true;
            }
            return result;
        }

        public SpellCheckProperty GetNextProperty()
        {
            this.m_CurrentPropertyListIndex += 1;                        
            return this.m_PropertyList[this.m_CurrentPropertyListIndex];
        }

        public SpellCheckProperty GetCurrentProperty()
        {
            return this.m_PropertyList[this.m_CurrentPropertyListIndex];
        }

        public void SetCurrentProperty(int index)
        {
            this.m_CurrentPropertyListIndex = index;
        }

        public void Skip()
        {                        
            //this.m_CurrentMatchIndex += 1;            
        }
    }
}