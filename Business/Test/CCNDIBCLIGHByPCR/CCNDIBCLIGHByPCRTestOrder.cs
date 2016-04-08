﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YellowstonePathology.Business.Persistence;

namespace YellowstonePathology.Business.Test.CCNDIBCLIGHByPCR
{
    [PersistentClass("tblCCNDIBCLIGHByPCRTestOrder", "tblPanelSetOrder", "YPIDATA")]
    public class CCNDIBCLIGHByPCRTestOrder : PanelSetOrder
    {
        private string m_Result;
        private string m_Interpretation;
        private string m_ProbeSetDetail;
        private string m_NucleiScored;
        private string m_References;

        public CCNDIBCLIGHByPCRTestOrder()
        {
        }

        public CCNDIBCLIGHByPCRTestOrder(string masterAccessionNo, string reportNo, string objectId,
            YellowstonePathology.Business.PanelSet.Model.PanelSet panelSet,
            YellowstonePathology.Business.Interface.IOrderTarget orderTarget,
            bool distribute)
			: base(masterAccessionNo, reportNo, objectId, panelSet, orderTarget, distribute)
		{
        }

        [PersistentProperty()]
        public string Result
        {
            get { return this.m_Result; }
            set
            {
                if (this.m_Result != value)
                {
                    this.m_Result = value;
                    this.NotifyPropertyChanged("Result");
                }
            }
        }

        [PersistentProperty()]
        public string Interpretation
        {
            get { return this.m_Interpretation; }
            set
            {
                if (this.m_Interpretation != value)
                {
                    this.m_Interpretation = value;
                    this.NotifyPropertyChanged("Interpretation");
                }
            }
        }

        [PersistentProperty()]
        public string ProbeSetDetail
        {
            get { return this.m_ProbeSetDetail; }
            set
            {
                if (this.m_ProbeSetDetail != value)
                {
                    this.m_ProbeSetDetail = value;
                    this.NotifyPropertyChanged("ProbeSetDetail");
                }
            }
        }

        [PersistentProperty()]
        public string NucleiScored
        {
            get { return this.m_NucleiScored; }
            set
            {
                if (this.m_NucleiScored != value)
                {
                    this.m_NucleiScored = value;
                    this.NotifyPropertyChanged("NucleiScored");
                }
            }
        }

        [PersistentProperty()]
        public string References
        {
            get { return this.m_References; }
            set
            {
                if (this.m_References != value)
                {
                    this.m_References = value;
                    this.NotifyPropertyChanged("References");
                }
            }
        }

        public override string ToResultString(YellowstonePathology.Business.Test.AccessionOrder accessionOrder)
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine("Result: " + this.m_Result);
            result.AppendLine();

            result.AppendLine("Interpretation: " + this.m_Interpretation);
            result.AppendLine();

            return result.ToString();
        }
    }
}

